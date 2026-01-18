using Godot;

namespace Prefabs {
	/*
	===================================================================================
	
	QuestAreaTrigger
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public partial class QuestAreaTrigger : QuestTriggerDefault {
		[Export]
		private CollisionShape2D _collisionObject;
		[Export]
		private float _minDistance;
		[Export]
		private string _areaName;

		private Node2D _target;
		private readonly Timer _checkTimer = new Timer() {
			WaitTime = 0.25f,
			OneShot = false
		};

		/*
		===============
		OnBodyShapeEntered
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bodyRid"></param>
		/// <param name="body"></param>
		/// <param name="bodyShapeIndex"></param>
		/// <param name="localShapeIndex"></param>
		private void OnBodyShapeEntered( Rid bodyRid, Node2D body, int bodyShapeIndex, int localShapeIndex ) {
			if ( body is Player player ) {
				GD.Print( "Player entered area" );
				_target = player;
				base.Activate();
			}
		}

		/*
		===============
		OnBodyShapeExited
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bodyRid"></param>
		/// <param name="body"></param>
		/// <param name="bodyShapeIndex"></param>
		/// <param name="localShapeIndex"></param>
		private void OnBodyShapeExited( Rid bodyRid, Node2D body, int bodyShapeIndex, int localShapeIndex ) {
			if ( body is Player ) {
				_target = null;
			}
		}

		/*
		===============
		OnCheckTimerTimeout
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		private void OnCheckTimerTimeout() {
			if ( _target.GlobalPosition.DistanceTo( GlobalPosition ) < _minDistance ) {
				base.Activate();
			}
		}

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

			_checkTimer.Connect( Timer.SignalName.Timeout, Callable.From( OnCheckTimerTimeout ) );
			AddChild( _checkTimer );
			
			var area2D = GetNode<Area2D>( "Area2D" );
			area2D.Connect( Area2D.SignalName.BodyShapeEntered, Callable.From<Rid, Node2D, int, int>( OnBodyShapeEntered ) );
			area2D.Connect( Area2D.SignalName.BodyShapeExited, Callable.From<Rid, Node2D, int, int>( OnBodyShapeExited ) );

			_collisionObject.Reparent( area2D );
		}
	};
};
