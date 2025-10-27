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
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace EventSystem {
	/*
	===================================================================================

	GameEvent

	===================================================================================
	*/
	/// <summary>
	/// <para>Generic class for managing an event.</para>
	/// <para>Inherit from this class to pass data between modules and events, if not needed, simply name the event, then publish it.</para>
	/// </summary>
	/// <example>
	/// Here's an example of how to use the <see cref="GameEvent"/> class (without any inheritance).
	/// <code lang="csharp">
	/// public class Example {
	/// 	public static void Main() {
	/// 		GameEvent gameEvent = new GameEvent( "ExampleEvent" );
	/// 		gameEvent.Subscribe( DoSomething } ); // "subscribe" to the event, this function will be called into every time the event is published
	/// 		gameEvent.Publish(); // this will publish the event to the GameEventBus, ensuring that all subscribers are notified that the event has been triggered.
	/// 	}
	/// 	
	/// 	// this will be called whenever the ExampleEvent is triggered
	/// 	public void DoSomething( in IGameEvent eventData, in IEventArgs args ) {
	/// 		Debug.Log( "Foo" );
	/// 	}
	/// };
	/// </code>
	/// </example>

	public class GameEvent : IGameEvent {
		/// <summary>
		/// Represents no arguments.
		/// </summary>
		public static readonly EmptyEventArgs EmptyArgs = new EmptyEventArgs();

		/// <summary>
		/// The name of the event.
		/// </summary>
		public string? Name => _name;
		private readonly string? _name;

		/// <summary>
		/// Other event objects that will automatically trigger whenever this event fires.
		/// </summary>
		public readonly List<IGameEvent> Friends = new List<IGameEvent>();

		/*
		===============
		GameEvent
		===============
		*/
		/// <summary>
		/// Creates a new GameEvent object with the debugging alias of <paramref name="name"/>.
		/// </summary>
		/// <param name="name">The name of the event, should be unique.</param>
		/// <exception cref="ArgumentException">Thrown if name is null or empty.</exception>
		public GameEvent( string? name ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			
			_name = name;
		}

		/*
		===============
		GameEvent
		===============
		*/
		/// <summary>
		/// Shouldn't be called, use <see cref="GameEvent( string? name )"/> instead. This is because while debugging, we want to know
		/// which events are being manipulated, both when and where.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		private GameEvent() {
			throw new InvalidOperationException( "Call GameEvent( string? name ) instead" );
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
			GC.SuppressFinalize( this );
		}

		/*
		===============
		Publish
		===============
		*/
		/// <summary>
		/// Publishes an event to the main <see cref="GameEventBus"/>.
		/// </summary>
		/// <param name="eventArgs"></param>
		/// <param name="singleThreaded"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void Publish( in IEventArgs eventArgs, bool singleThreaded = false ) {
			GameEventBus.Publish( this, in eventArgs, singleThreaded );
			for ( int i = 0; i < Friends.Count; i++ ) {
				Friends[ i ].Publish( in eventArgs );
			}
		}

		/*
		===============
		Subscribe
		===============
		*/
		/// <summary>
		/// Adds a new subscription to the GameEvent utilizing the <see cref="GameEventBus"/>.
		/// </summary>
		/// <param name="subscriber"></param>
		/// <param name="callback">The lambda or method to call when the event is triggered.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="callback"/> is null.</exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void Subscribe( object? subscriber, IGameEvent.EventCallback? callback ) {
			ArgumentNullException.ThrowIfNull( callback );
			GameEventBus.Subscribe( subscriber, this, callback );
		}

		/*
		===============
		Unsubscribe
		===============
		*/
		/// <summary>
		/// Removes the <paramref name="callback"/> from the GameEvent utilizing the <see cref="GameEventBus"/>.
		/// </summary>
		/// <param name="subscriber"></param>
		/// <param name="callback">The lambda or method to remove from the subscription list.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="callback"/> is null.</exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void Unsubscribe( object? subscriber, IGameEvent.EventCallback? callback ) {
			ArgumentNullException.ThrowIfNull( callback );
			GameEventBus.Unsubscribe( subscriber, this, callback );
		}

		/*
		===============
		Bind
		===============
		*/
		/// <summary>
		/// Links the provided <paramref name="otherEvent"/> game event to this event, so that whenever this event fires, the linked "friend"
		/// event will also fire. Basically, this creates an event alias.
		/// </summary>
		/// <param name="otherEvent">The event to pair to this event.</param>
		/// <exception cref="InvalidCastException">Thrown if <paramref name="otherEvent"/> doesn't match this event's type.</exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void Bind( in IGameEvent? otherEvent ) {
			ArgumentNullException.ThrowIfNull( otherEvent );
			if ( !otherEvent.GetType().Equals( GetType() ) ) {
				throw new InvalidCastException( nameof( otherEvent ) );
			}

			// prevent a self-bind so that we don't get any stack overflows
			if ( otherEvent == this ) {
				throw new InvalidOperationException( "You cannot bind an event to itself!" );
			}

			if ( HasFriend( in otherEvent, out _ ) ) {
				Console.PrintWarning( $"GameEvent.Bind: friendship between events '{Name}' and '{otherEvent.Name}' created twice." );
				return;
			}
			Friends.Add( otherEvent );
		}

		/*
		===============
		Unbind
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="otherEvent"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void Unbind( in IGameEvent? otherEvent ) {
			ArgumentNullException.ThrowIfNull( otherEvent );

			// prevent a self-bind so that we don't get any stack overflows
			if ( otherEvent == this ) {
				throw new InvalidOperationException( "You cannot bind an event to itself!" );
			}

			if ( !HasFriend( in otherEvent, out int index ) ) {
				Console.PrintError( $"GameEvent.Unbind: no friendship found between event '{Name}' and '{otherEvent.Name}'" );
				return;
			}
			Friends.RemoveAt( index );
		}

		/*
		===============
		HasFriend
		===============
		*/
		private bool HasFriend( in IGameEvent? otherEvent, out int index ) {
			for ( int i = 0; i < Friends.Count; i++ ) {
				if ( Friends[ i ].Equals( otherEvent ) ) {
					index = i;
					return true;
				}
			}
			index = -1;
			return false;
		}
	};
};