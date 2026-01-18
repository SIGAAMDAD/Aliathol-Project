using System;

namespace Game.Domain.Story.State {
	/// <summary>
	/// Quest behaviors
	/// </summary>
	[Flags]
	public enum QuestFlags : uint {
		None = 0,

		/// <summary>
		/// Can be repeated.
		/// </summary>
		Repeatable = 1 << 0,

		/// <summary>
		/// Main storyline quest.
		/// </summary>
		MainStory = 1 << 1,

		/// <summary>
		/// Optional content.
		/// </summary>
		SideQuest = 1 << 2,
		
		/// <summary>
		/// Must be completed within a specific timeframe.
		/// </summary>
		TimeSensitive = 1 << 3,

		/// <summary>
		/// Don't show until can start.
		/// </summary>
		HiddenUntilAvailable = 1 << 4,

		/// <summary>
		/// Automatically starts when made available
		/// </summary>
		AutoStart = 1 << 5,

		/// <summary>
		/// Can be failed.
		/// </summary>
		CanBeFailed = 1 << 6,

		/// <summary>
		/// Tied to an achievement
		/// </summary>
		Achievement = 1 << 7
	};
};