using Godot;
using System.Collections.Generic;

namespace Guide.Inputs
{
    /// <summary>
    /// The GUIDEInputState holds the current state of all input. It is basically a wrapper around Godot's Input
    /// class that provides some additional functionality like getting the information if any key or mouse button
    /// is currently pressed. It also is the single entry point for all input events from Godot, so we don't have
    /// process them in every GUIDEInput object and duplicate input handling code everywere. This also improves performance.
    /// </summary>
    public partial class GUIDEInputState : GodotObject
    {
        /// <summary>
        /// Device ID for a virtual joystick that means "any joystick".
        /// This relies on the fact that Godot's device IDs for joysticks are always >= 0.
        /// https://github.com/godotengine/godot/blob/80a3d205f1ad22e779a64921fb56d62b893881ae/core/input/input.cpp#L1821
        /// </summary>
        public const int ANY_JOY_DEVICE_ID = -1;

        /// <summary>
        /// We assign a virtual device ID for the virtual joystick inputs.
        /// Virtual joystick device IDs will be negative, starting with -2 and going down from there.
        /// This relies on the fact that Godot's device IDs for joysticks are always >= 0.
        /// </summary>
        public const int VIRTUAL_JOY_DEVICE_ID_OFFSET = -2;

        /// <summary>
        /// The set of currently connected virtual joy devices. Key is the device id,
        /// value is the number of virtual sticks connected with this device id.
        /// </summary>
        private Dictionary<int, int> _virtualJoyDevices = new();

        /// <summary>
        /// Signalled, when the keyboard state has changed.
        /// </summary>
        [Signal] public delegate void KeyboardStateChangedEventHandler();

        /// <summary>
        /// Signalled, when the mouse motion state has changed.
        /// </summary>
        [Signal] public delegate void MousePositionChangedEventHandler();

        /// <summary>
        /// Signalled, when the mouse button state has changed.
        /// </summary>
        [Signal] public delegate void MouseButtonStateChangedEventHandler();

        /// <summary>
        /// Signalled, when the joy button state has changed.
        /// </summary>
        [Signal] public delegate void JoyButtonStateChangedEventHandler();

        /// <summary>
        /// Signalled, when the joy axis state has changed.
        /// </summary>
        [Signal] public delegate void JoyAxisStateChangedEventHandler();

        /// <summary>
        /// Signalled, when the touch state has changed.
        /// </summary>
        [Signal] public delegate void TouchStateChangedEventHandler();

        // Keys that are currently pressed. Key is the key index, value is not important. The presence of a key in the dictionary
        // indicates that the key is currently pressed.
        private Dictionary<Key, bool> _keys = new();

        // Fingers that are currently touching the screen. Key is the finger index, value is the position (Vector2).
        private Dictionary<int, Vector2> _fingerPositions = new();

        // The mouse movement since the last frame.
        private Vector2 _mouseMovement = Vector2.Zero;

        // Mouse buttons that are currently pressed. Key is the button index, value is not important. The presence of a key
        // in the dictionary indicates that the button is currently pressed.
        private Dictionary<MouseButton, bool> _mouseButtons = new();

        // Joy buttons that are currently pressed. Key is device id, value is a dictionary with the button index as key. The
        // value of the inner dictionary is not important. The presence of a key in the inner dictionary indicates that the button
        // is currently pressed.
        private Dictionary<int, Dictionary<JoyButton, bool>> _joyButtons = new();

        // Current values of joy axes. Key is device id, value is a dictionary with the axis index as key.
        // The value of the inner dictionary is the axis value. Once an axis is actuated, it will be added to the dictionary.
        // We will not remove it anymore after that.
        private Dictionary<int, Dictionary<JoyAxis, float>> _joyAxes = new();

        // The current mapping of joy index to device id. This is used to map the joy index to the device id. A joy index
        // if -1 means "any device id".
        private Dictionary<int, int> _joyIndexToDeviceId = new();

        // This holds the state of keys that have changed this frame. The key is the key, the value is true if the key
        // was last pressed and false if it was last released.
        private Dictionary<Key, bool> _pendingKeys = new();

        // This holds the state of mouse buttons that have changed this frame. The key is the mouse button index, the value is
        // true, if the mouse button was last pressed and false if it was last released.
        private Dictionary<MouseButton, bool> _pendingMouseButtons = new();

        // This holds the state of joy buttons that have changed this frame. The key is the joy device id, the value is
        // a nested dictionary. The nested dictionary has the button index as key and true as value if the button was last
        // pressed or false if it was last released.
        private Dictionary<int, Dictionary<JoyButton, bool>> _pendingJoyButtons = new();

        public GUIDEInputState()
        {
            Input.JoyConnectionChanged += _RefreshJoyDeviceIds;
            _Clear();
        }

        /// <summary>
        /// Connects a new virtual joystick and returns its device id.
        /// The returned device id will be negative, starting with -2 and going down from there.
        /// Since virtual sticks are UI components and not real hardware, we need to give the
        /// UI elements the chance to tell to which virtual stick they belong. For this
        /// we introduce the stick_index. Any UI element tells which virtual stick it belongs to
        /// by providing the same stick_index.
        /// </summary>
        public int ConnectVirtualStick(int stickIndex)
        {
            // we treat an invalid stick index as a stick index of 0 but print an error
            // to let the user know something is wrong
            if (stickIndex < 0)
            {
                GD.PushError($"Invalid stick index {stickIndex} for virtual stick. Must be >= 0.");
                stickIndex = 0;
            }

            int deviceId = VIRTUAL_JOY_DEVICE_ID_OFFSET - stickIndex;
            if (_virtualJoyDevices.ContainsKey(deviceId))
            {
                // just record the additional connection and return the existing device id
                _virtualJoyDevices[deviceId] += 1;
                return deviceId;
            }

            // new device
            _virtualJoyDevices[deviceId] = 1;

            _RefreshJoyDeviceIds(0, false);

            return deviceId;
        }

        /// <summary>
        /// Disconnects the virtual joystick with the given device id.
        /// If no such device is connected, nothing happens.
        /// </summary>
        public void DisconnectVirtualStick(int deviceId)
        {
            if (!_virtualJoyDevices.ContainsKey(deviceId))
                return;

            int count = _virtualJoyDevices[deviceId];
            if (count > 1)
            {
                // just reduce the connection count, but don't remove the device yet
                _virtualJoyDevices[deviceId] -= 1;
                return;
            }

            // last connection went away, so we can remove the device
            _virtualJoyDevices.Remove(deviceId);
            _joyIndexToDeviceId.Remove(deviceId);

            if (_joyButtons.ContainsKey(deviceId))
            {
                _joyButtons.Remove(deviceId);
                EmitSignal("joy_button_state_changed");
            }

            if (_joyAxes.ContainsKey(deviceId))
            {
                _joyAxes.Remove(deviceId);
                EmitSignal("joy_axis_state_changed");
            }
        }

        // Used by the automated tests to make sure we don't have any leftovers from the
        // last test.
        private void _Clear()
        {
            _keys.Clear();
            _fingerPositions.Clear();
            _mouseMovement = Vector2.Zero;
            _mouseButtons.Clear();
            _joyButtons.Clear();
            _joyAxes.Clear();

            _RefreshJoyDeviceIds(0, false);

            // ensure we have an entry for the virtual "any device id"
            _joyButtons[ANY_JOY_DEVICE_ID] = new();
            _joyAxes[ANY_JOY_DEVICE_ID] = new();

            // also clear all virtual joy devices, these can be set up again by the next test
            foreach (int deviceId in _virtualJoyDevices.Keys)
            {
                _joyIndexToDeviceId.Remove(deviceId);
            }

            _virtualJoyDevices.Clear();

            // pending states are created on demand, so we don't need to clear them here
        }

        // Called when any joy device is connected or disconnected. This will refresh the joy device ids and clear out any
        // joy state which is not valid anymore. Will also notify relevant inputs.
        private void _RefreshJoyDeviceIds(long device, bool connected)
        {
            // refresh the joy device ids
            _joyIndexToDeviceId.Clear();
            // get the real joys from the input system
            Godot.Collections.Array<int> connectedJoys = Input.GetConnectedJoypads();
            // append the currently connected virtual joys
            foreach (int virtualDevice in _virtualJoyDevices.Keys)
            {
                connectedJoys.Add(virtualDevice);
            }
            for (int i = 0; i < connectedJoys.Count; i++)
            {
                int deviceId = connectedJoys[i];
                if (deviceId > 0)
                {
                    // godot's joys
                    _joyIndexToDeviceId[i] = deviceId;
                }
                else
                {
                    // virtual joys
                    _joyIndexToDeviceId[deviceId] = deviceId;
                }

                // ensure we have an inner dictionary for the device id
                // by setting this here, we don't need to check for the device id
                // on every input event
                if (!_joyButtons.ContainsKey(deviceId))
                    _joyButtons[deviceId] = new();
                if (!_joyAxes.ContainsKey(deviceId))
                    _joyAxes[deviceId] = new();
                if (!_pendingJoyButtons.ContainsKey(deviceId))
                    _pendingJoyButtons[deviceId] = new();
            }

            // add a virtual device id for the "any device id" case
            _joyIndexToDeviceId[-1] = ANY_JOY_DEVICE_ID;

            foreach (int deviceId in _pendingJoyButtons.Keys)
            {
                if (deviceId != ANY_JOY_DEVICE_ID && !connectedJoys.Contains(deviceId))
                {
                    _pendingJoyButtons.Remove(deviceId);
                }
            }

            bool dirty = false;
            // clear out any joy state which is not valid anymore
            foreach (int deviceId in new List<int>(_joyButtons.Keys))
            {
                if (deviceId != ANY_JOY_DEVICE_ID && !connectedJoys.Contains(deviceId))
                {
                    dirty = true;
                    _joyButtons.Remove(deviceId);
                }
            }

            if (dirty)
            {
                // notify all inputs that the joy state has changed
                EmitSignal("joy_button_state_changed");
            }

            dirty = false;
            foreach (int deviceId in new List<int>(_joyAxes.Keys))
            {
                if (deviceId != ANY_JOY_DEVICE_ID && !connectedJoys.Contains(deviceId))
                {
                    dirty = true;
                    _joyAxes.Remove(deviceId);
                }
            }

            if (dirty)
            {
                // notify all inputs that the joy state has changed
                EmitSignal("joy_axis_state_changed");
            }
        }

        /// <summary>
        /// Called at the end of the frame to reset the state before the next frame.
        /// </summary>
        public void Reset()
        {
            _mouseMovement = Vector2.Zero;

            // apply pending key state at end of the frame.
            foreach (Key key in new List<Key>(_pendingKeys.Keys))
            {
                bool isDown = _pendingKeys[key];
                if (isDown && !_keys.ContainsKey(key))
                {
                    _keys[key] = true;
                    // we emit one change event per changed key just like it would happen
                    // as if the keys were not pressed very fast. this is to ensure same
                    // execution order of things, so everything stays predictable
                    EmitSignal("keyboard_state_changed");
                }
                else if (!isDown && _keys.ContainsKey(key))
                {
                    _keys.Remove(key);
                    EmitSignal("keyboard_state_changed");
                }
            }

            _pendingKeys.Clear();

            // apply pending mouse button state
            foreach (MouseButton button in new List<MouseButton>(_pendingMouseButtons.Keys))
            {
                bool isDown = _pendingMouseButtons[button];
                if (isDown && !_mouseButtons.ContainsKey(button))
                {
                    _mouseButtons[button] = true;
                    EmitSignal("mouse_button_state_changed");
                }
                else if (!isDown && _mouseButtons.ContainsKey(button))
                {
                    _mouseButtons.Remove(button);
                    EmitSignal("mouse_button_state_changed");
                }
            }

            _pendingMouseButtons.Clear();

            // apply pending joy button state
            foreach (int joy in new List<int>(_pendingJoyButtons.Keys))
            {
                foreach (JoyButton button in new List<JoyButton>(_pendingJoyButtons[joy].Keys))
                {
                    bool changed = false;
                    bool isDown = _pendingJoyButtons[joy][button];
                    if (isDown && !_joyButtons[joy].ContainsKey(button))
                    {
                        _joyButtons[joy][button] = true;
                        changed = true;
                    }
                    else if (!isDown && _joyButtons[joy].ContainsKey(button))
                    {
                        _joyButtons[joy].Remove(button);
                        changed = true;
                    }

                    // only evaluate the ANY_JOY device if actually something changed.
                    // otherwise the inner value would not change
                    if (changed)
                    {
                        bool anyValue = false;
                        foreach (int inner in _joyButtons.Keys)
                        {
                            if (inner != ANY_JOY_DEVICE_ID && _joyButtons[inner].ContainsKey(button))
                            {
                                anyValue = true;
                                break;
                            }
                        }

                        if (anyValue)  // we don't need to check the change state here as we'r going to send an event anyways.
                        {
                            _joyButtons[ANY_JOY_DEVICE_ID][button] = true;
                        }
                        else
                        {
                            _joyButtons[ANY_JOY_DEVICE_ID].Remove(button);
                        }
                        EmitSignal("joy_button_state_changed");
                    }
                }
                // and clear out the pending buttons for this joy
                _pendingJoyButtons[joy].Clear();
            }
        }

        /// <summary>
        /// Processes an input event and updates the state.
        /// </summary>
        public void Input(InputEvent @event)
        {
            // print("%s - %s" % [Engine.get_process_frames(), event])
            // ----------------------- KEYBOARD -----------------------------
            if (@event is InputEventKey keyEvent)
            {
                Key index = keyEvent.PhysicalKeycode;

                // check if the key already changed value this frame
                // if so, record the change only, it will be applied at the
                // end of the frame
                if (_pendingKeys.ContainsKey(index))
                {
                    _pendingKeys[index] = keyEvent.Pressed;
                    return;
                }

                _pendingKeys[index] = keyEvent.Pressed;

                if (keyEvent.Pressed && !_keys.ContainsKey(index))
                {
                    _keys[index] = true;
                    EmitSignal("keyboard_state_changed");
                    return;
                }

                if (!keyEvent.Pressed && _keys.ContainsKey(index))
                {
                    _keys.Remove(index);
                    EmitSignal("keyboard_state_changed");
                    return;
                }

                return;
            }

            // ----------------------- MOUSE MOVEMENT -----------------------
            if (@event is InputEventMouseMotion mouseMotion)
            {
                // Emit the mouse moved signal with the distance moved
                _mouseMovement += mouseMotion.Relative;
                EmitSignal("mouse_position_changed");
                return;
            }

            // ----------------------- MOUSE BUTTONS -----------------------
            if (@event is InputEventMouseButton mouseButton)
            {
                MouseButton index = mouseButton.ButtonIndex;

                // check if the mouse button already changed value this frame
                // if so, record the change only, it will be applied at the
                // end of the frame
                if (_pendingMouseButtons.ContainsKey(index))
                {
                    _pendingMouseButtons[index] = mouseButton.Pressed;
                    return;
                }

                _pendingMouseButtons[index] = mouseButton.Pressed;

                if (mouseButton.Pressed && !_mouseButtons.ContainsKey(index))
                {
                    _mouseButtons[index] = true;
                    EmitSignal("mouse_button_state_changed");
                    return;
                }

                if (!mouseButton.Pressed && _mouseButtons.ContainsKey(index))
                {
                    _mouseButtons.Remove(index);
                    EmitSignal("mouse_button_state_changed");
                    return;
                }

                return;
            }

            // ----------------------- JOYSTICK BUTTONS -----------------------
            if (@event is InputEventJoypadButton joyButton)
            {
                int deviceId = joyButton.Device;
                JoyButton button = joyButton.ButtonIndex;

                // _refresh_joy_device_ids ensures we have an inner dictionary for the device id
                // so we don't need to check for it here

                if (_pendingJoyButtons[deviceId].ContainsKey(button))
                {
                    _pendingJoyButtons[deviceId][button] = joyButton.Pressed;
                    return;
                }

                _pendingJoyButtons[deviceId][button] = joyButton.Pressed;

                bool changed = false;
                if (joyButton.Pressed && !_joyButtons[deviceId].ContainsKey(button))
                {
                    _joyButtons[deviceId][button] = true;
                    changed = true;
                }
                else if (!joyButton.Pressed && _joyButtons[deviceId].ContainsKey(button))
                {
                    _joyButtons[deviceId].Remove(button);
                    changed = true;
                }

                // finally set the ANY_JOY_DEVICE_ID state based on what we know
                // only do this if the button value actually changed. Otherwise
                // the Any value would not change either.
                if (changed)
                {
                    bool anyValue = false;
                    foreach (int inner in _joyButtons.Keys)
                    {
                        if (inner != ANY_JOY_DEVICE_ID && _joyButtons[inner].ContainsKey(button))
                        {
                            anyValue = true;
                            break;
                        }
                    }

                    if (anyValue)
                    {
                        _joyButtons[ANY_JOY_DEVICE_ID][button] = true;
                    }
                    else
                    {
                        _joyButtons[ANY_JOY_DEVICE_ID].Remove(button);
                    }
                }

                // Emit the joy button state changed signal if something changed
                if (changed)
                {
                    EmitSignal("joy_button_state_changed");
                }
                return;
            }

            // ----------------------- JOYSTICK AXES -----------------------
            if (@event is InputEventJoypadMotion joyMotion)
            {
                int deviceId = joyMotion.Device;
                JoyAxis axis = joyMotion.Axis;

                // update the axis value
                _joyAxes[deviceId][axis] = joyMotion.AxisValue;

                // for the ANY_JOY_DEVICE_ID, we apply the maximum actuation of all devices (in any direction)
                float anyValue = 0.0f;
                float maximumActuation = 0.0f;
                foreach (int inner in _joyAxes.Keys)
                {
                    if (inner != ANY_JOY_DEVICE_ID && _joyAxes[inner].ContainsKey(axis))
                    {
                        float strength = Mathf.Abs(_joyAxes[inner][axis]);
                        if (strength > maximumActuation)
                        {
                            maximumActuation = strength;
                            anyValue = _joyAxes[inner][axis];
                        }
                    }
                }

                _joyAxes[ANY_JOY_DEVICE_ID][axis] = anyValue;

                // Emit the joy axis state changed signal
                EmitSignal("joy_axis_state_changed");
                return;
            }

            // ----------------------- TOUCH INPUT -----------------------

            if (@event is InputEventScreenTouch screenTouch)
            {
                if (screenTouch.Pressed)
                {
                    _fingerPositions[screenTouch.Index] = screenTouch.Position;
                }
                else
                {
                    _fingerPositions.Remove(screenTouch.Index);
                }

                EmitSignal("touch_state_changed");
                return;
            }

            if (@event is InputEventScreenDrag screenDrag)
            {
                _fingerPositions[screenDrag.Index] = screenDrag.Position;

                EmitSignal("touch_state_changed");
                return;
            }
        }

        /// <summary>
        /// Returns true if the key with the given index is currently pressed.
        /// </summary>
        public bool IsKeyPressed(Key key)
        {
            return _keys.ContainsKey(key);
        }

        /// <summary>
        /// Returns true if at least one key in the given array is currently pressed.
        /// </summary>
        public bool IsAtLeastOneKeyPressed(Key[] keys)
        {
            foreach (Key key in keys)
            {
                if (_keys.ContainsKey(key))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if all keys in the given array are currently pressed.
        /// </summary>
        public bool AreAllKeysPressed(Key[] keys)
        {
            foreach (Key key in keys)
            {
                if (!_keys.ContainsKey(key))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Returns true if currently any key is pressed.
        /// </summary>
        public bool IsAnyKeyPressed()
        {
            return _keys.Count > 0;
        }

        /// <summary>
        /// Gets the mouse movement since the last frame.
        /// If no movement has been detected, returns Vector2.ZERO.
        /// </summary>
        public Vector2 GetMouseDeltaSinceLastFrame()
        {
            // print("%s DELTA %s" % [Engine.get_process_frames(), _mouse_movement])
            return _mouseMovement;
        }

        /// <summary>
        /// Returns the current mouse position in the root viewport.
        /// </summary>
        public Vector2 GetMousePosition()
        {
            return GetViewport().GetMousePosition();
        }

        /// <summary>
        /// Returns true if the mouse button with the given index is currently pressed.
        /// </summary>
        public bool IsMouseButtonPressed(MouseButton buttonIndex)
        {
            return _mouseButtons.ContainsKey(buttonIndex);
        }

        /// <summary>
        /// Returns true if currently any mouse button is pressed.
        /// </summary>
        public bool IsAnyMouseButtonPressed()
        {
            return _mouseButtons.Count > 0;
        }

        /// <summary>
        /// Returns the current value of the given joy axis on the device with the given index. If no
        /// such device or axis exists, returns 0.0.
        /// </summary>
        public float GetJoyAxisValue(int index, JoyAxis axis)
        {
            if (!_joyIndexToDeviceId.TryGetValue(index, out int deviceId))
                return 0.0f;
            if (_joyAxes.TryGetValue(deviceId, out var inner))
                return inner.GetValueOrDefault(axis, 0.0f);
            return 0.0f;
        }

        /// <summary>
        /// Returns true, if the given joy button is currentely pressed on the device with the given index.
        /// </summary>
        public bool IsJoyButtonPressed(int index, JoyButton button)
        {
            if (!_joyIndexToDeviceId.TryGetValue(index, out int deviceId))
                return false;
            if (_joyButtons.TryGetValue(deviceId, out var inner))
                return inner.ContainsKey(button);
            return false;
        }

        /// <summary>
        /// Returns true, if currently any joy button is pressed on any device.
        /// </summary>
        public bool IsAnyJoyButtonPressed()
        {
            foreach (var inner in _joyButtons.Values)
            {
                if (inner.Count > 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if currently any joy axis is actuated with at least the given strength.
        /// </summary>
        public bool IsAnyJoyAxisActuated(float minimumStrength)
        {
            foreach (var inner in _joyAxes.Values)
            {
                foreach (float value in inner.Values)
                {
                    if (Mathf.Abs(value) >= minimumStrength)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the finger position of the finger at the given index.
        /// If finger_index is < 0, returns the average of all finger positions.
        /// Will only return a position if the amount of fingers
        /// currently touching matches finger_count.
        ///
        /// If no finger position can be determined, returns Vector2.INF.
        /// </summary>
        public Vector2 GetFingerPosition(int fingerIndex, int fingerCount)
        {
            // if we have no finger positions right now, we can cut it short here
            if (_fingerPositions.Count == 0)
                return Vector2.Inf;

            // If the finger count doesn't match we have no position right now
            if (_fingerPositions.Count != fingerCount)
                return Vector2.Inf;

            // if a finger index is set, use this fingers position, if available
            if (fingerIndex > -1)
                return _fingerPositions.GetValueOrDefault(fingerIndex, Vector2.Inf);

            Vector2 result = Vector2.Zero;
            foreach (Vector2 value in _fingerPositions.Values)
            {
                result += value;
            }

            result /= fingerCount;
            return result;
        }

        /// <summary>
        /// Returns the positions of all fingers currently touching.
        /// If no finger touches, returns an empty array.
        /// </summary>
        public Godot.Collections.Array<Vector2> GetFingerPositions()
        {
            var result = new Godot.Collections.Array<Vector2>();
            result.AddRange(_fingerPositions.Values);
            return result;
        }

        /// <summary>
        /// Returns true, if currently any finger is touching the screen.
        /// </summary>
        public bool IsAnyFingerDown()
        {
            return _fingerPositions.Count > 0;
        }
    }
}