using Matrix_Augmented.Core;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Matrix_Augmented
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class AboutPage : Page
	{

		#region Constructor

		public AboutPage()
		{
			this.InitializeComponent();
			BackCommand = new RelayCommand(() => BackCommandMethod());
			this.DataContext = new AboutViewModel();
			this.Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			AboutViewModel vm = (AboutViewModel)DataContext;

			for(int i=0; i<vm.Instructions.Length; ++i)
			{
				TextBlock tb = new TextBlock();
				tb.ConfigureForAboutTextBlock(i);
				Stack.Children.Add(tb);
			}
		}

		#endregion

		#region Back Command

		/// <summary>
		/// Command for the back button, does backward navigation
		/// </summary>
		private ICommand BackCommand { get; set; }

		/// <summary>
		/// Method for the <see cref="BackCommand"/>
		/// </summary>
		private void BackCommandMethod()
		{
			this.Frame.GoBack();
		}

		#endregion
	}
}
