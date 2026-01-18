using Godot;

namespace Guide.Triggers
{
    [Tool]
    /// A trigger that activates when the input is pushed down and then repeatedly sends trigger events at a fixed interval.
    /// Note: the trigger will be either triggering or ongoing until the input is released.
    /// Note: at most one pulse will be emitted per frame.
    public partial class GUIDETriggerPulse : GUIDETrigger
    {
        /// If true, the trigger will trigger immediately when the input is actuated. Otherwise, the trigger will wait for the initial delay.
        [Export]
        public bool TriggerOnStart { get; set; } = true;

        /// The delay after the initial actuation before pulsing begins.
        [Export]
        public float InitialDelay
        {
            get => _initialDelay;
            set => _initialDelay = Mathf.Max(0, value);
        }
        private float _initialDelay = 0.3f;

        /// The interval between pulses. Set to 0 to pulse every frame.
        [Export]
        public float PulseInterval
        {
            get => _pulseInterval;
            set => _pulseInterval = Mathf.Max(0, value);
        }
        private float _pulseInterval = 0.1f;

        /// Maximum number of pulses. If <= 0, the trigger will pulse indefinitely.
        [Export]
        public int MaxPulses { get; set; } = 0;

        private double _delayUntilNextPulse;
        private int _emittedPulses;

        public override bool IsSameAs(GUIDETrigger other)
        {
            if (other is not GUIDETriggerPulse pulse)
                return false;
            return Mathf.IsEqualApprox(_initialDelay, pulse._initialDelay) &&
                   Mathf.IsEqualApprox(_pulseInterval, pulse._pulseInterval) &&
                   MaxPulses == pulse.MaxPulses &&
                   TriggerOnStart == pulse.TriggerOnStart;
        }

        public override GUIDETriggerState _UpdateState(Vector3 input, double delta, GUIDEAction.GUIDEActionValueType valueType)
        {
            if (_IsActuated(input, valueType))
            {
                if (!_IsActuated(_LastValue, valueType))
                {
                    // we went from "not actuated" to actuated, pulsing starts
                    _delayUntilNextPulse = _initialDelay;
                    if (TriggerOnStart)
                        return GUIDETriggerState.TRIGGERED;
                    else
                        return GUIDETriggerState.ONGOING;
                }

                // if we already are pulsing and have exceeded the maximum number of pulses, we will not pulse anymore.
                if (MaxPulses > 0 && _emittedPulses >= MaxPulses)
                    return GUIDETriggerState.NONE;

                // subtract the delta from the delay until the next pulse
                _delayUntilNextPulse -= delta;

                if (_delayUntilNextPulse > 0)
                    // we are still waiting for the next pulse, nothing to do.
                    return GUIDETriggerState.ONGOING;

                // now delta could be larger than our pulse, in which case we loose a few pulses.
                // as we can pulse at most once per frame.

                // in case someone sets the pulse interval to 0, we will pulse every frame.
                if (Mathf.IsEqualApprox(_pulseInterval, 0))
                {
                    _delayUntilNextPulse = 0;
                    if (MaxPulses > 0)
                        _emittedPulses += 1;
                    return GUIDETriggerState.TRIGGERED;
                }

                // Now add the delay until the next pulse
                _delayUntilNextPulse += _pulseInterval;

                // If the interval is really small, we can potentially have skipped some pulses
                if (_delayUntilNextPulse <= 0)
                {
                    // we have skipped some pulses
                    var skippedPulses = (int)(-(_delayUntilNextPulse / _pulseInterval));
                    _delayUntilNextPulse += skippedPulses * _pulseInterval;
                    if (MaxPulses > 0)
                    {
                        _emittedPulses += skippedPulses;
                        if (_emittedPulses >= MaxPulses)
                            return GUIDETriggerState.NONE;
                    }
                }

                // Record a pulse and return triggered
                if (MaxPulses > 0)
                    _emittedPulses += 1;
                return GUIDETriggerState.TRIGGERED;
            }

            // if the input is not actuated, then the trigger is not triggered.
            _emittedPulses = 0;
            _delayUntilNextPulse = 0;
            return GUIDETriggerState.NONE;
        }

        public override string _EditorName()
        {
            return "Pulse";
        }

        public override string _EditorDescription()
        {
            return "Fires at an interval while the input is actuated.";
        }
    }
}