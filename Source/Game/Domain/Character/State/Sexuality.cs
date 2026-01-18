using System;

namespace Game.Domain.Character.State {
	public enum Sexuality : byte {
		Heterosexual,
		Bisexual,
		Homosexual,
		Pansexual,

		Count
	};

	/*
	===================================================================================
	
	SexualityExtensions
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public static class SexualityExtensions {
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
		public static string AsString( this Sexuality value ) => value switch {
			Sexuality.Heterosexual => "Heterosexual",
			Sexuality.Bisexual => "Bisexual",
			Sexuality.Homosexual => "Homosexual",
			Sexuality.Pansexual => "Pansexual",
			_ => throw new ArgumentOutOfRangeException( nameof( value ) )
		};
	};
};