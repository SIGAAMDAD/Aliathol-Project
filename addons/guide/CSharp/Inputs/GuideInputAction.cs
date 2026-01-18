using Godot;

namespace Guide.Inputs
{
    /// <summary>
    /// An input that mirrors the action's value while the action is triggered.
    /// </summary>
    [Tool]
    public partial class GUIDEInputAction : GUIDEInput
    {
        /// <summary>
        /// The action that this input should mirror. This is live tracked, so any change in
        /// the action will update the input.
        /// </summary>
        [Export]
        public GUIDEAction Action
        {
            get => _action;
            set
            {
                if (value == _action)
                    return;
                _action = value;
                EmitChanged();
            }
        }
        private GUIDEAction _action;

        public override void _BeginUsage()
        {
            if (IsInstanceValid(_action))
            {
                _action.Triggered += _On;
                _action.Completed += _Off;
                _action.Ongoing += _Off;
                if (_action.IsTriggered())
                {
                    _On();
                    return;
                }
            }
            // not triggered or no action.
            _Off();
        }

        public override void _EndUsage()
        {
            if (IsInstanceValid(_action))
            {
                _action.Triggered -= _On;
                _action.Completed -= _Off;
                _action.Ongoing -= _Off;
            }
        }

        private void _On()
        {
            // on is only called when the action is actually existing, so this is
            // always not-null here
            _Value = _action.ValueAxis3D;
        }

        private void _Off()
        {
            _Value = Vector3.Zero;
        }

        public override bool IsSameAs(GUIDEInput other)
        {
            return other is GUIDEInputAction actionInput && actionInput._action == _action;
        }

        public override string _ToString()
        {
            return $"(GUIDEInputAction: {_action})";
        }

        public override string _EditorName()
        {
            return "Action";
        }

        public override string _EditorDescription()
        {
            return "An input that mirrors the action's value while the action is triggered.";
        }

        public override GUIDEAction.GUIDEActionValueType _NativeValueType()
        {
            return GUIDEAction.GUIDEActionValueType.AXIS_3D;
        }
    }
}