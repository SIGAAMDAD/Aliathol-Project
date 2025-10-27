using EventSystem;
using Godot;
using GodotPlugins.Game;
using System;

namespace Menus {
	/*
	===================================================================================

	MenuSystem

	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	public sealed partial class MenuSystem : Control {
		private TitleMenu TitleMenu;
		private MainMenu MainMenu;
		private SaveSlotsMenu SaveSlotsMenu;

		private void EnableMenu( Control menu ) {
			menu.SetDeferred( Control.PropertyName.ProcessMode, (ulong)ProcessModeEnum.Always );
			menu.CallDeferred( Control.MethodName.Show );
		}

		private void DisableMenu( Control menu ) {
			menu.SetDeferred( Control.PropertyName.ProcessMode, (ulong)ProcessModeEnum.Disabled );
			menu.CallDeferred( Control.MethodName.Hide );
		}

		/*
		===============
		OnMenuStateChanged
		===============
		*/
		private void OnMenuStateChanged( in IGameEvent eventData, in IEventArgs args ) {
			if ( args is BaseMenu.MenuStateChangedEventData stateChanged ) {
				switch ( stateChanged.State ) {
					case State.Title:
						EnableMenu( TitleMenu );
						DisableMenu( SaveSlotsMenu );
						DisableMenu( MainMenu );
						break;
					case State.Main:
						DisableMenu( TitleMenu );
						DisableMenu( SaveSlotsMenu );
						EnableMenu( MainMenu );
						break;
					case State.SaveSlots:
						DisableMenu( TitleMenu );
						EnableMenu( SaveSlotsMenu );
						DisableMenu( MainMenu );
						break;
				}
			} else {
				throw new InvalidCastException( nameof( args ) );
			}
		}

		/*
		===============
		_Ready
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public override void _Ready() {
			base._Ready();

			TitleMenu = GetNode<TitleMenu>( "TitleScreen" );
			TitleMenu.SetMenuState.Subscribe( this, OnMenuStateChanged );

			MainMenu = GetNode<MainMenu>( "MainMenu" );
			MainMenu.SetMenuState.Subscribe( this, OnMenuStateChanged );

			SaveSlotsMenu = GetNode<SaveSlotsMenu>( "SaveSlotsMenu" );
			SaveSlotsMenu.SetMenuState.Subscribe( this, OnMenuStateChanged );
		}
	};
};