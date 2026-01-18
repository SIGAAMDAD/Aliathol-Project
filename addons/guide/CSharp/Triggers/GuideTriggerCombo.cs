using Godot;
using System.Collections.Generic;

namespace Guide.Triggers
{
    [Tool]
    public partial class GUIDETriggerCombo : GUIDETrigger
    {
        public enum ActionEventType
        {
            TRIGGERED = 1,
            STARTED = 2,
            ONGOING = 4,
            CANCELLED = 8,
            COMPLETED = 16
        }

        /// If set to true, the combo trigger will print information
        /// about state changes to the debug log.
        [Export]
        public bool EnableDebugPrint { get; set; } = false;

        [Export]
        public GUIDETriggerComboStep[] Steps { get; set; }

        [Export]
        public GUIDETriggerComboCancelAction[] CancellationActions { get; set; }

        private int _currentStep = -1;
        private double _remainingTime;

        public override bool IsSameAs(GUIDETrigger other)
        {
            if (other is not GUIDETriggerCombo combo)
                return false;
            if (Steps.Length != combo.Steps.Length)
                return false;
            if (CancellationActions.Length != combo.CancellationActions.Length)
                return false;

            for (var i = 0; i < Steps.Length; i++)
            {
                if (!Steps[i].IsSameAs(combo.Steps[i]))
                    return false;
            }

            for (var i = 0; i < CancellationActions.Length; i++)
            {
                if (!CancellationActions[i].IsSameAs(combo.CancellationActions[i]))
                    return false;
            }

            return true;
        }

        public override GUIDETriggerState _UpdateState(Vector3 input, double delta, GUIDEAction.GUIDEActionValueType valueType)
        {
            if (Steps.Length == 0)
            {
                GD.PushWarning("Combo with no steps will never fire.");
                return GUIDETriggerState.NONE;
            }

            // initial setup
            if (_currentStep == -1)
            {
                foreach (var step in Steps)
                    step._Prepare();
                foreach (var action in CancellationActions)
                    action._Prepare();
                _Reset();
            }

            var currentAction = Steps[_currentStep].Action;
            if (currentAction == null)
            {
                GD.PushWarning($"Step {_currentStep} has no action {ResourcePath}");
                return GUIDETriggerState.NONE;
            }

            // check if any of our cancellation actions fired
            foreach (var action in CancellationActions)
            {
                // if the action is the current action we don't count its firing as cancellation
                if (action.Action == currentAction)
                    continue;

                if (action._HasFired)
                {
                    if (EnableDebugPrint)
                        GD.Print($"Combo cancelled by action '{action.Action._EditorName()}'.");
                    _Reset();
                    return GUIDETriggerState.NONE;
                }
            }

            // check if any of the steps has fired out of order
            foreach (var step in Steps)
            {
                if (step.Action == currentAction)
                    continue;

                if (step._HasFired)
                {
                    if (EnableDebugPrint)
                        GD.Print($"Combo out of order step by action '{step.Action._EditorName()}'.");
                    _Reset();
                    return GUIDETriggerState.NONE;
                }
            }

            // check if we took too long (unless we're in the first step)
            if (_currentStep > 0)
            {
                _remainingTime -= delta;
                if (_remainingTime <= 0.0)
                {
                    if (EnableDebugPrint)
                        GD.Print($"Step time for step {_currentStep} exceeded.");
                    _Reset();
                    return GUIDETriggerState.NONE;
                }
            }

            // if the current action was fired, if so advance to the next
            if (Steps[_currentStep]._HasFired)
            {
                // reset this step, so it will not count as misfired next round
                Steps[_currentStep]._HasFired = false;
                if (_currentStep + 1 >= Steps.Length)
                {
                    // we finished the combo
                    if (EnableDebugPrint)
                        GD.Print("Combo fired.");
                    _Reset();
                    return GUIDETriggerState.TRIGGERED;
                }

                // otherwise, pick the next step
                _currentStep += 1;
                if (EnableDebugPrint)
                    GD.Print($"Combo advanced to step {_currentStep}.");
                _remainingTime = Steps[_currentStep].TimeToActuate;

                // Reset all steps and cancellation actions to "not fired" in
                // case they were triggered by this action. Otherwise a double-tap
                // would immediately fire for both taps once the first is through
                foreach (var step in Steps)
                    step._HasFired = false;
                foreach (var action in CancellationActions)
                    action._HasFired = false;
            }

            // and in any case we're still processing.
            return GUIDETriggerState.ONGOING;
        }

        private void _Reset()
        {
            if (EnableDebugPrint)
                GD.Print("Combo reset.");
            _currentStep = 0;
            _remainingTime = Steps[0].TimeToActuate;
            foreach (var step in Steps)
                step._HasFired = false;
            foreach (var action in CancellationActions)
                action._HasFired = false;
        }

        public override string _EditorName()
        {
            return "Combo";
        }

        public override string _EditorDescription()
        {
            return "Fires, when the input exceeds the actuation threshold.";
        }
    }
}