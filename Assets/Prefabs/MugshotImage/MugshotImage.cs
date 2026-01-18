using Game.Domain.UserInterface.Data;
using Game.Domain.UserInterface.Events;
using Game.Infrastructure;
using Godot;
using Nomad.Core.Events;
using Nomad.Core.Util;

namespace Game.Presentation.Screens.CharacterCreationMenu.MugshotList {
	/*
	===================================================================================
	
	MugshotImage
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public partial class MugshotImage : HBoxContainer {
		public InternString Id;
		public Texture2D Image;

		private VSeparator _marker => GetNode<VSeparator>( "Marker" );

		[Signal]
		public delegate void FocusedEventHandler( int id );

		private IGameEvent<ButtonClickedEventArgs> _clicked => GetNode<NomadBootstrapper>( "/root/NomadBootstrapper" ).ServiceLocator.GetService<IGameEventRegistryService>().GetEvent<ButtonClickedEventArgs>( EventNames.NAMESPACE, EventNames.BUTTON_CLICKED_EVENT );

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

			CallDeferred( MethodName.LinkFocusNodes );

			_marker.Hide();

			var image = GetNode<TextureRect>( "Image" );
			image.Texture = Image;

			var eventBus = GetNode<NomadBootstrapper>( "/root/NomadBootstrapper" ).ServiceLocator.GetService<IGodotEventBusService>();
			eventBus.ConnectSignal( this, MugshotImage.SignalName.FocusEntered, this, Callable.From( OnFocusEntered ) );
			eventBus.ConnectSignal( this, MugshotImage.SignalName.FocusExited, this, Callable.From( _marker.Hide ) );

			FocusMode = HBoxContainer.FocusModeEnum.All;
			GrabFocus();
		}

		/*
		===============
		LinkFocusNodes
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		private void LinkFocusNodes() {
			NodePath path = GetPath();

			FocusNeighborLeft = path;
			FocusNeighborRight = path;

			int index = GetIndex();
			Node parent = GetParent();
			int childCount = parent.GetChildCount();
			if ( childCount == 1 ) {
				FocusNeighborTop = path;
				FocusNeighborBottom = path;
			} else if ( index == 0 ) {
				FocusNeighborTop = parent.GetChild( childCount - 1 ).GetPath();
				FocusNeighborBottom = parent.GetChild( index + 1 ).GetPath();
			} else if ( index == childCount - 1 ) {
				FocusNeighborTop = parent.GetChild( index - 1 ).GetPath();
				FocusNeighborBottom = parent.GetChild( 0 ).GetPath();
			} else {
				FocusNeighborTop = parent.GetChild( index - 1 ).GetPath();
				FocusNeighborBottom = parent.GetChild( index + 1 ).GetPath();
			}
			FocusNeighborLeft = path;
			FocusNeighborRight = path;
			FocusNext = path;
			FocusPrevious = path;
		}

		/*
		===============
		OnMouseEntered
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		private void OnMouseEntered() {
		}

		/*
		===============
		OnFocusEntered
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		private void OnFocusEntered() {
			_marker.Show();
			EmitSignal( MugshotImage.SignalName.Focused, Id.GetHashCode() );
			_clicked.Publish( new ButtonClickedEventArgs( Id ) );
		}
	};
};