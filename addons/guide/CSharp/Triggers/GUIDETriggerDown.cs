using Godot;

namespace Guide.Triggers
{
    /// Fires, when the input exceeds the actuation threshold. This is
    /// the default trigger when no trigger is specified.
    [Tool]
    public partial class GUIDETriggerDown : GUIDETrigger
    {
        public override bool IsSameAs(GUIDETrigger other)
        {
            return other is GUIDETriggerDown;
        }

        public override GUIDETriggerState _UpdateState(Vector3 input, double delta, GUIDEAction.GUIDEActionValueType valueType)
        {
            // if the input is actuated, then the trigger is triggered.
            if (_IsActuated(input, valueType))
                return GUIDETriggerState.TRIGGERED;
            // otherwise, the trigger is not triggered.
            return GUIDETriggerState.NONE;
        }

        public override string _EditorName()
        {
            return "Down";
        }

        public override string _EditorDescription()
        {
            return "Fires, when the input exceeds the actuation threshold. This is\n" +
                   "the default trigger when no trigger is specified.";
        }
    }
}