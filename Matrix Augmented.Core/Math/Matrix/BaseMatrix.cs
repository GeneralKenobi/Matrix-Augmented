using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_Augmented.Core
{

	/// <summary>
	/// Base class for a Matrix
	/// Default for Fields is a 1x1 array
	/// </summary>
	public class BaseMatrix
	{

		#region Private Members

		private Fraction[,] mFields = new Fraction[1,1];

		#endregion

		#region Public Properties

		/// <summary>
		/// ID of this matrix
		/// </summary>
		public char ID { get; set; } = '\0';

		/// <summary>
		/// 2d Array of fractions which form the matrix
		/// <para/>
		/// First coordinate is the row, second is the column.
		/// Indexing starts at 0
		/// </summary>
		public Fraction[,] Fields
		{
			get => mFields;
			set
			{
				if (value != null && value.Rank != 2)
				{
					throw new Exception("Matrix has to be a 2d array");
				}

				mFields = value;
			}
		}

		#endregion

	}
}
