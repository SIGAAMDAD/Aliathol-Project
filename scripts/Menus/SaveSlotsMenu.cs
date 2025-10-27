using EventSystem;
using Menus.SelectionNodes;
using SaveSystem;
using System;

namespace Menus {
	/*
	===================================================================================

	SaveSlotsMenu

	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	public sealed partial class SaveSlotsMenu : BaseMenu {
		/*
		===============
		OnSetSaveSlot
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventData"></param>
		/// <param name="args"></param>
		private void OnSetSaveSlot( in IGameEvent eventData, in IEventArgs args ) {
			Console.PrintLine( "Save slot events" );
			if ( eventData is UIEvent slotButton ) {
				SaveManager.SetSlot( ( slotButton.Node as SaveSlotButton ).SlotIndex, "Unnamed" );
				SetMenuState.Publish( new MenuStateChangedEventData( State.Main ) );
			} else {
				throw new InvalidCastException( nameof( eventData ) );
			}
		}

		/*
		===============
		ConnectSaveSlots
		===============
		*/
		private void ConnectSaveSlots() {
			for ( int i = 0; i < SaveManager.MAX_SLOTS; i++ ) {
				SaveSlotButton button = GetNode( "MainContainer/SaveSlotsSelectContainer" ).GetChild( i ) as SaveSlotButton;
				button.Selected.Subscribe( this, OnSetSaveSlot );
			}
		}

		/*
		===============
		OnVisibilityChanged
		===============
		*/
		private void OnVisibilityChanged() {
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
			base._Ready();

			ConnectSaveSlots();
		}
	};
};