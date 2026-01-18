using Nomad.Core.Events;
using Game.Application.Character.Services.PlayerCharacter;
using Godot;

namespace Game.Application.Character.Services {
	/*
	===================================================================================
	
	PlayerSpawnService
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	public sealed class PlayerSpawnService {
		private readonly IGameEventRegistryService _eventFactory;

		public IGameEvent<EmptyEventArgs> PlayerSpawned => _playerSpawned;
		private readonly IGameEvent<EmptyEventArgs> _playerSpawned;

		/*
		===============
		PlayerSpawnService
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventFactory"></param>
		public PlayerSpawnService( IGameEventRegistryService eventFactory ) {
			_eventFactory = eventFactory;

			_playerSpawned = eventFactory.GetEvent<EmptyEventArgs>( nameof( PlayerSpawnService ), nameof( PlayerSpawned ) );
		}

		/*
		===============
		SpawnPlayer
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="playerInstance"></param>
		/// <returns></returns>
		public PlayerCharacterManager SpawnPlayer( Node2D playerInstance ) {
			PlayerCharacterManager player = new PlayerCharacterManager( playerInstance, _eventFactory );
			return player;
		}
	};
};