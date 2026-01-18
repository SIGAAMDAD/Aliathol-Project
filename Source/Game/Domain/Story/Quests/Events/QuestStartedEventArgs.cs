using Nomad.Core.Util;

namespace Game.Domain.Story.Events {
	/// <summary>
	/// Event that triggers when a quest has been started.
	/// </summary>
	/// <param name="Id"></param>
	public readonly record struct QuestStartedEventArgs(
		InternString Id
	);
};