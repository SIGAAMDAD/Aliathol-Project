/*
===========================================================================
The Nomad AGPL Source Code
Copyright (C) 2025 Noah Van Til

The Nomad Source Code is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published
by the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

The Nomad Source Code is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with The Nomad Source Code.  If not, see <http://www.gnu.org/licenses/>.

If you have questions concerning this license or the applicable additional
terms, you may contact me via email at nyvantil@gmail.com.
===========================================================================
*/

using System;

namespace EventSystem {
	/*
	===================================================================================
	
	IGameEvent
	
	===================================================================================
	*/
	/// <summary>
	/// The base interface for all events sent through the <see cref="GameEventBus"/>.
	/// </summary>
	/// <remarks>
	/// For any C programmers, treat this like a void* but for <see cref="GameEvent"/>.
	/// </remarks>

	public interface IGameEvent : IDisposable {
		public string? Name { get; }

		/// <summary>
		/// The default event callback function.
		/// </summary>
		/// <param name="eventBase">The event being triggered.</param>
		/// <param name="args">The arguments structure being passed between modules.</param>
		public delegate void EventCallback( in IGameEvent eventBase, in IEventArgs args );

		public void Publish( in IEventArgs eventArgs, bool singleThreaded = false );
	};
};