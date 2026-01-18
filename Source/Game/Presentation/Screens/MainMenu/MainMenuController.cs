using Game.Application.UI;
using Game.Domain.UserInterface.Events;
using Game.Domain.UserInterface.State;
using Game.Infrastructure;
using Godot;
using Nomad.Audio.Interfaces;
using Nomad.Core.Events;
using Nomad.Core.ServiceRegistry.Interfaces;
using Nomad.Events;
using System;

namespace Game.Presentation.Screens.MainMenu {
	/*
	===================================================================================
	
	MainMenuController
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	internal sealed class MainMenuController : IDisposable {
		private readonly MainMenuView _view;
		private readonly UIEventHelper _helper;

		private readonly DisposableSubscription<ButtonClickedEventArgs> _buttonClickedEvent;

		/*
		===============
		MainMenuController
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="locator"></param>
		/// <param name="view"></param>
		public MainMenuController( IServiceLocator locator, MainMenuView view ) {
			_view = view;

			var audioDevice = locator.GetService<IAudioDevice>();
			audioDevice.LoadBank( "res://Assets/Audio/Banks/Desktop/ui.bank" );

			_helper = locator.GetService<UIEventHelper>();
			_buttonClickedEvent = new DisposableSubscription<ButtonClickedEventArgs>( _helper.ButtonClicked, OnButtonClicked );
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
		}

		/*
		===============
		OnButtonClicked
		===============
		*/
		/// <summary>
		/// w
		/// </summary>
		/// <param name="args"></param>
		/// <exception cref="Exception"></exception>
		private void OnButtonClicked( in ButtonClickedEventArgs args ) {
			if ( args.ButtonId == _view.NewGameButton.ButtonId ) {
				_helper.MenuTransitionRequested.Publish( new MenuTransitionRequestedEventArgs( MenuState.Main, MenuState.CharacterCreation ) );
			} else if ( args.ButtonId == _view.SettingsButton.ButtonId ) {
				_helper.MenuTransitionRequested.Publish( new MenuTransitionRequestedEventArgs( MenuState.Main, MenuState.Settings ) );
			} else if ( args.ButtonId == _view.LoadGameButton.ButtonId ) {
				_helper.MenuTransitionRequested.Publish( new MenuTransitionRequestedEventArgs( MenuState.Main, MenuState.SaveSlots ) );
			} else if ( args.ButtonId == _view.QuitGameButton.ButtonId ) {
				OS.Kill( OS.GetProcessId() );
			}
		}
	};
};