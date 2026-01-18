using Game.Application.Character.Interfaces;
using Game.Application.Character.Services;

namespace Game.Presentation.Screens.CharacterCreationMenu {
	/*
	===================================================================================
	
	CharacterCreationMenuPresenter
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal sealed class CharacterCreationMenuPresenter {
		private readonly ICharacterDataProvider _dataProvider;
		private readonly CharacterDataService _dataService;

		/*
		===============
		CharacterCreationMenuPresenter
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dataProvider"></param>
		/// <param name="dataService"></param>
		public CharacterCreationMenuPresenter( ICharacterDataProvider dataProvider, CharacterDataService dataService ) {
			_dataProvider = dataProvider;
			_dataService = dataService;
		}
	};
};