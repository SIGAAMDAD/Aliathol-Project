using Game.Domain.Story.Events;
using Game.Domain.Story.Quests.Data;
using Game.Infrastructure;
using Game.Infrastructure.Story;
using Godot;
using Nomad.Core.Events;
using Nomad.Core.Util;
using System;

namespace Prefabs {
	/*
	===================================================================================
	
	QuestTriggerDefault
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public partial class QuestTriggerDefault : Node2D {
		[Export]
		private bool _isStart = false;
		[Export]
		private StringName _conditionName;
		[Export]
		private Resource _questResource;
		[Export]
		private Variant _value;

		private bool _activated = false;
		private IGameEventRegistryService _eventFactory;

		/*
		===============
		Activate
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		protected virtual void Activate() {
			var questId = new InternString( _questResource.Get( QuestRepository.QuestIdMetaDataName ).AsStringName() );
			if ( _isStart ) {
				_eventFactory.GetEvent<QuestActivateEventArgs>( EventNames.NAMESPACE, EventNames.QUEST_ACTIVATE_EVENT ).Publish( new QuestActivateEventArgs( questId ) );
				_activated = true;
				return;
			} else if ( _activated ) {
				return;
			}
			_eventFactory.GetEvent<QuestConditionChangedEventArgs>( EventNames.NAMESPACE, EventNames.QUEST_CONDITION_CHANGED_EVENT ).Publish( new QuestConditionChangedEventArgs( questId, new( _conditionName ), GetValue() ) );
			_activated = true;
		}

		/*
		===============
		GetValue
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		/// <exception cref="InvalidCastException"></exception>
		private Any GetValue() => _value.VariantType switch {
			Variant.Type.Bool => Any.From( _value.AsBool() ),
			Variant.Type.Int => Any.From( _value.AsInt32() ),
			Variant.Type.Float => Any.From( _value.AsSingle() ),
			Variant.Type.String => Any.From( new InternString( _value.AsString() ) ),
			Variant.Type.StringName => Any.From( new InternString( _value.AsStringName() ) ),
			_ => throw new InvalidCastException()
		};

		/*
		===============
		_Ready
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public override void _Ready() {
			base._Ready();

			_eventFactory = GetNode<NomadBootstrapper>( "/root/NomadBootstrapper" ).ServiceLocator.GetService<IGameEventRegistryService>();
		}
	};
};