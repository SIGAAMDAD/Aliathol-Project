using Game.Domain.Story.Models;
using Game.Infrastructure.Story;
using Nomad.Core.Events;
using Nomad.Core.Logger;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game.Application.Story.Quests {
	/*
	===================================================================================
	
	QuestService
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public sealed class QuestService : IQuestService {
		public IReadOnlyList<QuestId> Quests => _questList;
		private readonly List<QuestId> _questList;
		
		private readonly QuestRepository _repository;

		/*
		===============
		QuestService
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public QuestService( ILoggerService logger, IGameEventRegistryService eventFactory ) {
			_repository = new QuestRepository( logger, eventFactory );
		}
		
		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public void Dispose() {
			_repository.Dispose();
		}

		public async ValueTask StartQuest( QuestId id ) {
		}
	};
};