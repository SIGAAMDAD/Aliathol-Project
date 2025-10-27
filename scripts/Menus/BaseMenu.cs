using EventSystem;
using Godot;
using Menus.SelectionNodes;
using System.Runtime.InteropServices;

namespace Menus {
	/*
	===================================================================================
	
	BaseMenu
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public partial class BaseMenu : Control {
		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
		public readonly struct MenuStateChangedEventData : IEventArgs {
			public readonly State State;
			public MenuStateChangedEventData( State state ) {
				State = state;
			}
		};

		public readonly GameEvent SetMenuState = new GameEvent( nameof( SetMenuState ) );
	};
};