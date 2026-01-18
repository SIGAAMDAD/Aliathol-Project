using Godot;

namespace Guide.Inputs
{
    /// <summary>
    /// Base class for joystick inputs.
    /// </summary>
    [Tool]
    public partial class GUIDEInputJoyBase : GUIDEInput
    {
        /// <summary>
        /// The index of the connected joy pad to check.
        /// -1 = Any connected joy pad
        ///  0 = First connected joy pad
        ///  1 = Second connected joy pad
        ///  2 = Third connected joy pad
        ///  3 = Fourth connected joy pad
        /// -2 = First virtual joy pad
        /// -3 = Second virtual joy pad
        /// </summary>
        [Export(PropertyHint.Enum, "Any:-1,1:0,2:1,3:2,4:3,Virtual 1:-2,Virtual 2:-3,Virtual 3:-4,Virtual 4:-5")]
        public int JoyIndex
        {
            get => _joyIndex;
            set
            {
                if (value == _joyIndex)
                    return;
                _joyIndex = value;
                EmitChanged();
            }
        }
        private int _joyIndex = -1;

        public override DeviceType _DeviceType()
        {
            return DeviceType.JOY;
        }
    }
}