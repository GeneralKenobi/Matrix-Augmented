using Matrix_Augmented.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Matrix_Augmented
{

	/// <summary>
	/// Helper methods for TextBlock
	/// </summary>
	public static class TextBlockHelpers
	{

		public static void ConfigureForAboutTextBlock(this TextBlock textblock, int index)
		{
			// Style it
			textblock.Style = Application.Current.Resources["AboutTextBlockStyle"] as Style;

			// Setup the binding
			Binding textBinding = new Binding()
			{
				Path = new PropertyPath($"Instructions[{index}]"),
				Mode = BindingMode.TwoWay,
			};

			// And set it
			textblock.SetBinding(TextBlock.TextProperty, textBinding);
		}
	}
}
