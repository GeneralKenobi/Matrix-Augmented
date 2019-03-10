using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_Augmented.Core
{
	/// <summary>
	/// Class containing helpers for lists of matrices
	/// </summary>
	public static class ListOfMatricesHelpers
	{
		
		/// <summary>
		/// Returns the first free id
		/// If none is available, throws an exception
		/// </summary>
		/// <param name="matrices"></param>
		/// <returns></returns>
		public static char GetFirstFreeID(this List<BaseMatrix> matrices)
		{
			// For all lowercase letters...
			for(char c='a'; c<='z'; ++c)
			{
				// If there isn't a matrix with this ID...
				if (matrices.TrueForAll((x) => x.ID != c))
				{
					// Return it
					return c;
				}
			}

			// For all upppercase letters...
			for (char c = 'A'; c <= 'Z'; ++c)
			{
				// If there isn't a matrix with this ID...
				if (matrices.TrueForAll((x) => x.ID != c))
				{
					// Return it
					return c;
				}
			}

			throw new Exception("No more available IDs");
		}


		/// <summary>
		/// Returns the first free id
		/// If none is available, throws an exception
		/// </summary>
		/// <param name="matrices"></param>
		/// <returns></returns>
		public static int GetFirstFreeCalcID(this List<CalcMatrix> matrices)
		{
			// For each number in this range...
			for(int i=1111; i<=9999; ++i)
			{
				// If none matrix has that id
				if (matrices.TrueForAll((x) => x.CalcID != i))
				{
					// return it
					return i;
				}
			}

			// Otherwise, if all were taken (highly unlikely), throw an exception
			throw new Exception("Operation was to complex");
		}

		/// <summary>
		/// Copies all elements from originalList to newList
		/// </summary>
		/// <param name="originalList">List to copy from</param>
		/// <param name="newList">List to copy to</param>
		public static void CopyToCalcMatrixList(this List<Matrix> originalList, List<CalcMatrix> newList)
		{
			// Copy all matrices defined by user in there
			originalList.ForEach((x) =>
			{
				int id = newList.GetFirstFreeCalcID();

				newList.Add(new CalcMatrix(x.ID, id, x.Fields));
			});
		}

		/// <summary>
		/// Copies all elements from originalList to newList
		/// </summary>
		/// <param name="originalList">List to copy from</param>
		public static List<CalcMatrix> CopyToCalcMatrixList(this List<Matrix> originalList)
		{
			List<CalcMatrix> newList = new List<CalcMatrix>();

			// Copy all matrices defined by user in there
			originalList.ForEach((x) =>
			{
				int id = newList.GetFirstFreeCalcID();

				newList.Add(new CalcMatrix(x.ID, id, x.Fields));
			});

			return newList;
		}
	}
}
