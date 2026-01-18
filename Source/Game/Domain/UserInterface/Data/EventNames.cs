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

using Game.Application.UI;

namespace Game.Domain.UserInterface.Data {
	/*
	===================================================================================
	
	EventNames
	
	===================================================================================
	*/
	/// <summary>
	/// String constants for UI related events.
	/// </summary>

	public static class EventNames {
		public const string NAMESPACE = nameof( UIEventHelper );

		public const string BUTTON_CLICKED_EVENT = $"{NAMESPACE}:{nameof( UIEventHelper.ButtonClicked )}";
		public const string BUTTON_FOCUSED_EVENT = $"{NAMESPACE}:{nameof( UIEventHelper.ButtonFocused )}";
		public const string BUTTON_UNFOCUSED_EVENT = $"{NAMESPACE}:{nameof( UIEventHelper.ButtonUnfocused )}";

		public const string OPTION_CHECKBOX_TOGGLED_EVENT = $"{NAMESPACE}:{nameof( UIEventHelper.OptionCheckboxToggle )}";
		public const string OPTION_SLIDER_TOGGLED_LEFT_EVENT = $"{NAMESPACE}:{nameof( UIEventHelper.OptionSliderToggleLeft )}";
		public const string OPTION_SLIDER_TOGGLED_RIGHT_EVENT = $"{NAMESPACE}:{nameof( UIEventHelper.OptionSliderToggleRight )}";
		public const string OPTION_SLIDER_VALUE_CHANGED_EVENT = $"{NAMESPACE}:{nameof( UIEventHelper.OptionSliderValueChanged )}";
		public const string OPTION_LIST_VALUE_SET_EVENT = $"{NAMESPACE}:{nameof( UIEventHelper.OptionListValueSet )}";
		public const string OPTION_LIST_FOCUSED_EVENT = $"{NAMESPACE}:{nameof( UIEventHelper.OptionListFocused )}";

		public const string MUGSHOT_FOCUSED_EVENT = $"{NAMESPACE}:{nameof( UIEventHelper.OptionSliderValueChanged )}";

		public const string MENU_OPENED_EVENT = $"{NAMESPACE}:{nameof( UIEventHelper.MenuOpened )}";

		public const string MENU_STATE_CHANGED_EVENT = "UIEvents:MenuStateChanged";

		public const string MENU_TRANSITION_REQUESTED_EVENT = $"{NAMESPACE}:MenuTransitionRequested";
		public const string MENU_TRANSITION_COMPLETED_EVENT = $"{NAMESPACE}:MenuTransitionCompleted";

		public const string SAVE_SLOT_SELECTED_EVENT = $"{NAMESPACE}:SaveSlotSelected";
	};
};