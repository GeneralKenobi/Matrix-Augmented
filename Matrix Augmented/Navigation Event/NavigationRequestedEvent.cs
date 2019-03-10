using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_Augmented
{

	/// <summary>
	/// Event for when a matrix is resized
	/// </summary>
	/// <param name="sender">Matrix that is resized</param>
	/// <param name="e">Information about the resize</param>
	public delegate void NavigationRequestedEventHandler(object sender, NavigationRequestedEventArgs e);


	/// <summary>
	/// Enum with types of pages used in this app
	/// </summary>
	public enum PageType
	{
		MainPage = 0,
		AboutPage = 1,
	}

	/// <summary>
	/// Class for argument of the resize event
	/// </summary>
	public class NavigationRequestedEventArgs : EventArgs
	{

		#region Public Properties

		/// <summary>
		/// Type of the page to navigate to
		/// </summary>
		public PageType Type { get; private set; }

		#endregion

		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="type">Page to navigate to</param>
		public NavigationRequestedEventArgs(PageType type)
		{
			Type = type;
		}

		#endregion
	}
}
