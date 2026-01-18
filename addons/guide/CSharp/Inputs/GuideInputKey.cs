using Godot;
using System.Linq;

namespace Guide.Inputs
{
    [Tool]
    public partial class GUIDEInputKey : GUIDEInput
    {
        /// <summary>
        /// The physical keycode of the key.
        /// </summary>
        [Export]
        public Key Key
        {
            get => _key;
            set
            {
                if (value == _key)
                    return;
                _key = value;
                EmitChanged();
            }
        }
        private Key _key;

        /// <summary>
        /// Whether shift must be pressed.
        /// </summary>
        [Export]
        public bool Shift
        {
            get => _shift;
            set
            {
                if (value == _shift)
                    return;
                _shift = value;
                EmitChanged();
            }
        }
        private bool _shift = false;

        /// <summary>
        /// Whether control must be pressed.
        /// </summary>
        [Export]
        public bool Control
        {
            get => _control;
            set
            {
                if (value == _control)
                    return;
                _control = value;
                EmitChanged();
            }
        }
        private bool _control = false;

        /// <summary>
        /// Whether alt must be pressed.
        /// </summary>
        [Export]
        public bool Alt
        {
            get => _alt;
            set
            {
                if (value == _alt)
                    return;
                _alt = value;
                EmitChanged();
            }
        }
        private bool _alt = false;

        /// <summary>
        /// Whether meta/win/cmd must be pressed.
        /// </summary>
        [Export]
        public bool Meta
        {
            get => _meta;
            set
            {
                if (value == _meta)
                    return;
                _meta = value;
                EmitChanged();
            }
        }
        private bool _meta = false;

        /// <summary>
        /// Whether this input should fire if additional
        /// modifier keys are currently pressed.
        /// </summary>
        [Export]
        public bool AllowAdditionalModifiers
        {
            get => _allowAdditionalModifiers;
            set
            {
                if (value == _allowAdditionalModifiers)
                    return;
                _allowAdditionalModifiers = value;
                EmitChanged();
            }
        }
        private bool _allowAdditionalModifiers = true;

        /// <summary>
        /// Helper array. All keys that must be pressed for this input to considered actuated.
        /// </summary>
        private Key[] _mustBePressed;

        /// <summary>
        /// Helper array. All keys that must not be pressed for this input to considered actuated.
        /// </summary>
        private Key[] _mustNotBePressed;

        public override void _BeginUsage()
        {
            var mustBePressed = new System.Collections.Generic.List<Key> { _key };

            // also add the modifiers to the list of keys that must be pressed
            if (_shift)
                mustBePressed.Add(Key.Shift);
            if (_control)
                mustBePressed.Add(Key.Ctrl);
            if (_alt)
                mustBePressed.Add(Key.Alt);
            if (_meta)
                mustBePressed.Add(Key.Meta);

            _mustBePressed = mustBePressed.ToArray();

            var mustNotBePressed = new System.Collections.Generic.List<Key>();
            // now unless additional modifiers are allowed, add all modifiers
            // that are not required to the list of keys that must not be pressed
            // except if the bound key is actually the modifier itself
            if (!_allowAdditionalModifiers)
            {
                if (!_shift && _key != Key.Shift)
                    mustNotBePressed.Add(Key.Shift);
                if (!_control && _key != Key.Ctrl)
                    mustNotBePressed.Add(Key.Ctrl);
                if (!_alt && _key != Key.Alt)
                    mustNotBePressed.Add(Key.Alt);
                if (!_meta && _key != Key.Meta)
                    mustNotBePressed.Add(Key.Meta);
            }

            _mustNotBePressed = mustNotBePressed.ToArray();

            // subscribe to input events
            _State.KeyboardStateChanged += _Refresh;
            _Refresh();
        }

        public override void _EndUsage()
        {
            // unsubscribe from input events
            _State.KeyboardStateChanged -= _Refresh;
        }

        private void _Refresh()
        {
            // We are actuated if all keys that must be pressed are pressed and none of the keys that must not be pressed
            // are pressed.
            bool isActuated = _State.AreAllKeysPressed(_mustBePressed) && !_State.IsAtLeastOneKeyPressed(_mustNotBePressed);
            _Value = new Vector3(isActuated ? 1.0f : 0.0f, 0, 0);
        }

        public override bool IsSameAs(GUIDEInput other)
        {
            return other is GUIDEInputKey keyInput &&
                keyInput._key == _key &&
                keyInput._shift == _shift &&
                keyInput._control == _control &&
                keyInput._alt == _alt &&
                keyInput._meta == _meta &&
                keyInput._allowAdditionalModifiers == _allowAdditionalModifiers;
        }

        public override string _ToString()
        {
            return $"(GUIDEInputKey: key={_key}, shift={_shift}, alt={_alt}, control={_control}, meta={_meta})";
        }

        public override string _EditorName()
        {
            return "Key";
        }

        public override string _EditorDescription()
        {
            return "A button press on the keyboard.";
        }

        public override GUIDEAction.GUIDEActionValueType _NativeValueType()
        {
            return GUIDEAction.GUIDEActionValueType.BOOL;
        }

        public override DeviceType _DeviceType()
        {
            return DeviceType.KEYBOARD;
        }
    }
}