using Godot;
using System.Collections.Generic;

namespace Guide {
	[Icon( "res://addons/guide/guide_internal.svg" )]
	[Tool]
	public partial class GUIDEActionMapping : Resource {
		[Export]
		public GUIDEAction Action {
			get => _action;
			set {
				if ( _action == value ) return;
				_action = value;
				EmitChanged();
			}
		}
		private GUIDEAction _action;

		[Export]
		public GUIDEInputMapping[] InputMappings {
			get => _inputMappings;
			set {
				if ( _inputMappings == value ) return;
				_inputMappings = value;
				EmitChanged();
			}
		}
		private GUIDEInputMapping[] _inputMappings;
	}
};