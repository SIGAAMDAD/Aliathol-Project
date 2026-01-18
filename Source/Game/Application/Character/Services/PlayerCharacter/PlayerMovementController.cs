using Game.Domain.Character.Data.PlayerCharacter;
using Game.Domain.Character.Events.PlayerCharacter;
using Game.Domain.Character.State;
using Godot;
using Nomad.Core.Events;
using System;

namespace Game.Application.Character.Services.PlayerCharacter {
	/*
	===================================================================================
	
	PlayerMovementController
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public sealed class PlayerMovementController : IDisposable {
		private static readonly StringName @TriggeredSignalName = "triggered";
		private static readonly StringName @CompletedSignalName = "completed";
		private static readonly StringName @ValueAxis2DPropertyName = "value_axis_2d";
		private static readonly StringName @EnableMappingContextMethodName = "enable_mapping_context";

		private readonly AnimatedSprite2D _animation;
		private readonly Resource _moveAction;
		private readonly Node2D _baseNode;

		private const float SPEED = 10.0f;
		private const float SMOOTH_FACTOR = -8.0f;

		private Vector2 _position = Vector2.Zero;
		private Vector2 _moveDirection = Vector2.Zero;
		private Vector2 _velocity = Vector2.Zero;
		private MoveDirection _direction = MoveDirection.South;

		public IGameEvent<PlayerStartMovingEventArgs> StartMoving => _startMoving;
		private readonly IGameEvent<PlayerStartMovingEventArgs> _startMoving;

		public IGameEvent<PlayerEndMovingEventArgs> EndMoving => _endMoving;
		private readonly IGameEvent<PlayerEndMovingEventArgs> _endMoving;
		
		/*
		===============
		PlayerMovementController
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="baseNode"></param>
		/// <param name="eventFactory"></param>
		public PlayerMovementController( Node2D baseNode, IGameEventRegistryService eventFactory ) {
			_baseNode = baseNode;
			_animation = baseNode.GetNode<AnimatedSprite2D>( "AnimatedSprite2D" );
			_position = baseNode.GlobalPosition;

			_startMoving = eventFactory.GetEvent<PlayerStartMovingEventArgs>( EventNames.NAMESPACE, EventNames.PLAYER_START_MOVING_EVENT );
			_endMoving = eventFactory.GetEvent<PlayerEndMovingEventArgs>( EventNames.NAMESPACE, EventNames.PLAYER_END_MOVING_EVENT );

			Node guideNode = baseNode.GetNode( "/root/GUIDE" );
			guideNode.Call( EnableMappingContextMethodName, ResourceLoader.Load( "res://Assets/Config/BindMaps/KeyboardMapping.tres" ) );

			// FIXME!
			_moveAction = ResourceLoader.Load( "res://Assets/Config/BindMaps/KeyboardAndMouse/Move.tres" );
			_moveAction.Connect( TriggeredSignalName, Callable.From( OnMoveTriggered ) );
			_moveAction.Connect( CompletedSignalName, Callable.From( OnMoveCompleted ) );
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
			_startMoving.Dispose();
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
			Vector2 targetVelocity = _moveDirection * SPEED;
			_velocity += ( targetVelocity - _velocity ) * (float)( 1.0f - Math.Exp( SMOOTH_FACTOR * delta ) );
			_position += _velocity;
			_baseNode.GlobalPosition = _position;
		}

		/*
		===============
		OnMoveTriggered
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		private void OnMoveTriggered() {
			_moveDirection = _moveAction.Get( ValueAxis2DPropertyName ).AsVector2();

			if ( Math.Abs( _moveDirection.X ) > Math.Abs( _moveDirection.Y ) ) {
				if ( _moveDirection.X > 0.0f ) {
					_direction = MoveDirection.East;
				}
				if ( _moveDirection.X < 0.0f ) {
					_direction = MoveDirection.West;
				}
			} else {
				if ( _moveDirection.Y > 0.0f ) {
					_direction = MoveDirection.South;
				}
				if ( _moveDirection.Y < 0.0f ) {
					_direction = MoveDirection.North;
				}
			}
			_startMoving.Publish( new PlayerStartMovingEventArgs( _direction ) );
		}

		/*
		===============
		OnMoveCompleted
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		private void OnMoveCompleted() {
			_moveDirection = Vector2.Zero;
			_endMoving.Publish( new PlayerEndMovingEventArgs() );
		}
	};
};