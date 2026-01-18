using Godot;
using Guide.Inputs;
using System.Collections.Generic;

namespace Guide {
	public partial class GUIDEReset : Node {
		public List<GUIDEInput> _InputsToReset = new();

		public override void _EnterTree() {
			ProcessPriority = 10000000;
		}

		public override void _Process( double delta ) {
			foreach ( var input in _InputsToReset )
				input._Reset();

			GUIDE._InputState._Reset();
		}
	}
};