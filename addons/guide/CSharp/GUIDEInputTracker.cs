using Godot;

namespace Guide {
	/// Tracker that tracks input for a window and injects it into GUIDE.
	/// Will automatically keep track of sub-windows.
	public partial class GUIDEInputTracker : Node {
		public static Callable _Instrument;

		static GUIDEInputTracker( GUIDE guide ) {
			_Instrument = Callable.From<Viewport>( ( Viewport viewport ) => {
				if ( viewport.HasMeta( "x-guide-instrumented" ) )
					return;

				var tracker = new GUIDEInputTracker();
				tracker.ProcessMode = ProcessModeEnum.Always;
				viewport.AddChild( tracker, false, InternalMode.Back );
				viewport.GuiFocusChanged += tracker._ControlFocused;
			} );
		}

		// Catches unhandled input and forwards it to GUIDE
		public override void _UnhandledInput( InputEvent @event ) {
			GUIDE.InjectInput( @event );
		}

		// Some ... creative code ... to catch events from popup windows
		// that are spawned by Godot's control nodes.
		private void _ControlFocused( Control control ) {
			if ( control is OptionButton optionButton ) {
				_Instrument.Call( optionButton.GetPopup() );
			} else if ( control is ColorPickerButton colorPicker ) {
				_Instrument.Call( colorPicker.GetPopup() );
			} else if ( control is MenuButton menuButton ) {
				_Instrument.Call( menuButton.GetPopup() );
			} else if ( control is TabContainer container ) {
				_Instrument.Call( container.GetPopup() );
			}
		}
	}
};