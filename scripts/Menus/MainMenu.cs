using EventSystem;
using Godot;
using SaveSystem;

namespace Menus {
	/*
	===================================================================================

	MainMenu

	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	public sealed partial class MainMenu : BaseMenu {
		/*
		===============
		OnPlayGameButtonPressed
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventData"></param>
		/// <param name="args"></param>
		private void OnPlayGameButtonPressed( in IGameEvent eventData, in IEventArgs args ) {
			Console.PrintLine( "PLAYING GAME" );
			if ( SaveManager.HasProgressInCurrentSlot() ) {
				SaveManager.Load();
			} else {
				SetMenuState.Publish( new MenuStateChangedEventData( State.CharacterCreation ) );
			}
		}

		/*
		===============
		OnQuitButtonPressed
		===============
		*/
		private void OnQuitButtonPressed( in IGameEvent eventData, in IEventArgs args ) {
			GetTree().Quit();
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
			SelectionNodes.Button playGameButton = GetNode<SelectionNodes.Button>( "MainContainer/OptionsContainer/PlayGameButton" );
			playGameButton.Activated.Subscribe( this, OnPlayGameButtonPressed );

			//SelectionNodes.Button creditsButton = GetNode<SelectionNodes.Button>( "CreditsButton" );

			SelectionNodes.Button quitButton = GetNode<SelectionNodes.Button>( "MainContainer/OptionsContainer/QuitGameButton" );
			quitButton.Activated.Subscribe( this, OnQuitButtonPressed );
		}
	};
};