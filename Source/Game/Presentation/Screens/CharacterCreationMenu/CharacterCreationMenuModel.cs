namespace Game.Presentation.Screens.CharacterCreationMenu {
	/*
	===================================================================================
	
	CharacterCreationMenuModel
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal sealed class CharacterCreationMenuModel {
		public CharacterCreationMenuState State { get; private set; }

		public CharacterCreationMenuModel( CharacterCreationMenuState state ) {
			State = state;
		}
	};
};