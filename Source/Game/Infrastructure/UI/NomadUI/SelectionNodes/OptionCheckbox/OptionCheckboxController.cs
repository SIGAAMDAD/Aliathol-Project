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
using Game.Domain.UserInterface.Events;
using Game.Infrastructure.UI.NomadUI.SelectionNodes.Interfaces;
using Game.Infrastructure.UI.NomadUI.SelectionNodes.OptionNode;
using Nomad.Core.Events;
using Nomad.Core.Util;

namespace Game.Infrastructure.UI.NomadUI.SelectionNodes.OptionCheckbox {
	/*
	===================================================================================
	
	OptionCheckboxController
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public class OptionCheckboxController : OptionNodeController<IOptionCheckboxView>, IOptionCheckboxController {
		public InternString CheckboxId => _view.CheckboxId;

		public bool Value {
			get => _value;
			set {
				_value = value;
				_view.SetValue( value );
			}
		}
		private bool _value;

		private readonly UIEventHelper _helper;

		/*
		===============
		OptionCheckboxController
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventBus"></param>
		/// <param name="eventFactory"></param>
		/// <param name="view"></param>
		public OptionCheckboxController( UIEventHelper helper, IGodotEventBusService eventBus, OptionCheckboxView view )
			: base( eventBus, view )
		{
			_view.SetValue( false );
			_helper = helper;

			eventBus.ConnectSignal( view.Left, Godot.Button.SignalName.Pressed, view.Left, OnToggled );
			eventBus.ConnectSignal( view.Right, Godot.Button.SignalName.Pressed, view.Right, OnToggled );
		}

		/*
		===============
		OnToggled
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		private void OnToggled() {
			_value = !_value;
			_helper.OptionCheckboxToggle.Publish( new OptionCheckboxToggleEventArgs( _view.CheckboxId, _value ) );
		}
	};
};