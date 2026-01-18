namespace Game.Domain.Story.Quests.Data {
	public static class EventNames {
		public const string NAMESPACE = $"{nameof( Story )}:{nameof( Quests )}";
		
		public const string QUEST_COMPLETED_EVENT = $"{NAMESPACE}:QuestCompleted";
		public const string QUEST_STARTED_EVENT = $"{NAMESPACE}:QuestStarted";
		public const string QUEST_OBJECTIVE_COMPLETED_EVENT = $"{NAMESPACE}:QuestObjectiveCompleted";
		public const string QUEST_OBJECTIVE_ACTIVATE_EVENT = $"{NAMESPACE}:QuestObjectiveActivate";

		public const string QUEST_RECEIVED_EVENT = $"{NAMESPACE}:QuestReceived";
		public const string QUEST_ACTIVATE_EVENT = $"{NAMESPACE}:QuestActivate";
		public const string QUEST_CONDITION_CHANGED_EVENT = $"{NAMESPACE}:QuestConditionChanged";
	};
};