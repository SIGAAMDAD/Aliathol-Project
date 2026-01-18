using Godot;

namespace Guide.Triggers
{
    [Tool]
    /// A trigger that activates when the input is pushed down. Will only emit a
    /// trigger event once. Holding the input will not trigger further events.
    public partial class GUIDETriggerPressed : GUIDETrigger
    {
        public override bool IsSameAs(GUIDETrigger other)
        {
            return other is GUIDETriggerPressed;
        }

        public override GUIDETriggerState _UpdateState(Vector3 input, double delta, GUIDEAction.GUIDEActionValueType valueType)
        {
            if (_IsActuated(input, valueType))
            {
                if (!_IsActuated(_LastValue, valueType))
                    return GUIDETriggerState.TRIGGERED;
            }
            return GUIDETriggerState.NONE;
        }

        public override string _EditorName()
        {
            return "Pressed";
        }

        public override string _EditorDescription()
        {
            return "Fires once, when the input exceeds actuation threshold. Holding the input\n" +
                   "will not fire additional triggers.";
        }
    }
}