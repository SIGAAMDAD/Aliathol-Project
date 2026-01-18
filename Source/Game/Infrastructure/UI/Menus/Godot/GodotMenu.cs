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

using Godot;

namespace Game.Infrastructure.UI.Menus {
	/*
	===================================================================================
	
	GodotMenu
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal sealed class GodotMenu : IMenu {
		private readonly Control _node;

		/*
		===============
		GodotMenu
		===============
		*/
		/// <summary>
		/// Creates a GodotMenu.
		/// </summary>
		/// <param name="node"></param>
		public GodotMenu( Control node ) {
			_node = node;
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
			_node.Dispose();
		}

		/*
		===============
		Enable
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public void Enable() {
			_node.SetDeferred( Control.PropertyName.ProcessMode, (long)Control.ProcessModeEnum.Always );
			_node.CallDeferred( Control.MethodName.Show );
		}

		/*
		===============
		Disable
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public void Disable() {
			_node.SetDeferred( Control.PropertyName.ProcessMode, (long)Control.ProcessModeEnum.Disabled );
			_node.CallDeferred( Control.MethodName.Hide );
		}
	};
};