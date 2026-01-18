/*
===========================================================================
The Nomad MPL Source Code
Copyright (C) 2025-2026 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/

using Game.Infrastructure.UI.NomadUI.SelectionNodes.Interfaces;
using Godot;
using Nomad.Core.Util;

namespace Game.Infrastructure.UI.NomadUI.SelectionNodes.NomadButton {
	/*
	===================================================================================
	
	NomadButtonView
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// 
	/// </remarks>
	/// <param name="owner"></param>
	/// <param name="animationConfig"></param>

	public class NomadButtonView( NomadButtonNode owner, NomadButtonAnimation animationConfig ) : INomadButtonView {
		private static readonly NodePath @PositionNodePath = "position";
		private static readonly NodePath @ScaleNodePath = "scale";

		public InternString ButtonId => _buttonId;
		private readonly InternString _buttonId = new( owner.GetPath() );

		public NomadButtonNode Owner => owner;

		/*
		===============
		SetText
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="text"></param>
		public void SetText( string text ) {
			owner.Text = text;
		}

		/*
		===============
		AnimateHover
		===============
		*/
		/// <summary>
		/// Activates button animations.
		/// </summary>
		public void AnimateHover() {
			HoverPositionAnimation();
			HoverScaleAnimation();
		}

		/*
		===============
		HoverPositionAnimation
		===============
		*/
		private void HoverPositionAnimation() {
			if ( !animationConfig.AnimatePosition ) {
				return;
			}
			Tweening(
				PositionNodePath,
				owner.IsFocused ? animationConfig.PositionValue : Vector2.Zero,
				animationConfig.Duration
			);
		}

		/*
		===============
		HoverScaleAnimation
		===============
		*/
		private void HoverScaleAnimation() {
			if ( !animationConfig.AnimateScale ) {
				return;
			}
			Tweening(
				ScaleNodePath,
				owner.IsFocused ? new Vector2( animationConfig.ScaleIntensity, animationConfig.ScaleIntensity ) : Vector2.One,
				animationConfig.Duration
			);
		}

		/*
		===============
		Tweening
		===============
		*/
		private async void Tweening( NodePath property, Variant finalValue, float duration ) {
			Tween tween = owner.CreateTween().SetTrans( animationConfig.TransitionType );
			tween.TweenProperty( owner, property, finalValue, duration );
			await owner.ToSignal( tween, Tween.SignalName.Finished );
			tween.Kill();
		}
	};
};