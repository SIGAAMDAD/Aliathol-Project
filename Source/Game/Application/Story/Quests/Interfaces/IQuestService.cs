using Game.Domain.Story.Models;
using System;
using System.Collections.Generic;

namespace Game.Application.Story.Quests {
	/*
	===================================================================================
	
	IQuestService
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public interface IQuestService : IDisposable {
		IReadOnlyList<QuestId> Quests { get; }
	};
};