using System;

namespace Game.Domain.Character.State {
	/// <summary>
	/// A character's in-game age.
	/// </summary>
	public enum Age : byte {
		Young,
		MiddleAged,
		Old,
		Ancient,

		Count
	};

	/*
	===================================================================================
	
	AgeExtensions
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public static class AgeExtensions {
		/*
		===============
		AsString
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static string AsString( this Age value ) => value switch {
			Age.Young => "Young",
			Age.MiddleAged => "Middle Aged",
			Age.Old => "Old",
			Age.Ancient => "Ancient",
			_ => throw new ArgumentOutOfRangeException( nameof( value ) )
		};
	};
};