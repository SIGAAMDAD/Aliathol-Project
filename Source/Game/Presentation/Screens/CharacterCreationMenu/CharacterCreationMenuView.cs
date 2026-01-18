using Game.Infrastructure.UI.NomadUI.SelectionNodes.NomadButton;
using Game.Infrastructure.UI.NomadUI.SelectionNodes.OptionList;
using Game.Presentation.Screens.CharacterCreationMenu.MugshotList;
using Godot;

namespace Game.Presentation.Screens.CharacterCreationMenu {
	/*
	===================================================================================
	
	CharacterCreationMenuView
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal record CharacterCreationMenuView( Control owner ) {
		public Control Owner => owner;
		
		public NomadButtonView BackToMainMenuButton => _backToMainMenuButton;
		private readonly NomadButtonView _backToMainMenuButton = owner.GetNode<NomadButtonNode>( "%BackToMainMenuButton" ).View;

		public NomadButtonView FinishedButton => _finishedButton;
		private readonly NomadButtonView _finishedButton = owner.GetNode<NomadButtonNode>( "%FinishedButton" ).View;

		public MugshotListView ClassList => _classList;
		private readonly MugshotListView _classList = owner.GetNode<MugshotList.MugshotList>( "%ClassList" ).View;

		public MugshotListView RaceList => _raceList;
		private readonly MugshotListView _raceList = owner.GetNode<MugshotList.MugshotList>( "%RaceList" ).View;

		public MugshotListView OriginList => _originList;
		private readonly MugshotListView _originList = owner.GetNode<MugshotList.MugshotList>( "%OriginList" ).View;

		public OptionListView AgeList => _ageList;
		private readonly OptionListView _ageList = owner.GetNode<OptionList>( "%AgeList" ).View;

		public OptionListView GenderList => _genderList;
		private readonly OptionListView _genderList = owner.GetNode<OptionList>( "%GenderList" ).View;

		public OptionListView SexualityList => _sexualityList;
		private readonly OptionListView _sexualityList = owner.GetNode<OptionList>( "%SexualityList" ).View;

		public VBoxContainer AestheticContainer => _aestheticContainer;
		private readonly VBoxContainer _aestheticContainer = owner.GetNode<VBoxContainer>( "%AestheticOptions" );
	};
};