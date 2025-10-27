using Godot;

namespace Menus.SelectionNodes {
	public interface ISelectionNode {
		public bool IsFocused { get; }
		public StyleBoxTexture FocusedStyleBox { get; }

		public void OnFocused();
		public void OnUnfocused();
		public void DisableMouseFocus();
	};
};