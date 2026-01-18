using Game.Infrastructure.UI.NomadUI.SelectionNodes.NomadButton;
using Godot;

namespace Game.Presentation.Screens.SaveSlotsMenu {
	/*
	===================================================================================
	
	SaveSlotsMenuView
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public readonly struct SaveSlotsMenuView( SaveSlotsMenu owner ) {
		public SaveSlotsMenu Owner => owner;

		public NomadButtonView BackButton => owner.GetNode<NomadButtonNode>( "%BackButton" ).View;
		public NomadButtonView LoadButton => owner.GetNode<NomadButtonNode>( "%LoadButton" ).View;

		public VBoxContainer SaveSlotsList => owner.GetNode<VBoxContainer>( "%SlotsList" );
	};
};