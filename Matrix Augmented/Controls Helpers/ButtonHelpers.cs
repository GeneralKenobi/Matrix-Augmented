using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Matrix_Augmented.Core;

namespace Matrix_Augmented
{
	/// <summary>
	/// Class containing helpers for Buttons
	/// </summary>
	public static class ButtonHelpers
	{
		/// <summary>
		/// Configures the Button as a button changing matrices (the top menu).
		/// Sets style, configures binding.
		/// </summary>
		/// <param name="button">Button to configure</param>
		/// <param name="id">ID of the matrix it represents</param>
		public static void ConfigureForMatrixButton(this Button button, char id)
		{
			// Style it
			button.Style = Application.Current.Resources["TopMenuButton"] as Style;

			// Assign the id as content
			button.Content = id;

			// And as the parameter for command
			button.CommandParameter = id;

			// Setup the binding
			Binding commandBinding = new Binding()
			{
				Path = new PropertyPath(nameof(ViewModel.SetCurrentMatrixCommand)),
				Mode = BindingMode.TwoWay,
			};

			// And set it
			button.SetBinding(Button.CommandProperty, commandBinding);
		}
	}
}
