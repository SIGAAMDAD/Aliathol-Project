using Godot;

namespace Guide.Triggers
{
    [Tool]
    /// Triggers depending on whether the input changes while actuated. This trigger is
    /// is implicit, so it must succeed for all other triggers to succeed.
    public partial class GUIDETriggerStability : GUIDETrigger
    {
        public enum TriggerWhen
        {
            /// Input must be stable
            INPUT_IS_STABLE,
            /// Input must change
            INPUT_CHANGES
        }

        /// The maximum amount that the input can change after actuation before it is
        /// considered "changed".
        [Export]
        public float MaxDeviation { get; set; } = 1;

        /// When should the trigger trigger?
        [Export]
        public TriggerWhen Trigger { get; set; } = TriggerWhen.INPUT_IS_STABLE;

        private Vector3 _initialValue;
        private bool _deviated;

        public override bool IsSameAs(GUIDETrigger other)
        {
            return other is GUIDETriggerStability stability &&
                   Trigger == stability.Trigger &&
                   Mathf.IsEqualApprox(MaxDeviation, stability.MaxDeviation);
        }

        public override GUIDETriggerType _GetTriggerType()
        {
            return GUIDETriggerType.IMPLICIT;
        }

        public override GUIDETriggerState _UpdateState(Vector3 input, double delta, GUIDEAction.GUIDEActionValueType valueType)
        {
            if (_IsActuated(input, valueType))
            {
                if (_deviated)
                {
                    if (Trigger == TriggerWhen.INPUT_IS_STABLE)
                        return GUIDETriggerState.NONE;
                    return GUIDETriggerState.TRIGGERED;
                }

                if (!_IsActuated(_LastValue, valueType))
                {
                    // we went from "not actuated" to actuated, start
                    _initialValue = input;
                    if (Trigger == TriggerWhen.INPUT_IS_STABLE)
                        return GUIDETriggerState.TRIGGERED;
                    else
                        return GUIDETriggerState.ONGOING;
                }

                // calculate how far the input is from the initial value
                if (_initialValue.DistanceSquaredTo(input) > (MaxDeviation * MaxDeviation))
                {
                    _deviated = true;
                    if (Trigger == TriggerWhen.INPUT_IS_STABLE)
                        return GUIDETriggerState.NONE;
                    return GUIDETriggerState.TRIGGERED;
                }

                if (Trigger == TriggerWhen.INPUT_IS_STABLE)
                    return GUIDETriggerState.TRIGGERED;

                return GUIDETriggerState.ONGOING;
            }

            // if the input is not actuated
            _deviated = false;
            return GUIDETriggerState.NONE;
        }

        public override string _EditorName()
        {
            return "Stability";
        }

        public override string _EditorDescription()
        {
            return "Triggers depending on whether the input changes while actuated. This trigger\n" +
                   "is implicit, so it must succeed for all other triggers to succeed.";
        }
    }
}