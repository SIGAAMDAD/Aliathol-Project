using Nomad.Core.Util;

namespace Game.Domain.UserInterface.CharacterCreation.Events {
	public readonly record struct MugshotListItemFocusedEventArgs(
		InternString ListId,
		InternString ItemId
	);
};