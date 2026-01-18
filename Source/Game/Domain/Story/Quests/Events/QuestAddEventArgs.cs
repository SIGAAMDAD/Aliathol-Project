using Nomad.Core.Util;

namespace Game.Domain.Story.Events {
	/// <summary>
	/// Event that triggers
	/// </summary>
	/// <param name="QuestId"></param>
	public readonly record struct QuestAddEventArgs(
		InternString QuestId
	);
};