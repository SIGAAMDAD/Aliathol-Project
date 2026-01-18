using Game.Domain.Character.Data.PlayerCharacter;
using Game.Domain.Character.Events.PlayerCharacter;
using Game.Domain.Character.State;
using Game.Infrastructure;
using Godot;
using Nomad.Core.Events;
using Nomad.GodotServer.Rendering.Interfaces;
using System;

namespace Game.Application.Character.Services.PlayerCharacter {
	/*
	===================================================================================
	
	PlayerAnimator
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public sealed class PlayerAnimator : IDisposable {
		private readonly IAnimationEntity _animatedSprite;
		private MoveDirection _direction;
		
		/*
		===============
		PlayerAnimator
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public PlayerAnimator( Node2D baseNode, IGameEventRegistryService eventFactory ) {
			var animatedSprite = baseNode.GetNode<AnimatedSprite2D>( "AnimatedSprite2D" );
			_animatedSprite = baseNode.GetNode<NomadBootstrapper>( "/root/NomadBootstrapper" ).ServiceLocator.GetService<IEntitySystemService>().CreateAnimator( animatedSprite );

			var playerStartMoving = eventFactory.GetEvent<PlayerStartMovingEventArgs>( EventNames.NAMESPACE, EventNames.PLAYER_START_MOVING_EVENT );
			playerStartMoving.Subscribe( this, OnStartMoving );

			var playerEndMoving = eventFactory.GetEvent<PlayerEndMovingEventArgs>( EventNames.NAMESPACE, EventNames.PLAYER_END_MOVING_EVENT );
			playerEndMoving.Subscribe( this, OnEndMoving );
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
			_animatedSprite.Dispose();
		}

		/*
		===============
		Update
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="delta"></param>
		public void Update( float delta ) {
			_animatedSprite.Update( delta );
		}

		/*
		===============
		OnStartMoving
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		private void OnStartMoving( in PlayerStartMovingEventArgs args ) {
			_direction = args.Direction;
			switch ( args.Direction ) {
				case MoveDirection.West:
					_animatedSprite.Play( "walk_horizontal" );
					_animatedSprite.FlipH = true;
					break;
				case MoveDirection.East:
					_animatedSprite.Play( "walk_horizontal" );
					_animatedSprite.FlipH = false;
					break;
				case MoveDirection.North:
				case MoveDirection.South:
					_animatedSprite.Play( "walk_down" );
					break;
			}
		}

		/*
		===============
		OnEndMoving
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		private void OnEndMoving( in PlayerEndMovingEventArgs args ) {
			switch ( _direction ) {
				case MoveDirection.West:
					_animatedSprite.Play( "idle_horizontal" );
					_animatedSprite.FlipH = true;
					break;
				case MoveDirection.East:
					_animatedSprite.Play( "idle_horizontal" );
					_animatedSprite.FlipH = false;
					break;
				case MoveDirection.North:
				case MoveDirection.South:
					_animatedSprite.Play( "idle_down" );
					break;
			}
		}
	};
};