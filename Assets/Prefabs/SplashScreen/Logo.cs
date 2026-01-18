using Game.Infrastructure;
using Godot;
using Nomad.Core.Events;

namespace Game.Prefabs.SplashScreen {
	/*
	===================================================================================
	
	Logo
	
	===================================================================================
	*/
	/// <summary>
	/// Handles logo showcasing in the splash screen.
	/// </summary>
	
	public partial class Logo : Control {
		[Export]
		private float _duration = 1.0f;
		[Export]
		private Control _logo;

		public IGameEvent<EmptyEventArgs> AnimationFinished => _animationFinished;
		private readonly IGameEvent<EmptyEventArgs> _animationFinished;

		/*
		===============
		Logo
		===============
		*/
		/// <summary>
		/// Creates a Logo node.
		/// </summary>
		public Logo() {
			var eventFactory = GetNode<NomadBootstrapper>( "/root/NomadBootstrapper" ).ServiceLocator.GetService<IGameEventRegistryService>();
			_animationFinished = eventFactory.GetEvent<EmptyEventArgs>( nameof( Logo ), nameof( AnimationFinished ) );
		}

		/*
		===============
		OnFinished
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		private void OnFinished() {
			_animationFinished.Publish( new EmptyEventArgs() );
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

			var godotEventBus = GetNode<NomadBootstrapper>( "/root/NomadBootstrapper" ).ServiceLocator.GetService<IGodotEventBusService>();
			if ( _logo is VideoStreamPlayer player ) {
				godotEventBus.ConnectSignal( player, VideoStreamPlayer.SignalName.Finished, this, OnFinished );
			} else {
				var timer = new Timer() {
					WaitTime = _duration
				};
				timer.CallDeferred( Timer.MethodName.Start );
				godotEventBus.ConnectSignal( timer, Timer.SignalName.Timeout, this, OnFinished );
			}
		}
	};
};