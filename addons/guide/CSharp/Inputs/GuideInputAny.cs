using Godot;

namespace Guide.Inputs
{
    /// <summary>
    /// Input that triggers if any input from the given device class
    /// is given.
    /// </summary>
    [Tool]
    public partial class GUIDEInputAny : GUIDEInput
    {
        /// <summary>
        /// Should input from mouse buttons be considered?
        /// </summary>
        [Export]
        public bool MouseButtons { get; set; } = false;

        /// <summary>
        /// Should input from mouse movement be considered?
        /// </summary>
        [Export]
        public bool MouseMovement { get; set; } = false;

        /// <summary>
        /// Minimum movement distance of the mouse before it is considered
        /// moving.
        /// </summary>
        [Export]
        public float MinimumMouseMovementDistance { get; set; } = 1.0f;

        /// <summary>
        /// Should input from gamepad/joystick buttons be considered?
        /// </summary>
        [Export]
        public bool JoyButtons { get; set; } = false;

        /// <summary>
        /// Should input from gamepad/joystick axes be considered?
        /// </summary>
        [Export]
        public bool JoyAxes { get; set; } = false;

        /// <summary>
        /// Minimum strength of a single joy axis actuation before it is considered
        /// as actuated.
        /// </summary>
        [Export]
        public float MinimumJoyAxisActuationStrength { get; set; } = 0.2f;

        /// <summary>
        /// Should input from the keyboard be considered?
        /// </summary>
        [Export]
        public bool Keyboard { get; set; } = false;

        /// <summary>
        /// Should input from touch be considered?
        /// </summary>
        [Export]
        public bool Touch { get; set; } = false;

        public override bool _NeedsReset()
        {
            // Needs reset because we cannot detect the absence of input.
            return true;
        }

        public override void _BeginUsage()
        {
            // subscribe to relevant input events
            if (MouseMovement)
                _State.MousePositionChanged += _Refresh;
            if (MouseButtons)
                _State.MouseButtonStateChanged += _Refresh;
            if (Keyboard)
                _State.KeyboardStateChanged += _Refresh;
            if (JoyButtons)
                _State.JoyButtonStateChanged += _Refresh;
            if (JoyAxes)
                _State.JoyAxisStateChanged += _Refresh;
            if (Touch)
                _State.TouchStateChanged += _Refresh;

            _Refresh();
        }

        public override void _EndUsage()
        {
            // unsubscribe from input events
            if (MouseMovement)
                _State.MousePositionChanged -= _Refresh;
            if (MouseButtons)
                _State.MouseButtonStateChanged -= _Refresh;
            if (Keyboard)
                _State.KeyboardStateChanged -= _Refresh;
            if (JoyButtons)
                _State.JoyButtonStateChanged -= _Refresh;
            if (JoyAxes)
                _State.JoyAxisStateChanged -= _Refresh;
            if (Touch)
                _State.TouchStateChanged -= _Refresh;
        }

        private void _Refresh()
        {
            // if the input was already actuated this frame, remain
            // actuated, even if more input events come in. Input will
            // reset at the end of the frame.
            if (!_Value.IsZeroApprox())
                return;

            if (Keyboard && _State.IsAnyKeyPressed())
            {
                _Value = Vector3.Right;
                return;
            }

            if (MouseButtons && _State.IsAnyMouseButtonPressed())
            {
                _Value = Vector3.Right;
                return;
            }

            if (MouseMovement && _State.GetMouseDeltaSinceLastFrame().Length() >= MinimumMouseMovementDistance)
            {
                _Value = Vector3.Right;
                return;
            }

            if (JoyButtons && _State.IsAnyJoyButtonPressed())
            {
                _Value = Vector3.Right;
                return;
            }

            if (JoyAxes && _State.IsAnyJoyAxisActuated(MinimumJoyAxisActuationStrength))
            {
                _Value = Vector3.Right;
                return;
            }

            if (Touch && _State.IsAnyFingerDown())
            {
                _Value = Vector3.Right;
                return;
            }

            _Value = Vector3.Zero;
        }

        public override bool IsSameAs(GUIDEInput other)
        {
            return other is GUIDEInputAny anyInput &&
                MouseButtons == anyInput.MouseButtons &&
                MouseMovement == anyInput.MouseMovement &&
                JoyButtons == anyInput.JoyButtons &&
                JoyAxes == anyInput.JoyAxes &&
                Keyboard == anyInput.Keyboard &&
                Touch == anyInput.Touch &&
                Mathf.IsEqualApprox(MinimumMouseMovementDistance, anyInput.MinimumMouseMovementDistance) &&
                Mathf.IsEqualApprox(MinimumJoyAxisActuationStrength, anyInput.MinimumJoyAxisActuationStrength);
        }

        public override string _EditorName()
        {
            return "Any Input";
        }

        public override string _EditorDescription()
        {
            return "Input that triggers if any input from the given device class is given.";
        }

        public override GUIDEAction.GUIDEActionValueType _NativeValueType()
        {
            return GUIDEAction.GUIDEActionValueType.BOOL;
        }

        public override DeviceType _DeviceType()
        {
            DeviceType result = DeviceType.NONE;
            if (JoyAxes || JoyButtons)
                result |= DeviceType.JOY;
            if (MouseButtons || MouseMovement)
                result |= DeviceType.MOUSE;
            if (Keyboard)
                result |= DeviceType.KEYBOARD;
            if (Touch)
                result |= DeviceType.TOUCH;

            return result;
        }
    }
}