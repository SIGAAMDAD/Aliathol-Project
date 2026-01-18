using Godot;

namespace Guide.Triggers
{
    [Tool]
    /// A trigger that activates when the input is held down for a certain amount of time.
    public partial class GUIDETriggerHold : GUIDETrigger
    {
        /// The time for how long the input must be held.
        [Export]
        public float HoldThreshold { get; set; } = 1.0f;

        /// If true, the trigger will only fire once until the input is released. Otherwise the trigger will fire every frame.
        [Export]
        public bool IsOneShot { get; set; } = false;

        private double _accumulatedTime;
        private bool _didShoot;

        public override bool IsSameAs(GUIDETrigger other)
        {
            return other is GUIDETriggerHold hold &&
                   IsOneShot == hold.IsOneShot &&
                   Mathf.IsEqualApprox(HoldThreshold, hold.HoldThreshold);
        }

        public override GUIDETriggerState _UpdateState(Vector3 input, double delta, GUIDEAction.GUIDEActionValueType valueType)
        {
            // if the input is actuated, accumulate time and check if the hold threshold has been reached
            if (_IsActuated(input, valueType))
            {
                _accumulatedTime += delta;

                if (_accumulatedTime >= HoldThreshold)
                {
                    // if the trigger is one shot and we already shot, then we will not trigger again.
                    if (IsOneShot && _didShoot)
                        return GUIDETriggerState.NONE;
                    else
                    {
                        // otherwise, we will just trigger.
                        _didShoot = true;
                        return GUIDETriggerState.TRIGGERED;
                    }
                }
                else
                {
                    // if the hold threshold has not been reached, then the trigger is ongoing.
                    return GUIDETriggerState.ONGOING;
                }
            }
            else
            {
                // if the input is not actuated, then the trigger is not triggered and we reset the accumulated time.
                // and our one shot flag.
                _accumulatedTime = 0;
                _didShoot = false;
                return GUIDETriggerState.NONE;
            }
        }

        public override string _EditorName()
        {
            return "Hold";
        }

        public override string _EditorDescription()
        {
            return "Fires, once the input has remained actuated for hold_threshold seconds.\n" +
                   "My fire once or repeatedly.";
        }
    }
}