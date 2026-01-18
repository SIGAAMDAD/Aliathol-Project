using Godot;
using Nomad.Core.Events;

namespace Game.Application.Character.Services.PlayerCharacter {
	/*
	===================================================================================
	
	PlayerCharacterManager
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public sealed class PlayerCharacterManager {
		private readonly PlayerMovementController _movementController;
		private readonly PlayerAnimator _animator;
		
		/*
		===============
		PlayerCharacterManager
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="baseNode"></param>
		/// <param name="eventFactory"></param>
		public PlayerCharacterManager( Node2D baseNode, IGameEventRegistryService eventFactory ) {
			_movementController = new PlayerMovementController( baseNode, eventFactory );
			_animator = new PlayerAnimator( baseNode, eventFactory );
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
			_movementController.Update( delta );
			_animator.Update( delta );
		}
	};
};