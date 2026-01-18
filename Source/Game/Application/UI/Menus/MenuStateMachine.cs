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

using Game.Domain.UserInterface.Data;
using Game.Domain.UserInterface.Events;
using Godot;
using Nomad.Core.Events;
using Nomad.Core.Util;
using System.Collections.Generic;

namespace Game.Application.UI.Menus {
	/*
	===================================================================================
	
	MenuStateMachine
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	/// <param name="currentMenu"></param>
	/// <param name="states"></param>

	public class MenuStateMachine<TState>( InternString menuId, TState currentMenu, IGameEventRegistryService eventFactory, IReadOnlyDictionary<TState, Control?> states )
		where TState : unmanaged
	{
		public TState CurrentState => currentMenu;
		public InternString OwnerId => menuId;

		private readonly IReadOnlyDictionary<TState, Control?> _states = states;

		public IGameEvent<MenuStateChangedEventArgs<TState>> StateChanged => eventFactory.GetEvent<MenuStateChangedEventArgs<TState>>( EventNames.NAMESPACE, EventNames.MENU_STATE_CHANGED_EVENT );

		/*
		===============
		SetState
		===============
		*/
		/// <summary>
		/// Changes the state to the requested index.
		/// </summary>
		/// <param name="stateId"></param>
		public void SetState( TState stateId ) {
			if ( !_states.TryGetValue( stateId, out Control? newState ) ) {
				// NOTE: this might need... extra verification
				return;
			}

			TState oldMenu = currentMenu;
			_states[ oldMenu ]?.CallDeferred( Control.MethodName.Hide );
			currentMenu = stateId;
			newState?.CallDeferred( Control.MethodName.Show );

			StateChanged.Publish( new MenuStateChangedEventArgs<TState>( menuId, oldMenu, currentMenu ) );
		}
	};
};