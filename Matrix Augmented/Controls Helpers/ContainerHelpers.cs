using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Matrix_Augmented
{
	/// <summary>
	/// Class containing helpers for container elements
	/// </summary>
	public static class ContainerHelpers
	{

		/// <summary>
		/// Returns the first child for which the predicate is true.
		/// If none was found, returns null 
		/// </summary>
		/// <typeparam name="T">Type of the child to look for, derived from <see cref="UIElement"/></typeparam>
		/// <param name="panel">Container to look in</param>
		/// <param name="predicate">Predicate to use</param>
		/// <returns>The element found, if nothing was found null</returns>
		public static T FindChild<T>(this Panel panel, Predicate<T> predicate)
			where T : UIElement
		{
			// For each child in the container...
			foreach(var item in panel.Children)
			{
				// If it's of type T...
				if(item is T t)
				{
					// If it passes the predicate...
					if(predicate(t))
					{
						// Return it
						return t;
					}
				}
			}

			// If nothing was found, return null
			return null;
		}
	}
}
