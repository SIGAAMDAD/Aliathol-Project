using Godot;

namespace Guide.Modifiers
{
    [Tool]
    /// Applies a separate curve to each input axis.
    public partial class GUIDEModifierCurve : GUIDEModifier
    {
        /// The curve to apply to the x axis
        [Export]
        public Curve Curve { get; set; } = DefaultCurve();

        /// Apply modifier to X axis
        [Export]
        public bool X { get; set; } = true;

        /// Apply modifier to Y axis
        [Export]
        public bool Y { get; set; } = true;

        /// Apply modifier to Z axis
        [Export]
        public bool Z { get; set; } = true;

        /// Create default curve resource with a smoothstep, 0.0 - 1.0 input/output range
        public static Curve DefaultCurve()
        {
            var curve = new Curve();
            curve.AddPoint(new Vector2(0.0f, 0.0f));
            curve.AddPoint(new Vector2(1.0f, 1.0f));
            return curve;
        }

        public override bool IsSameAs(GUIDEModifier other)
        {
            return other is GUIDEModifierCurve curve &&
                   Curve == curve.Curve &&
                   X == curve.X &&
                   Y == curve.Y &&
                   Z == curve.Z;
        }

        public override Vector3 _ModifyInput(Vector3 input, double delta, GUIDEAction.GUIDEActionValueType valueType)
        {
            // Curve should never be null
            if (Curve == null)
            {
                GD.PushError("No curve added to Curve modifier.");
                return input;
            }

            if (!input.IsFinite())
                return Vector3.Inf;

            // Return vector with enabled axes modified, others remain unchanged.
            return new Vector3(
                X ? Curve.Sample(input.X) : input.X,
                Y ? Curve.Sample(input.Y) : input.Y,
                Z ? Curve.Sample(input.Z) : input.Z
            );
        }

        public override string _EditorName()
        {
            return "Curve";
        }

        public override string _EditorDescription()
        {
            return "Applies a curve to each input axis.";
        }
    }
}