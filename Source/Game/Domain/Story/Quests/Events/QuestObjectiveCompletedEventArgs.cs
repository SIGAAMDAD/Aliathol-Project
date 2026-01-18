using Nomad.Core.Util;

namespace Game.Domain.Story.Events {
	/// <summary>
	/// Event that triggers when an objective in a quest has been completed.
	/// </summary>
	/// <param name="QuestId"></param>
	/// <param name="ObjectiveId"></param>
	public readonly record struct QuestObjectiveCompletedEventArgs(
		InternString QuestId,
		InternString ObjectiveId
	);
};