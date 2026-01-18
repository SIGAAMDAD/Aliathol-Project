using Game.Domain.UserInterface.CharacterCreation.Events;
using Game.Infrastructure;
using Game.Infrastructure.Caching;
using Godot;
using Nomad.Core.Events;
using Nomad.Core.Util;
using System.Collections.Generic;

namespace Game.Presentation.Screens.CharacterCreationMenu.MugshotList {
	/*
	===================================================================================
	
	MugshotListController
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	/// <param name="view"></param>

	public class MugshotListController( MugshotListView view ) {
		private readonly VSeparator _marker = new VSeparator();

		/*
		===============
		SetOptions
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="values"></param>
		public void SetOptions( IReadOnlyList<Mugshot> values ) {
			view.ClearItems();
			for ( int i = 0; i < values.Count; i++ ) {
				AddItem( values[ i ].Image, values[ i ].Name, values[ i ].Id );
			}
		}

		/*
		===============
		AddItem
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="image"></param>
		/// <param name="name"></param>
		/// <param name="id"></param>
		private void AddItem( Texture2D image, string name, InternString id ) {
			SceneCache.Instance.GetCached( FilePath.FromResourcePath( "res://Assets/Prefabs/MugshotImage/MugshotImage.tscn" ) ).Get( out var resource );

			var mugshot = resource.Instantiate<MugshotImage>();
			mugshot.Name = name;
			mugshot.Id = id;
			mugshot.Image = image;

			var eventBus = view.Owner.GetNode<NomadBootstrapper>( "/root/NomadBootstrapper" ).ServiceLocator.GetService<IGodotEventBusService>();
			eventBus.ConnectSignal( mugshot, MugshotImage.SignalName.Focused, mugshot, Callable.From<int>( OnMugshotItemSelected ) );

			view.OptionList.AddChild( mugshot );
			mugshot.GrabFocus();
		}

		/*
		===============
		OnMugshotItemSelected
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		private void OnMugshotItemSelected( int id ) {
			var cachedId = new InternString( id );
			var eventFactory = view.Owner.GetNode<NomadBootstrapper>( "/root/NomadBootstrapper" ).ServiceLocator.GetService<IGameEventRegistryService>();
			eventFactory.GetEvent<MugshotListItemFocusedEventArgs>( nameof( CharacterCreationMenu ), "MugshotFocusedEvent" ).Publish( new MugshotListItemFocusedEventArgs( view.Owner.Id, cachedId ) );
		}
	};
};