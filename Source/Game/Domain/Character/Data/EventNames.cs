namespace Game.Domain.Character.Data {
	/*
	===================================================================================
	
	EventNames
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public static class EventNames {
		public const string NAMESPACE = "Domain:Character";

		public const string CHARACTER_CLASS_SELECTED_EVENT = $"{NAMESPACE}:CharacterClassSelected";
		public const string CHARACTER_RACE_SELECTED_EVENT = $"{NAMESPACE}:CharacterRaceSelected";
		public const string CHARACTER_ORIGIN_SELECTED_EVENT = $"{NAMESPACE}:CharacterOriginSelected";
		public const string CHARACTER_AGE_SELECTED_EVENT = $"{NAMESPACE}:CharacterAgeSelected";
		public const string CHARACTER_GENDER_SELECTED_EVENT = $"{NAMESPACE}:CharacterGenderSelected";
		public const string CHARACTER_SEXUALITY_SELECTED_EVENT = $"{NAMESPACE}:CharacterSexualitySelected";

		public const string PLAYER_SPAWNED_EVENT = $"{NAMESPACE}:PlayerSpawned";
	};
};