using Godot;

namespace Guide.Modifiers
{
    [Tool]
    [Icon("res://addons/guide/modifiers/guide_modifier.svg")]
    public partial class GUIDEModifier : Resource
    {
        /// Returns whether this modifier is the same as the other modifier.
        /// This is used to determine if a modifier can be reused during context switching.
        public virtual bool IsSameAs(GUIDEModifier other)
        {
            return this == other;
        }

        /// Called when the modifier is started to be used by GUIDE. Can be used to perform
        /// initializations.
        public virtual void _BeginUsage()
        {
        }

        /// Called, when the modifier is no longer used by GUIDE. Can be used to perform
        /// cleanup.
        public virtual void _EndUsage()
        {
        }

        /// Called to modify the input value before it is passed to the triggers.
        public virtual Vector3 _ModifyInput(Vector3 input, double delta, GUIDEAction.GUIDEActionValueType valueType)
        {
            return input;
        }

        /// The name as it should be displayed in the editor.
        public virtual string _EditorName()
        {
            return "";
        }

        /// The description as it should be displayed in the editor.
        public virtual string _EditorDescription()
        {
            return "";
        }

        /// Whether this modifier needs physics processing. This is queried once
        /// when the modifier is used, not every frame.
        public virtual bool _NeedsPhysicsProcess()
        {
            return false;
        }

        /// Called to update any internal state of the modifier during physics processing.
        /// Only called if _needs_physics_process() returns true.
        public virtual void _PhysicsProcess(double delta)
        {
        }
    }
}