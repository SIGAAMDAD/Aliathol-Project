using Godot;

namespace Guide.Inputs
{
    [Tool]
    public partial class GUIDEInputMouseButton : GUIDEInput
    {
        [Export]
        public MouseButton Button
        {
            get => _button;
            set
            {
                if (value == _button)
                    return;
                _button = value;
                EmitChanged();
            }
        }
        private MouseButton _button = MouseButton.Left;

        // The value that this input will be reset to at the end of the frame.
        private Vector3 _resetTo;
        private bool _wasPressedThisFrame;

        public override bool _NeedsReset()
        {
            // mouse wheel up and down can potentially send multiple inputs within a single frame
            // so we need to smooth this out a bit.
            return _button == MouseButton.WheelUp || _button == MouseButton.WheelDown;
        }

        public override void _Reset()
        {
            _wasPressedThisFrame = false;
            _Value = _resetTo;
        }

        public override void _BeginUsage()
        {
            // subscribe to mouse button events
            _State.MouseButtonStateChanged += _Refresh;
            _Refresh();
        }

        public override void _EndUsage()
        {
            // unsubscribe from mouse button events
            _State.MouseButtonStateChanged -= _Refresh;
        }

        private void _Refresh()
        {
            bool isPressed = _State.IsMouseButtonPressed(_button);

            if (_NeedsReset())
            {
                // we always reset to the last event we received in a frame
                // so after the frame is over we're still in sync.
                _resetTo = new Vector3(isPressed ? 1.0f : 0.0f, 0, 0);

                if (isPressed)
                    _wasPressedThisFrame = true;

                if (!isPressed && _wasPressedThisFrame)
                {
                    // keep pressed state for this frame
                    return;
                }

                _Value = new Vector3(isPressed ? 1.0f : 0.0f, 0, 0);
            }
            else
            {
                _Value = new Vector3(isPressed ? 1.0f : 0.0f, 0, 0);
            }
        }

        public override bool IsSameAs(GUIDEInput other)
        {
            return other is GUIDEInputMouseButton mouseButton && mouseButton._button == _button;
        }

        public override string _ToString()
        {
            return $"(GUIDEInputMouseButton: button={_button})";
        }

        public override string _EditorName()
        {
            return "Mouse Button";
        }

        public override string _EditorDescription()
        {
            return "A press of a mouse button. The mouse wheel is also a button.";
        }

        public override GUIDEAction.GUIDEActionValueType _NativeValueType()
        {
            return GUIDEAction.GUIDEActionValueType.BOOL;
        }

        public override DeviceType _DeviceType()
        {
            return DeviceType.MOUSE;
        }
    }
}