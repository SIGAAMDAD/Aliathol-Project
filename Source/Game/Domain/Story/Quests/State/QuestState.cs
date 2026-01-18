namespace Game.Domain.Story.Quests.State {
	public enum QuestState : byte {
		Hidden,
		Available,
		Active,
		Completed,
		Failed
	};
};