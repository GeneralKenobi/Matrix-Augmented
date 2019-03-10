using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_Augmented.Core
{
	/// <summary>
	/// Type of operation to perform on a matrix
	/// </summary>
	public enum MatrixOperationType
	{
		Transposition = 1,
		Inverse = 2,
		Determinant = 3,
		Rank = 4,
	}	
}
