namespace Game.Domain.Story.Models {
	public record QuestDefinition {
		public QuestId Id { get; }
		public string Name { get; }
		public string Description { get; }
		public string NextQuestId { get; }
	};
};