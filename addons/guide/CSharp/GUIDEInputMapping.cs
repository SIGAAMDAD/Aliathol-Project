using Godot;
using Guide.Inputs;
using Guide.Modifiers;
using Guide.Triggers;
using System.Collections.Generic;

namespace Guide {
	[Icon( "res://addons/guide/guide_internal.svg" )]
	[Tool]
	public partial class GUIDEInputMapping : Resource {
		[Export]
		public bool OverrideActionSettings {
			get => _overrideActionSettings;
			set {
				if ( _overrideActionSettings == value ) return;
				_overrideActionSettings = value;
				EmitChanged();
			}
		}
		private bool _overrideActionSettings;

		[Export]
		public bool IsRemappable {
			get => _isRemappable;
			set {
				if ( _isRemappable == value ) return;
				_isRemappable = value;
				EmitChanged();
			}
		}
		private bool _isRemappable;

		[Export]
		public string DisplayName {
			get => _displayName;
			set {
				if ( _displayName == value ) return;
				_displayName = value;
				EmitChanged();
			}
		}
		private string _displayName = "";

		[Export]
		public string DisplayCategory {
			get => _displayCategory;
			set {
				if ( _displayCategory == value ) return;
				_displayCategory = value;
				EmitChanged();
			}
		}
		private string _displayCategory = "";

		[ExportGroup( "Mappings" )]
		[Export]
		public GUIDEInput Input {
			get => _input;
			set {
				if ( _input == value ) return;
				_input = value;
				EmitChanged();
			}
		}
		private GUIDEInput _input;

		[Export]
		public GUIDEModifier[] Modifiers {
			get => _modifiers;
			set {
				if ( _modifiers == value ) return;
				_modifiers = value;
				EmitChanged();
			}
		}
		private GUIDEModifier[] _modifiers;

		[Export]
		public GUIDETrigger[] Triggers {
			get => _triggers;
			set {
				if ( _triggers == value ) return;
				_triggers = value;
				EmitChanged();
			}
		}
		private GUIDETrigger[] _triggers;

		public float _TriggerHoldThreshold = -1.0f;

		public GUIDETrigger.GUIDETriggerState _State = GUIDETrigger.GUIDETriggerState.NONE;
		public Vector3 _Value = Vector3.Zero;

		private List<GUIDETrigger> _triggerList = new();
		private int _implicitCount;
		private int _explicitCount;

		public void _Initialize( GUIDEAction.GUIDEActionValueType valueType ) {
			_triggerList.Clear();

			_implicitCount = 0;
			_explicitCount = 0;
			_TriggerHoldThreshold = -1.0f;

			if ( _triggers.Length == 0 ) {
				var defaultTrigger = new GUIDETriggerDown { ActuationThreshold = 0 };
				_explicitCount = 1;
				_triggerList.Add( defaultTrigger );
				return;
			}

			var inputValue = _input?._Value ?? Vector3.Zero;

			foreach ( var modifier in _modifiers )
				inputValue = modifier._ModifyInput( inputValue, 0, valueType );

			foreach ( var trigger in _triggers ) {
				switch ( trigger._GetTriggerType() ) {
					case GUIDETrigger.GUIDETriggerType.EXPLICIT:
						_explicitCount++;
						break;
					case GUIDETrigger.GUIDETriggerType.IMPLICIT:
						_implicitCount++;
						break;
				}
				_triggerList.Add( trigger );

				if ( trigger is GUIDETriggerHold hold ) {
					if ( _TriggerHoldThreshold == -1 )
						_TriggerHoldThreshold = hold.HoldThreshold;
					else
						_TriggerHoldThreshold = Mathf.Min( _TriggerHoldThreshold, hold.HoldThreshold );
				}

				trigger._LastValue = inputValue;
			}
		}

		public void _UpdateState( double delta, GUIDEAction.GUIDEActionValueType valueType ) {
			var inputValue = _input?._Value ?? Vector3.Zero;

			foreach ( var modifier in _modifiers )
				inputValue = modifier._ModifyInput( inputValue, delta, valueType );

			_Value = inputValue;

			var triggeredImplicits = 0;
			var triggeredExplicits = 0;
			var triggeredBlocked = 0;

			var result = GUIDETrigger.GUIDETriggerState.NONE;
			foreach ( var trigger in _triggerList ) {
				var triggerResult = trigger._UpdateState( _Value, delta, valueType );
				trigger._LastValue = _Value;

				var triggerType = trigger._GetTriggerType();
				if ( triggerResult == GUIDETrigger.GUIDETriggerState.TRIGGERED ) {
					switch ( triggerType ) {
						case GUIDETrigger.GUIDETriggerType.EXPLICIT:
							triggeredExplicits++;
							break;
						case GUIDETrigger.GUIDETriggerType.IMPLICIT:
							triggeredImplicits++;
							break;
						case GUIDETrigger.GUIDETriggerType.BLOCKING:
							triggeredBlocked++;
							break;
					}
				}

				if ( triggerType == GUIDETrigger.GUIDETriggerType.EXPLICIT )
					result = (GUIDETrigger.GUIDETriggerState)Mathf.Max( (int)result, (int)triggerResult );
			}

			if ( triggeredBlocked > 0 ) {
				_State = GUIDETrigger.GUIDETriggerState.NONE;
				return;
			}

			if ( triggeredImplicits < _implicitCount ) {
				_State = GUIDETrigger.GUIDETriggerState.NONE;
				return;
			}

			if ( _explicitCount == 0 && _implicitCount > 0 ) {
				_State = GUIDETrigger.GUIDETriggerState.TRIGGERED;
				return;
			}

			_State = result;
		}
	}
};