using Game.Infrastructure.Systems;

namespace Game.Presentation.Screens.PauseMenu {
	/*
	===================================================================================
	
	PauseMenuPresenter
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public sealed class PauseMenuPresenter {
		private readonly PauseMenuView _view;
		private readonly PauseMenuModel _model;

		/*
		===============
		PauseMenuPresenter
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="view"></param>
		public PauseMenuPresenter( PauseMenuView view, PauseMenuModel model ) {
			_view = view;
			_model = model;
		}

		/*
		===============
		OnResumeGame
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public void TogglePause() {
			_model.TogglePause();
			_view.Toggle( _model.IsPaused );
		}

		/*
		===============
		OnExitToMainMenu
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public void OnExitToMainMenu() {
			_view.GetTree().Paused = false;
			_model.SetState( GameState.TitleScreen );
		}

		/*
		===============
		OnQuitGame
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public void OnQuitGame() {
			_view.GetTree().Paused = false;
			_view.GetTree().Quit();
		}
	};
};