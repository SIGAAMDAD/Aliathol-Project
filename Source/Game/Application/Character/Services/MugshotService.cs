using Game.Application.Character.Interfaces;
using Game.Presentation.Screens.CharacterCreationMenu;
using Godot;
using Nomad.Core.Util;
using Nomad.ResourceCache;
using System;

namespace Game.Application.Character.Services {
	/*
	===================================================================================
	
	MugshotService
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal sealed class MugshotService {
		private readonly IResourceCacheService<Texture, FilePath> _textureCache;

		/*
		===============
		MugshotService
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="cacheService"></param>
		public MugshotService( IResourceCacheService<Texture, FilePath> cacheService ) {
			_textureCache = cacheService;
		}

		/*
		===============
		ConvertRaceList
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		/// <exception cref="InvalidCastException"></exception>
		public Mugshot[] ConvertRaceList( ICharacterDataProvider provider ) {
			var raceList = provider.AvailableRaces;
			var mugshots = new Mugshot[ raceList.Length ];

			for ( int i = 0; i < mugshots.Length; i++ ) {
				if ( raceList[ i ].Mugshot is not Texture2D texture2D ) {
					throw new InvalidCastException( "Mugshot images must be a Texture2D!" );
				}
				mugshots[ i ] = new Mugshot(
					texture2D,
					raceList[ i ].Id,
					raceList[ i ].Name,
					raceList[ i ].Description
				);
			}
			return mugshots;
		}

		/*
		===============
		ConvertClassList
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="provider"></param>
		/// <returns></returns>
		/// <exception cref="InvalidCastException"></exception>
		public Mugshot[] ConvertClassList( ICharacterDataProvider provider ) {
			var classList = provider.AvailableClasses;
			var mugshots = new Mugshot[ classList.Length ];

			for ( int i = 0; i < mugshots.Length; i++ ) {
				if ( classList[ i ].Mugshot is not Texture2D texture2D ) {
					throw new InvalidCastException( "Mugshot images must be a Texture2D!" );
				}
				mugshots[ i ] = new Mugshot(
					texture2D,
					classList[ i ].Id,
					classList[ i ].Name,
					classList[ i ].Description
				);
			}
			return mugshots;
		}

		/*
		===============
		ConvertOriginList
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="provider"></param>
		/// <returns></returns>
		/// <exception cref="InvalidCastException"></exception>
		public Mugshot[] ConvertOriginList( ICharacterDataProvider provider ) {
			var originList = provider.AvailableOrigins;
			var mugshots = new Mugshot[ originList.Length ];

			for ( int i = 0; i < mugshots.Length; i++ ) {
				if ( originList[ i ].Mugshot is not Texture2D texture2D ) {
					throw new InvalidCastException( "Mugshot images must be a Texture2D!" );
				}
				mugshots[ i ] = new Mugshot(
					texture2D,
					originList[ i ].Id,
					originList[ i ].Name,
					originList[ i ].Description
				);
			}
			return mugshots;
		}
	};
};