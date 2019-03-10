

using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Matrix_Augmented.Core
{
	/// <summary>
	/// Base class for view models in this application
	/// </summary>
	public class BaseViewModel : INotifyPropertyChanged
	{
		/// <summary>
		/// The event that is fired when any child property changed its value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

		/// <summary>
		/// Call this to fire a <see cref="PropertyChanged"/> event
		/// </summary>
		/// <param name="name"></param>
		public void OnPropertyChanged(string name)
		{
			PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

		public async Task RunCommand(Expression<Func<bool>> updatingFlag, Func<Task> action)
		{
			// Check if the flag is set (function is already running)
			if(updatingFlag.GetPropertyValue())
			{
				return;
			}

			// Set flag to true to indicate that we are running
			updatingFlag.SetPropertyValue(true);

			try
			{
				// Run the action
				await action();
			}
			finally
			{
				// Set the property flag back to false after finishing
				updatingFlag.SetPropertyValue(false);
			}
			
		}
	}

	
}
