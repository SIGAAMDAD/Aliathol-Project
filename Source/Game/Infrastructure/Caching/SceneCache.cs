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
using Nomad.Core.Events;
using Nomad.Core.Logger;
using Nomad.Core.Util;
using Nomad.ResourceCache;
using System;

namespace Game.Infrastructure.Caching {
	/*
	===================================================================================
	
	SceneCache
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public static class SceneCache {
		public static BaseCache<PackedScene, FilePath> Instance => _sceneCache.Value;
		private static readonly Lazy<BaseCache<PackedScene, FilePath>> _sceneCache = new Lazy<BaseCache<PackedScene, FilePath>>( Create, true );

		private static BaseCache<PackedScene, FilePath> Create() {
			var bootstrapper = ( (Node)Engine.GetMainLoop().Get( SceneTree.PropertyName.Root ) ).GetNode<NomadBootstrapper>( "/root/NomadBootstrapper" );
			return new BaseCache<PackedScene, FilePath>(
				bootstrapper.ServiceLocator.GetService<ILoggerService>(),
				bootstrapper.ServiceLocator.GetService<IGameEventRegistryService>(),
				new GodotLoader<PackedScene>()
			);
		}
	};
};