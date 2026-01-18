using Game.Presentation.Screens.LoadingScreen;
using Godot;

namespace Game.Infrastructure.Systems {
	/*
	===================================================================================
	
	SceneManager
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public partial class SceneManager {
		private readonly SceneTree _sceneTree;
		private readonly LoadingScreen _loadingScreen;

		/*
		===============
		SceneTree
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sceneTree"></param>
		public SceneManager( SceneTree sceneTree ) {
			_sceneTree = sceneTree;
			_loadingScreen = sceneTree.Root.GetNode<LoadingScreen>( "/root/LoadingScreen" );

			GameStateManager.GameStateChanged.Subscribe( this, OnStateChanged );
		}

		/*
		===============
		OnStateChanged
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		private void OnStateChanged( in GameStateChangedEventArgs args ) {
			if ( args.NewState == GameState.Level && args.OldState == GameState.TitleScreen ) {
				_loadingScreen.Show();
				_sceneTree.ChangeSceneToFile( "res://Assets/Prefabs/Regions/World.tscn" );
			} else if ( args.NewState == GameState.TitleScreen ) {
				_loadingScreen.Show();
				_sceneTree.ChangeSceneToFile( "res://Source/Game/Presentation/Screens/MainMenu/MainMenu.tscn" );
			}
		}
	};
};