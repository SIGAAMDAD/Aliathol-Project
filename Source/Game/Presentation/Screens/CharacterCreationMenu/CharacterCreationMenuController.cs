using Game.Application.Character;
using Game.Application.Character.Interfaces;
using Game.Application.Character.Services;
using Game.Application.UI;
using Game.Application.UI.Menus;
using Game.Domain.Character.Events;
using Game.Domain.UserInterface.CharacterCreation.Events;
using Game.Domain.UserInterface.Events;
using Game.Domain.UserInterface.State;
using Game.Infrastructure.Caching;
using Game.Infrastructure.Character;
using Game.Infrastructure.Systems;
using Game.Infrastructure.UI.NomadUI.SelectionNodes.OptionList;
using Game.Presentation.Screens.CharacterCreationMenu.MugshotList;
using Godot;
using Nomad.Core.Events;
using Nomad.Core.Logger;
using Nomad.Core.Memory;
using Nomad.Core.ServiceRegistry.Interfaces;
using Nomad.Core.Util;
using Nomad.Events;
using System;
using System.Collections.Generic;

namespace Game.Presentation.Screens.CharacterCreationMenu {
	/*
	===================================================================================
	
	CharacterCreationMenuController

	FIXME: this does too much
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	internal sealed class CharacterCreationMenuController : IDisposable {
		private readonly MugshotListController _raceListController;
		private readonly MugshotListController _classListController;
		private readonly MugshotListController _originListController;

		private readonly CharacterCreationMenuView _view;
		private readonly ICharacterDataProvider _characterDataProvider;
		private readonly CharacterDataService _characterDataService;
		private readonly MugshotService _mugshotService;

		private readonly ILoggerService _logger;
		private readonly MenuStateMachine<CharacterCreationMenuState> _stateManager;
		private readonly UIEventHelper _helper;

		private MugshotListView _currentView;

		private InternString _menuId => StringPool.Intern( nameof( CharacterCreationMenu ) );

		private readonly DisposableSubscription<MugshotListItemFocusedEventArgs> _mugshotFocusedEvent;
		private readonly DisposableSubscription<ButtonClickedEventArgs> _buttonClickedEvent;
		private readonly DisposableSubscription<MenuStateChangedEventArgs<CharacterCreationMenuState>> _menuStateChangedEvent;

		private readonly DisposableSubscription<CharacterRaceSelectedEventArgs> _raceSelectedEvent;
		private readonly DisposableSubscription<CharacterClassSelectedEventArgs> _classSelectedEvent;
		private readonly DisposableSubscription<CharacterOriginSelectedEventArgs> _originSelectedEvent;

		/*
		===============
		CharacterCreationMenuController
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="locator"></param>
		/// <param name="view"></param>
		public CharacterCreationMenuController( IServiceLocator locator, in CharacterCreationMenuView view ) {
			_view = view;

			var eventFactory = locator.GetService<IGameEventRegistryService>();
			_logger = locator.GetService<ILoggerService>();

			_stateManager = new MenuStateMachine<CharacterCreationMenuState>( _menuId, CharacterCreationMenuState.Race, eventFactory,
				new Dictionary<CharacterCreationMenuState, Control?> {
					[ CharacterCreationMenuState.Race ] = _view.RaceList.Owner,
					[ CharacterCreationMenuState.Class ] = _view.ClassList.Owner,
					[ CharacterCreationMenuState.Origin ] = _view.OriginList.Owner,
					[ CharacterCreationMenuState.Aesthetic ] = _view.AestheticContainer,
					[ CharacterCreationMenuState.Count ] = null
				}
			);

			_helper = locator.GetService<UIEventHelper>();

			var eventBus = locator.GetService<IGodotEventBusService>();

			_characterDataProvider = new CharacterDataProvider();
			_characterDataService = new CharacterDataService( eventFactory, _characterDataProvider );
			_raceListController = new MugshotListController( view.RaceList );
			_classListController = new MugshotListController( view.ClassList );
			_originListController = new MugshotListController( view.OriginList );

			var ageListController = new OptionListController( _helper, eventBus, view.AgeList );
			ageListController.SetOptions( _characterDataService.AgeStrings );

			var genderListController = new OptionListController( _helper, eventBus, view.GenderList );
			genderListController.SetOptions( _characterDataService.GenderStrings );

			var sexualityListController = new OptionListController( _helper, eventBus, view.SexualityList );
			sexualityListController.SetOptions( _characterDataService.SexualityStrings );

			_currentView = _view.RaceList;

			_mugshotService = new MugshotService( TextureCache.Instance );
			var task = _characterDataProvider.LoadAll( _logger );

			_buttonClickedEvent = new DisposableSubscription<ButtonClickedEventArgs>( _helper.ButtonClicked, OnButtonClicked );
			_mugshotFocusedEvent = new DisposableSubscription<MugshotListItemFocusedEventArgs>( eventFactory.GetEvent<MugshotListItemFocusedEventArgs>( nameof( CharacterCreationMenu ), "MugshotFocusedEvent" ), OnImageListItemFocused );
			_menuStateChangedEvent = new DisposableSubscription<MenuStateChangedEventArgs<CharacterCreationMenuState>>( _stateManager.StateChanged, OnMenuStateChanged );

			_originSelectedEvent = new DisposableSubscription<CharacterOriginSelectedEventArgs>( _characterDataService.OriginSelected, OnOriginSelected );
			_classSelectedEvent = new DisposableSubscription<CharacterClassSelectedEventArgs>( _characterDataService.ClassSelected, OnClassSelected );
			_raceSelectedEvent = new DisposableSubscription<CharacterRaceSelectedEventArgs>( _characterDataService.RaceSelected, OnRaceSelected );

			task.Wait();

			GetOrigins();
			GetClasses();
			GetRaces();
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
			_buttonClickedEvent.Dispose();
			_mugshotFocusedEvent.Dispose();
			_menuStateChangedEvent.Dispose();

			_raceSelectedEvent.Dispose();
			_classSelectedEvent.Dispose();
			_originSelectedEvent.Dispose();
		}

		/*
		===============
		GetRaces
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		private void GetRaces() {
			_raceListController.SetOptions( _mugshotService.ConvertRaceList( _characterDataProvider ) );
			OnRaceSelected( new CharacterRaceSelectedEventArgs( _characterDataProvider.AvailableRaces[ 0 ].Id ) );
		}

		/*
		===============
		GetClasses
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		private void GetClasses() {
			_classListController.SetOptions( _mugshotService.ConvertClassList( _characterDataProvider ) );
			OnClassSelected( new CharacterClassSelectedEventArgs( _characterDataProvider.AvailableClasses[ 0 ].Id ) );
		}

		/*
		===============
		GetOrigins
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		private void GetOrigins() {
			_originListController.SetOptions( _mugshotService.ConvertOriginList( _characterDataProvider ) );
			OnOriginSelected( new CharacterOriginSelectedEventArgs( _characterDataProvider.AvailableOrigins[ 0 ].Id ) );
		}

		/*
		===============
		SetDescriptionLabel
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="description"></param>
		private void SetDescriptionLabel( int name, int description ) {
			// FIXME: replace with translation
			_currentView.SetName( new InternString( name ) );
			_currentView.SetDescription( new InternString( description ) );
		}

		/*
		===============
		OnClassSelected
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		private void OnClassSelected( in CharacterClassSelectedEventArgs args ) {
			var selected = _characterDataProvider.GetClassById( args.Id );
			SetDescriptionLabel( selected.Name, selected.Description );
		}

		/*
		===============
		OnRaceSelected
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		private void OnRaceSelected( in CharacterRaceSelectedEventArgs args ) {
			var selected = _characterDataProvider.GetRaceById( args.Id );
			SetDescriptionLabel( selected.Name, selected.Description );
		}

		/*
		===============
		OnOriginSelected
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		private void OnOriginSelected( in CharacterOriginSelectedEventArgs args ) {
			var selected = _characterDataProvider.GetOriginById( args.Id );
			SetDescriptionLabel( selected.Name, selected.Description );
		}

		/*
		===============
		OnMenuStateChanged
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		private void OnMenuStateChanged( in MenuStateChangedEventArgs<CharacterCreationMenuState> args ) {
			if ( args.MenuId != _menuId ) {
				return;
			}

			if ( args.CurrentState == CharacterCreationMenuState.Count ) {
				GameStateManager.Instance.SetGameState( GameState.Level );
//				_helper.MenuTransitionRequested.Publish( new MenuTransitionRequestedEventArgs( MenuState.CharacterCreation, MenuState.Loading ) );
			} else if ( args.CurrentState == CharacterCreationMenuState.Count - 1 ) {
				_view.FinishedButton.SetText( "START GAME!" );
			} else if ( args.CurrentState == CharacterCreationMenuState.Race ) {
				_currentView = _view.RaceList;
			} else if ( args.CurrentState == CharacterCreationMenuState.Class ) {
				_currentView = _view.ClassList;
			} else if ( args.CurrentState == CharacterCreationMenuState.Origin ) {
				_currentView = _view.OriginList;
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
		/// <returns></returns>
		private void OnButtonClicked( in ButtonClickedEventArgs args ) {
			if ( args.ButtonId == _view.BackToMainMenuButton.ButtonId ) {
				_helper.MenuTransitionRequested.Publish( new MenuTransitionRequestedEventArgs( MenuState.CharacterCreation, MenuState.Main ) );
			} else if ( args.ButtonId == _view.FinishedButton.ButtonId ) {
				_stateManager.SetState( _stateManager.CurrentState + 1 );
			}
		}

		/*
		===============
		OnListFocused
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		private void OnImageListItemFocused( in MugshotListItemFocusedEventArgs args ) {
			if ( args.ListId == StatConstants.Class ) {
				_characterDataService.ClassSelected.Publish( new CharacterClassSelectedEventArgs( args.ItemId ) );
			} else if ( args.ListId == StatConstants.Race ) {
				_characterDataService.RaceSelected.Publish( new CharacterRaceSelectedEventArgs( args.ItemId ) );
			} else if ( args.ListId == StatConstants.Origin ) {
				_characterDataService.OriginSelected.Publish( new CharacterOriginSelectedEventArgs( args.ItemId ) );
			}
		}
	};
};