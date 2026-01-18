using Nomad.Core.Util;
using System;

namespace Game.Domain.Character.Models.DTOs {
	internal record CharacterRaceDto(
		InternString Id,
		InternString Name,
		InternString Description,
		IDisposable Mugshot
	);
};