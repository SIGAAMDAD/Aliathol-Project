using Game.Application.Character.Interfaces;
using Game.Domain.Character.Models.DTOs;
using Game.Infrastructure.Caching;
using Game.Infrastructure.Character.GodotResource;
using Godot;
using Nomad.Core.Logger;
using Nomad.Core.Memory;
using Nomad.Core.Util;
using Nomad.ResourceCache;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Game.Infrastructure.Character {
	/*
	===================================================================================
	
	CharacterDataProvider
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal sealed class CharacterDataProvider : ICharacterDataProvider {
		private readonly FilePath RACE_RESOURCE_PATH = FilePath.FromResourcePath( "res://Assets/CharacterData/Races/" );
		private readonly FilePath CLASS_RESOURCE_PATH = FilePath.FromResourcePath( "res://Assets/CharacterData/Classes/" );
		private readonly FilePath ORIGIN_RESOURCE_PATH = FilePath.FromResourcePath( "res://Assets/CharacterData/Origins/" );

		public ImmutableArray<CharacterClassDto> AvailableClasses => _classCache;
		public ImmutableArray<CharacterRaceDto> AvailableRaces => _raceCache;
		public ImmutableArray<CharacterOriginDto> AvailableOrigins => _originCache;

		private readonly Dictionary<int, int> _classIndexMap = new Dictionary<int, int>();
		private readonly Dictionary<int, int> _raceIndexMap = new Dictionary<int, int>();
		private readonly Dictionary<int, int> _originIndexMap = new Dictionary<int, int>();

		private ImmutableArray<CharacterClassDto> _classCache;
		private ImmutableArray<CharacterRaceDto> _raceCache;
		private ImmutableArray<CharacterOriginDto> _originCache;

		/*
		===============
		GetClassById
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public CharacterClassDto GetClassById( InternString id ) {
			return _classCache[ _classIndexMap[ id ] ];
		}

		/*
		===============
		GetRaceById
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public CharacterRaceDto GetRaceById( InternString id ) {
			return _raceCache[ _raceIndexMap[ id ] ];
		}

		/*
		===============
		GetOriginById
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public CharacterOriginDto GetOriginById( InternString id ) {
			return _originCache[ _originIndexMap[ id ] ];
		}

		/*
		===============
		LoadAll
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="logger"></param>
		/// <returns></returns>
		public async Task LoadAll( ILoggerService logger ) {
			await LoadRaces( logger );
			await LoadClasses( logger );
			await LoadOrigins( logger );
		}

		/*
		===============
		LoadClasses
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="logger"></param>
		/// <returns></returns>
		private async ValueTask LoadClasses( ILoggerService logger ) {
			var classes = System.IO.Directory.GetFiles( CLASS_RESOURCE_PATH.OSPath );

			var cache = new CharacterClassDto[ classes.Length ];
			_classIndexMap.EnsureCapacity( classes.Length );

			for ( int i = 0; i < classes.Length; i++ ) {
				var path = FilePath.FromNative( classes[ i ] );
				var id = new InternString( path.GetFileName().GetFile().GetBaseName() );
				try {
					ResourceCache.Instance.GetCached( path ).Get( out var resource );
					if ( resource is not ClassResource classResource ) {
						throw new InvalidCastException( "Class resource isn't a ClassResource type!" );
					}
					_classIndexMap[ id ] = i;
					cache[ i ] = MapClassToDto( id, classResource );
				} catch ( Exception e ) {
					logger.PrintError( $"Exception thrown while loading class resource '{path}'\n{e}" );
					throw;
				}
			}
			_classCache = [ .. cache ];
		}

		/*
		===============
		LoadRaces
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="logger"></param>
		/// <returns></returns>
		private async ValueTask LoadRaces( ILoggerService logger ) {
			var races = System.IO.Directory.GetFiles( RACE_RESOURCE_PATH.OSPath );

			var cache = new CharacterRaceDto[ races.Length ];
			_raceIndexMap.EnsureCapacity( races.Length );

			for ( int i = 0; i < races.Length; i++ ) {
				var path = FilePath.FromNative( races[ i ] );
				var id = new InternString( path.GetFileName().GetFile().GetBaseName() );
				try {
					ResourceCache.Instance.GetCached( path ).Get( out var resource );
					if ( resource is not RaceResource raceResource ) {
						throw new InvalidCastException( "Race resource isn't a RaceResource type!" );
					}
					_raceIndexMap[ id ] = i;
					cache[ i ] = MapRaceToDto( id, raceResource );
				} catch ( Exception e ) {
					logger.PrintError( $"Exception thrown while loading race resource '{path}'\n{e}" );
					throw;
				}
			}
			_raceCache = [ .. cache ];
		}

		/*
		===============
		LoadOrigins
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="logger"></param>
		/// <returns></returns>
		private async ValueTask LoadOrigins( ILoggerService logger ) {
			var origins = System.IO.Directory.GetFiles( ORIGIN_RESOURCE_PATH.OSPath );

			var cache = new CharacterOriginDto[ origins.Length ];
			_originIndexMap.EnsureCapacity( origins.Length );

			for ( int i = 0; i < origins.Length; i++ ) {
				var path = FilePath.FromNative( origins[ i ] );
				var id = new InternString( path.GetFileName().GetFile().GetBaseName() );
				try {
					ResourceCache.Instance.GetCached( path ).Get( out var resource );
					if ( resource is not OriginResource originResource ) {
						throw new InvalidCastException( "Origin resource isn't a OriginResource type!" );
					}
					_originIndexMap[ id ] = i;
					cache[ i ] = MapOriginToDto( id, originResource );
				} catch ( Exception e ) {
					logger.PrintError( $"Exception thrown while loading race resource '{path}'\n{e}" );
					throw;
				}
			}
			_originCache = [ .. cache ];
		}

		/*
		===============
		MapRaceToDto
		===============
		*/
		/// <summary>
		/// Maps a godot race resource to the domain dto.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="resource"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private CharacterRaceDto MapRaceToDto( InternString id, RaceResource resource ) {
			return new CharacterRaceDto(
				id,
				StringPool.Intern( TranslationServer.Translate( resource.Name ) ),
				StringPool.Intern( TranslationServer.Translate( resource.Description ) ),
				resource.Mugshot
			);
		}

		/*
		===============
		MapClassToDto
		===============
		*/
		/// <summary>
		/// Maps a godot class resource to the domain dto.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="resource"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private CharacterOriginDto MapOriginToDto( InternString id, OriginResource resource ) {
			return new CharacterOriginDto(
				id,
				StringPool.Intern( TranslationServer.Translate( resource.Name ) ),
				StringPool.Intern( TranslationServer.Translate( resource.Description ) ),
				resource.Mugshot
			);
		}

		/*
		===============
		MapClassToDto
		===============
		*/
		/// <summary>
		/// Maps a godot class resource to the domain dto.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="resource"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private CharacterClassDto MapClassToDto( InternString id, ClassResource resource ) {
			return new CharacterClassDto(
				id,
				StringPool.Intern( TranslationServer.Translate( resource.Name ) ),
				StringPool.Intern( TranslationServer.Translate( resource.Description ) ),
				resource.Mugshot
			);
		}
	};
};