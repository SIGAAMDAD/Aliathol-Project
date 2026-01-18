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
using Game.Domain.UserInterface.State;
using Game.Infrastructure.Caching;
using Godot;
using Nomad.Core.Util;
using Nomad.Events;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Game.Application.UI.Menus {
	/*
	===================================================================================
	
	MenuManager
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	public sealed class MenuManager : IDisposable {
		private MenuState _currentState = MenuState.None;
		private MenuState _previousState = MenuState.None;

		private readonly ImmutableDictionary<MenuState, FilePath> _scenePaths = new Dictionary<MenuState, FilePath>() {
			[ MenuState.Main ] = new( "res://Source/Game/Presentation/Screens/MainMenu/MainMenu.tscn", PathType.Resource ),
			[ MenuState.Loading ] = new( "res://Source/Game/Presentation/Screens/LoadingScreen/LoadingScreen.tscn", PathType.Resource ),
			[ MenuState.Settings ] = new( "res://Source/Game/Presentation/Screens/SettingsMenu/SettingsMenu.tscn", PathType.Resource ),
			[ MenuState.CharacterCreation ] = new( "res://Source/Game/Presentation/Screens/CharacterCreationMenu/CharacterCreationMenu.tscn", PathType.Resource ),
			[ MenuState.SaveSlots ] = new( "res://Source/Game/Presentation/Screens/SaveSlotsMenu/SaveSlotsMenu.tscn", PathType.Resource ),
		}.ToImmutableDictionary();
		private readonly Dictionary<MenuState, Control> _sceneCache = new();
		private readonly MenuStateMachine<MenuState> _menuStateMachine;

		private Control _currentMenuInstance;

		private readonly Node _worldNode;
		private readonly UIEventHelper _helper;

		private readonly DisposableSubscription<MenuTransitionRequestedEventArgs> _menuTransitionRequestedEvent;

		/*
		===============
		MenuManager
		===============
		*/
		/// <summary>
		/// Creates a MenuManager
		/// </summary>
		/// <param name="worldNode"></param>
		/// <param name="helper"></param>
		public MenuManager( Node worldNode, UIEventHelper helper ) {
			_worldNode = worldNode;
			_helper = helper;

			_menuTransitionRequestedEvent = new DisposableSubscription<MenuTransitionRequestedEventArgs>( helper.MenuTransitionRequested, OnMenuTransitionRequested );
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
			foreach ( var menu in _sceneCache ) {
				SceneCache.Instance.Unload( FilePath.FromResourcePath( menu.Value.SceneFilePath ) );
			}
			_currentMenuInstance = null;

			_menuTransitionRequestedEvent.Dispose();
		}

		/*
		===============
		TransitionToMenu
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="newState"></param>
		/// <returns></returns>
		public void TransitionToMenu( MenuState newState ) {
			if ( _currentState == newState ) {
				return;
			}

			_previousState = _currentState;
			if ( _currentMenuInstance != null ) {
				_currentMenuInstance.SetDeferred( Node.PropertyName.ProcessMode, (long)Node.ProcessModeEnum.Disabled );
				_currentMenuInstance.CallDeferred( Control.MethodName.Hide );
			}

			if ( !_sceneCache.TryGetValue( newState, out var menu ) ) {
				SceneCache.Instance.GetCached( _scenePaths[ newState ] ).Get( out var resource );
				menu = resource.Instantiate<Control>();
				_sceneCache[ newState ] = menu;
				_worldNode.CallDeferred( Node.MethodName.AddChild, menu );
			}

			_currentMenuInstance = menu;
			_currentState = newState;
			_currentMenuInstance.SetDeferred( Node.PropertyName.ProcessMode, (long)Node.ProcessModeEnum.Always );
			_currentMenuInstance.CallDeferred( Control.MethodName.Show );

			_helper.MenuTransitionCompleted.Publish( new MenuTransitionCompletedEventArgs( _currentState, _previousState ) );
		}

		/*
		===============
		OnMenuTransitionRequested
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		private void OnMenuTransitionRequested( in MenuTransitionRequestedEventArgs args ) {
			TransitionToMenu( args.ToState );
		}
	};
};