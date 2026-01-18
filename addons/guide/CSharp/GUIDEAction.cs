using Godot;
using System;

namespace Guide {
	[Tool]
	[Icon( "res://addons/guide/guide_action.svg" )]
	public partial class GUIDEAction : Resource {
		public enum GUIDEActionValueType {
			BOOL = 0,
			AXIS_1D = 1,
			AXIS_2D = 2,
			AXIS_3D = 3
		}

		public enum GUIDEActionState {
			TRIGGERED,
			ONGOING,
			COMPLETED
		}

		[Export]
		public StringName Name {
			get => _name;
			set {
				if ( _name == value ) return;
				_name = value;
				EmitChanged();
			}
		}
		private StringName _name;

		[Export]
		public GUIDEActionValueType ActionValueType {
			get => _actionValueType;
			set {
				if ( _actionValueType == value ) return;
				_actionValueType = value;
				EmitChanged();
			}
		}
		private GUIDEActionValueType _actionValueType = GUIDEActionValueType.BOOL;

		[Export]
		public bool BlockLowerPriorityActions {
			get => _blockLowerPriorityActions;
			set {
				if ( _blockLowerPriorityActions == value ) return;
				_blockLowerPriorityActions = value;
				EmitChanged();
			}
		}
		private bool _blockLowerPriorityActions = true;

		[ExportCategory( "Godot Actions" )]
		[Export]
		public bool EmitAsGodotActions {
			get => _emitAsGodotActions;
			set {
				if ( _emitAsGodotActions == value ) return;
				_emitAsGodotActions = value;
				EmitChanged();
			}
		}
		private bool _emitAsGodotActions = false;

		[ExportCategory( "Action Remapping" )]
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
		private string _displayName;

		[Export]
		public string DisplayCategory {
			get => _displayCategory;
			set {
				if ( _displayCategory == value ) return;
				_displayCategory = value;
				EmitChanged();
			}
		}
		private string _displayCategory;

		[Signal]
		public delegate void TriggeredEventHandler();

		[Signal]
		public delegate void JustTriggeredEventHandler();

		[Signal]
		public delegate void StartedEventHandler();

		[Signal]
		public delegate void OngoingEventHandler();

		[Signal]
		public delegate void CompletedEventHandler();

		[Signal]
		public delegate void CancelledEventHandler();

		public GUIDEActionState _LastState = GUIDEActionState.COMPLETED;

		private bool _valueBool;
		public bool ValueBool => _valueBool;

		public float ValueAxis1d => _value.X;

		private Vector2 _valueAxis2d = Vector2.Zero;
		public Vector2 ValueAxis2d => _valueAxis2d;

		private Vector3 _value = Vector3.Zero;
		public Vector3 ValueAxis3d => _value;

		private double _elapsedSeconds;
		public double ElapsedSeconds => _elapsedSeconds;

		private double _elapsedRatio;
		public double ElapsedRatio => _elapsedRatio;

		private double _triggeredSeconds;
		public double TriggeredSeconds => _triggeredSeconds;

		public double _TriggerHoldThreshold = -1.0;

		public void _Triggered( Vector3 value, double delta ) {
			_triggeredSeconds += delta;
			_elapsedRatio = 1.0;
			_UpdateValue( value );
			if ( _LastState != GUIDEActionState.TRIGGERED )
				EmitSignal( SignalName.JustTriggered );
			_LastState = GUIDEActionState.TRIGGERED;
			EmitSignal( SignalName.Triggered );
			_EmitGodotActionMaybe( true );
		}

		public void _Started( Vector3 value ) {
			_elapsedRatio = 0.0;
			_UpdateValue( value );
			_LastState = GUIDEActionState.ONGOING;
			EmitSignal( SignalName.Started );
			EmitSignal( SignalName.Ongoing );
		}

		public void _Ongoing( Vector3 value, double delta ) {
			_elapsedSeconds += delta;
			if ( _TriggerHoldThreshold > 0 )
				_elapsedRatio = _elapsedSeconds / _TriggerHoldThreshold;
			_UpdateValue( value );
			var wasTriggered = _LastState == GUIDEActionState.TRIGGERED;
			_LastState = GUIDEActionState.ONGOING;
			EmitSignal( SignalName.Ongoing );
			if ( wasTriggered )
				_EmitGodotActionMaybe( false );
		}

		public void _Cancelled( Vector3 value ) {
			_elapsedSeconds = 0;
			_elapsedRatio = 0;
			_UpdateValue( value );
			_LastState = GUIDEActionState.COMPLETED;
			EmitSignal( SignalName.Cancelled );
			EmitSignal( SignalName.Completed );
		}

		public void _Completed( Vector3 value ) {
			_elapsedSeconds = 0;
			_elapsedRatio = 0;
			_triggeredSeconds = 0;
			_UpdateValue( value );
			_LastState = GUIDEActionState.COMPLETED;
			EmitSignal( SignalName.Completed );
			_EmitGodotActionMaybe( false );
		}

		private void _EmitGodotActionMaybe( bool pressed ) {
			if ( !_emitAsGodotActions )
				return;

			if ( _name.IsEmpty ) {
				GD.PushError( "Cannot emit action into Godot's system because name is empty." );
				return;
			}

			var godotAction = new InputEventAction {
				Action = _name,
				Strength = _value.X,
				Pressed = pressed
			};
			Input.ParseInputEvent( godotAction );
		}

		public void _UpdateValue( Vector3 value ) {
			switch ( _actionValueType ) {
				case GUIDEActionValueType.BOOL:
				case GUIDEActionValueType.AXIS_1D:
					_valueBool = Mathf.Abs( value.X ) > 0;
					_valueAxis2d = new Vector2( Mathf.Abs( value.X ), 0 );
					_value = new Vector3( value.X, 0, 0 );
					break;
				case GUIDEActionValueType.AXIS_2D:
					_valueBool = Mathf.Abs( value.X ) > 0;
					_valueAxis2d = new Vector2( value.X, value.Y );
					_value = new Vector3( value.X, value.Y, 0 );
					break;
				case GUIDEActionValueType.AXIS_3D:
					_valueBool = Mathf.Abs( value.X ) > 0;
					_valueAxis2d = new Vector2( value.X, value.Y );
					_value = value;
					break;
			}
		}

		public bool IsTriggered() => _LastState == GUIDEActionState.TRIGGERED;

		public bool IsCompleted() => _LastState == GUIDEActionState.COMPLETED;

		public bool IsOngoing() => _LastState == GUIDEActionState.ONGOING;

		public string _EditorName() {
			if ( !string.IsNullOrEmpty( _displayName ) )
				return _displayName;

			if ( !_name.IsEmpty )
				return _name.ToString();

			return ResourcePath.GetFile().Replace( ".tres", "" );
		}
	}
};