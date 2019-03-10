using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_Augmented.Core
{

	/// <summary>
	/// View model for the about page
	/// </summary>
	public class AboutViewModel : BaseViewModel
	{

		#region Public Properties

		/// <summary>
		/// Array of instructions that are to be shown on screen
		/// </summary>
		public string[] Instructions { get; set; }

		#endregion

		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public AboutViewModel()
		{
			// Create the array
			Instructions = new string[6];

			// And set the instructions
			Instructions[0] = "Equation input instructions:";
			Instructions[1] = "Please use capital letters for the matrices (lowercase letters are reserved for special actions)";
			Instructions[2] = "Use 'i(something)' to represent the inverse of something";
			Instructions[3] = "Use t(something) to represent the transposition of something";
			Instructions[4] = "Use d(something) to calculate the determinant of something";
			Instructions[5] = "Use r(something) to calculate the rank of something";
		}

		#endregion
	}
}
