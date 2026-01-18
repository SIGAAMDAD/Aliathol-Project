using Game.Domain.Character.State;
using Nomad.Core.Util;

namespace Game.Domain.Character.Models {
	/// <summary>
	/// A data set storing the player's immutable data.
	/// </summary>
	/// <param name="ClassId"></param>
	/// <param name="RaceId"></param>
	/// <param name="OriginId"></param>
	/// <param name="Age"></param>
	/// <param name="Gender"></param>
	/// <param name="Sexuality"></param>
	public readonly record struct PlayerCharacterData(
		InternString ClassId,
		InternString RaceId,
		InternString OriginId,
		Age Age,
		Gender Gender,
		Sexuality Sexuality
	);
};