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
using System;
using System.Collections.Immutable;

namespace Game.Infrastructure.UI.NomadUI.SelectionNodes.OptionList {
	/*
	===================================================================================
	
	OptionListController
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	public class OptionListController : OptionNodeController<IOptionListView>, IOptionListController {
		private static readonly NodePath LEFT_BUTTON_NODEPATH = "LeftIcon";
		private static readonly NodePath RIGHT_BUTTON_NODEPATH = "RightIcon";

		public InternString ListId => _view.ListId;

		public int Value => _value;
		private int _value;

		private ImmutableArray<string> _items;

		private readonly UIEventHelper _helper;

		/*
		===============
		OptionListController
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventBus"></param>
		/// <param name="view"></param>
		public OptionListController( UIEventHelper helper, IGodotEventBusService eventBus, OptionListView view )
			: base( eventBus, view )
		{
			_helper = helper;
			eventBus.ConnectSignal( view.Owner.GetNode<Button>( LEFT_BUTTON_NODEPATH ), Button.SignalName.Pressed, view.Owner, OnPrevToggle );
			eventBus.ConnectSignal( view.Owner.GetNode<Button>( RIGHT_BUTTON_NODEPATH ), Button.SignalName.Pressed, view.Owner, OnNextToggle );
		}

		/*
		===============
		SetOptions
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="items"></param>
		public void SetOptions( ImmutableArray<string> items ) {
			_items = items;

			// we don't know how many elements we have so just reset
			SetValue( 0 );
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
		/// <exception cref="InvalidOperationException"></exception>
		public void SetValue( int value ) {
			if ( _items == null ) {
				throw new InvalidOperationException();
			}
			_value = value;
			_view.SetOption( _items[ value ] );
		}

		/*
		===============
		OnPrevToggle
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		private void OnPrevToggle() {
			if ( _items == null ) {
				return;
			}
			_value--;
			if ( _value < 0 ) {
				_value = _items.Length - 1;
			}
			_view.SetOption( _items[ _value ] );

			_helper.OptionListValueSet.Publish( new OptionListValueSetEventArgs( _view.ListId, _value ) );
		}

		/*
		===============
		OnNextToggle
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		private void OnNextToggle() {
			if ( _items == null ) {
				return;
			}
			_value++;
			if ( _value >= _items.Length ) {
				_value = 0;
			}
			_view.SetOption( _items[ _value ] );

			_helper.OptionListValueSet.Publish( new OptionListValueSetEventArgs( _view.ListId, _value ) );
		}
	};
};