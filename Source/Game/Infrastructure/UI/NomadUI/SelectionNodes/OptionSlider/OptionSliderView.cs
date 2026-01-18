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

using Game.Infrastructure.UI.NomadUI.SelectionNodes.Interfaces;
using Game.Infrastructure.UI.NomadUI.SelectionNodes.OptionNode;
using Godot;
using Nomad.Core.Util;

namespace Game.Infrastructure.UI.NomadUI.SelectionNodes.OptionSlider {
	/*
	===================================================================================
	
	OptionSliderView
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	public sealed class OptionSliderView : OptionNodeView<OptionSlider>, IOptionSliderView {
		public InternString SliderId => _owner.SliderId;

		public HSlider Slider => _input;
		public float Value => (float)_input.Value;

		private readonly HSlider _input;
		private readonly Godot.Label _valueLabel;

		/*
		===============
		OptionSliderView
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public OptionSliderView( OptionSlider owner, float min, float max )
			: base( owner )
		{
			_input = _owner.GetNode<HSlider>( "Input" );
			_input.MinValue = min;
			_input.MaxValue = max;
			_input.Connect( HSlider.SignalName.ValueChanged, Callable.From<float>( OnValueChanged ) );

			_valueLabel = _input.GetNode<Godot.Label>( "Value" );
		}

		/*
		===============
		SetValue
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public void SetValue( float value ) {
			_input.Value = value;
		}

		/*
		===============
		OnValueChanged
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		private void OnValueChanged( float value ) {
			_valueLabel.Text = value.ToString();
		}
	};
};