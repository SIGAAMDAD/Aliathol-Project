using System;

namespace SaveSystem {
	/*
	===================================================================================
	
	SlotMetadata
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public sealed class SlotMetadata {
		public string Name { get; private set; }
		public int AccessedYear { get; private set; }
		public int AccessedMonth { get; private set; }
		public int AccessedDay { get; private set; }
		public float Completed { get; private set; }
		public bool Exists { get; private set; }

		public SlotMetadata( string? name, float completed, bool exists ) {
			ArgumentException.ThrowIfNullOrEmpty( name );

			Name = name;
			AccessedYear = DateTime.Now.Year;
			AccessedMonth = DateTime.Now.Month;
			AccessedDay = DateTime.Now.Day;
			Completed = completed;
			Exists = exists;
		}

		/*
		===============
		Write
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		public void Write( System.IO.BinaryWriter writer ) {
			writer.Write( Name );
			writer.Write( AccessedYear );
			writer.Write( AccessedMonth );
			writer.Write( AccessedDay );
			writer.Write( Completed );
		}

		/*
		===============
		Read
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		public void Read( System.IO.BinaryReader reader ) {
			Name = reader.ReadString();
			AccessedYear = reader.ReadInt32();
			AccessedMonth = reader.ReadInt32();
			AccessedDay = reader.ReadInt32();
			Completed = reader.ReadSingle();
		}
	};
};