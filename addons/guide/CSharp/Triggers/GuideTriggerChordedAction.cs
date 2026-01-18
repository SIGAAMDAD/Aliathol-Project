using Godot;

namespace Guide.Triggers
{
    /// Fires, when the given action is currently triggering. This trigger is implicit,
    /// so it will prevent the action from triggering even if other triggers are successful.
    [Tool]
    public partial class GUIDETriggerChordedAction : GUIDETrigger
    {
        [Export]
        public GUIDEAction Action { get; set; }

        public override bool IsSameAs(GUIDETrigger other)
        {
            if (other is not GUIDETriggerChordedAction chorded)
                return false;
            return Action == chorded.Action;
        }

        public override GUIDETriggerType _GetTriggerType()
        {
            return GUIDETriggerType.IMPLICIT;
        }

        public override GUIDETriggerState _UpdateState(Vector3 input, double delta, GUIDEAction.GUIDEActionValueType valueType)
        {
            if (Action == null)
            {
                GD.PushWarning("Chorded trigger without action will never trigger.");
                return GUIDETriggerState.NONE;
            }

            if (Action.IsTriggered())
                return GUIDETriggerState.TRIGGERED;
            return GUIDETriggerState.NONE;
        }

        public override string _EditorName()
        {
            return "Chorded Action";
        }

        public override string _EditorDescription()
        {
            return "Fires, when the given action is currently triggering. This trigger is implicit,\n" +
                   "so it will prevent the action from triggering even if other triggers are successful.";
        }
    }
}