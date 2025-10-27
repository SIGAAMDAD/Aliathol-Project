using EventSystem;
using Godot;
using Menus.SelectionNodes;
using SaveSystem;

namespace Menus {
	/*
	===================================================================================

	SaveSlotButton

	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	public sealed partial class SaveSlotButton : HBoxContainer {
		[Export]
		public int SlotIndex { get; private set; }

		private SelectionNodes.Button SelectButton;
		private Godot.Button DeleteButton;

		public readonly UIEvent Selected;
		public readonly UIEvent Highlighted;

		/*
		===============
		SaveSlotButton
		===============
		*/
		public SaveSlotButton() {
			Selected = new UIEvent( this, nameof( Selected ) );
			Highlighted = new UIEvent( this, nameof( Highlighted  ) );
		}

		/*
		===============
		OnSelectButtonPressed
		===============
		*/
		private void OnSelectButtonPressed( in IGameEvent eventData, in IEventArgs args ) {
			Selected.Publish( GameEvent.EmptyArgs );
		}

		/*
		===============
		OnDeleteButtonPressed
		===============
		*/
		private void OnDeleteButtonPressed() {
			SaveManager.DeleteSlot( SlotIndex );
			Update();
		}

		/*
		===============
		DateToString
		===============
		*/
		private string DateToString( SaveSystem.SlotMetadata info ) {
			return $"{info.AccessedYear}, {info.AccessedMonth} {info.AccessedDay}";
		}

		/*
		===============
		OnFocusEntered
		===============
		*/
		private void OnFocusEntered() {
			SelectButton.GrabFocus();
			Highlighted.Publish( GameEvent.EmptyArgs );
		}

		/*
		===============
		Update
		===============
		*/
		public void Update() {
			SlotMetadata info = SaveManager.GetSlotInfo( SlotIndex );
			string text = $"DATA {SlotIndex} ";

			if ( info.Exists ) {
				text += DateToString( info );
				DeleteButton.Text = "ERASE";
			} else {
				DeleteButton.Text = "[EMPTY]";
			}

			SelectButton.Text = text;
		}

		/*
		===============
		_Ready
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public override void _Ready() {
			SelectButton = GetNode<SelectionNodes.Button>( "SelectButton" );
			DeleteButton = GetNode<Godot.Button>( "DeleteButton" );

			GameEventBus.ConnectSignal( DeleteButton, Godot.Button.SignalName.Pressed, this, OnDeleteButtonPressed );
			SelectButton.Activated.Subscribe( this, OnSelectButtonPressed );

			Update();

			GameEventBus.ConnectSignal( this, SignalName.FocusEntered, this, OnFocusEntered );
		}
	};
};