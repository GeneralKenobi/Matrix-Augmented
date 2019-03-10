using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_Augmented
{
	/// <summary>
	/// Event for when the menu is hidden/shown
	/// </summary>
	/// <param name="sender">The menu</param>
	/// <param name="e">Information about the state change</param>
	public delegate void MenuStateChangeEventHandler(object sender, MenuStateChangeEventArgs e);

	/// <summary>
	/// State of the menu
	/// </summary>
	public enum MenuStateChangeEventType
	{
		/// <summary>
		/// Menu will be shown
		/// </summary>
		Shown = 0,

		/// <summary>
		/// Menu will be hidden
		/// </summary>
		Hidden = 1,
	}

	/// <summary>
	/// Argument for the <see cref="MenuStateChangeEventHandler"/> event
	/// </summary>
	public class MenuStateChangeEventArgs : EventArgs
	{
		/// <summary>
		/// Type of the state change
		/// </summary>
		public MenuStateChangeEventType Type { get; private set; }

		/// <summary>
		/// Offset of the menu in respect to its default position (hidden)
		/// </summary>
		public double Offset { get; private set; }

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="type">Type of the state change</param>
		/// <param name="offset">Offset of the menu in respect to its default position (hidden)</param>
		public MenuStateChangeEventArgs(MenuStateChangeEventType type, double offset)
		{
			Type = type;
			Offset = offset;
		}
	}
}
