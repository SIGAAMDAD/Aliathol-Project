namespace Game.Domain.UserInterface.Events {
	public readonly record struct SaveSlotSelectedEventArgs(
		int SlotIndex
	);
};