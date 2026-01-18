using Godot;

namespace Guide.Inputs
{
    [Tool]
    [Icon("res://addons/guide/inputs/guide_input.svg")]
    /// A class representing some actuated input.
    public partial class GUIDEInput : Resource
    {
        public enum DeviceType
        {
            /// The input originates from no device (e.g. virtual inputs).
            NONE = 0,
            /// The input originates from a keyboard.
            KEYBOARD = 1,
            /// The input originates from a mouse.
            MOUSE = 2,
            /// The input originates from a joystick / gamepad.
            JOY = 4,
            /// The input originates from a touch device.
            TOUCH = 8,
        }

        /// The type of device from which this input originates. Note that this can
        /// also be a combination of devices (e.g. for the any input).
        public DeviceType DeviceTypeProp => _DeviceType();

        /// The current value of this input. Depending on the input type only parts of the
        /// returned vector may be relevant.
        public Vector3 _Value = Vector3.Zero;

        /// The current input state. This will be set by GUIDE when the input is used.
        public GUIDEInputState _State = null;

        /// Whether this input needs a reset per frame. _input is only called when
        /// there is input happening, but some GUIDE inputs may need to be reset
        /// in the absence of input.
        public virtual bool _NeedsReset()
        {
            return false;
        }

        /// Resets the input value to the default value. Is called once per frame if
        /// _needs_reset returns true.
        public virtual void _Reset()
        {
            _Value = Vector3.Zero;
        }

        /// Returns whether this input is the same input as the other input.
        public virtual bool IsSameAs(GUIDEInput other)
        {
            return false;
        }

        /// Called when the input is started to be used by GUIDE. Can be used to perform
        /// initializations. The state object can be used to subscribe to input events
        /// and to get the current input state.
        public virtual void _BeginUsage()
        {
        }

        /// Called, when the input is no longer used by GUIDE. Can be used to perform
        /// cleanup.
        public virtual void _EndUsage()
        {
        }

        /// The name of this input as it should be shown in the editor.
        public virtual string _EditorName()
        {
            return "";
        }

        /// The description of this input as it should be shown in the editor.
        public virtual string _EditorDescription()
        {
            return "";
        }

        /// The native value type of this input (e.g. which kind of value will the
        /// input produce).
        public virtual GUIDEAction.GUIDEActionValueType _NativeValueType()
        {
            return (GUIDEAction.GUIDEActionValueType)(-1);
        }

        /// The device type from which this input originates.
        public virtual DeviceType _DeviceType()
        {
            return DeviceType.NONE;
        }
    }
}