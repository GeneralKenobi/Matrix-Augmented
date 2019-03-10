using Matrix_Augmented.Core;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Matrix_Augmented
{
	public sealed partial class BottomMenu : UserControl
    {
		#region Private Members

		/// <summary>
		/// Flag denoting if this control is currently shown or hidden
		/// </summary>
		private bool IsShown { get; set; } = true;

		/// <summary>
		/// Flag denoting if the EquationBox is currently shown
		/// </summary>
		private bool IsEquationBoxShown { get; set; } = false;

		#endregion

		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public BottomMenu()
        {
            this.InitializeComponent();
			this.SizeChanged += OnSizeChanged;
			// Generate the command to show/collapse itself
			ShowHideMenuCommand = new RelayCommand(async () => await ShowHide());
			ShowHideEquationBoxCommand = new RelayCommand(() => ShowHideEquationBoxCommandMethod());
		}

		#endregion

		#region On Size Changed

		/// <summary>
		/// Method adjusting the position of this control (only when hidden) after size has changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			// If the menu is currently shown, nothing to do here
			if(IsShown)
			{
				return;
			}

			double offset = this.ActualHeight - 80;

			this.SlideAsync(Orientation.Vertical, 0, offset, 0);
		}

		#endregion

		#region Commands

		/// <summary>
		/// Command expanding/collapsing this element
		/// </summary>
		public ICommand ShowHideMenuCommand { get; set; }


		public ICommand ShowHideEquationBoxCommand { get; set; }

		#endregion

		#region Show/Collapse

		private async Task ShowHide()
		{
			double offset = this.ActualHeight - 80;

			// Notify all clients that the menu state is about to change
			OnMenuStateChanging(new MenuStateChangeEventArgs(IsShown ? MenuStateChangeEventType.Hidden : MenuStateChangeEventType.Shown, IsShown ? 0 : offset));

			// Call the slide method, if we're shown start at 0, if not at the negative offset
			// If we're shown go to offset, if we're not go back to 0
			await this.SlideAsync(Orientation.Vertical, IsShown ? 0 : offset, IsShown ? offset : 0, easing: new SineEase());

			// Notify all clients that the menu state has just changed
			OnMenuStateChanged(new MenuStateChangeEventArgs(IsShown ? MenuStateChangeEventType.Hidden : MenuStateChangeEventType.Shown, IsShown ? 0 : offset));

			// Flip the IsShown flag;
			IsShown = !IsShown;
		}

		#endregion

		#region ShowHideEquationBox

		private async Task ShowHideEquationBoxCommandMethod()
		{
			if(IsEquationBoxShown)
			{
				EqBox.OpacityAnimation(1, 0, Core.Constants.VeryShortAnimation, new SineEase());
				await EqBox.SlideAsync(Orientation.Vertical, -85, 0, Core.Constants.ShortAnimation);
				EqBox.Visibility = Visibility.Collapsed;
			}
			else
			{				
				EqBox.Visibility = Visibility.Visible;
				EqBox.SlideAsync(Orientation.Vertical, 0, -85, Core.Constants.ShortAnimation);
				EqBox.OpacityAnimation(0, 1, Core.Constants.VeryShortAnimation, new SineEase());
			}

			IsEquationBoxShown = !IsEquationBoxShown;
		}
		
		#endregion

		#region Menu State Changing Event

		/// <summary>
		/// Event for when the MenuState is about to change
		/// </summary>
		public event MenuStateChangeEventHandler MenuStateChanging;

		/// <summary>
		/// Method for the <see cref="MenuStateChanging"/> event
		/// </summary>
		/// <param name="e"></param>
		private void OnMenuStateChanging(MenuStateChangeEventArgs e) => MenuStateChanging?.Invoke(this, e);

		#endregion

		#region Menu State Changed Event

		/// <summary>
		/// Event for when the menu state has just changed
		/// </summary>
		public event MenuStateChangeEventHandler MenuStateChanged;

		/// <summary>
		/// Method for the <see cref="MenuStateChanged"/> event
		/// </summary>
		/// <param name="e"></param>
		private void OnMenuStateChanged(MenuStateChangeEventArgs e) => MenuStateChanged?.Invoke(this, e);

		#endregion

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

		#region Navigation Command

		/// <summary>
		/// Passes on the notification to the parent page
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void ChildRequestedNavigation(object sender, NavigationRequestedEventArgs e)
		{
			// Notify the listener (parent page)
			OnNavigationRequested(e);
		}

		#endregion		
	}
}

