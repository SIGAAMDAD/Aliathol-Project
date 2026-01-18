using Game.Application.Configuration.Enums;
using Godot;

namespace Game.Presentation.UserInterface.HeadsUpDisplay {
	/*
	===================================================================================
	
	LocationLabel
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public sealed class LocationLabel : HUDComponent {
		public override bool Visible => throw new System.NotImplementedException();
		private bool _visible;

		public override Color Modulate => throw new System.NotImplementedException();

		public override HUDPreset Visibility => throw new System.NotImplementedException();

		public override float FadeTime => throw new System.NotImplementedException();

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public override void Dispose() {
		}
	};
};