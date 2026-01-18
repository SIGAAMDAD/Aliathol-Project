using Game.Domain.Character.Models.DTOs;
using Nomad.Core.Logger;
using Nomad.Core.Util;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Game.Application.Character.Interfaces {
	/*
	===================================================================================
	
	ICharacterDataProvider
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal interface ICharacterDataProvider {
		ImmutableArray<CharacterClassDto> AvailableClasses { get; }
		ImmutableArray<CharacterRaceDto> AvailableRaces { get; }
		ImmutableArray<CharacterOriginDto> AvailableOrigins { get; }

		Task LoadAll( ILoggerService logger );

		CharacterClassDto GetClassById( InternString id );
		CharacterRaceDto GetRaceById( InternString id );
		CharacterOriginDto GetOriginById( InternString id );
	};
};