using Godot;

namespace Guide.Modifiers
{
    /// Inverts input per axis.
    [Tool]
    public partial class GUIDEModifierNegate : GUIDEModifier
    {
        /// Whether the X axis should be inverted.
        [Export]
        public bool X
        {
            get => _x;
            set
            {
                if (_x == value) return;
                _x = value;
                _UpdateCaches();
                EmitChanged();
            }
        }
        private bool _x = true;

        /// Whether the Y axis should be inverted.
        [Export]
        public bool Y
        {
            get => _y;
            set
            {
                if (_y == value) return;
                _y = value;
                _UpdateCaches();
                EmitChanged();
            }
        }
        private bool _y = true;

        /// Whether the Z axis should be inverted.
        [Export]
        public bool Z
        {
            get => _z;
            set
            {
                if (_z == value) return;
                _z = value;
                _UpdateCaches();
                EmitChanged();
            }
        }
        private bool _z = true;

        private Vector3 _multiplier = Vector3.One * -1;

        public override bool IsSameAs(GUIDEModifier other)
        {
            return other is GUIDEModifierNegate negate &&
                   X == negate.X &&
                   Y == negate.Y &&
                   Z == negate.Z;
        }

        private void _UpdateCaches()
        {
            _multiplier.X = X ? -1 : 1;
            _multiplier.Y = Y ? -1 : 1;
            _multiplier.Z = Z ? -1 : 1;
        }

        public override Vector3 _ModifyInput(Vector3 input, double delta, GUIDEAction.GUIDEActionValueType valueType)
        {
            if (!input.IsFinite())
                return Vector3.Inf;

            return input * _multiplier;
        }

        public override string _EditorName()
        {
            return "Negate";
        }

        public override string _EditorDescription()
        {
            return "Inverts input per axis.";
        }
    }
}