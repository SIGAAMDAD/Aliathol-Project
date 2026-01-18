using Game.Domain.Character.State;

namespace Game.Domain.Character.Events {
	/// <summary>
	/// 
	/// </summary>
	/// <param name="Value"></param>
	public readonly record struct CharacterSexualitySelectedEventArgs(
		Sexuality Value
	);
};