using Game.Application.UI;
using Game.Domain.UserInterface.Events;
using Game.Domain.UserInterface.State;
using Game.Presentation.Screens.SaveSlotsMenu.SaveSlotList;
using Godot;
using Nomad.Core.ServiceRegistry.Interfaces;
using Nomad.Events;
using Nomad.Save.Services;
using System;
using System.Collections.Generic;

namespace Game.Presentation.Screens.SaveSlotsMenu {
	/*
	===================================================================================
	
	SaveSlotsMenuController
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal sealed class SaveSlotsMenuController : IDisposable {
		private readonly SaveSlotsMenuView _view;
		private readonly UIEventHelper _helper;

		private readonly List<SaveSlotButton> _buttons = new();
		private int _currentSlot = 0;

		private readonly DisposableSubscription<SaveSlotSelectedEventArgs> _slotSelectedEvent;
		private readonly DisposableSubscription<ButtonClickedEventArgs> _buttonClickedEvent;

		/*
		===============
		SaveSlotsMenuController
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dataProvider"></param>
		/// <param name="view"></param>
		public SaveSlotsMenuController( IServiceLocator locator, SaveSlotsMenuView view ) {
			_view = view;
			_helper = locator.GetService<UIEventHelper>();

			_slotSelectedEvent = new DisposableSubscription<SaveSlotSelectedEventArgs>(
				_helper.SaveSlotSelected,
				OnSaveSlotSelected
			);
			_buttonClickedEvent = new DisposableSubscription<ButtonClickedEventArgs>(
				_helper.ButtonClicked,
				OnButtonClicked
			);

			LoadSaveSlots( locator.GetService<ISaveDataProvider>() );
		}

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public void Dispose() {
			_slotSelectedEvent.Dispose();
			_buttonClickedEvent.Dispose();
		}

		/*
		===============
		LoadSaveSlots
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dataProvider"></param>
		private void LoadSaveSlots( ISaveDataProvider dataProvider ) {
			var containerList = _view.SaveSlotsList;
			var dataList = dataProvider.ListSaveFiles();

			_buttons.EnsureCapacity( dataList.Count );
			for ( int i = 0; i < dataList.Count; i++ ) {
				var button = new Button();
				_buttons.Add( new SaveSlotButton( _helper, button, i, dataList[ i ] ) );
				containerList.AddChild( button );
			}
		}

		/*
		===============
		OnButtonClicked
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		private void OnButtonClicked( in ButtonClickedEventArgs args ) {
			if ( args.ButtonId == _view.BackButton.ButtonId ) {
				_helper.MenuTransitionRequested.Publish( new MenuTransitionRequestedEventArgs( MenuState.SaveSlots, MenuState.Main ) );
			}
		}

		/*
		===============
		OnSaveSlotSelected
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		private void OnSaveSlotSelected( in SaveSlotSelectedEventArgs args ) {
			_currentSlot = args.SlotIndex;
		}
	};
};