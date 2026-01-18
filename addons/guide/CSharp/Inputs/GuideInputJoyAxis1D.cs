using Godot;

namespace Guide.Inputs
{
    /// <summary>
    /// Input from a single joy axis.
    /// </summary>
    [Tool]
    public partial class GUIDEInputJoyAxis1D : GUIDEInputJoyBase
    {
        /// <summary>
        /// The joy axis to sample
        /// </summary>
        [Export]
        public JoyAxis Axis
        {
            get => _axis;
            set
            {
                if (value == _axis)
                    return;
                _axis = value;
                EmitChanged();
            }
        }
        private JoyAxis _axis = JoyAxis.LeftX;

        public override void _BeginUsage()
        {
            _State.JoyAxisStateChanged += _Refresh;
        }

        public override void _EndUsage()
        {
            _State.JoyAxisStateChanged -= _Refresh;
        }

        private void _Refresh()
        {
            _Value = new Vector3(_State.GetJoyAxisValue(JoyIndex, _axis), 0, 0);
        }

        public override bool IsSameAs(GUIDEInput other)
        {
            return other is GUIDEInputJoyAxis1D axis1D &&
                axis1D._axis == _axis &&
                axis1D.JoyIndex == JoyIndex;
        }

        public override string _ToString()
        {
            return $"(GUIDEInputJoyAxis1D: axis={_axis}, joy_index={JoyIndex})";
        }

        public override string _EditorName()
        {
            return "Joy Axis 1D";
        }

        public override string _EditorDescription()
        {
            return "The input from a single joy axis.";
        }

        public override GUIDEAction.GUIDEActionValueType _NativeValueType()
        {
            return GUIDEAction.GUIDEActionValueType.AXIS_1D;
        }
    }
}