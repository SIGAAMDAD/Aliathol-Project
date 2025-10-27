using EventSystem;
using Godot;
using System;

namespace Menus.SelectionNodes {
	/*
	===================================================================================
	
	UIEvent
	
	===================================================================================
	*/
	/// <summary>
	/// Inherits from <see cref="GameEvent"/> to make UI node based events slightly less boilerplatey.
	/// Shouldn't be a static event, this should be instantiated per unique ui element
	/// </summary>

	public sealed class UIEvent : GameEvent {
		public readonly Control Node;

		public UIEvent( Control node, string? name )
			: base( name )
		{
			ArgumentNullException.ThrowIfNull( node );

			Node = node;
		}
	};
};