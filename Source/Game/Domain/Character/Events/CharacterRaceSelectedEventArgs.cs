using Nomad.Core.Util;

namespace Game.Domain.Character.Events {
	/// <summary>
	/// 
	/// </summary>
	/// <param name="Id"></param>
	public readonly record struct CharacterRaceSelectedEventArgs(
		InternString Id
	);
};