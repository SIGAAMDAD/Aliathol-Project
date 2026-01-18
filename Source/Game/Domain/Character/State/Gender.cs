using System;

namespace Game.Domain.Character.State {
	/// <summary>
	/// 
	/// </summary>
	public enum Gender : byte {
		Male,
		Female,
		TransMale,
		TransFemale,
		NonBinary,

		Count
	};

	/*
	===================================================================================
	
	GenderExtensions
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public static class GenderExtensions {
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
		public static string AsString( this Gender value ) => value switch {
			Gender.Male => "Male",
			Gender.Female => "Female",
			Gender.TransMale => "Transgender Male",
			Gender.TransFemale => "Transgender Female",
			Gender.NonBinary => "Non Binary",
			_ => throw new ArgumentOutOfRangeException( nameof( value ) )
		};
	};
};