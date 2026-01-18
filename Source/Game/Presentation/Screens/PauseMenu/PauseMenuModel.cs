using Game.Infrastructure.Systems;

namespace Game.Presentation.Screens.PauseMenu {
	/*
	===================================================================================
	
	PauseMenuModel
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public sealed class PauseMenuModel {
		public bool IsPaused => _stateManager.GameState == GameState.Paused;

		private GameState _prevState;
		private readonly GameStateManager _stateManager;

		/*
		===============
		PauseMenuModel
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public PauseMenuModel( GameStateManager gameState ) {
			_stateManager = gameState;
		}

		/*
		===============
		TogglePause
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public void TogglePause() {
			if ( _stateManager.GameState == GameState.Level ) {
				SetState( GameState.Paused );
			} else if ( _stateManager.GameState == GameState.Paused ) {
				SetState( GameState.Level );
			}
		}

		/*
		===============
		SetState
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="gameState"></param>
		public void SetState( GameState gameState ) {
			_stateManager.SetGameState( gameState );
		}

		/*
		===============
		OpenSettingsMenu
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public void OpenSettingsMenu() {
		}
	};
};