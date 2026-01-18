using Nomad.Core.Util;
using System;

namespace Game.Domain.Character.Models.DTOs {
	/// <summary>
	/// 
	/// </summary>
	/// <param name="Name"></param>
	/// <param name="Description"></param>
	/// <param name="Mugshot"></param>
	internal record CharacterOriginDto(
		InternString Id,
		InternString Name,
		InternString Description,
		IDisposable Mugshot
	);
};