using Godot;

namespace Menus {
	public sealed partial class TitleMenu : BaseMenu {
		/*
		===============
		_UnhandledInput
		===============
		*/
		public override void _UnhandledInput( InputEvent @event ) {
			base._UnhandledInput( @event );

			if ( Input.IsActionJustPressed( "ui_cancel" ) ) {
				GetTree().Quit();
			} else {
				SetMenuState.Publish( new MenuStateChangedEventData( State.SaveSlots ) );
			}
		}
	};
};