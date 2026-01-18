using Godot;

namespace Guide.Modifiers
{
    [Tool]
    /// Scales the input by the given value and optionally, delta time.
    public partial class GUIDEModifierScale : GUIDEModifier
    {
        /// The scale by which the input should be scaled.
        [Export]
        public Vector3 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                EmitChanged();
            }
        }
        private Vector3 _scale = Vector3.One;

        /// If true, delta time will be multiplied in addition to the scale.
        [Export]
        public bool ApplyDeltaTime
        {
            get => _applyDeltaTime;
            set
            {
                _applyDeltaTime = value;
                EmitChanged();
            }
        }
        private bool _applyDeltaTime;

        public override bool IsSameAs(GUIDEModifier other)
        {
            return other is GUIDEModifierScale scale &&
                   ApplyDeltaTime == scale.ApplyDeltaTime &&
                   Scale.IsEqualApprox(scale.Scale);
        }

        public override Vector3 _ModifyInput(Vector3 input, double delta, GUIDEAction.GUIDEActionValueType valueType)
        {
            if (!input.IsFinite())
                return Vector3.Inf;

            if (ApplyDeltaTime)
                return input * Scale * (float)delta;
            else
                return input * Scale;
        }

        public override string _EditorName()
        {
            return "Scale";
        }

        public override string _EditorDescription()
        {
            return "Scales the input by the given value and optionally, delta time.";
        }
    }
}