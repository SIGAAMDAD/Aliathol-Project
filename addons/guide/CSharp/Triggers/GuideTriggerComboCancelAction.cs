using Godot;

namespace Guide.Triggers
{
    [Icon("res://addons/guide/guide_internal.svg")]
    public partial class GUIDETriggerComboCancelAction : Resource
    {
        [Export]
        public GUIDEAction Action { get; set; }

        [Export(PropertyHint.Flags, "Triggered:1,Started:2,Ongoing:4,Cancelled:8,Completed:16")]
        public int CompletionEvents { get; set; } = (int)GUIDETriggerCombo.ActionEventType.TRIGGERED;

        public bool _HasFired;

        public bool IsSameAs(GUIDETriggerComboCancelAction other)
        {
            return Action == other.Action &&
                   CompletionEvents == other.CompletionEvents;
        }

        public void _Prepare()
        {
            if ((CompletionEvents & (int)GUIDETriggerCombo.ActionEventType.TRIGGERED) != 0)
                Action.Triggered += _Fired;
            if ((CompletionEvents & (int)GUIDETriggerCombo.ActionEventType.STARTED) != 0)
                Action.Started += _Fired;
            if ((CompletionEvents & (int)GUIDETriggerCombo.ActionEventType.ONGOING) != 0)
                Action.Ongoing += _Fired;
            if ((CompletionEvents & (int)GUIDETriggerCombo.ActionEventType.CANCELLED) != 0)
                Action.Cancelled += _Fired;
            if ((CompletionEvents & (int)GUIDETriggerCombo.ActionEventType.COMPLETED) != 0)
                Action.Completed += _Fired;
            _HasFired = false;
        }

        private void _Fired()
        {
            _HasFired = true;
        }
    }
}