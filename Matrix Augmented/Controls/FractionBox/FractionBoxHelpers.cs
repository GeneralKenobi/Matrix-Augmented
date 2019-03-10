using System.Collections.Generic;
using Matrix_Augmented.Core;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Matrix_Augmented
{
	/// <summary>
	/// Class containing helper methods for FractionBox
	/// </summary>
	public static class FractionBoxHelpers
	{
		/// <summary>
		/// Configures and sets the binding for ShowDecimalProperty in FractionBox
		/// </summary>
		/// <param name="fractionBox">FractionBox to set the binding to</param>
		/// <param name="vm">ViewModel to use</param>
		private static void ConfigureShowDecimalBinding(this FractionBox fractionBox, ViewModel vm)
		{
			// Create the binding and set the properties
			Binding binding = new Binding()
			{
				Path = new PropertyPath("ShowDecimal"),
				Mode = BindingMode.OneWay,
				Source = vm,				
			};

			// Assign it
			BindingOperations.SetBinding(fractionBox, FractionBox.ShowDecimalProperty, binding);
		}

		/// <summary>
		/// Sets all properties for the FractionBox to be ready to add to the MainGrid
		/// </summary>
		/// <param name="fractionBox">FractionBox to set</param>
		/// <param name="vm">ViewModel to assign to</param>
		/// <param name="row">Row for this FractionBox (indexing starts at 0)</param>
		/// <param name="column">Column for this FractionBox (indexing starts at 0)</param>
		public static void SetForMainGrid(this FractionBox fractionBox, ViewModel vm, int row, int column)
		{
			// Set the alignment to the top left corner
			fractionBox.HorizontalAlignment = HorizontalAlignment.Left;
			fractionBox.VerticalAlignment = VerticalAlignment.Top;

			// Set it's position in the Canvas
			fractionBox.Margin = new Thickness(column * Constants.ColumnWidth, row * Constants.RowHeight, 0, 0);

			// Set the index of this FractionBox
			fractionBox.Index = $"{row + 1}x{column + 1}";

			// Configure the binding for ShowDecimalProperty
			fractionBox.ConfigureShowDecimalBinding(vm);

			// Set the tabindex so consequent tabs go horizontally
			fractionBox.TabIndex = row * 10 + column + 2;

			// Set the datancontext to the Fraction
			fractionBox.DataContext = vm.CurrentMatrix.Fields[row, column];
		}

		/// <summary>
		/// Finds all FractionBox children located in the given row in the container and returns them in a list
		/// </summary>
		/// <param name="container">Container to search</param>
		/// <param name="row">Row to find (Indexing starts at 1)</param>
		/// <returns></returns>
		public static List<FractionBox> FindChildrenInRow(this Panel container, int row)
		{
			List<FractionBox> marked = new List<FractionBox>();

			// For each FractionBox in MainGrid
			foreach (UIElement item in container.Children)
			{
				if (item is FractionBox fractionBox)
				{
					int xIndex = fractionBox.Index.IndexOf('x');

					// If the row in FractionBox's index is equal to given row
					// plus 1 (because index in FractionBox starts at 1)...
					if (fractionBox.Index.Substring(0, xIndex) == row.ToString())
					{
						// Mark it
						marked.Add(fractionBox);
					}
				}
			}

			return marked;
		}

		/// <summary>
		/// Finds all FractionBox children located in the given column in the container and returns them in a list
		/// </summary>
		/// <param name="container">Container to search</param>
		/// <param name="column">Column to find (Indexing starts at 1)</param>
		/// <returns></returns>
		public static List<FractionBox> FindChildrenInColumn(this Panel container, int column)
		{
			List<FractionBox> marked = new List<FractionBox>();

			// For each FractionBox in MainGrid
			foreach (UIElement item in container.Children)
			{
				if (item is FractionBox fractionBox)
				{
					int xIndex = fractionBox.Index.IndexOf('x');

					// If the row in FractionBox's index is equal to given row
					// plus 1 (because index in FractionBox starts at 1)...
					if (fractionBox.Index.Substring(xIndex + 1) == column.ToString())
					{
						// Mark it
						marked.Add(fractionBox);
					}
				}
			}

			return marked;
		}

	}
}
