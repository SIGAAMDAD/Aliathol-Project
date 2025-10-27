using Godot;
using System;

namespace SaveSystem {
	/*
	===================================================================================
	
	SaveSlot
	
	===================================================================================
	*/
	/// <summary>
	/// Stores the data of the game state for a player's progress.
	/// </summary>
	
	public sealed class SaveSlot {
		private const ulong MAGIC = 0x345FFADE;
		private static readonly string SAVE_DIRECTORY = "user://SaveData";

		public SlotMetadata Metadata { get; private set; }

		public readonly int Slot;
		public readonly string Filepath;

		/*
		===============
		SaveSlot
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="slot"></param>
		public SaveSlot( int slot ) {
			Slot = slot;

			try {
				System.IO.Directory.CreateDirectory( ProjectSettings.GlobalizePath( SAVE_DIRECTORY ) );
			}
			finally {
				bool exists = System.IO.File.Exists( Filepath );
				Metadata = new SlotMetadata( "Unnamed", 0.0f, exists );
				Filepath = ProjectSettings.GlobalizePath( $"{SAVE_DIRECTORY}/data{Slot}.sav" );
				if ( exists ) {
					LoadHeader();
				}
			}
		}

		/*
		===============
		Delete
		===============
		*/
		/// <summary>
		/// Deletes the save slot's file (the associated data).
		/// </summary>
		public void Delete() {
			System.IO.File.Delete( Filepath );
		}

		/*
		===============
		Create
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		public void Create( string? name ) {
			ArgumentException.ThrowIfNullOrEmpty( name );

			using System.IO.FileStream stream = new System.IO.FileStream( Filepath, System.IO.FileMode.Create );
			using System.IO.BinaryWriter writer = new System.IO.BinaryWriter( stream );

			SaveHeader( writer );
		}

		/*
		===============
		LoadHeader
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		public void LoadHeader( System.IO.BinaryReader? reader = null ) {
			if ( reader == null ) {
				using System.IO.FileStream stream = new System.IO.FileStream( Filepath, System.IO.FileMode.Open );
				reader = new System.IO.BinaryReader( stream );
			}

			if ( reader.ReadUInt64() != MAGIC ) {
				return;
			}
			Metadata.Read( reader );
		}

		/*
		===============
		SaveHeader
		===============
		*/
		public void SaveHeader( System.IO.BinaryWriter writer ) {
			writer.Write( MAGIC );
			Metadata.Write( writer );
		}
	};
};