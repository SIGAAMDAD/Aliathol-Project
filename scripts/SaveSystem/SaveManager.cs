using EventSystem;
using Godot;
using Menus;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SaveSystem {
	/*
	===================================================================================
	
	SaveManager
	
	===================================================================================
	*/
	/// <summary>
	/// Manages save slots, loading and writing game state to and from disk.
	/// </summary>

	public partial class SaveManager : Node {
		public const int MAX_SLOTS = 3;

		public readonly List<SaveSlot> Slots = new List<SaveSlot>( MAX_SLOTS );

		private int CurrentSlot = 0;

		private static SaveManager Instance;

		public static readonly GameEvent SaveGame = new GameEvent( nameof( SaveGame ) );
		public static readonly GameEvent LoadGame = new GameEvent( nameof( LoadGame ) );

		/*
		===============
		Save
		===============
		*/
		public static void Save() {
			SaveGame.Publish( GameEvent.EmptyArgs );
		}

		/*
		===============
		Load
		===============
		*/
		public static void Load() {
			LoadGame.Publish( GameEvent.EmptyArgs );
		}

		/*
		===============
		SetSlot
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="slot"></param>
		/// <param name="name"></param>
		public static void SetSlot( int slot, string? name ) {
			ArgumentOutOfRangeException.ThrowIfLessThan( slot, 0, nameof( slot ) );
			ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual( slot, MAX_SLOTS, nameof( slot ) );
			ArgumentException.ThrowIfNullOrEmpty( name );

			if ( !Instance.Slots[ slot ].Metadata.Exists ) {
				Instance.CurrentSlot = slot;
				Instance.Slots[ Instance.CurrentSlot ].Create( name );
			}
		}

		/*
		===============
		HasProgressInCurrentSlot
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static bool HasProgressInCurrentSlot() {
			return Instance.Slots[ Instance.CurrentSlot ].Metadata.Completed > 0.0f;
		}

		/*
		===============
		DeleteSlot
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="slot"></param>
		public static void DeleteSlot( int slot ) {
			Instance.Slots[ slot ].Delete();
		}

		/*
		===============
		GetSlotInfo
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="slot"></param>
		/// <returns></returns>
		public static SlotMetadata GetSlotInfo( int slot ) {
			if ( slot < 0 || slot >= MAX_SLOTS ) {
				throw new ArgumentOutOfRangeException( nameof( slot ) );
			}
			return Instance.Slots[ slot ].Metadata;
		}

		/*
		===============
		LoadSaveSlots
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		private void LoadSaveSlots() {
			for ( int i = 0; i < MAX_SLOTS; i++ ) {
				Slots.Add( new SaveSlot( i ) );
			}
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

			LoadSaveSlots();

			Instance = this;
		}
	};
};