using Game.Domain.Story.Quests.State;
using Godot;
using Nomad.Core.Util;
using System.Collections.Generic;

namespace Game.Infrastructure.Story.Entities {
	/*
	===================================================================================
	
	QuestInstance
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public sealed class QuestInstance( InternString questId, Resource questResource, List<Resource> objectives, Dictionary<string, Variant> conditions ) {
		public InternString Id => questId;
		public Resource QuestResource => questResource;
		public List<Resource> Objecties => objectives;
		public Dictionary<string, Variant> Conditions => conditions;

		public QuestState State => _state;
		private QuestState _state = QuestState.Hidden;
	};
};