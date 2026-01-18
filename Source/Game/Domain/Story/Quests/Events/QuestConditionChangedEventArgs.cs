using Nomad.Core.Util;

namespace Game.Domain.Story.Events {
	public readonly record struct QuestConditionChangedEventArgs(
		InternString QuestId,
		InternString ConditionId,
		Any Value
	);
};