using Godot;

namespace Guide.Modifiers
{
    [Tool]
    /// Inputs between the lower and upper threshold are mapped 0 -> 1.
    /// Values outside the thresholds are clamped.
    public partial class GUIDEModifierDeadzone : GUIDEModifier
    {
        /// Lower threshold for the deadzone.
        [Export(PropertyHint.Range, "0,1")]
        public float LowerThreshold
        {
            get => _lowerThreshold;
            set
            {
                if (value > UpperThreshold)
                    _lowerThreshold = UpperThreshold;
                else
                    _lowerThreshold = value;
                EmitChanged();
            }
        }
        private float _lowerThreshold = 0.2f;

        /// Upper threshold for the deadzone.
        [Export(PropertyHint.Range, "0,1")]
        public float UpperThreshold
        {
            get => _upperThreshold;
            set
            {
                if (value < LowerThreshold)
                    _upperThreshold = LowerThreshold;
                else
                    _upperThreshold = value;
                EmitChanged();
            }
        }
        private float _upperThreshold = 1.0f;

        public override bool IsSameAs(GUIDEModifier other)
        {
            return other is GUIDEModifierDeadzone deadzone &&
                   Mathf.IsEqualApprox(LowerThreshold, deadzone.LowerThreshold) &&
                   Mathf.IsEqualApprox(UpperThreshold, deadzone.UpperThreshold);
        }

        private float _Rescale(float value)
        {
            return Mathf.Min(1.0f, (Mathf.Max(0.0f, Mathf.Abs(value) - LowerThreshold) / (UpperThreshold - LowerThreshold))) * Mathf.Sign(value);
        }

        public override Vector3 _ModifyInput(Vector3 input, double delta, GUIDEAction.GUIDEActionValueType valueType)
        {
            if (UpperThreshold <= LowerThreshold)
                return input;

            if (!input.IsFinite())
                return Vector3.Inf;

            switch (valueType)
            {
                case GUIDEAction.GUIDEActionValueType.BOOL:
                case GUIDEAction.GUIDEActionValueType.AXIS_1D:
                    return new Vector3(_Rescale(input.X), input.Y, input.Z);

                case GUIDEAction.GUIDEActionValueType.AXIS_2D:
                    var v2d = new Vector2(input.X, input.Y);
                    if (v2d.IsZeroApprox())
                        return new Vector3(0, 0, input.Z);
                    v2d = v2d.Normalized() * _Rescale(v2d.Length());
                    return new Vector3(v2d.X, v2d.Y, input.Z);

                case GUIDEAction.GUIDEActionValueType.AXIS_3D:
                    if (input.IsZeroApprox())
                        return Vector3.Zero;
                    return input.Normalized() * _Rescale(input.Length());

                default:
                    GD.PushError("Unsupported value type. This is a bug. Please report it.");
                    return input;
            }
        }

        public override string _EditorName()
        {
            return "Deadzone";
        }

        public override string _EditorDescription()
        {
            return "Inputs between the lower and upper threshold are mapped 0 -> 1.\n" +
                   "Values outside the thresholds are clamped.";
        }
    }
}