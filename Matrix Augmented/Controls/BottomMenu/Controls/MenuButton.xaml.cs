using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Matrix_Augmented
{
	public sealed partial class MenuButton : UserControl
    {
        public MenuButton()
        {
            this.InitializeComponent();
        }

		#region Text Dependency Property

		/// <summary>
		/// Property used to assign text to the control
		/// </summary>
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
			"Text",
			typeof(string),
			typeof(MenuButton),
			new PropertyMetadata(null)
			);

		/// <summary>
		/// String holding the text assigned to the control
		/// </summary>
		public string Text
		{
			get => (string)GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}

		#endregion


		#region Button Command Property

		public ICommand ButtonCommand
		{
			get => (ICommand)GetValue(ButtonCommandProperty);
			set => SetValue(ButtonCommandProperty, value);
		}


		public static readonly DependencyProperty ButtonCommandProperty =
			DependencyProperty.Register("ButtonCommand", typeof(ICommand), typeof(MenuButton), new PropertyMetadata(null));

		#endregion

	}
}
