using Godot;

namespace Game.Infrastructure.Character.GodotResource {
	/*
	===================================================================================
	
	ClassResource
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public sealed partial class ClassResource : Resource {
		[Export]
		public StringName Name { get; private set; }
		[Export]
		public StringName Description { get; private set; }
		[Export]
		public Texture2D Mugshot { get; private set; }
	};
};