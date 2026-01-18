using Godot;

namespace Guide.Modifiers
{
    [Tool]
    /// Converts input to canvas coordinates relative to the viewport.
    public partial class GUIDEModifierCanvasCoordinates : GUIDEModifier
    {
        public override bool IsSameAs(GUIDEModifier other)
        {
            return other is GUIDEModifierCanvasCoordinates;
        }

        public override Vector3 _ModifyInput(Vector3 input, double delta, GUIDEAction.GUIDEActionValueType valueType)
        {
            if (!input.IsFinite())
                return Vector3.Inf;

            // Assuming input is in screen space, convert to canvas coordinates
            var viewport = Engine.GetMainLoop() as SceneTree;
            if (viewport != null)
            {
                var canvasTransform = viewport.Root.GetCanvasTransform();
                var canvasInput = canvasTransform.AffineInverse() * new Vector2(input.X, input.Y);
                return new Vector3(canvasInput.X, canvasInput.Y, input.Z);
            }

            return input;
        }

        public override string _EditorName()
        {
            return "Canvas Coordinates";
        }

        public override string _EditorDescription()
        {
            return "Converts input to canvas coordinates.";
        }
    }
}