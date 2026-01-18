/*
===========================================================================
The Nomad MPL Source Code
Copyright (C) 2025-2026 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/

using Game.Domain.UserInterface.Events;
using Game.Domain.UserInterface.Data;
using Nomad.Core.Events;
using System;

namespace Game.Application.UI {
	/*
	===================================================================================
	
	UIEventHelper
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public sealed class UIEventHelper( IGameEventRegistryService eventFactory ) : IDisposable {
		public IGameEvent<ButtonClickedEventArgs> ButtonClicked => eventFactory.GetEvent<ButtonClickedEventArgs>( EventNames.NAMESPACE, EventNames.BUTTON_CLICKED_EVENT );
		public IGameEvent<ButtonFocusedEventArgs> ButtonFocused => eventFactory.GetEvent<ButtonFocusedEventArgs>( EventNames.NAMESPACE, EventNames.BUTTON_FOCUSED_EVENT );
		public IGameEvent<ButtonUnfocusedEventArgs> ButtonUnfocused => eventFactory.GetEvent<ButtonUnfocusedEventArgs>( EventNames.NAMESPACE, EventNames.BUTTON_UNFOCUSED_EVENT );
		
		public IGameEvent<OptionCheckboxToggleEventArgs> OptionCheckboxToggle => eventFactory.GetEvent<OptionCheckboxToggleEventArgs>( EventNames.NAMESPACE, EventNames.OPTION_CHECKBOX_TOGGLED_EVENT );
		public IGameEvent<OptionListFocusedEventArgs> OptionListFocused => eventFactory.GetEvent<OptionListFocusedEventArgs>( EventNames.NAMESPACE, EventNames.OPTION_LIST_FOCUSED_EVENT );
		public IGameEvent<OptionListValueSetEventArgs> OptionListValueSet => eventFactory.GetEvent<OptionListValueSetEventArgs>( EventNames.NAMESPACE, EventNames.OPTION_LIST_VALUE_SET_EVENT );
		public IGameEvent<OptionSliderValueChangedEventArgs> OptionSliderValueChanged => eventFactory.GetEvent<OptionSliderValueChangedEventArgs>( EventNames.NAMESPACE, EventNames.OPTION_SLIDER_VALUE_CHANGED_EVENT );
		public IGameEvent<OptionSliderValueChangedEventArgs> OptionSliderToggleRight => eventFactory.GetEvent<OptionSliderValueChangedEventArgs>( EventNames.NAMESPACE, EventNames.OPTION_SLIDER_TOGGLED_RIGHT_EVENT );
		public IGameEvent<OptionSliderValueChangedEventArgs> OptionSliderToggleLeft => eventFactory.GetEvent<OptionSliderValueChangedEventArgs>( EventNames.NAMESPACE, EventNames.OPTION_SLIDER_TOGGLED_LEFT_EVENT );

		public IGameEvent<MenuOpenedEventArgs> MenuOpened => eventFactory.GetEvent<MenuOpenedEventArgs>( EventNames.NAMESPACE, EventNames.MENU_OPENED_EVENT );
		public IGameEvent<MenuTransitionCompletedEventArgs> MenuTransitionCompleted => eventFactory.GetEvent<MenuTransitionCompletedEventArgs>( EventNames.NAMESPACE, EventNames.MENU_TRANSITION_COMPLETED_EVENT );
		public IGameEvent<MenuTransitionRequestedEventArgs> MenuTransitionRequested => eventFactory.GetEvent<MenuTransitionRequestedEventArgs>( EventNames.NAMESPACE, EventNames.MENU_TRANSITION_REQUESTED_EVENT );

		public IGameEvent<SaveSlotSelectedEventArgs> SaveSlotSelected => eventFactory.GetEvent<SaveSlotSelectedEventArgs>( EventNames.NAMESPACE, EventNames.SAVE_SLOT_SELECTED_EVENT );

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public void Dispose() {
			eventFactory.ClearEventsInNamespace( EventNames.NAMESPACE );
		}
	};
};