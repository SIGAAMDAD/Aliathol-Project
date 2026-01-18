using Game.Infrastructure;
using Godot;

namespace Game.Presentation.Screens.SaveSlotsMenu {
	/*
	===================================================================================
	
	SaveSlotsMenu
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public partial class SaveSlotsMenu : Control {
		private SaveSlotsMenuView _view;
		private SaveSlotsMenuController _controller;

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

			var serviceLocator = GetNode<NomadBootstrapper>( "/root/NomadBootstrapper" ).ServiceLocator;

			_view = new SaveSlotsMenuView( this );
			_controller = new SaveSlotsMenuController( serviceLocator, _view );
		}
	};
};
