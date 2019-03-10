using Matrix_Augmented.Core;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Matrix_Augmented
{
	public sealed partial class EquationBox : UserControl
    {
        public EquationBox()
        {
            this.InitializeComponent();
			HelpCommand = new RelayCommand(() => HelpCommandMethod());
        }

		#region Navigation Requested Event

		/// <summary>
		/// Event for when navigation is requested
		/// </summary>
		public event NavigationRequestedEventHandler NavigationRequested;

		/// <summary>
		/// Method for the <see cref="NavigationRequested"/> event
		/// </summary>
		/// <param name="e"></param>
		private void OnNavigationRequested(NavigationRequestedEventArgs e) => NavigationRequested?.Invoke(this, e);

		#endregion

		#region Help Navigation Command

		private ICommand HelpCommand { get; set; }

		private void HelpCommandMethod()
		{
			// Notify the listener (parent page)
			OnNavigationRequested(new NavigationRequestedEventArgs(PageType.AboutPage));
		}

		#endregion

		#region Input Control

		private void InputControl(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox t)
			{
				t.Text = t.Text.Trim();
			}
		}

		#endregion
	}
}
