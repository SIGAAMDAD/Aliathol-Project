using Godot;

namespace Guide.Triggers
{
    [Tool]
    [Icon("res://addons/guide/triggers/guide_trigger.svg")]
    public partial class GUIDETrigger : Resource
    {
        public enum GUIDETriggerState
        {
            /// The trigger did not fire.
            NONE,
            /// The trigger's conditions are partially met
            ONGOING,
            /// The trigger has fired.
            TRIGGERED
        }

        public enum GUIDETriggerType
        {
            // If there are more than one explicit triggers at least one must trigger
            // for the action to trigger.
            EXPLICIT = 1,
            // All implicit triggers must trigger for the action to trigger.
            IMPLICIT = 2,
            // All blocking triggers prevent the action from triggering.
            BLOCKING = 3
        }

        [Export]
        public float ActuationThreshold { get; set; } = 0.5f;

        public Vector3 _LastValue;

        /// Returns whether this trigger is the same as the other trigger.
        /// This is used to determine if a trigger can be reused during context switching.
        public virtual bool IsSameAs(GUIDETrigger other)
        {
            return this == other;
        }

        /// Returns the trigger type of this trigger.
        public virtual GUIDETriggerType _GetTriggerType()
        {
            return GUIDETriggerType.EXPLICIT;
        }

        public virtual GUIDETriggerState _UpdateState(Vector3 input, double delta, GUIDEAction.GUIDEActionValueType valueType)
        {
            return GUIDETriggerState.NONE;
        }

        public bool _IsActuated(Vector3 input, GUIDEAction.GUIDEActionValueType valueType)
        {
            switch (valueType)
            {
                case GUIDEAction.GUIDEActionValueType.AXIS_1D:
                case GUIDEAction.GUIDEActionValueType.BOOL:
                    return _IsAxis1dActuated(input);
                case GUIDEAction.GUIDEActionValueType.AXIS_2D:
                    return _IsAxis2dActuated(input);
                case GUIDEAction.GUIDEActionValueType.AXIS_3D:
                    return _IsAxis3dActuated(input);
            }
            return false;
        }

        /// Checks if a 1D input is actuated.
        public bool _IsAxis1dActuated(Vector3 input)
        {
            return Mathf.IsFinite(input.X) && Mathf.Abs(input.X) > ActuationThreshold;
        }

        /// Checks if a 2D input is actuated.
        public bool _IsAxis2dActuated(Vector3 input)
        {
            return Mathf.IsFinite(input.X) && Mathf.IsFinite(input.Y) && new Vector2(input.X, input.Y).LengthSquared() > ActuationThreshold * ActuationThreshold;
        }

        /// Checks if a 3D input is actuated.
        public bool _IsAxis3dActuated(Vector3 input)
        {
            return input.IsFinite() && input.LengthSquared() > ActuationThreshold * ActuationThreshold;
        }

        /// The name as it should be displayed in the editor.
        public virtual string _EditorName()
        {
            return "GUIDETrigger";
        }

        /// The description as it should be displayed in the editor.
        public virtual string _EditorDescription()
        {
            return "";
        }
    }
}