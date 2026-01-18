using Game.Application.Character.Services;
using Game.Application.Character.Services.PlayerCharacter;
using Game.Application.Story.Quests;
using Game.Infrastructure;
using Godot;
using Nomad.Core.Events;
using Nomad.Core.Logger;

namespace Prefabs {
	/*
	===================================================================================
	
	World
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public partial class World : Node {
		[Export]
		private Node2D _startPosition;
		[Export]
		private PackedScene _playerScene;

		private PlayerSpawnService _spawnService;
		private PlayerCharacterManager _playerManager;

		/*
		===============
		_Ready
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public override void _Ready() {
			base._Ready();

			var bootstrapper = GetNode<NomadBootstrapper>( "/root/NomadBootstrapper" );
			var locator = bootstrapper.ServiceLocator;
			var serviceFactory = bootstrapper.ServicesFactory;

			var eventFactory = locator.GetService<IGameEventRegistryService>();

			serviceFactory.RegisterSingleton<IQuestService>( new QuestService( locator.GetService<ILoggerService>(), eventFactory ) );
			_spawnService = serviceFactory.RegisterSingleton<PlayerSpawnService>( new PlayerSpawnService( eventFactory ) );

			Node2D player = _playerScene.Instantiate<Node2D>();
			player.GlobalPosition = _startPosition.GlobalPosition;
			AddChild( player );

			_playerManager = _spawnService.SpawnPlayer( player );
		}

		/*
		===============
		_Process
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="delta"></param>
		public override void _Process( double delta ) {
			base._Process( delta );
		}

		/*
		===============
		_PhysicsProcess
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="delta"></param>
		public override void _PhysicsProcess( double delta ) {
			base._PhysicsProcess( delta );

			float fixedDelta = (float)delta;
			_playerManager.Update( fixedDelta );
		}
	};
};
