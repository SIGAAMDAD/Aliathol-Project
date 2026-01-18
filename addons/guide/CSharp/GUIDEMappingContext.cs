using Godot;
using System.Collections.Generic;

namespace Guide {
	[Tool]
	[Icon( "res://addons/guide/guide_mapping_context.svg" )]
	public partial class GUIDEMappingContext : Resource {
		[Signal]
		public delegate void EnabledEventHandler();

		[Signal]
		public delegate void DisabledEventHandler();

		[Export]
		public string DisplayName {
			get => _displayName;
			set {
				if ( _displayName == value ) return;
				_displayName = value;
				EmitChanged();
			}
		}
		private string _displayName;

		[Export]
		public GUIDEActionMapping[] Mappings {
			get => _mappings;
			set {
				if ( _mappings == value ) return;
				_mappings = value;
				EmitChanged();
			}
		}
		private GUIDEActionMapping[] _mappings;

		public string _EditorName() {
			if ( !string.IsNullOrEmpty( _displayName ) )
				return _displayName;
			else
				return ResourcePath.GetFile();
		}
	}
};