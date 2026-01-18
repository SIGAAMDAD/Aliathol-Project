using Game.Domain.Character.State;

namespace Game.Domain.Character.Events.PlayerCharacter {
	/// <summary>
	/// 
	/// </summary>
	/// <param name="Direction"></param>
	public readonly record struct PlayerStartMovingEventArgs(
		MoveDirection Direction
	);
};