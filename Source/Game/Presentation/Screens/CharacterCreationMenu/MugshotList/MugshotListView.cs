using Godot;

namespace Game.Presentation.Screens.CharacterCreationMenu.MugshotList {
	/*
	===================================================================================
	
	MugshotListView
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public sealed class MugshotListView( MugshotList owner ) {
		public MugshotList Owner => owner;

		public VBoxContainer OptionList => owner.GetNode<VBoxContainer>( "OptionList" );

		private Label _nameLabel => owner.GetNode<Label>( "%NameLabel" );
		private RichTextLabel _descriptionLabel => owner.GetNode<RichTextLabel>( "%DescriptionLabel" );

		/*
		===============
		ClearItems
		===============
		*/
		/// <summary>
		/// Clears all mugshots in the list.
		/// </summary>
		public void ClearItems() {
			var optionList = OptionList;
			var children = optionList.GetChildren();
			for ( int i = 0; i < children.Count; i++ ) {
				var child = children[ i ];
				optionList.RemoveChild( child );
				child.QueueFree();
			}
		}

		/*
		===============
		SetName
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		public void SetName( string name ) {
			_nameLabel.Text = name;
		}

		/*
		===============
		SetDescription
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="description"></param>
		public void SetDescription( string description ) {
			_descriptionLabel.ParseBbcode( description );
		}
	};
};