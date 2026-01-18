using Godot;

namespace Guide.Modifiers
{
    [Tool]
    /// Simulates a virtual cursor based on input.
    public partial class GUIDEModifierVirtualCursor : GUIDEModifier
    {
        [Export]
        public float Speed { get; set; } = 100.0f;

        private Vector2 _cursorPosition;

        public override bool IsSameAs(GUIDEModifier other)
        {
            return other is GUIDEModifierVirtualCursor cursor && Mathf.IsEqualApprox(Speed, cursor.Speed);
        }

        public override void _BeginUsage()
        {
            _cursorPosition = Vector2.Zero;
        }

        public override Vector3 _ModifyInput(Vector3 input, double delta, GUIDEAction.GUIDEActionValueType valueType)
        {
            if (!input.IsFinite())
                return Vector3.Inf;

            _cursorPosition += new Vector2(input.X, input.Y) * Speed * (float)delta;
            return new Vector3(_cursorPosition.X, _cursorPosition.Y, input.Z);
        }

        public override bool _NeedsPhysicsProcess()
        {
            return false;
        }

        public override string _EditorName()
        {
            return "Virtual Cursor";
        }

        public override string _EditorDescription()
        {
            return "Simulates a virtual cursor based on input.";
        }
    }
}