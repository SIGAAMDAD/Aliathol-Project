using EventSystem;
using Godot;
using System.Runtime.CompilerServices;

namespace Menus.SelectionNodes {
	/*
	===================================================================================
	
	Button
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	public partial class Button : Godot.Button, ISelectionNode {
		private static readonly StringName @HoverThemeStyleBoxName = "hover";
		private static readonly NodePath @ModulateColorThemePropertyName = "modulate_color";

		[Export( PropertyHint.Range, "0,10,0.001,or_greater" )]
		public float Duration = 1.0f;

		[Export]
		public bool AnimateScale = true;
		[Export]
		public bool AnimatePosition = false;
		[Export]
		public Tween.TransitionType TransitionType;

		[ExportGroup( "Scale Properties", "scale_" )]
		[Export]
		public float ScaleIntensity = 1.10f;

		[ExportGroup( "Position Properties", "position_" )]
		[Export]
		public Vector2 PositionValue = new Vector2( 0.0f, -4.0f );

		/// <summary>
		/// s
		/// </summary>
		public bool IsFocused => _isFocused;
		private bool _isFocused = false;
		
		/// <summary>
		/// 
		/// </summary>
		public StyleBoxTexture FocusedStyleBox => StyleBox;

		private Tween AnimationTween;
		private StyleBoxTexture StyleBox;

		private Tween Tween;
		private Vector2 ButtonStartPos = Vector2.Zero;

		/// <summary>
		/// 
		/// </summary>
		public readonly UIEvent Activated;

		/*
		===============
		Button
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public Button() {
			Activated = new UIEvent( this, nameof( Activated ) );
		}

		/*
		===============
		DisableMouseFocus
		===============
		*/
		/// <summary>
		/// Disables the focus of another UI element pinned by the mouse.
		/// </summary>
		public void DisableMouseFocus() {
			Control focusNode = GetViewport().GuiGetHoveredControl();
			if ( focusNode != null && focusNode is Button button ) {
				button.OnUnfocused();
			}
		}

		/*
		===============
		OnFocused
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public virtual void OnFocused() {
			DisableMouseFocus();
			_isFocused = true;

			//UIAudioManager.OnButtonFocused();
		}

		/*
		===============
		OnUnfocused
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public void OnUnfocused() {
			_isFocused = false;
		}

		/*
		===============
		HoverScaleAnimation
		===============
		*/
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private void HoverScaleAnimation() {
			if ( !AnimateScale ) {
				return;
			}
			Tweening(
				this,
				"scale",
				_isFocused ? new Vector2( ScaleIntensity, ScaleIntensity ) : Vector2.One,
				Duration
			);
		}

		/*
		===============
		HoverPositionAnimation
		===============
		*/
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private void HoverPositionAnimation() {
			if ( !AnimatePosition ) {
				return;
			}
			Tweening(
				this,
				"position",
				_isFocused ? ButtonStartPos + PositionValue : ButtonStartPos,
				Duration
			);
		}

		/*
		===============
		Tweening
		===============
		*/
		private async void Tweening( GodotObject obj, NodePath property, Variant finalValue, float duration ) {
			Tween = CreateTween().SetParallel( true ).SetTrans( TransitionType );
			Tween.TweenProperty( obj, property, finalValue, duration );
			await ToSignal( Tween, Tween.SignalName.Finished );
			Tween.Kill();
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

			GameEventBus.ConnectSignal( this, Button.SignalName.FocusEntered, this, Callable.From( OnFocused ) );
			GameEventBus.ConnectSignal( this, Button.SignalName.MouseEntered, this, Callable.From( OnFocused ) );
			GameEventBus.ConnectSignal( this, Button.SignalName.FocusExited, this, Callable.From( OnUnfocused ) );
			GameEventBus.ConnectSignal( this, Button.SignalName.MouseExited, this, Callable.From( OnUnfocused ) );
			GameEventBus.ConnectSignal( this, Button.SignalName.Pressed, this, Callable.From( () => Activated.Publish( GameEvent.EmptyArgs ) ) );

			StyleBox = (StyleBoxTexture)GetThemeStylebox( HoverThemeStyleBoxName );
		}

		/*
		===============
		_Process
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="delta"></param>
		public override void _Process( double delta ) {
			base._Process( delta );

			HoverScaleAnimation();
			HoverPositionAnimation();
		}
	};
};