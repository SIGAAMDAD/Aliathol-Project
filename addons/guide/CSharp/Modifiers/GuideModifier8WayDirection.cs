using Godot;

namespace Guide.Modifiers
{
    [Tool]
    /// Snaps input to 8-way directions (up, down, left, right, and diagonals).
    public partial class GUIDEModifier8WayDirection : GUIDEModifier
    {
        public override bool IsSameAs(GUIDEModifier other)
        {
            return other is GUIDEModifier8WayDirection;
        }

        public override Vector3 _ModifyInput(Vector3 input, double delta, GUIDEAction.GUIDEActionValueType valueType)
        {
            if (!input.IsFinite())
                return Vector3.Inf;

            var length = input.Length();
            if (length == 0)
                return Vector3.Zero;

            var normalized = input / length;

            // Snap to 8 directions
            var angle = Mathf.Atan2(normalized.Y, normalized.X);
            var snappedAngle = Mathf.Round(angle / (Mathf.Pi / 4)) * (Mathf.Pi / 4);

            var snappedX = Mathf.Cos(snappedAngle);
            var snappedY = Mathf.Sin(snappedAngle);

            return new Vector3(snappedX * length, snappedY * length, input.Z);
        }

        public override string _EditorName()
        {
            return "8-Way Direction";
        }

        public override string _EditorDescription()
        {
            return "Snaps input to 8-way directions.";
        }
    }
}