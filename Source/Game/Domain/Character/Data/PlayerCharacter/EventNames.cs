using Game.Application.Character.Services.PlayerCharacter;

namespace Game.Domain.Character.Data.PlayerCharacter {
	public static class EventNames {
		public const string NAMESPACE = nameof( PlayerCharacter );

		public const string PLAYER_CHANGE_MOVE_DIRECTION_EVENT = $"{NAMESPACE}:PlayerChangeMoveDirection";
		public const string PLAYER_START_MOVING_EVENT = $"{NAMESPACE}:{nameof( PlayerMovementController.StartMoving )}";
		public const string PLAYER_END_MOVING_EVENT = $"{NAMESPACE}:{nameof( PlayerMovementController.EndMoving )}";
	};
};