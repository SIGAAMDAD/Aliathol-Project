using Game.Application.UI;
using Game.Domain.UserInterface.Events;
using Game.Infrastructure;
using Game.Infrastructure.Systems;
using Game.Infrastructure.UI.NomadUI.SelectionNodes.NomadButton;
using Godot;
using Nomad.Events;

namespace Game.Presentation.Screens.PauseMenu {
	/*
	===================================================================================

	PauseMenu

	===================================================================================
	*/
	/// <summary>
	/// Handles pause menu operations.
	/// </summary>

	public sealed partial class PauseMenuView : CanvasLayer {
		[Export]
		public NomadButtonNode ResumeButton { get; private set; }
		[Export]
		public NomadButtonNode SettingsButton { get; private set; }
		[Export]
		public NomadButtonNode ExitToMainMenuButton { get; private set; }
		[Export]
		public NomadButtonNode QuitGameButton { get; private set; }

		private PauseMenuPresenter _presenter;
		private DisposableSubscription<ButtonClickedEventArgs> _buttonClicked;

		/*
		===============
		Toggle
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="visible"></param>
		public void Toggle( bool visible ) {
			GetTree().Paused = visible;
			Visible = visible;
		}

		/*
		===============
		OnButtonClicked
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		private void OnButtonClicked( in ButtonClickedEventArgs args ) {
			if ( args.ButtonId == ResumeButton.View.ButtonId ) {
				_presenter.TogglePause();
			} else if ( args.ButtonId == ExitToMainMenuButton.View.ButtonId ) {
				_presenter.OnExitToMainMenu();
			} else if ( args.ButtonId == QuitGameButton.View.ButtonId ) {
				_presenter.OnQuitGame();
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

			_presenter = new PauseMenuPresenter( this, new PauseMenuModel( GameStateManager.Instance ) );

			_buttonClicked = new DisposableSubscription<ButtonClickedEventArgs>(
				GetNode<NomadBootstrapper>( "/root/NomadBootstrapper" ).ServiceLocator.GetService<UIEventHelper>().ButtonClicked,
				OnButtonClicked
			);
		}

		/*
		===============
		_ExitTree
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public override void _ExitTree() {
			base._ExitTree();

			_buttonClicked.Dispose();
		}

		/*
		===============
		_UnhandledInput
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="event"></param>
		public override void _UnhandledInput( InputEvent @event ) {
			base._UnhandledInput( @event );

			if ( Input.IsActionJustPressed( "pause" ) ) {
				_presenter.TogglePause();
			}
		}
	};
};