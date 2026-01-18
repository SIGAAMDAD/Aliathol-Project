using Game.Application.Character.Interfaces;
using Game.Domain.Character.Events;
using Nomad.Core.Events;
using System;
using Game.Domain.Character.Data;
using Game.Domain.Character.Models;
using System.Collections.Immutable;
using Game.Domain.Character.State;

namespace Game.Application.Character.Services {
	/*
	===================================================================================
	
	CharacterDataService
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal sealed class CharacterDataService : IDisposable {
		public ImmutableArray<string> AgeStrings => _ageStrings;
		private readonly ImmutableArray<string> _ageStrings;

		public ImmutableArray<string> GenderStrings => _genderStrings;
		private readonly ImmutableArray<string> _genderStrings;

		public ImmutableArray<string> SexualityStrings => _sexualityStrings;
		private readonly ImmutableArray<string> _sexualityStrings;

		private readonly ICharacterDataProvider _dataProvider;

		private PlayerCharacterData _characterData;

		public IGameEvent<CharacterClassSelectedEventArgs> ClassSelected => _classSelected;
		private readonly IGameEvent<CharacterClassSelectedEventArgs> _classSelected;

		public IGameEvent<CharacterRaceSelectedEventArgs> RaceSelected => _raceSelected;
		private readonly IGameEvent<CharacterRaceSelectedEventArgs> _raceSelected;

		public IGameEvent<CharacterOriginSelectedEventArgs> OriginSelected => _originSelected;
		private readonly IGameEvent<CharacterOriginSelectedEventArgs> _originSelected;

		public IGameEvent<CharacterAgeSelectedEventArgs> AgeSelected => _ageSelected;
		private readonly IGameEvent<CharacterAgeSelectedEventArgs> _ageSelected;

		public IGameEvent<CharacterGenderSelectedEventArgs> GenderSelected => _genderSelected;
		private readonly IGameEvent<CharacterGenderSelectedEventArgs> _genderSelected;

		public IGameEvent<CharacterSexualitySelectedEventArgs> SexualitySelected => _sexualitySelected;
		private readonly IGameEvent<CharacterSexualitySelectedEventArgs> _sexualitySelected;

		/*
		===============
		CharacterDataService
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventFactory"></param>
		/// <param name="dataProvider"></param>
		public CharacterDataService( IGameEventRegistryService eventFactory, ICharacterDataProvider dataProvider ) {
			_dataProvider = dataProvider;

			_characterData = new PlayerCharacterData();

			_classSelected = eventFactory.GetEvent<CharacterClassSelectedEventArgs>( EventNames.NAMESPACE, EventNames.CHARACTER_CLASS_SELECTED_EVENT );
			_classSelected.Subscribe( this, OnClassSelected );

			_raceSelected = eventFactory.GetEvent<CharacterRaceSelectedEventArgs>( EventNames.NAMESPACE, EventNames.CHARACTER_RACE_SELECTED_EVENT );
			_raceSelected.Subscribe( this, OnRaceSelected );
			
			_originSelected = eventFactory.GetEvent<CharacterOriginSelectedEventArgs>( EventNames.NAMESPACE, EventNames.CHARACTER_ORIGIN_SELECTED_EVENT );
			_originSelected.Subscribe( this, OnOriginSelected );

			_ageSelected = eventFactory.GetEvent<CharacterAgeSelectedEventArgs>( EventNames.NAMESPACE, EventNames.CHARACTER_AGE_SELECTED_EVENT );
			_ageSelected.Subscribe( this, OnAgeSelected );

			_genderSelected = eventFactory.GetEvent<CharacterGenderSelectedEventArgs>( EventNames.NAMESPACE, EventNames.CHARACTER_GENDER_SELECTED_EVENT );
			_genderSelected.Subscribe( this, OnGenderSelected );

			_sexualitySelected = eventFactory.GetEvent<CharacterSexualitySelectedEventArgs>( EventNames.NAMESPACE, EventNames.CHARACTER_SEXUALITY_SELECTED_EVENT );
			_sexualitySelected.Subscribe( this, OnSexualitySelected );

			var sexualityStrings = new string[ (int)Sexuality.Count ];
			for ( Sexuality sexuality = 0; sexuality < Sexuality.Count; sexuality++ ) {
				sexualityStrings[ (int)sexuality ] = sexuality.AsString();
			}
			_sexualityStrings = [ .. sexualityStrings ];

			var genderStrings = new string[ (int)Gender.Count ];
			for ( Gender gender = 0; gender < Gender.Count; gender++ ) {
				genderStrings[ (int)gender ] = gender.AsString();
			}
			_genderStrings = [ .. genderStrings ];

			var ageStrings = new string[ (int)Age.Count ];
			for ( Age age = 0; age < Age.Count; age++ ) {
				ageStrings[ (int)age ] = age.AsString();
			}
			_ageStrings = [ .. ageStrings ];
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
			_classSelected.Dispose();
			_raceSelected.Dispose();
			_originSelected.Dispose();	
		}

		/*
		===============
		OnClassSelected
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		private void OnClassSelected( in CharacterClassSelectedEventArgs args ) {
			var selected = _dataProvider.GetClassById( args.Id );
			_characterData = _characterData with { ClassId = selected.Id };
		}

		/*
		===============
		OnRaceSelected
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		private void OnRaceSelected( in CharacterRaceSelectedEventArgs args ) {
			var selected = _dataProvider.GetRaceById( args.Id );
			_characterData = _characterData with { RaceId = selected.Id };
		}

		/*
		===============
		OnOriginSelected
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		private void OnOriginSelected( in CharacterOriginSelectedEventArgs args ) {
			var selected = _dataProvider.GetOriginById( args.Id );
			_characterData = _characterData with { OriginId = selected.Id };
		}

		/*
		===============
		OnAgeSelected
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		private void OnAgeSelected( in CharacterAgeSelectedEventArgs args ) {
		}

		/*
		===============
		OnGenderSelected
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		private void OnGenderSelected( in CharacterGenderSelectedEventArgs args ) {
		}

		/*
		===============
		OnSexualitySelected
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		private void OnSexualitySelected( in CharacterSexualitySelectedEventArgs args ) {
		}
	};
};