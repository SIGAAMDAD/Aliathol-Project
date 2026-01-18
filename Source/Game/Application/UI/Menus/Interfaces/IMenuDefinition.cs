using System;
using System.Collections.Immutable;

namespace Game.Infrastructure.UI.Menus {
	/*
	===================================================================================
	
	IMenuDefinition
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public interface IMenuDefinition {
		IImmutableDictionary<string, IMenu> MenuStates { get; }
	};
};