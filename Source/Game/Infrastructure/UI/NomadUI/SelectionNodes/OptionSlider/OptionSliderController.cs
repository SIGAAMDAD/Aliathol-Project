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
using Godot;
using Nomad.Core.Events;
using Nomad.Core.Util;

namespace Game.Infrastructure.UI.NomadUI.SelectionNodes.OptionSlider {
	/*
	===================================================================================
	
	OptionSliderController
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public class OptionSliderController : OptionNodeController<IOptionSliderView>, IOptionSliderController {
		public InternString SliderId => _view.SliderId;

		public float Value {
			get => (float)_view.Slider.Value;
			set {
				_view.SetValue( value );
			}
		}

		private readonly UIEventHelper _helper;

		/*
		===============
		OptionSliderController
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventBus"></param>
		/// <param name="view"></param>
		public OptionSliderController( UIEventHelper helper, IGodotEventBusService eventBus, OptionSliderView view )
			: base( eventBus, view )
		{
			_helper = helper;
			eventBus.ConnectSignal( view.Slider, HSlider.SignalName.ValueChanged, view.Slider, Callable.From<float>( OnValueChanged ) );
		}

		/*
		===============
		OnValueChanged
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		private void OnValueChanged( float value ) {
			_helper.OptionSliderValueChanged.Publish( new OptionSliderValueChangedEventArgs( _view.SliderId, (float)_view.Slider.Value ) );
		}
	};
};