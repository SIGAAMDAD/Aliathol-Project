using Godot;

namespace Guide.Modifiers
{
    /// Returns the magnitude of the input value.
    [Tool]
    public partial class GUIDEModifierMagnitude : GUIDEModifier
    {
        public override bool IsSameAs(GUIDEModifier other)
        {
            return other is GUIDEModifierMagnitude;
        }

        public override Vector3 _ModifyInput(Vector3 input, double delta, GUIDEAction.GUIDEActionValueType valueType)
        {
            if (!input.IsFinite())
                return Vector3.Inf;

            return new Vector3(input.Length(), 0, 0);
        }

        public override string _EditorName()
        {
            return "Magnitude";
        }

        public override string _EditorDescription()
        {
            return "Returns the magnitude of the input vector.";
        }
    }
}