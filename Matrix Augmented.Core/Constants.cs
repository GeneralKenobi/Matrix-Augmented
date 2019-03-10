using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_Augmented.Core
{
	public static class Constants
	{
		/// <summary>
		/// All decimal values will be rounded to this place after the dot
		/// </summary>
		public static int Precision => 4;

		/// <summary>
		/// Maximum number of rows/columns a matrix can have
		/// </summary>
		public static int MaxMatrixSize => 10;

		/// <summary>
		/// Time a very short animation takes, in miliseconds
		/// </summary>
		public static int VeryShortAnimation => 150;

		/// <summary>
		/// Time a short animation takes, in miliseconds
		/// </summary>
		public static int ShortAnimation => 300;

		/// <summary>
		/// Time an average animation takes, in miliseconds
		/// </summary>
		public static int AverageAnimation => 500;

		/// <summary>
		/// Time a long animation takes, in miliseconds
		/// </summary>
		public static int LongAnimation => 800;
	}
}
