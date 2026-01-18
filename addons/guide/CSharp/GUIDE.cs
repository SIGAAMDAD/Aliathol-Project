using Godot;
using Guide.Inputs;
using Guide.Modifiers;
using Guide.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Guide {
	public partial class GUIDE : Node {
		private const string GUIDESetPath = "res://addons/guide/guide_set.gd";
		private const string GUIDEResetPath = "res://addons/guide/guide_reset.gd";
		private const string GUIDEInputTrackerPath = "res://addons/guide/guide_input_tracker.gd";

		[Signal]
		public delegate void InputMappingsChangedEventHandler();

		// The currently active contexts. Key is the context, value is the priority
		private Dictionary<GUIDEMappingContext, int> _activeContexts = new();

		// The currently active action mappings.
		private List<GUIDEActionMapping> _activeActionMappings = new();

		// The currently active remapping config.
		private GUIDERemappingConfig _activeRemappingConfig;

		// All currently active inputs as collected from the active input mappings
		private GUIDESet _activeInputs = new();

		// All currently active modifiers as collected from the active input mappings
		private GUIDESet _activeModifiers = new();

		// A dictionary of actions sharing input. Key is the action, value
		// is an array of lower-priority actions that share input with the
		// key action.
		private Dictionary<GUIDEAction, List<GUIDEAction>> _actionsSharingInput = new();

		// A reference to the reset node which resets inputs that need a reset per frame
		// This is an extra node because the reset should run at the end of the frame
		// before new input is processed at the beginning of the frame.
		private GUIDEReset _resetNode;

		// The current input state. This is used to track the state of the inputs
		// and serves as a basis for the GUIDEInputs.
		private GUIDEInputState _inputState;

		// A lock, preventing a mapping context change while a mapping
		// context change is currently in progress.
		private bool _locked = false;

		public override void _Ready() {
			ProcessMode = ProcessModeEnum.Always;
			_resetNode = new GUIDEReset();
			_inputState = new GUIDEInputState();
			_inputState.Reset();
			AddChild( _resetNode );
			// attach to the current viewport to get input events
			GUIDEInputTracker._Instrument.CallDeferred( GetViewport() );

			GetTree().NodeAdded += _OnNodeAdded;

			// Emit a change of input mappings whenever a joystick was connected
			// or disconnected.
			Input.JoyConnectionChanged += ( device, connected ) => EmitSignal( SignalName.InputMappingsChanged );
		}

		// Called when a node is added to the tree. If the node is a window
		// GUIDE will instrument it to get events when the window is focused.
		private void _OnNodeAdded( Node node ) {
			if ( node is not Window )
				return;

			GUIDEInputTracker._Instrument.Call( node );
		}

		// Injects input into GUIDE. GUIDE will call this automatically but
		// can also be used to manually inject input for GUIDE to handle
		public void InjectInput( InputEvent @event ) {
			if ( @event is InputEventAction )
				return; // we don't react to Godot's built-in events

			// The input state is the sole consumer of input events. It will notify
			// GUIDEInputs when relevant input events happen. This way we don't need
			// to process input events multiple times and at the same time always have
			// the full picture of the input state.
			_inputState.Input( @event );
		}

		// Applies an input remapping config. This will override all input bindings in the
		// currently loaded mapping contexts with the bindings from the configuration.
		// Note that GUIDE will not track changes to the remapping config. If your remapping
		// config changes, you will need to call this method again.
		public void SetRemappingConfig( GUIDERemappingConfig config ) {
			_activeRemappingConfig = config;
			_UpdateCaches();
		}

		// Enables the given context with the given priority. Lower numbers have higher priority. If
		// disable_others is set to true, all other currently enabled mapping contexts will be disabled.
		public void EnableMappingContext( GUIDEMappingContext context, bool disableOthers = false, int priority = 0 ) {
			if ( !IsInstanceValid( context ) ) {
				GD.PushError( "Null context given. Ignoring." );
				return;
			}

			if ( disableOthers )
				_activeContexts.Clear();

			_activeContexts[ context ] = priority;
			_UpdateCaches();
			// notify listeners that the context was enabled
			context.EmitSignal( GUIDEMappingContext.SignalName.Enabled );
		}

		// Disables the given mapping context.
		public void DisableMappingContext( GUIDEMappingContext context ) {
			if ( !IsInstanceValid( context ) ) {
				GD.PushError( "Null context given. Ignoring." );
				return;
			}

			_activeContexts.Remove( context );
			_UpdateCaches();
			// notify listeners that the context was disabled
			context.EmitSignal( GUIDEMappingContext.SignalName.Disabled );
		}

		// Checks whether the given mapping context is currently enabled.
		public bool IsMappingContextEnabled( GUIDEMappingContext context ) {
			return _activeContexts.ContainsKey( context );
		}

		// Returns the currently enabled mapping contexts
		public List<GUIDEMappingContext> GetEnabledMappingContexts() {
			return new List<GUIDEMappingContext>( _activeContexts.Keys );
		}

		// Updates all currently active modifiers
		public override void _PhysicsProcess( double delta ) {
			foreach ( var modifier in _activeModifiers.Values().Cast<GUIDEModifier>() ) {
				modifier._PhysicsProcess( delta );
			}
		}

		// Processes all currently active actions
		public override void _Process( double delta ) {
			var blockedActions = new GUIDESet();

			foreach ( var actionMapping in _activeActionMappings ) {
				var action = actionMapping.Action;

				// Walk over all input mappings for this action and consolidate state
				// and result value.
				var consolidatedValue = Vector3.Zero;
				var consolidatedTriggerState = GUIDETrigger.GUIDETriggerState.NONE;

				foreach ( var inputMapping in actionMapping.InputMappings ) {
					inputMapping._UpdateState( delta, action.ActionValueType );
					consolidatedValue += inputMapping._Value;
					consolidatedTriggerState = (GUIDETrigger.GUIDETriggerState)Mathf.Max( (int)consolidatedTriggerState, (int)inputMapping._State );
				}

				// we do the blocking check only here because triggers may need to run anyways
				// (e.g. to collect hold times).
				if ( blockedActions.Has( action ) )
					consolidatedTriggerState = GUIDETrigger.GUIDETriggerState.NONE;

				if ( action.BlockLowerPriorityActions && consolidatedTriggerState == GUIDETrigger.GUIDETriggerState.TRIGGERED && _actionsSharingInput.ContainsKey( action ) ) {
					foreach ( var blockedAction in _actionsSharingInput[ action ] ) {
						blockedActions.Add( blockedAction );
					}
				}

				// Now state change events.
				switch ( action._LastState ) {
					case GUIDEAction.GUIDEActionState.TRIGGERED:
						switch ( consolidatedTriggerState ) {
							case GUIDETrigger.GUIDETriggerState.NONE:
								action._Completed( consolidatedValue );
								break;
							case GUIDETrigger.GUIDETriggerState.ONGOING:
								action._Ongoing( consolidatedValue, delta );
								break;
							case GUIDETrigger.GUIDETriggerState.TRIGGERED:
								action._Triggered( consolidatedValue, delta );
								break;
						}
						break;
					case GUIDEAction.GUIDEActionState.ONGOING:
						switch ( consolidatedTriggerState ) {
							case GUIDETrigger.GUIDETriggerState.NONE:
								action._Cancelled( consolidatedValue );
								break;
							case GUIDETrigger.GUIDETriggerState.ONGOING:
								action._Ongoing( consolidatedValue, delta );
								break;
							case GUIDETrigger.GUIDETriggerState.TRIGGERED:
								action._Triggered( consolidatedValue, delta );
								break;
						}
						break;
					case GUIDEAction.GUIDEActionState.COMPLETED:
						switch ( consolidatedTriggerState ) {
							case GUIDETrigger.GUIDETriggerState.NONE:
								// make sure the value updated but don't emit any other events
								action._UpdateValue( consolidatedValue );
								break;
							case GUIDETrigger.GUIDETriggerState.ONGOING:
								action._Started( consolidatedValue );
								break;
							case GUIDETrigger.GUIDETriggerState.TRIGGERED:
								action._Triggered( consolidatedValue, delta );
								break;
						}
						break;
				}
			}
		}

		// This updates the caches of active inputs, action mappings and modifiers. It's sort of expensive to run
		// but it is only run when contexts are enabled/disabled or remapping configs are applied and it saves
		// a lot of processing time during the actual input processing. It also simplifies the input processing
		// code as all the rules for how inputs, actions and modifiers are consolidated are already applied here.
		// This is called automatically when contexts are enabled/disabled or remapping configs are applied.
		private void _UpdateCaches() {
			if ( _locked ) {
				GD.PushError( "Mapping context changed again while processing a change. Ignoring to avoid endless loop." );
				return;
			}

			_locked = true;

			var sortedContexts = _activeContexts.Keys.OrderBy( c => _activeContexts[ c ] ).ToList();

			// The actions we already have processed. Same action may appear in different
			// contexts, so if we find the same action twice, only the first instance wins.
			var processedActions = new GUIDESet();
			// The new inputs that we will use for the action mappings.
			var newInputs = new GUIDESet();
			// The new action mappings that we will use from now on.
			var newActionMappings = new List<GUIDEActionMapping>();
			// The new modifiers that we will use
			var newModifiers = new GUIDESet();

			// Step 0: walk over the new contexts and save over all inputs and modifiers that we
			// are going to keep. This is needed to ensure that we don't reset inputs and that if
			// new mappings don't create copies of existing inputs if they have a higher priority
			// than the existing ones (see https://github.com/godotneers/G.U.I.D.E/issues/94).
			foreach ( var context in sortedContexts ) {
				foreach ( var actionMapping in context.Mappings ) {
					foreach ( var existingMapping in _activeActionMappings ) {
						if ( _IsSameActionMapping( existingMapping, actionMapping ) ) {
							// we will keep using this mapping, so we will make sure its inputs and modifiers
							// are kept and not duplicated. We don't add the action mapping to the new action mappings
							// yet, because the order of the action mappings is important and we will
							// add it later when we process the action mappings.

							foreach ( var inputMapping in existingMapping.InputMappings ) {
								if ( inputMapping.Input != null )
									newInputs.Add( inputMapping.Input );

								foreach ( var modifier in inputMapping.Modifiers )
									newModifiers.Add( modifier );
							}
						}
					}
				}
			}

			// Step 1: Collect all action mappings from the currently enabled contexts.
			foreach ( var context in sortedContexts ) {
				var position = 0;
				foreach ( var actionMapping in context.Mappings ) {
					position++;
					var action = actionMapping.Action;

					// Mapping may be misconfigured, so we need to handle the case
					// that the action is missing.
					if ( action == null ) {
						GD.PushWarning( $"Mapping at position {position} in context {context.ResourcePath} has no action set. This mapping will be ignored." );
						continue;
					}

					// If the action was already configured in a higher priority context,
					// we'll skip it.
					if ( processedActions.Has( action ) )
						continue;

					processedActions.Add( action );

					// If the action mapping is the same as one that is already active,
					// we use the existing one instead of creating a new one.
					// We do this to avoid losing state in the triggers and modifiers when
					// switching contexts. See https://github.com/godotneers/G.U.I.D.E/issues/67
					// for details. In addition there is no need to create new objects
					// if we already have a functional one (though the comparison of the mappings
					// is likely more expensive than the creation of a new one).
					var foundExisting = false;
					foreach ( var existingMapping in _activeActionMappings ) {
						if ( _IsSameActionMapping( existingMapping, actionMapping ) ) {
							// we found an existing mapping, so we can just use it
							// and we can skip the rest of the processing for this mapping.
							newActionMappings.Add( existingMapping );
							foundExisting = true;
							break;
						}
					}

					if ( foundExisting )
						continue;

					// We consolidate the inputs here, so we'll internally build a new
					// action mapping that uses consolidated inputs rather than the
					// original ones. This achieves multiple things:
					// - if two actions check for the same input, we only need to
					//   process the input once instead of twice.
					// - it allows us to prioritize input, if two actions check for
					//   the same input. This way the first action can consume the
					//   input and not have it affect further actions.
					// - we make sure nobody shares triggers as they are stateful and
					//   should not be shared.

					var effectiveMapping = new GUIDEActionMapping();
					effectiveMapping.Action = action;
					_CopyMeta( actionMapping, effectiveMapping );

					// the trigger hold threshold is the minimum time that the input must be held
					// down before the action triggers. This is used to hint the UI about
					// how long the input must be held down. We collect this while iterating
					// over the input mappings.
					var triggerHoldThreshold = -1.0f;

					// now update the action and input mappings
					for ( var index = 0; index < actionMapping.InputMappings.Length; index++ ) {
						var inputMapping = actionMapping.InputMappings[ index ];
						// get the input that is assigned to this action mapping
						var boundInput = inputMapping.Input;

						// if the re-mapping has an override for the input (e.g. the player has changed
						// the default binding to something else), apply it.
						if ( _activeRemappingConfig != null && _activeRemappingConfig._Has( context, action, index ) )
							boundInput = _activeRemappingConfig._GetBoundInputOrNull( context, action, index );

						// make a new input mapping
						var newInputMapping = new GUIDEInputMapping();

						// bound_input can be null for combo mappings, so check that
						if ( boundInput != null ) {
							// check if we already have this kind of input
							// first try to find it in the currently active inputs, this way we don't need to recreate
							// inputs that are already active.
							Variant? existing = _activeInputs.FirstMatch( it => ( (GUIDEInput)it ).IsSameAs( boundInput ) );
							existing ??= newInputs.FirstMatch( it => ( (GUIDEInput)it ).IsSameAs( boundInput ) );

							if ( existing.HasValue )
								boundInput = (GUIDEInput)existing;

							// ensure that the input is initialized and ready to be used
							if ( !_IsUsed( boundInput ) ) {
								boundInput._State = _inputState;
								boundInput._BeginUsage();
								_MarkUsed( boundInput, true );
							}

							newInputs.Add( boundInput );
						}

						// copy metadata as this may be important for formatting
						newInputMapping.Input = boundInput;
						newInputMapping.DisplayName = inputMapping.DisplayName;
						newInputMapping.DisplayCategory = inputMapping.DisplayCategory;
						newInputMapping.OverrideActionSettings = inputMapping.OverrideActionSettings;
						newInputMapping.IsRemappable = inputMapping.IsRemappable;
						_CopyMeta( inputMapping, newInputMapping );

						// modifiers cannot be re-bound so we can just use the one
						// from the original configuration. this is also needed for shared
						// modifiers to work.
						newInputMapping.Modifiers = inputMapping.Modifiers;
						// track the modifiers, so we can later only disable the ones we don't need anymore.
						foreach ( var modifier in newInputMapping.Modifiers ) {
							newModifiers.Add( modifier );

							// initialize the modifier if it is not already in use
							if ( !_IsUsed( modifier ) ) {
								modifier._BeginUsage();
								_MarkUsed( modifier, true );
							}
						}

						// triggers also cannot be re-bound but we still make a copy
						// to ensure that no shared triggers exist.
						newInputMapping.Triggers = new();

						foreach ( var trigger in inputMapping.Triggers )
							newInputMapping.Triggers.Add( trigger.Duplicate() );

						// now initialize the input mapping
						newInputMapping._Initialize( action.ActionValueType );
						// collect the hold threshold
						var mappingHoldThreshold = newInputMapping._TriggerHoldThreshold;
						// smallest hold threshold that isn't negative wins
						if ( triggerHoldThreshold < 0 || mappingHoldThreshold < triggerHoldThreshold )
							triggerHoldThreshold = mappingHoldThreshold;

						// and add it to the new mapping
						effectiveMapping.InputMappings.Add( newInputMapping );
					}

					// finally we set the hold threshold for the action
					action._TriggerHoldThreshold = triggerHoldThreshold;

					// if any binding remains, add the mapping to the list of active
					// action mappings
					if ( effectiveMapping.InputMappings.Count > 0 )
						newActionMappings.Add( effectiveMapping );
				}
			}

			// now we can clean up stuff, that we don't need anymore.
			// we start with the inputs that are no longer used.
			foreach ( var input in _activeInputs.Values().Cast<GUIDEInput>() ) {
				// because we consolidated inputs, we can do an instance check rather than
				// a is_same_as check.
				if ( newInputs.Has( input ) )
					continue;

				// this input is no longer used, so we can reset it
				// and notify it that it is no longer used.
				input._Reset();
				input._EndUsage();
				input._State = null;
				_MarkUsed( input, false );
			}

			// and now the consolidated inputs are the new active inputs.
			_activeInputs = newInputs;
			// only modifiers that require physics processing are considered "active" modifiers
			_activeModifiers = newModifiers.Filter( it => ( (GUIDEModifier)it )._NeedsPhysicsProcess() );
			// only enable physics_processing if we actually have an active modifiers
			SetPhysicsProcess( !_activeModifiers.IsEmpty() );

			// Now action mappings and their modifiers.
			foreach ( var mapping in _activeActionMappings ) {
				if ( newActionMappings.Contains( mapping ) )
					continue;

				// Cancel all actions that are going away, so they don't end up in a weird state.
				switch ( mapping.Action._LastState ) {
					case GUIDEAction.GUIDEActionState.ONGOING:
						mapping.Action._Cancelled( Vector3.Zero );
						break;
					case GUIDEAction.GUIDEActionState.TRIGGERED:
						mapping.Action._Completed( Vector3.Zero );
						break;
				}

				// notify all modifiers they are no longer in use
				foreach ( var inputMapping in mapping.InputMappings ) {
					foreach ( var modifier in inputMapping.Modifiers ) {
						// because modifiers can be shared, we need to check if the modifier
						// is still in use by any other action mapping that remains in use.
						if ( !newModifiers.Has( modifier ) ) {
							modifier._EndUsage();
							_MarkUsed( modifier, false );
						}
					}
				}
			}

			// and now we can assign the new action mappings
			_activeActionMappings = newActionMappings;

			// prepare the action input share lookup table
			_actionsSharingInput.Clear();
			for ( var i = 0; i < _activeActionMappings.Count; i++ ) {
				var mapping = _activeActionMappings[ i ];

				if ( mapping.Action.BlockLowerPriorityActions ) {
					// first find out if the action uses any chorded actions and
					// collect all inputs that this action uses
					var chordedActions = new GUIDESet();
					var inputs = new GUIDESet();
					var blockedActions = new GUIDESet();
					foreach ( var inputMapping in mapping.InputMappings ) {
						if ( inputMapping.Input != null )
							inputs.Add( inputMapping.Input );

						foreach ( var trigger in inputMapping.Triggers ) {
							if ( trigger is GUIDETriggerChordedAction chorded && chorded.Action != null )
								chordedActions.Add( chorded.Action );
						}
					}

					// Now the action that has a chorded action (A) needs to make sure that
					// the chorded action it depends upon (B) is not blocked (otherwise A would
					// never trigger) and if that chorded action (B) in turn depends on chorded actions. So
					// if chorded actions build a chain, we need to keep the full
					// chain unblocked. In addition we need to add the inputs of all
					// these chorded actions to the list of blocked inputs.
					for ( var j = i + 1; j < _activeActionMappings.Count; j++ ) {
						var innerMapping = _activeActionMappings[ j ];
						// this is a chorded action that is used by one other action
						// in the chain.
						if ( chordedActions.Has( innerMapping.Action ) ) {
							foreach ( var inputMapping in innerMapping.InputMappings ) {
								// put all of its inputs into the list of blocked inputs
								if ( inputMapping.Input != null )
									inputs.Add( inputMapping.Input );

								// also if this mapping in turn again depends on a chorded
								// action, ad this one to the list of chorded actions
								foreach ( var trigger in inputMapping.Triggers ) {
									if ( trigger is GUIDETriggerChordedAction chorded && chorded.Action != null )
										chordedActions.Add( chorded.Action );
								}
							}
						}
					}

					// now find lower priority actions that share input
					for ( var j = i + 1; j < _activeActionMappings.Count; j++ ) {
						var innerMapping = _activeActionMappings[ j ];
						if ( chordedActions.Has( innerMapping.Action ) )
							continue;

						foreach ( var inputMapping in innerMapping.InputMappings ) {
							if ( inputMapping.Input == null )
								continue;

							// because we consolidated input, we can now do an == comparison
							// to find equal input.
							if ( inputs.Has( inputMapping.Input ) ) {
								blockedActions.Add( innerMapping.Action );
								break;
							}
						}
					}

					if ( !blockedActions.IsEmpty() )
						_actionsSharingInput[ mapping.Action ] = blockedActions.Values().Cast<GUIDEAction>().ToList();
				}
			}

			// collect which inputs we need to reset per frame
			_resetNode._InputsToReset.Clear();
			foreach ( var input in _activeInputs.Values().Cast<GUIDEInput>() ) {
				if ( input._NeedsReset() )
					_resetNode._InputsToReset.Add( input );
			}

			// run a round of _process so we can be sure our actions are
			// up-to-date
			_Process( 0.0 );

			// unlock
			_locked = false;

			// and notify interested parties that the input mappings have changed
			EmitSignal( SignalName.InputMappingsChanged );
		}

		// Helper function which determines whether two action mappings are the same.
		// They are the same if they have the same action, the same input mappings
		// the same modifiers and the same triggers. Same doesn't necessarily mean
		// they are the same instance, but rather that they are equivalent in terms of
		// their configuration.
		private static bool _IsSameActionMapping( GUIDEActionMapping a, GUIDEActionMapping b ) {
			// If its the same instance, we can just return true.
			if ( a == b )
				return true;

			// If they don't have the same action, they cannot be the same.
			if ( a.Action != b.Action )
				return false;

			// If they don't have the same number of input mappings, they cannot be the same.
			if ( a.InputMappings.Count != b.InputMappings.Count )
				return false;

			// Now check all input mappings.
			for ( var i = 0; i < a.InputMappings.Count; i++ ) {
				var inputMappingA = a.InputMappings[ i ];
				var inputMappingB = b.InputMappings[ i ];

				var inputA = inputMappingA.Input;
				var inputB = inputMappingB.Input;

				if ( inputA != null && inputB != null ) {
					// If the inputs are not the same, they cannot be the same.
					if ( !inputMappingA.Input.IsSameAs( inputMappingB.Input ) )
						return false;
				} else if ( inputA != inputB ) {
					// If one input is null and the other is not, they cannot be the same.
					return false;
				}

				// If the modifiers are not the same, they cannot be the same.
				if ( inputMappingA.Modifiers.Count != inputMappingB.Modifiers.Count )
					return false;

				for ( var j = 0; j < inputMappingA.Modifiers.Count; j++ ) {
					var modifierA = inputMappingA.Modifiers[ j ];
					var modifierB = inputMappingB.Modifiers[ j ];

					if ( modifierA != null && modifierB != null ) {
						// If the modifiers are not the same, they cannot be the same.
						if ( !modifierA.IsSameAs( modifierB ) )
							return false;
					} else if ( modifierA != modifierB ) {
						// If one modifier is null and the other is not, they cannot be the same.
						return false;
					}
				}

				// If the triggers are not the same, they cannot be the same.
				if ( inputMappingA.Triggers.Count != inputMappingB.Triggers.Count )
					return false;

				for ( var j = 0; j < inputMappingA.Triggers.Count; j++ ) {
					var triggerA = inputMappingA.Triggers[ j ];
					var triggerB = inputMappingB.Triggers[ j ];

					if ( triggerA != null && triggerB != null ) {
						// If the triggers are not the same, they cannot be the same.
						if ( !triggerA.IsSameAs( triggerB ) )
							return false;
					} else if ( triggerA != triggerB ) {
						// If one trigger is null and the other is not, they cannot be the same.
						return false;
					}
				}
			}

			return true;
		}

		private static void _MarkUsed( GodotObject obj, bool value ) {
			if ( value )
				obj.SetMeta( "__guide_in_use", value );
			else
				obj.RemoveMeta( "__guide_in_use" );
		}

		private static bool _IsUsed( GodotObject obj ) {
			return obj.HasMeta( "__guide_in_use" );
		}

		private static void _CopyMeta( GodotObject source, GodotObject target ) {
			var keys = source.GetMetaList();
			foreach ( var key in keys ) {
				target.SetMeta( (string)key, source.GetMeta( key ) );
			}
		}
	}

	public class GUIDERemappingConfig {
	}
};