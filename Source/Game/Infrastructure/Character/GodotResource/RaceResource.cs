using Godot;

namespace Game.Infrastructure.Character.GodotResource {
	/*
	===================================================================================
	
	RaceResource
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public sealed partial class RaceResource : Resource {
		[Export]
		public StringName Name { get; private set; }
		[Export]
		public StringName Description { get; private set; }
		[Export]
		public Texture Mugshot { get; private set; }
	};
};