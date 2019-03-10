using Matrix_Augmented.Core;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Matrix_Augmented
{
	public sealed partial class FractionBox : UserControl
	{
		#region Constructor

		public FractionBox()
		{
			this.InitializeComponent();
		}

		#endregion

		#region Index Dependency Property

		/// <summary>
		/// Property used to assign the index
		/// </summary>
		public static readonly DependencyProperty IndexProperty = DependencyProperty.Register(
			"Index",
			typeof(string),
			typeof(FractionBox),
			new PropertyMetadata(null)
			);

		/// <summary>
		/// String holding the index of this FractionBox
		/// </summary>
		public string Index
		{
			get => (string)GetValue(IndexProperty);
			set => SetValue(IndexProperty, value);
		}

		#endregion

		#region ShowDecimal Dependency Property

		/// <summary>
		/// Property used to determine whether to show decimal or standard representation of the fraction
		/// </summary>
		public static readonly DependencyProperty ShowDecimalProperty = DependencyProperty.Register(
			"ShowDecimal",
			typeof(bool),
			typeof(FractionBox),
			new PropertyMetadata(null)
			);

		/// <summary>
		/// Bool holding the information on whether to show decimal or standard representation of the fraction
		/// </summary>
		public bool ShowDecimal
		{
			get => (bool)GetValue(ShowDecimalProperty);
			set => SetValue(ShowDecimalProperty, value);
		}

		#endregion

		#region Fraction Input Control

		/// <summary>
		/// Second Check.
		/// Makes sure the rest of the input is valid (for example makes sure '-' is in front)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void FractionTextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
		{
			// Get the current position in the textbox
			int position = sender.SelectionStart;

			// First, trim the text
			sender.Text = sender.Text.Trim();

			// If the '-' is present and is not in front, remove it
			int dashIndex = sender.Text.IndexOf('-');
			if (dashIndex > 0)
			{
				// Remove the improper dash
				sender.Text = sender.Text.Remove(dashIndex, 1);

				// Restore the correct position
				sender.SelectionStart = position - 1;
				position = sender.SelectionStart;
			}
		}



		/// <summary>
		/// First check.
		/// Method filtering invalid input for the Numerator and Denominator TextBoxes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FractionInputControl(object sender, KeyRoutedEventArgs e)
		{
			switch (e.Key)
			{
				// TODO: List all allowed keys here
				case VirtualKey.Number0:
				case VirtualKey.Number1:
				case VirtualKey.Number2:
				case VirtualKey.Number3:
				case VirtualKey.Number4:
				case VirtualKey.Number5:
				case VirtualKey.Number6:
				case VirtualKey.Number7:
				case VirtualKey.Number8:
				case VirtualKey.Number9:
				case VirtualKey.NumberPad0:
				case VirtualKey.NumberPad1:
				case VirtualKey.NumberPad2:
				case VirtualKey.NumberPad3:
				case VirtualKey.NumberPad4:
				case VirtualKey.NumberPad5:
				case VirtualKey.NumberPad6:
				case VirtualKey.NumberPad7:
				case VirtualKey.NumberPad8:
				case VirtualKey.NumberPad9:
					{
						// Valid keys, let them proceed
					}
					break;				

				case (VirtualKey)0xBD:  // -
					{
						// If this special character was already includded, don't let it pass
						if (((TextBox)sender).Text.Contains("-"))
						{
							e.Handled = true;
						}
					}
					break;

				default:
					{
						// Stop all other keys
						e.Handled = true;
					}
					break;
			}
		}


		#endregion

		#region Decimal Input Control

		/// <summary>
		/// Second Check.
		/// Makes sure the rest of the input is valid (for example makes sure '-' is in front)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void DecimalTextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
		{
			// Get the current position in the textbox
			int position = sender.SelectionStart;

			// First, trim the text
			sender.Text = sender.Text.Trim();

			// Replace the comma with with a decimal point
			sender.Text = sender.Text.Replace(',', '.');

			// If the '-' is present and is not in front, remove it
			int dashIndex = sender.Text.IndexOf('-');
			if (dashIndex > 0)
			{
				// Remove the improper dash
				sender.Text = sender.Text.Remove(dashIndex, 1);

				// Restore the correct position
				sender.SelectionStart = position - 1;
				position = sender.SelectionStart;
			}
		}
		
		/// <summary>
		/// First check.
		/// Method filtering invalid input for the Decimal TextBox.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DecimalInputControl(object sender, KeyRoutedEventArgs e)
		{
			switch (e.Key)
			{
				// TODO: List all allowed keys here
				case VirtualKey.Number0:
				case VirtualKey.Number1:
				case VirtualKey.Number2:
				case VirtualKey.Number3:
				case VirtualKey.Number4:
				case VirtualKey.Number5:
				case VirtualKey.Number6:
				case VirtualKey.Number7:
				case VirtualKey.Number8:
				case VirtualKey.Number9:
				case VirtualKey.NumberPad0:
				case VirtualKey.NumberPad1:
				case VirtualKey.NumberPad2:
				case VirtualKey.NumberPad3:
				case VirtualKey.NumberPad4:
				case VirtualKey.NumberPad5:
				case VirtualKey.NumberPad6:
				case VirtualKey.NumberPad7:
				case VirtualKey.NumberPad8:
				case VirtualKey.NumberPad9:
					{
						// Valid keys, let them proceed
					}
					break;

				case (VirtualKey)0xBE:  // .
				case (VirtualKey)0xBC:  // ,
					{
						// If this special character was already included, don't let it pass
						if (((TextBox)sender).Text.Contains("."))
						{
							e.Handled = true;
						}
					}
					break;

				case (VirtualKey)0xBD:	// -
					{
						// If this special character was already included, don't let it pass
						if (((TextBox)sender).Text.Contains("-"))
						{
							e.Handled = true;
						}
					}
					break;

				default:
					{
						// Stop all other keys
						e.Handled = true;
					}
					break;
			}
		}

		/// <summary>
		/// Third Check. Performs cosmetic changes to the input
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DecimalLosingFocus(object sender, RoutedEventArgs e)
		{
			if (sender is TextBox tb && tb.Text.Length > 0)
			{
				// Check for decimal point at the beginning
				if (tb.Text[0] == '.')
				{
					tb.Text = tb.Text.Insert(0, "0");
				}

				// Check for '-.' at the beginning
				if (tb.Text[0] == '-' && tb.Text.Length > 1 && tb.Text[1] == '.')
				{
					tb.Text = tb.Text.Insert(1, "0");
				}

				// Check for '-' and '.' at the end
				for (int i = 0; i < 2; ++i)
				{
					if (tb.Text[tb.Text.Length - 1] == '.' || tb.Text[tb.Text.Length - 1] == '-')
					{
						tb.Text = tb.Text.Remove(tb.Text.Length - 1);
					}
				}

				// If the user's input is equivalent to 0, reset the input
				double value;
				if (double.TryParse(tb.Text, out value))
				{
					if (value == 0)
					{
						tb.Text = string.Empty;
					}
				}

			}
		}

		#endregion

	}
}