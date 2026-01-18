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
using Game.Infrastructure.Audio;
using Nomad.Audio.Interfaces;

namespace Game.Application.UI.EventHandlers {
	/*
	===================================================================================
	
	AudioUIEventHandler
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public sealed class AudioUIEventHandler : IUIEventHandler {
		private readonly IAudioEmitter _emitter;

		/*
		===============
		AudioUIEventHandler
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="emitterFactory"></param>
		/// <param name="eventRegistry"></param>
		public AudioUIEventHandler( IEmitterFactory emitterFactory, UIEventHelper helper ) {
			_emitter = emitterFactory.CreateEmitter( "SoundCategory:UI" );

			helper.ButtonClicked.Subscribe( this, OnButtonClicked );
			helper.ButtonFocused.Subscribe( this, OnButtonFocused );
			helper.OptionListFocused.Subscribe( this, OnListFocused );
			helper.OptionListValueSet.Subscribe( this, OnListValueSet );
		}

		/*
		===============
		OnListValueSet
		===============
		*/
		private void OnListValueSet( in OptionListValueSetEventArgs args ) {
			_emitter.PlaySound( AudioConstants.BUTTON_PRESSED );
		}

		/*
		===============
		OnButtonFocused
		===============
		*/
		private void OnListFocused( in OptionListFocusedEventArgs args ) {
			_emitter.PlaySound( AudioConstants.BUTTON_FOCUSED );
		}

		/*
		===============
		OnButtonClicked
		===============
		*/
		private void OnButtonClicked( in ButtonClickedEventArgs args ) {
			_emitter.PlaySound( AudioConstants.BUTTON_PRESSED );
		}

		/*
		===============
		OnButtonFocused
		===============
		*/
		private void OnButtonFocused( in ButtonFocusedEventArgs args ) {
			_emitter.PlaySound( AudioConstants.BUTTON_FOCUSED );
		}
	};
};