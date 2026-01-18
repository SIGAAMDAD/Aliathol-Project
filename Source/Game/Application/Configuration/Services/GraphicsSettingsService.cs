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
using Game.Application.Configuration.Enums;
using Game.Application.Configuration.Registries;
using Game.Domain.Configuration.Enums;
using Nomad.Core.Exceptions;
using Nomad.CVars;
using System.Collections.Generic;

namespace Game.Application.Configuration.Services {
	/*
	===================================================================================
	
	GraphicsSettingsService
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public sealed class GraphicsSettingsService : IGraphicsSettingsService {
		private readonly record struct EffectsConfiguration( GraphicsSettingsService service ) : IEffectsConfiguration {
			private readonly GraphicsSettingsService _service = service;

			public AnimationQuality AnimationQuality {
				get => _service._config.Effects.AnimationQuality;
				set => _service._config = _service._config with {
					Effects = _service._config.Effects with { AnimationQuality = value }
				};
			}
			public ParticleQuality ParticleQuality {
				get => _service._config.Effects.ParticleQuality;
				set => _service._config = _service._config with {
					Effects = _service._config.Effects with { ParticleQuality = value }
				};
			}

			public void Set( EffectsConfig config ) {
				_service._config.Effects = config;
			}
		};
		private readonly record struct LightingConfiguration( GraphicsSettingsService service ) : ILightingConfiguration {
			private readonly GraphicsSettingsService _service = service;

			public bool BakedLights {
				get => _service._config.Lighting.BakedLights;
				set => _service._config = _service._config with {
					Lighting = _service._config.Lighting with { BakedLights = value }
				};
			}
			public bool PhysicallyBasedRendering {
				get => _service._config.Lighting.PhysicallyBasedRendering;
				set => _service._config = _service._config with {
					Lighting = _service._config.Lighting with { PhysicallyBasedRendering = value }
				};
			}
			public bool BloomEnabled {
				get => _service._config.Lighting.BloomEnabled;
				set => _service._config = _service._config with {
					Lighting = _service._config.Lighting with { BloomEnabled = value }
				};
			}
			public bool ForceVertexShading {
				get => _service._config.Lighting.ForceVertexShading;
				set => _service._config = _service._config with {
					Lighting = _service._config.Lighting with { ForceVertexShading = value }
				};
			}

			public void Set( LightingConfig config ) {
				_service._config.Lighting = config;
			}
		};
		private readonly record struct ShadowConfiguration( GraphicsSettingsService service ) : IShadowConfiguration {
			private readonly GraphicsSettingsService _service = service;

			public ShadowFilterQuality ShadowFilterType {
				get => _service._config.Shadows.ShadowFilterType;
				set => _service._config = _service._config with {
					Shadows = _service._config.Shadows with { ShadowFilterType = value }
				};
			}
			public ShadowAtlasSize ShadowAtlasSize {
				get => _service._config.Shadows.ShadowAtlasSize;
				set => _service._config = _service._config with {
					Shadows = _service._config.Shadows with { ShadowAtlasSize = value }
				};
			}
			public float ShadowFilterSmooth {
				get => _service._config.Shadows.ShadowFilterSmooth;
				set => _service._config = _service._config with {
					Shadows = _service._config.Shadows with { ShadowFilterSmooth = value }
				};
			}

			public void Set( ShadowConfig config ) {
				_service._config.Shadows = config;
			}
		};

		public IEffectsConfiguration Effects => _effectsConfig;
		private readonly EffectsConfiguration _effectsConfig;

		public ILightingConfiguration Lighting => _lightingConfig;
		private readonly LightingConfiguration _lightingConfig;

		public IShadowConfiguration Shadow => _shadowConfig;
		private readonly ShadowConfiguration _shadowConfig;

		private GraphicsConfig _config;

		private readonly ICVarSystemService _cvarSystem;

		/*
		===============
		GraphicsSettingsService
		===============
		*/
		public GraphicsSettingsService( ICVarSystemService cvarSystem ) {
			GraphicsCVars.Register( cvarSystem );

			_effectsConfig = new EffectsConfiguration( this );
			_lightingConfig = new LightingConfiguration( this );
			_shadowConfig = new ShadowConfiguration( this );

			_cvarSystem = cvarSystem;
			_config = new GraphicsConfig {
				Lighting = new LightingConfig {
					BakedLights = GetCVar<bool>( "r.BakedLights" ).Value,
					BloomEnabled = GetCVar<bool>( "r.BloomEnabled" ).Value,
					PhysicallyBasedRendering = GetCVar<bool>( "r.PhysicallyBasedRendering" ).Value,
					
				},
				Shadows = new ShadowConfig {
					ShadowAtlasSize = GetCVar<ShadowAtlasSize>( "r.ShadowAtlasSize" ).Value,
					ShadowFilterSmooth = GetCVar<float>( "r.ShadowFilterSmooth" ).Value,
					ShadowFilterType = GetCVar<ShadowFilterQuality>( "r.ShadowFilterType" ).Value
				},
				Effects = new EffectsConfig {
					AnimationQuality = GetCVar<AnimationQuality>( "r.AnimationQuality" ).Value,
					ParticleQuality = GetCVar<ParticleQuality>( "r.ParticleQuality" ).Value
				}
			};
		}

		/*
		===============
		SetConfig
		===============
		*/
		public void SetConfig( GraphicsConfig config ) {
			_config = config;
		}

		private ICVar<T> GetCVar<T>( string name ) =>
			_cvarSystem.GetCVar<T>( name ) ?? throw new CVarMissing( name );

		public GraphicsConfig GetCurrentConfig() {
			throw new System.NotImplementedException();
		}

		public IReadOnlyList<string> GetShadowAtlasSizes() {
			throw new System.NotImplementedException();
		}

		public IReadOnlyList<string> GetEffectsQualityList() {
			throw new System.NotImplementedException();
		}

		public IReadOnlyList<string> GetLightingQualityList() {
			throw new System.NotImplementedException();
		}

		public IReadOnlyList<string> GetShadowFilterQualityList() {
			throw new System.NotImplementedException();
		}

		public void SetPreset( QualitySetting preset ) {
			throw new System.NotImplementedException();
		}

		public QualitySetting GetPreset() {
			throw new System.NotImplementedException();
		}

		public QualitySetting GetEffectsPreset() {
			throw new System.NotImplementedException();
		}

		public QualitySetting GetLightingPreset() {
			throw new System.NotImplementedException();
		}

		public QualitySetting GetShadowPreset() {
			throw new System.NotImplementedException();
		}
	};
};