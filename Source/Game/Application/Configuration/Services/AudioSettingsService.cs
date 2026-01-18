/*
===========================================================================
The Nomad MPL Source Code
Copyright (C) 2025-2026 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/

using Game.Application.Common.Models.Interfaces;
using Game.Application.Common.Models.ValueObjects;
using Nomad.Core;
using Nomad.Audio.Interfaces;
using Nomad.Core.Exceptions;
using Nomad.CVars;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace Game.Application.Configuration.Services {
	/*
	===================================================================================
	
	AudioSettingsService
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	internal sealed class AudioSettingsService : IAudioSettingsService {
		private readonly record struct AudioConfiguration( AudioSettingsService owner ) : IAudioConfiguration {
			private readonly AudioSettingsService _owner = owner;

			public bool EffectsOn {
				get => _owner._config.EffectsOn;
				set => _owner._config = _owner._config with { EffectsOn = value };
			}
			public bool MusicOn {
				get => _owner._config.MusicOn;
				set => _owner._config = _owner._config with { MusicOn = value };
			}
			public float EffectsVolume {
				get => _owner._config.EffectsVolume;
				set => _owner._config = _owner._config with { EffectsVolume = value };
			}
			public float MusicVolume {
				get => _owner._config.MusicVolume;
				set => _owner._config = _owner._config with { MusicVolume = value };
			}
			public float MasterVolume {
				get => _owner._config.MasterVolume;
				set => _owner._config = _owner._config with { MasterVolume = value };
			}
			public int OutputAudioDevice {
				get => _owner._config.OutputDeviceIndex;
				set => _owner._config = _owner._config with { OutputDeviceIndex = value };
			}

			public string AudioDriver {
				get => _owner._config.AudioDriver;
				set => _owner._config = _owner._config with { AudioDriver = value };
			}
		};

		public IAudioConfiguration Config => _audioConfig;
		private readonly AudioConfiguration _audioConfig;

		private AudioConfig _config;

		private readonly ICVarSystemService _cvarSystem;
		private readonly IAudioDevice _driver;

		/*
		===============
		AudioSettingsService
		===============
		*/
		public AudioSettingsService( ICVarSystemService cvarSystem, IAudioDevice driverService ) {
			_cvarSystem = cvarSystem;
			_driver = driverService;

			var audioDriver = GetCVar<string>( Constants.CVars.Audio.AUDIO_DRIVER );
			audioDriver.Value = _driver.AudioDriver;

			_audioConfig = new AudioConfiguration( this );
			
			_config = new AudioConfig {
				EffectsOn = GetCVar<bool>( Constants.CVars.Audio.EFFECTS_ON ).Value,
				EffectsVolume = GetCVar<float>( Constants.CVars.Audio.EFFECTS_VOLUME ).Value,
				MusicOn = GetCVar<bool>( Constants.CVars.Audio.MUSIC_ON ).Value,
				MusicVolume = GetCVar<float>( Constants.CVars.Audio.MUSIC_VOLUME ).Value,
				AudioDriver = audioDriver.Value,
				OutputDeviceIndex = GetCVar<int>( Constants.CVars.Audio.OUTPUT_DEVICE_INDEX ).Value
			};
		}

		/*
		===============
		SetConfig
		===============
		*/
		public void SetConfig( AudioConfig config ) {
			_config = config;
		}

		/*
		===============
		GetAudioDevices
		===============
		*/
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public IEnumerable<string> GetAudioDevices()
			=> _driver.GetOutputDevices();

		/*
		===============
		GetAudioDrivers
		===============
		*/
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public IEnumerable<string> GetAudioDrivers()
			=> _driver.GetAudioDrivers();

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private ICVar<T> GetCVar<T>( string name ) =>
			_cvarSystem.GetCVar<T>( name ) ?? throw new CVarMissing( name );
	};
};