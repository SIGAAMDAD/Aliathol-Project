using Nomad.Core.Util;

namespace Game.Domain.Story.Events {
	public readonly record struct QuestObjectiveActivateEventArgs(
		InternString QuestId,
		InternString ObjectiveId
	);
};