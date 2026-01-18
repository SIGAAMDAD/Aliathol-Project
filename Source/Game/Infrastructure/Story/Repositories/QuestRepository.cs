using Game.Domain.Story.Events;
using Game.Domain.Story.Quests.Data;
using Game.Infrastructure.Caching;
using Game.Infrastructure.Story.Entities;
using Godot;
using Nomad.Core.Events;
using Nomad.Core.Logger;
using Nomad.Core.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Game.Infrastructure.Story {
	/*
	===================================================================================
	
	QuestRepository
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	public sealed class QuestRepository : IDisposable {
		private static readonly FilePath QUEST_ASSET_DIRECTORY = FilePath.FromResourcePath( "res://Assets/Quests/" );
#if DEBUG
		private const string QUEST_RESOURCE_EXTENSION = "tres";
#else
		private const string QUEST_RESOURCE_EXTENSION = ".res";
#endif
		private record QuestData(
			InternString Id,
			Resource Quest
		);

		private const string CONDITION_PLAYER_STATE = nameof( CONDITION_PLAYER_STATE );
		private const string PLAYER_STATE_CHECK_MET_NPC = "CheckMetNPC";
		private const string PLAYER_STATE_IS_IN_AREA = "IsInArea";

		private static readonly StringName @NodesPropertyName = "nodes";
		private static readonly StringName @OptionalPropertyName = "optional";

		private static readonly StringName @ActivateObjectiveMethodName = "activate_objective";

		public static readonly StringName @QuestIdMetaDataName = "id";
		public static readonly StringName @ObjectiveIdMetaDataName = "OBJECTIVE_ID";
		private static readonly StringName @QuestObjectiveConditionsMetaDataName = "conditions";

		private readonly ConcurrentDictionary<Resource, QuestInstance> _questInstanceCache = new();
		private readonly ConcurrentDictionary<Resource, string> _questInstanceToId = new();
		private readonly ConcurrentDictionary<string, Resource> _questIdToInstance = new();
		private readonly ImmutableDictionary<string, Resource> _questLookup;

		private QuestInstance? _currentInstance;

		public IGameEvent<QuestCompletedEventArgs> QuestCompleted => _questCompleted;
		private readonly IGameEvent<QuestCompletedEventArgs> _questCompleted;

		public IGameEvent<QuestStartedEventArgs> QuestStarted => _questStarted;
		private readonly IGameEvent<QuestStartedEventArgs> _questStarted;

		public IGameEvent<QuestObjectiveCompletedEventArgs> ObjectiveCompleted => _objectiveCompleted;
		private readonly IGameEvent<QuestObjectiveCompletedEventArgs> _objectiveCompleted;

		public IGameEvent<QuestObjectiveActivateEventArgs> ObjectiveActive => _objectiveActivate;
		private readonly IGameEvent<QuestObjectiveActivateEventArgs> _objectiveActivate;

		/*
		===============
		QuestRepository
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="eventFactory"></param>
		public QuestRepository( ILoggerService logger, IGameEventRegistryService eventFactory ) {
			// hook the mediation
			Questify.ConnectConditionQueryRequested( OnConditionQueryRequested );
			Questify.ConnectQuestCompleted( OnQuestCompleted );
			Questify.ConnectQuestObjectiveAdded( OnQuestObjectiveAdded );
			Questify.ConnectQuestObjectiveCompleted( OnQuestObjectiveCompleted );
			Questify.ConnectQuestStarted( OnQuestStarted );

			var questActivated = eventFactory.GetEvent<QuestActivateEventArgs>( EventNames.NAMESPACE, EventNames.QUEST_ACTIVATE_EVENT );
			questActivated.Subscribe( this, OnQuestActivate );

			var questConditionChanged = eventFactory.GetEvent<QuestConditionChangedEventArgs>( EventNames.NAMESPACE, EventNames.QUEST_CONDITION_CHANGED_EVENT );
			questConditionChanged.Subscribe( this, OnQuestConditionChanged );

			_questCompleted = eventFactory.GetEvent<QuestCompletedEventArgs>( EventNames.NAMESPACE, EventNames.QUEST_COMPLETED_EVENT );
			_questStarted = eventFactory.GetEvent<QuestStartedEventArgs>( EventNames.NAMESPACE, EventNames.QUEST_STARTED_EVENT );
			_objectiveCompleted = eventFactory.GetEvent<QuestObjectiveCompletedEventArgs>( EventNames.NAMESPACE, EventNames.QUEST_OBJECTIVE_COMPLETED_EVENT );
			_objectiveActivate = eventFactory.GetEvent<QuestObjectiveActivateEventArgs>( EventNames.NAMESPACE, EventNames.QUEST_OBJECTIVE_ACTIVATE_EVENT );

			_questLookup = LoadQuests( QUEST_ASSET_DIRECTORY ).ToImmutableDictionary();
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
			Questify.Clear();
		}

		/*
		===============
		StartQuest
		===============
		*/
		/// <summary>
		/// Begins a quest and maps out all the objectives.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="resource"></param>
		private void StartQuest( string id, Resource resource ) {
			try {
				Resource quest = Questify.Instantiate( resource );

				GD.Print( "starting quest..." );

				Resource[] nodes = quest.Get( NodesPropertyName ).AsGodotObjectArray<Resource>();

				List<Resource> objectives = new List<Resource>();
				for ( int i = 0; i < nodes.Length; i++ ) {
					var node = nodes[ i ];
					if ( node.HasMethod( ActivateObjectiveMethodName ) ) {
						objectives.Add( node );
					}
				}

				Godot.Collections.Dictionary<StringName, Variant> objectiveConditions = quest.Get( QuestObjectiveConditionsMetaDataName ).AsGodotDictionary<StringName, Variant>();
				var conditions = new Dictionary<string, Variant>( objectiveConditions.Count );
				foreach ( var condition in objectiveConditions ) {
					conditions[ condition.Key ] = condition.Value;
				}

				_currentInstance = new QuestInstance(
					new( id ),
					quest,
					objectives,
					conditions
				);
				_questInstanceCache[ quest ] = _currentInstance;
				_questInstanceToId[ quest ] = id;
				_questIdToInstance[ id ] = quest;

				Questify.StartQuest( quest );
			} catch ( Exception e ) {
				GD.PushError( e );
			}
		}

		/*
		===============
		LoadQuests
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="directory"></param>
		private Dictionary<string, Resource> LoadQuests( FilePath directory ) {
			var quests = new Dictionary<string, Resource>();

			static void FindQuestResources( FilePath folder, Dictionary<string, Resource> quests ) {
				var files = System.IO.Directory.GetFiles( folder.OSPath );
				var folders = System.IO.Directory.GetDirectories( folder.OSPath );

				for ( int i = 0; i < files.Length; i++ ) {
					var fileName = files[ i ];
					if ( fileName.GetExtension() == QUEST_RESOURCE_EXTENSION ) {
						ResourceCache.Instance.GetCached( FilePath.FromResourcePath( FilePath.FromNative( fileName ).GodotPath ) ).Get( out var resource );
						//var id = new InternString(  );
						var id = resource.Get( QuestIdMetaDataName ).AsStringName();
						quests[ id ] = resource;
						GD.Print( $"Adding quest {id} to cache..." );
					}
				}
				for ( int i = 0; i < folders.Length; i++ ) {
					FindQuestResources( FilePath.FromNative( folders[ i ] ), quests );
				}
			}
			FindQuestResources( directory, quests );

			return quests;
		}

		/*
		===============
		OnQuestActivate
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		private void OnQuestActivate( in QuestActivateEventArgs args ) {
			GD.Print( $"Triggering quest {(string)args.QuestId}" );
			if ( _questLookup.TryGetValue( (string)args.QuestId, out var data ) ) {
				StartQuest( args.QuestId, data );
			} else {
				GD.PushError( $"Quest {(string)args.QuestId} not found!" );
			}
		}

		/*
		===============
		OnQuestConditionChanged
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		private void OnQuestConditionChanged( in QuestConditionChangedEventArgs args ) {
			GD.Print( $"Changing quest condition {(string)args.ConditionId} in quest {(string)args.QuestId}" );
			if ( _questInstanceCache.TryGetValue( _questIdToInstance[ args.QuestId ], out var data ) ) {
				if ( data.Conditions.TryGetValue( args.ConditionId, out var value ) ) {
					data.Conditions[ args.ConditionId ] = value.VariantType switch {
						Variant.Type.Bool => Variant.From( args.Value.GetValue<bool>() ),
						Variant.Type.Int => Variant.From( args.Value.GetValue<int>() ),
						Variant.Type.Float => Variant.From( args.Value.GetValue<float>() ),
						Variant.Type.String or Variant.Type.StringName => Variant.From( (string)args.Value.GetValue<InternString>() ),
						_ => throw new ArgumentOutOfRangeException( nameof( value.VariantType ) )
					};
				}
			}
		}

		/*
		===============
		OnQuestStarted
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="quest"></param>
		private void OnQuestStarted( Resource quest ) {
			GD.Print( "Quest started!" );
			if ( !_questInstanceCache.TryGetValue( quest, out var data ) ) {
				return;
			}
			_questStarted.Publish( new QuestStartedEventArgs( data.Id ) );
			Questify.ToggleUpdatePolling( true );
		}

		/*
		===============
		OnQuestObjectiveCompleted
		===============
		*/
		/// <summary>
		/// Godot callback for when a quest objective has been completed.
		/// </summary>
		/// <param name="quest"></param>
		/// <param name="objective"></param>
		private void OnQuestObjectiveCompleted( Resource quest, Resource objective ) {
			if ( !_questInstanceCache.TryGetValue( quest, out var data ) ) {
				return;
			}
			_objectiveCompleted.Publish( new QuestObjectiveCompletedEventArgs( data.Id, new( objective.GetMeta( ObjectiveIdMetaDataName ).AsString() ) ) );
			GD.Print( "Quest objective completed!" );
		}

		/*
		===============
		OnQuestObjectiveAdded
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="quest"></param>
		/// <param name="objective"></param>
		private void OnQuestObjectiveAdded( Resource quest, Resource objective ) {
			if ( !_questInstanceCache.TryGetValue( quest, out var data ) ) {
				return;
			}
			_objectiveActivate.Publish( new QuestObjectiveActivateEventArgs( data.Id, new( objective.GetMeta( ObjectiveIdMetaDataName ).AsString() ) ) );
			GD.Print( "Quest Objective Added" );
		}

		/*
		===============
		OnQuestCompleted
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="resource"></param>
		private void OnQuestCompleted( Resource quest ) {
			if ( !_questInstanceCache.TryGetValue( quest, out var data ) ) {
				return;
			}
			_questCompleted.Publish( new QuestCompletedEventArgs( data.Id ) );
			Questify.ToggleUpdatePolling( false );
			GD.Print( "Quest Completed" );
		}

		/*
		===============
		OnConditionQueryRequested
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="requester"></param>
		private void OnConditionQueryRequested( string type, string key, Variant value, Resource requester ) {
			switch ( type ) {
				case CONDITION_PLAYER_STATE: {
						if ( _currentInstance.Conditions.TryGetValue( key, out var data ) ) {
							Questify.SetConditionCompleted( requester, data.AsBool() == value.AsBool() );
						}
						break;
					}
			}
		}
	};
};