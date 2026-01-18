using Game.Infrastructure;
using Godot;

namespace Game.Presentation.Screens.CharacterCreationMenu {
	/*
	===================================================================================
	
	CharacterCreationMenu
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public partial class CharacterCreationMenu : Control {
		private CharacterCreationMenuView _view;
		private CharacterCreationMenuController _controller;

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

			var bootstrapper = GetNode<NomadBootstrapper>( "/root/NomadBootstrapper" );

			_view = new CharacterCreationMenuView( this );
			_controller = new CharacterCreationMenuController(
				bootstrapper.ServiceLocator,
				_view
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

			_controller.Dispose();
		}
	};
};