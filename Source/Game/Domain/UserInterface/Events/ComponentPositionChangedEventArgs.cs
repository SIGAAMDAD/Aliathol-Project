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

using Nomad.Core.Util;
using System.Numerics;

namespace Game.Domain.UserInterface.Events {
	/// <summary>
	/// Event that triggers when a UI component's position has changed.
	/// </summary>
	/// <param name="ComponentId"></param>
	/// <param name="Position"></param>
	public readonly record struct ComponentPositionChangedEventArgs(
		InternString ComponentId,
		Vector2 Position
	);
};