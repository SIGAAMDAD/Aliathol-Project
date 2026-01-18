using Godot;
using Game.Infrastructure.UI.NomadUI.SelectionNodes.NomadButton;
using Game.Infrastructure.UI.NomadUI.SelectionNodes.Interfaces;

namespace Game.Presentation.Screens.MainMenu {
	/*
	===================================================================================

	MainMenuView

	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	internal sealed class MainMenuView( Control owner ) {
		public Control Owner => owner;

		public NomadButtonView NewGameButton => _newGame;
		private readonly NomadButtonView _newGame = owner.GetNode<NomadButtonNode>( "%NewGameButton" ).View;

		public INomadButtonView LoadGameButton => _loadGame;
		private readonly NomadButtonView _loadGame = owner.GetNode<NomadButtonNode>( "%LoadGameButton" ).View;

		public NomadButtonView SettingsButton => _settings;
		private readonly NomadButtonView _settings = owner.GetNode<NomadButtonNode>( "%SettingsButton" ).View;

		public NomadButtonView CreditsButton => _credits;
		private readonly NomadButtonView _credits = owner.GetNode<NomadButtonNode>( "%CreditsButton" ).View;

		public NomadButtonView QuitGameButton => _quitGame;
		private readonly NomadButtonView _quitGame = owner.GetNode<NomadButtonNode>( "%QuitGameButton" ).View;
	};
};