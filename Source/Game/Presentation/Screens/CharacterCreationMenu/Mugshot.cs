using Godot;
using Nomad.Core.Util;

namespace Game.Presentation.Screens.CharacterCreationMenu {
	public record Mugshot(
		Texture2D Image,
		InternString Id,
		string Name,
		string Description
	);
};