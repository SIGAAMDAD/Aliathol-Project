using Godot;

namespace Guide.Modifiers
{
    [Tool]
    /// Converts input to window-relative coordinates.
    public partial class GUIDEModifierWindowRelative : GUIDEModifier
    {
        public override bool IsSameAs(GUIDEModifier other)
        {
            return other is GUIDEModifierWindowRelative;
        }

        public override Vector3 _ModifyInput(Vector3 input, double delta, GUIDEAction.GUIDEActionValueType valueType)
        {
            if (!input.IsFinite())
                return Vector3.Inf;

            // Assuming input is in global screen space, make it relative to window
            var window = Engine.GetMainLoop() as SceneTree;
            if (window != null && window.Root is Window rootWindow)
            {
                var windowPosition = rootWindow.Position;
                return new Vector3(input.X - windowPosition.X, input.Y - windowPosition.Y, input.Z);
            }

            return input;
        }

        public override string _EditorName()
        {
            return "Window Relative";
        }

        public override string _EditorDescription()
        {
            return "Converts input to window-relative coordinates.";
        }
    }
}