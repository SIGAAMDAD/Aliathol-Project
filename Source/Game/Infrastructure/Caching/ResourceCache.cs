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
	
	ResourceCache
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public static class ResourceCache {
		public static BaseCache<Resource, FilePath> Instance => _resourceCache.Value;
		private static readonly Lazy<BaseCache<Resource, FilePath>> _resourceCache = new Lazy<BaseCache<Resource, FilePath>>( Create, true );

		private static BaseCache<Resource, FilePath> Create() {
			var bootstrapper = ( (Node)Engine.GetMainLoop().Get( SceneTree.PropertyName.Root ) ).GetNode<NomadBootstrapper>( "/root/NomadBootstrapper" );
			return new BaseCache<Resource, FilePath>(
				bootstrapper.ServiceLocator.GetService<ILoggerService>(),
				bootstrapper.ServiceLocator.GetService<IGameEventRegistryService>(),
				new GodotLoader<Resource>()
			);
		}
	};
};