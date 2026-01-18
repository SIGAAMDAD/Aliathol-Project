using Game.Application.UI;
using Game.Domain.UserInterface.Events;
using Godot;
using Nomad.Save.ValueObjects;

namespace Game.Presentation.Screens.SaveSlotsMenu.SaveSlotList {
	/*
	===================================================================================
	
	SaveSlotButton
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public sealed class SaveSlotButton {
		private readonly UIEventHelper _helper;
		private readonly int _index;

		/*
		===============
		SaveSlotButton
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="button"></param>
		/// <param name="metadata"></param>
		public SaveSlotButton( UIEventHelper helper, Button button, int index, SaveFileMetadata metadata ) {
			button.Text = $"{metadata.FileName} {metadata.LastAccessTime}";
			button.Connect( Button.SignalName.Pressed, Callable.From( OnPressed ) );

			_helper = helper;
			_index = index;
		}

		/*
		===============
		OnPressed
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		private void OnPressed() {
			_helper.SaveSlotSelected.Publish( new SaveSlotSelectedEventArgs( _index ) );
		}
	};
};
