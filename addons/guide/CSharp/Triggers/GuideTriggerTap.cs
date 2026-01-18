using Godot;

namespace Guide.Triggers
{
    [Tool]
    /// A trigger that activates when the input is tapped and released before the time threshold is reached.
    public partial class GUIDETriggerTap : GUIDETrigger
    {
        /// The time threshold for the tap to be considered a tap.
        [Export]
        public float TapThreshold { get; set; } = 0.2f;

        private double _accumulatedTime;

        public override bool IsSameAs(GUIDETrigger other)
        {
            return other is GUIDETriggerTap tap &&
                   Mathf.IsEqualApprox(TapThreshold, tap.TapThreshold);
        }

        public override GUIDETriggerState _UpdateState(Vector3 input, double delta, GUIDEAction.GUIDEActionValueType valueType)
        {
            if (_IsActuated(input, valueType))
            {
                // if the input was actuated before, and the tap threshold has been exceeded, the trigger is locked down
                // until the input is released and we can exit out early
                if (_IsActuated(_LastValue, valueType) && _accumulatedTime > TapThreshold)
                    return GUIDETriggerState.NONE;

                // accumulate time
                _accumulatedTime += delta;

                if (_accumulatedTime < TapThreshold)
                    return GUIDETriggerState.ONGOING;
                else
                    // we have exceeded the tap threshold, so the tap is not triggered.
                    return GUIDETriggerState.NONE;
            }
            else // not actuated right now
            {
                // if the input was actuated before...
                if (_IsActuated(_LastValue, valueType))
                {
                    // ... and the accumulated time is less than the threshold, then the tap is triggered.
                    if (_accumulatedTime < TapThreshold)
                    {
                        _accumulatedTime = 0;
                        return GUIDETriggerState.TRIGGERED;
                    }

                    // Otherwise, the tap is not triggered, but we reset the accumulated time
                    // so the trigger is now again ready to be triggered.
                    _accumulatedTime = 0;
                }

                // in either case, the trigger is not triggered.
                return GUIDETriggerState.NONE;
            }
        }

        public override string _EditorName()
        {
            return "Tap";
        }

        public override string _EditorDescription()
        {
            return "Fires when the input is actuated and released within the given timeframe.";
        }
    }
}