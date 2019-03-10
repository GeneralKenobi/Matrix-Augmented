using Windows.UI.Xaml.Controls;
using Matrix_Augmented.Core;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Matrix_Augmented
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		#region Constructor

		public MainPage()
		{
			this.InitializeComponent();
			this.Loaded += MainPageLoaded;
			this.SizeChanged += OnSizeChanged;
			this.NavigationCacheMode = NavigationCacheMode.Enabled;

			// Create a new ViewModel
			var vm = new ViewModel();

			// Assign the delegates for events in the viewmodel
			vm.CurrentMatrixResize += CurrentMatrixResized;
			vm.MatrixAddedDeleted += MatrixAddedDeleted;
			vm.CurrentMatrixModified += CurrentMatrixChanged;

			// Finally set the data context to vm
			this.DataContext = vm;
		}
		
		#endregion

		#region OnLoaded

		private async void MainPageLoaded(object sender, RoutedEventArgs e)
		{
			ViewModel vm = (ViewModel)DataContext;
			
			// If it's the first load and there are no matrices
			if (vm.Matrices.Count == 0)
			{
				// Create the first matrix
				vm.NewMatrixCommand.Execute(null);

				// Wait for the command to finish
				await Task.Delay(Core.Constants.ShortAnimation);

				// Set it as current
				vm.SetCurrentMatrixCommand.Execute(vm.Matrices[0].ID);

				// Set the padding so that BottomMenu doesn't initially cover up the FractionBoxes
				MainGrid.Padding = new Thickness(0, 0, 0, Math.Min((this.ActualHeight - 155) / 2, 400));
			}
		}

		#endregion

		#region Current Matrix Resized

		/// <summary>
		/// Fired when currently presented matrix was resized
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CurrentMatrixResized(object sender, ResizeEventArgs e)
		{
			switch(e.Type)
			{
				case ResizeType.NewRow:
					{
						GenerateNewRowOfFractionBoxes(e.NewRows, e.NewColumns);
					}
					break;

				case ResizeType.NewColumn:
					{
						GenerateNewColumnOfFractionBoxes(e.NewRows, e.NewColumns);
					}
					break;

				case ResizeType.RemovedRow:
					{
						DeleteRowOfFractionBoxes(e.OldRows);
					}
					break;

				case ResizeType.RemovedColumn:
					{
						DeleteColumnOfFractionBoxes(e.OldColumns);
					}
					break;
			}
		}

		#endregion

		#region Removal of FractionBoxes		

		/// <summary>
		/// Deletes all FractionBoxes that are located in this row
		/// </summary>
		/// <param name="row">Row to delete (indexing starts at 1)</param>
		private async void DeleteRowOfFractionBoxes(int row)
		{
			// Gather the FractionBoxes which are to be deleted in a list
			List<FractionBox> toDelete = MainGrid.FindChildrenInRow(row);

			// Do the same for FractionBoxes which are directly over them
			List<FractionBox> upperRow = MainGrid.FindChildrenInRow(row - 1);

			// Slide the upper row down and almost wait for it to finish
			upperRow.ForEach((x) => x.SlideAsync(Orientation.Vertical, 0, Constants.RowHeight, Core.Constants.ShortAnimation));
			await Task.Delay(Core.Constants.ShortAnimation-50);

			// Scroll up to avoid stutter due to the shrink in MainGrid's size
			MainGridScrollViewer.ChangeView(0, MainGridScrollViewer.VerticalOffset-Constants.RowHeight, 1, false);

			// Wait for the rest of the last animation
			await Task.Delay(50);

			// Remove the FractionBoxes
			toDelete.ForEach((x) => MainGrid.Children.Remove(x));
			
			// Slide the upper row back up
			upperRow.ForEach((x) => x.SlideAsync(Orientation.Vertical, Constants.RowHeight, 0, Core.Constants.ShortAnimation));
		}
		
		/// <summary>
		/// Deletes all FractionBoxes that are located in this column
		/// </summary>
		/// <param name="column">Column to delete (indexing starts at 1)</param>
		private async Task DeleteColumnOfFractionBoxes(int column)
		{
			// Gather the FractionBoxes which are to be deleted in a list
			List<FractionBox> toDelete = MainGrid.FindChildrenInColumn(column);

			// Do the same for FractionBoxes which are on the left
			List<FractionBox> leftColumn = MainGrid.FindChildrenInColumn(column - 1);

			// Slide the left column right and almost wait for it to finish
			leftColumn.ForEach((x) => x.SlideAsync(Orientation.Horizontal, 0, Constants.ColumnWidth, Core.Constants.ShortAnimation));
			await Task.Delay(Core.Constants.ShortAnimation-50);

			// Scroll left to avoid stutter due to the shrink in MainGrid's size
			MainGridScrollViewer.ChangeView(MainGridScrollViewer.HorizontalOffset - Constants.RowHeight, 0, 1, false);

			// Wait for the rest of the last animation
			await Task.Delay(50);

			// Remove the FractionBoxes
			toDelete.ForEach((x) => MainGrid.Children.Remove(x));

			// Slide the left column back
			leftColumn.ForEach((x) => x.SlideAsync(Orientation.Horizontal, Constants.RowHeight, 0, Core.Constants.ShortAnimation));
		}

		#endregion

		#region AdditionOfFractionBoxes

		/// <summary>
		/// Generates and places new row FractionBoxes in the main grid
		/// </summary>
		/// <param name="row">Row in which the new FractionBoxes will be placed. Indexing starts at 1</param>
		/// <param name="columns">Number of columns to generate (i.e. number of columns in the matrix). Indexing starts at 1</param>
		private async void GenerateNewRowOfFractionBoxes(int row, int columns)
		{
			// Get the upper row of FractionBoxes
			List<FractionBox> upperRow = MainGrid.FindChildrenInRow(row - 1);

			// Slide them down and await
			upperRow.ForEach((x) => x.SlideAsync(Orientation.Vertical, 0, Constants.RowHeight, Core.Constants.ShortAnimation));
			await Task.Delay(Core.Constants.ShortAnimation);

			// Create a new FractionBox in each column in the exact position
			// of the FractionBoxes that were animated down
			for (int i = 0; i < columns; ++i)
			{
				AddNewFractionBox(row - 1, i, SlideInAnimationType.None, 0);
			}

			// Animate the upper row back up
			upperRow.ForEach((x) => x.SlideAsync(Orientation.Vertical, Constants.RowHeight, 0, Core.Constants.ShortAnimation));
		}

		/// <summary>
		/// Generates and places new column of FractionBoxes in the main grid
		/// </summary>
		/// <param name="rows">Number of rows to generate (i.e. number of rows in the matrix). Indexing starts at 1</param>
		/// <param name="column">Column in which the new FractionBoxes will be placed. Indexing starts at 1</param>
		private async void GenerateNewColumnOfFractionBoxes(int rows, int column)
		{
			// Get the left column of FractionBoxes
			List<FractionBox> leftColumn = MainGrid.FindChildrenInColumn(column - 1);

			// Slide them right and await
			leftColumn.ForEach((x) => x.SlideAsync(Orientation.Horizontal, 0, Constants.ColumnWidth, Core.Constants.ShortAnimation));
			await Task.Delay(Core.Constants.ShortAnimation);

			// Create a new FractionBox in each row in the exact position
			// of the FractionBoxes that were animated right
			for (int i = 0; i < rows; ++i)
			{
				AddNewFractionBox(i, column - 1, SlideInAnimationType.None, 0);
			}

			// Animate the left column back
			leftColumn.ForEach((x) => x.SlideAsync(Orientation.Horizontal, Constants.RowHeight, 0, Core.Constants.ShortAnimation));

		}

		/// <summary>
		/// Creates and adds a new, fully configured FractionBox to the MainGrid (doesn't animate it in)
		/// </summary>
		/// <param name="row">Row to add the new FractionBox to</param>
		/// <param name="column">Column to add the new FractionBox to</param>
		/// <param name="animationType">Slide in animation to perform on the new FractionBox</param>
		/// <param name="duration">Duration of the animation in miliseconds</param>
		private void AddNewFractionBox(int row, int column, SlideInAnimationType animationType, int duration=500)
		{
			var viewModel = this.DataContext as ViewModel;

			// Create a new FractionBox
			var fractionBox = new FractionBox();

			// Set all required properties
			fractionBox.SetForMainGrid(viewModel, row, column);

			// Add the FractionBox to the MainCanvas
			MainGrid.Children.Add(fractionBox);

			// Animate the FractionBox in
			switch (animationType)
			{
				case SlideInAnimationType.FromBottom:
					{
						fractionBox.SlideAsync(Orientation.Vertical,
							this.ActualHeight + MainGridScrollViewer.VerticalOffset, 0, duration, 0, new SineEase());
					}
					break;

				case SlideInAnimationType.FromRight:
					{
						fractionBox.SlideAsync(Orientation.Horizontal,
							this.ActualWidth + MainGridScrollViewer.HorizontalOffset, 0, duration, 0, new SineEase());
					}
					break;

				case SlideInAnimationType.FromUpperFractionBox:
					{
						fractionBox.SlideAsync(Orientation.Vertical, -Constants.RowHeight, 0, duration, 0, new SineEase());
					}
					break;

				case SlideInAnimationType.FromLeftFractionBox:
					{
						fractionBox.SlideAsync(Orientation.Horizontal, -Constants.ColumnWidth, 0, duration, 0, new SineEase());
					}
					break;
			}
		}

		/// <summary>
		/// Animates out the FractionBox, then removes if from the parent
		/// </summary>
		/// <param name="fractionBox">FractionBox to animate and remove</param>
		/// <param name="parent">Parent containing the FractionBox</param>
		/// <param name="animationType">Type of the animation to perform</param>
		/// <param name="duration">Duration of the animation, in miliseconds</param>
		/// <returns></returns>
		private async Task AnimateOutAndRemove(FractionBox fractionBox, SlideOutAnimationType animationType, int duration=500)
		{
			// Animate the FractionBox out
			switch (animationType)
			{
				case SlideOutAnimationType.ToBottom:
					{
						await fractionBox.SlideAsync(Orientation.Vertical, 0,
							this.ActualHeight + MainGridScrollViewer.VerticalOffset, duration, 0, new SineEase());
					}
					break;

				case SlideOutAnimationType.ToRight:
					{
						await fractionBox.SlideAsync(Orientation.Horizontal, 0,
							this.ActualWidth + MainGridScrollViewer.HorizontalOffset, duration, 0, new SineEase());
					}
					break;

				case SlideOutAnimationType.ToUpperFractionBox:
					{
						await fractionBox.SlideAsync(Orientation.Vertical, 0, -Constants.RowHeight, duration, 0, new SineEase());
					}
					break;

				case SlideOutAnimationType.ToLeftFractionBox:
					{
						await fractionBox.SlideAsync(Orientation.Horizontal, 0, -Constants.ColumnWidth, duration, 0, new SineEase());
					}
					break;
			}

			// Detach the FractionBox from the parent
			MainGrid.Children.Remove(fractionBox);
		}

		#endregion

		#region Showing / Hiding Full Matrix

		/// <summary>
		/// Shows currently selected matrix from the viewmodel (by creating new FractionBoxes)
		/// Therefore, the UI elements representing the old one should be deleted before calling this method
		/// </summary>
		private async Task AnimateInCurrentMatrixAsync()
		{
			// Get the viewmodel
			ViewModel vm = this.DataContext as ViewModel;

			// Generate a random number generator to determine the Orientation of animations
			Random rd = new Random();

			// For every row in the current matrix...
			for(int i=0; i<vm.CurrentMatrix.Rows; ++i)
			{
				// For every element in that row...
				for(int j=0; j<vm.CurrentMatrix.Columns; ++j)
				{
					// Create a new FractionBox and animate it in randomely (50% for animation from bottom and 50% for animation from right)
					AddNewFractionBox(i, j, rd.Next(2) == 1 ? SlideInAnimationType.FromBottom : SlideInAnimationType.FromRight, Core.Constants.ShortAnimation);
				}
			}

			// Delay the task in case the caller wants to wait for the animation to end
			await Task.Delay(Core.Constants.ShortAnimation);
		}

		/// <summary>
		/// Animates out and removes all currently shown FractionBoxes
		/// </summary>
		private async Task AnimateOutCurrentMatrixAsync()
		{
			// Generate a random number generator to determine the Orientation of animations
			Random rd = new Random();			

			foreach(var item in MainGrid.Children)
			{
				if(item is FractionBox fractionBox)
				{
					AnimateOutAndRemove(fractionBox,
						rd.Next(2) == 0 ? SlideOutAnimationType.ToBottom : SlideOutAnimationType.ToRight,
						Core.Constants.ShortAnimation);
				}
			}

			// Delay the task in case the caller wants to wait for the animations to end
			await Task.Delay(Core.Constants.ShortAnimation);
		}


		#endregion

		#region Private Helpers



		#endregion

		#region On Size Changed

		/// <summary>
		/// Method adjusting the padding in the MainGrid on window size changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			// If the size didn't change much
			if(e.NewSize == e.PreviousSize)
			{
				return;
			}

			if (MainGrid.Padding.Bottom != 0)
			{
				// Set the padding to the height of the BottomMenu
				MainGrid.Padding = new Thickness(0, 0, 0, Math.Min((this.ActualHeight - 155) / 2, 400));
			}

		}

		#endregion

		#region Methods managing the padding of the Main Grid after Show/Collapse of BottomMenu

		/// <summary>
		/// Adjusts the padding of the MainGrid when the bottom menu was shown
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BottomMenuStateChanged(object sender, MenuStateChangeEventArgs e)
		{
			// If the mene wasn't shown, return
			if (e.Type != MenuStateChangeEventType.Shown)
			{
				return;
			}

			// Set the bottom padding to offset
			MainGrid.Padding = new Thickness(0, 0, 0, Math.Min(e.Offset, 400));
		}

		/// <summary>
		/// Adjusts the padding of the MainGrid when the bottom menu will be hidden
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BottomMenuStateChanging(object sender, MenuStateChangeEventArgs e)
		{
			// If the menu isn't being hidden, return
			if (e.Type != MenuStateChangeEventType.Hidden)
			{
				return;
			}

			// Set the bottom padding to 0
			MainGrid.Animate(MainGrid.Padding, new Thickness(MainGrid.Padding.Left, MainGrid.Padding.Top, MainGrid.Padding.Right, 0), 350);
		}

		#endregion

		#region Handlers of addition / removal of matrices

		/// <summary>
		/// Manages the MatrixAddedDeletedEvent
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MatrixAddedDeleted(object sender, MatricesModifiedEventArgs e)
		{			
			switch (e.Type)
			{
				case OperationType.Addition:
					{
						AddNewMatrixButton(e.ID);
					}
					break;

				case OperationType.Removal:
					{
						DeleteMatrixButton(e.ID);
					}
					break;
			}
		}

		/// <summary>
		/// Adds a new matrix button the the UI
		/// </summary>
		/// <param name="id">ID of the new matrix</param>
		private async Task AddNewMatrixButton(char id)
		{
			// Create a new button
			var button = new Button();

			// Configure it
			button.ConfigureForMatrixButton(id);
			
			// If it's the first button, just add it
			if (id == 'A')
			{
				MatricesStackPanel.Children.Insert(0, button);
			}
			else
			{
				// Otherwise look for the button with ID that should be on the left
				// ex: when adding 'C', look for B
				char idToLookFor = id;
				--idToLookFor;

				// Find the button on the left
				var buttonOnTheLeft = MatricesStackPanel.FindChild<Button>((x) => ((char)x.Content) == idToLookFor);

				// Make sure it's not null
				if (buttonOnTheLeft != null)
				{
					// Calculate the offset to slide by
					double offset = buttonOnTheLeft.ActualWidth + buttonOnTheLeft.Margin.Left + buttonOnTheLeft.Margin.Right;

					// Slide all buttons that are to the right of the new button by offset and wait for these animations to end
					await MatricesStackPanel.SlideChildren<Button>(Orientation.Horizontal, 0, offset,
									Core.Constants.VeryShortAnimation, (x) => ((char)x.Content) > id);

					// Add the new button on the correct position
					MatricesStackPanel.Children.Insert(MatricesStackPanel.Children.IndexOf(buttonOnTheLeft) + 1, button);

					// As the new item was added, the buttons on the right were moved so we can change their offset back to 0, this time instantly 
					MatricesStackPanel.SlideChildren<Button>(Orientation.Horizontal, offset, 0,	0, (x) => ((char)x.Content) > id);
				}
				else
				{
					// else if the button on the left wasn't found, just add the new button
					MatricesStackPanel.Children.Add(button);
				}
			}
			

			// Animate the new button in
			button.SlideAsync(Orientation.Horizontal, this.ActualWidth, 0, Core.Constants.ShortAnimation);
		}

		/// <summary>
		/// Deletes a matrix button from the UI
		/// </summary>
		/// <param name="id">ID of the matrix to delete</param>
		private async Task DeleteMatrixButton(char id)
		{
			// Find the button with ID == id
			Button toDelete = MatricesStackPanel.FindChild<Button>((x) => ((char)x.Content) == id);			

			// If nothing was found (which shouldn't occure, but if it did)
			if(toDelete==null)
			{
				return;
			}

			// Animate it out
			toDelete.SlideAsync(Orientation.Horizontal, 0, this.ActualWidth, Core.Constants.ShortAnimation);

			// Slide by ActualWidth+leftMargin+rightMargin
			double offset = toDelete.ActualWidth + toDelete.Margin.Left + toDelete.Margin.Right;

			// Slide all button on the right to the left by offset and wait for animation to end
			await MatricesStackPanel.SlideChildren<Button>(Orientation.Horizontal, 0, -offset, Core.Constants.VeryShortAnimation, (x) => ((char)x.Content) > id);
			
			// Remove the button
			MatricesStackPanel.Children.Remove(toDelete);

			// Now instantly move all animated items back to their origin (so that they have 0 offset in the end)
			await MatricesStackPanel.SlideChildren<Button>(Orientation.Horizontal, -offset, 0, 0, (x) => ((char)x.Content) > id);
		}

		#endregion

		#region Handler of Current Matrix Changed event

		/// <summary>
		/// Shows the new matrix
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void CurrentMatrixChanged(object sender, MatricesModifiedEventArgs e)
		{

			foreach (var item in MatricesStackPanel.Children)
			{
				if (item is Button b)
				{
					b.Foreground = (Brush)Application.Current.Resources["WhiteBrush"];
					b.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
					if((char)b.Content == e.ID)
					{
						b.Foreground = (Brush)Application.Current.Resources["BlueBrush"];
						b.BorderBrush = (Brush)Application.Current.Resources["BlueBrush"];
					}
				}
			}

			await AnimateOutCurrentMatrixAsync();

			// Just to be certain there are no more children, clear them
			MainGrid.Children.Clear();
			
			// Make a dummy TextBox so that focus goes to it whenever the user clicks on the grid
			MainGrid.Children.Add(new TextBox()
			{
				Opacity = 0,
				IsReadOnly = true,
				Margin = new Thickness(0, 0, 200, 200),
				TabIndex = 1,
			});

			AnimateInCurrentMatrixAsync();
		}

		#endregion

		#region Handler of Navigation Requeste event

		private void ChildRequestedNavigation(object sender, NavigationRequestedEventArgs e)
		{
			this.Frame.Navigate(typeof(AboutPage));
		}

		#endregion
	}
}
