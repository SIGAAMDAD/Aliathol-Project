using Godot;

namespace Game.Presentation.Screens.LoadingScreen {
	/*
	===================================================================================
	
	LoadingScreen
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public sealed partial class LoadingScreen : CanvasLayer {
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

			GetTree().Connect( SceneTree.SignalName.SceneChanged, Callable.From( OnCheckShow ) );
			Connect( SignalName.VisibilityChanged, Callable.From( OnVisibilityChanged ) );
		}

		/*
		===============
		OnVisibilityChanged
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		private void OnVisibilityChanged() {
			ProcessMode = Visible ? ProcessModeEnum.Always : ProcessModeEnum.Disabled;
		}

		/*
		===============
		OnCheckShow
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		private void OnCheckShow() {
			Hide();
		}
	};
};