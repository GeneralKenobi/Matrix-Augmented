using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_Augmented.Core
{
	public static class ArrayHelpers
	{

		#region Construction Helpers

		/// <summary>
		/// Creates a new object for each position (in the given 2d array) which is null.
		/// Uses default construtor
		/// </summary>
		/// <typeparam name="T">Type of the array</typeparam>
		/// <param name="array">2d array of the type T</param>
		public static void RemoveNull<T>(this T[,] array)
			where T: new()
		{
			// For each row...
			for (int i = 0; i < array.GetLength(0); ++i)
			{
				// For each element in that row...
				for (int j = 0; j < array.GetLength(1); ++j)
				{
					// If it's null, make a new instance
					if (array[i, j] == null)
					{
						array[i, j] = new T();
					}
				}
			}
		}

		/// <summary>
		/// Returns a copy of the array
		/// </summary>
		/// <typeparam name="T">Type of the array</typeparam>
		/// <param name="array">Array to copy</param>
		/// <returns>Copy of the passed array</returns>
		public static Fraction[,] Copy(this Fraction[,] array)			
		{
			Fraction[,] copy = new Fraction[array.Rows(), array.Columns()];

			// For each row...
			for(int i=0; i<array.Rows(); ++i)
			{
				// For each field in that row...
				for(int j=0; j<array.Columns(); ++j)
				{
					copy[i, j] = new Fraction(array[i, j]);
				}
			}

			return copy;
		}

		/// <summary>
		/// Sets all fields in the matrix to 0
		/// </summary>
		/// <param name="matrix">Matrix to reset</param>
		public static void Reset(this Fraction[,] matrix)
		{
			// For each row...
			for(int i=0; i<matrix.Rows(); ++i)
			{
				// For each element in that row...
				for(int j=0; j<matrix.Columns(); ++j)
				{
					// Set it to 0
					matrix[i, j].Numerator=0;
					matrix[i, j].Denominator = 1;
				}
			}
		}

		/// <summary>
		/// If the matrix is square, transforms it to an identity matrix
		/// (only nonzero values are 1s on the main diagonal)
		/// </summary>
		/// <param name="matrix">Matrix to become identity matrix (MODIFIED)</param>
		public static void Identity(this Fraction[,] matrix)
		{
			// If the matrix isn't square, return
			if(!matrix.IsSquare())
			{
				return;
			}

			// Reset the matrix
			matrix.Reset();

			// For each row...
			for(int i=0; i<matrix.Rows(); ++i)
			{
				// Set it's diagonal element to 1
				matrix[i, i].Numerator = 1;
				matrix[i, i].Denominator = 1;
			}
		}

		#endregion		

		#region Transposition		
		
		/// <summary>
		/// Returns a transposed matrix (writes each column as a row)
		/// <para/>
		/// 1 2 3 | -> | 1 4 7
		/// <para/>
		/// 4 5 6 | -> | 2 5 8
		/// <para/>
		/// 7 8 9 | -> | 3 6 9
		/// </summary>
		/// <param name="matrix">Matrix to transpose</param>
		public static Fraction[,] Transpose(this Fraction[,] matrix)
		{
			// Create a new matrix to store the result
			Fraction[,] result = new Fraction[matrix.Columns(), matrix.Rows()];

			// For each row...
			for(int i=0; i<matrix.Rows(); ++i)
			{
				// For each element in that row...
				for(int j=0; j<matrix.Columns(); ++j)
				{
					// Write it to the corresponding column in result
					result[j, i] = matrix[i, j];
				}
			}

			return result;
		}		
		
		#endregion	

		#region Row-Echelon Form

		/// <summary>
		/// Transforms this matrix into a row-echelon form. Returns the number of performed row swap
		/// </summary>
		/// <param name="matrix">Matrix to transform</param>
		/// <param name="unityPivots">If true, all pivot numbers will be either 0 or 1</param>
		/// <returns>Number of row swaps performed</returns>
		public static int RowEchelon(this Fraction[,] matrix, bool unityPivots = true)
		{
			int smallerDimension = Math.Min(matrix.Rows(), matrix.Columns());

			int performedRowSwaps = 0;
			int expectedPivot = 0;

			// We continue until we ran out of columns/rows (whicher is smaller dimension)
			// i is the current row we're in
			for(int i=0; i<smallerDimension; ++i)
			{
				// If there are no more non-zero rows, end the algorithm
				if (!matrix.NonzeroBelow(i))
				{
					break;
				}			

				// Find the pivot of the current row
				int pivot = matrix.Pivot(i);

				// If it's not the one we expect...
				if(pivot != expectedPivot)
				{
					// Try to find a row below which contains such pivot
					int rowWithPivot = matrix.RowWithPivot(expectedPivot, i);

					// If none was found
					if(rowWithPivot==-1)
					{
						// Increase the expectec pivot by 1						
						++expectedPivot;						
					}
					else
					{
						// Otherwise swap rows
						matrix.SwapRows(i, rowWithPivot);
						++performedRowSwaps;
					}

					// Decrement i to go through this row once again
					// And end this iteration
					--i;
					continue;
				}

				// If we got here it means we have our desired pivot

				// For each row below...
				for(int j = i+1; j<matrix.Rows(); ++j)
				{
					// Subtract each row so that entries on the left of the pivot will be 0
					Fraction scalar = matrix[j, pivot] / matrix[i, pivot];
					matrix.SubtractRowFromRow(i, j, scalar);
				}
			}

			// Finally, divide each row by the diagonal value if the caller wants it
			if (unityPivots)
			{
				matrix.DivideRowsByPivots();
			}

			return performedRowSwaps;
		}

		#endregion

		#region Triangular Matrix

		/// <summary>
		/// Transforms the given matrix into a triangular matrix
		/// </summary>
		/// <param name="matrix">Matrix to transform. Has to be square</param>
		/// <param name="augmented">Optional matrix on which exact same operations will be performed
		/// It should have at least the same number of rows as <paramref name="matrix"/></param>
		public static void Triangular(this Fraction[,] matrix, Fraction[,] augmented = null)
		{
			// If the given matrix isn't square, return
			if(!matrix.IsSquare())
			{
				return;
			}

			// If the augmented matrix doesn't have enough rows
			if(augmented?.Rows() < matrix.Rows())
			{
				// Make it null for this method
				augmented = null;
			}

			int expectedPivot = 0;

			// We continue until we ran out of columns/rows (whicher is smaller dimension)
			// i is the current row we're in
			for (int i = 0; i < matrix.Rows(); ++i)
			{
				// If there are no more non-zero rows, end the algorithm
				if (!matrix.NonzeroBelow(i))
				{
					break;
				}
				
				// Find the pivot of the current row
				int pivot = matrix.Pivot(i);

				// If it's not the one we expect...
				if (pivot != expectedPivot)
				{
					// Try to find a row below which contains such pivot
					int rowWithPivot = matrix.RowWithPivot(expectedPivot, i + 1);

					// If none was found
					if (rowWithPivot == -1)
					{
						// Increase the expectec pivot by 1						
						++expectedPivot;
					}
					else
					{
						// Otherwise swap rows
						matrix.SwapRows(i, rowWithPivot);
						augmented?.SwapRows(i, rowWithPivot);
					}

					// Decrement i to go through this row once again
					// And end this iteration
					--i;
					continue;
				}

				// If we got here it means we have our desired pivot

				// For each row below...
				for (int j = i + 1; j < matrix.Rows(); ++j)
				{
					// Subtract each row so that entries on the left of the pivot will be 0
					Fraction scalar = matrix[j, pivot] / matrix[i, pivot];

					matrix.SubtractRowFromRow(i, j, scalar);
					augmented?.SubtractRowFromRow(i, j, scalar);
				}

				++expectedPivot;
			}
		}

		#endregion

		#region Inverse

		/// <summary>
		/// Returns the inverse of the provided matrix
		/// </summary>
		/// <param name="matrix">Matrix to transform. Has to be sqaure. (MODIFIED)</param>
		/// <returns>Matrix that is inverse of the transformed matrix</returns>
		public static Fraction[,] Inverse(this Fraction[,] matrix)
		{
			// Make a copy to work on
			Fraction[,] copy = matrix.Copy();

			// Create an identity matrix
			Fraction[,] identity = new Fraction[copy.Rows(), copy.Columns()];
			identity.RemoveNull();
			identity.Identity();

			// Transform the first matrix into triangular matrix
			// and simultaneously do the same operations on identity matrix
			copy.Triangular(identity);

			// Make sure all entries on the diagonal are 1 or 0
			copy.DivideRowsByDiagonalEntries(identity);

			// For each row starting from the bottom...
			for (int row = copy.Rows() - 1; row >= 0; --row)
			{
				// If the entry on the main diagonal isn't 0...
				if (copy[row, row] != 0)
				{
					for (int rowAbove = 0; rowAbove < row; ++rowAbove)
					{
						// Find the scalar to multiply each element by
						Fraction scalar = copy[rowAbove, row] / copy[row, row];

						// Subtract rows in both matrices
						copy.SubtractRowFromRow(row, rowAbove, scalar);
						identity.SubtractRowFromRow(row, rowAbove, scalar);
					}
				}
			}

			// Now copy is an identity matrix
			// And identity is the inverse

			return identity;
		}

		#endregion

		#region LU Decomposition

		/// <summary>
		/// Decomposes the given matrix (but doesn't modify) into a lower triangular matrix
		/// and upper triangular matrix, so that matrix = lower*upper
		/// </summary>
		/// <param name="matrix">Matrix to decompose (doesn't modify)</param>
		/// <param name="lower">Matrix in which the lower triangular matrix will be saved</param>
		/// <param name="upper">Matrix in which the upper triangular matrix will be saved</param>
		public static void LUDecomposition(this Fraction[,] matrix, out Fraction[,] lower, out Fraction[,] upper)
		{
			// If the matrix isn't square, don't do anything
			if (!matrix.IsSquare())
			{
				lower = null;
				upper = null;
				return;
			}

			// copy matrix to U
			upper = matrix.Copy();

			// Initialize L as a zero-matrix of the same size as U
			lower = new Fraction[upper.Rows(), upper.Columns()];
			lower.RemoveNull();			

			// Currently expected pivot in the matrix (the first non-zero element in the row)
			int expectedPivot = 0;

			// We continue until we ran out of non-zero rows	
			for (int currentRow = 0; currentRow < upper.Rows(); ++currentRow)
			{
				// If there are no more non-zero rows below, end the algorithm
				if (!matrix.NonzeroBelow(currentRow))
				{
					break;
				}

				// Find the pivot of the current row
				int pivot = upper.Pivot(currentRow);

				// If it's not the one we expect...
				if (pivot != expectedPivot)
				{
					// Try to find a row below which contains such pivot
					int rowWithPivot = upper.RowWithPivot(expectedPivot, currentRow + 1);

					// If none was found
					if (rowWithPivot == -1)
					{
						// Increase the expected pivot by 1
						++expectedPivot;
					}
					else
					{
						// Otherwise swap rows with it
						upper.SwapRows(currentRow, rowWithPivot);
						lower.SwapRows(currentRow, rowWithPivot);
					}

					// Decrement currentRow to go through this row once again
					// And end this iteration
					--currentRow;
					continue;
				}

				// If we got here it means that current row has the proper pivot				

				// For each row below...
				for (int rowBelow = currentRow + 1; rowBelow < upper.Rows(); ++rowBelow)
				{
					// Make a scalar which is a quation of the elements in the pivot column: rowBelow/CurrentRow
					Fraction scalar = upper[rowBelow, pivot] / upper[currentRow, pivot];

					// Subtract each row so that entries on the left and below the pivot will be 0
					upper.SubtractRowFromRow(currentRow, rowBelow, scalar);
					lower[rowBelow, currentRow] = new Fraction(scalar);
				}

				// Increase the pivot (we're moving to the right)
				++expectedPivot;
			}

			// Finally, fill the main diagonal the lower matrix with 1s
			for(int i=0; i<lower.Rows(); ++i)
			{
				lower[i, i] = new Fraction(1);
			}
		}

		/// <summary>
		/// Decomposes the given matrix (but doesn't modify) into a lower triangular matrix
		/// and upper triangular matrix, so that matrix = lower*upper
		/// </summary>
		/// <param name="matrix">Matrix to decompose (doesn't modify)</param>
		/// <param name="lower">Matrix in which the lower triangular matrix will be saved</param>
		/// <param name="upper">Matrix in which the upper triangular matrix will be saved</param>
		public static void LUDecompositionBis(this Fraction[,] matrix, out Fraction[,] lower, out Fraction[,] upper)
		{
			
			// copy matrix to U
			upper = matrix.Copy();

			// Initialize L as a zero-matrix of the same size as U
			lower = new Fraction[upper.Rows(), upper.Rows()];
			lower.RemoveNull();

			// Currently expected pivot in the matrix (the first non-zero element in the row)
			int expectedPivot = 0;

			int smallerDimension = Math.Min(matrix.Rows(), matrix.Columns());

			// We continue until we ran out of non-zero rows	
			for (int currentRow = 0; currentRow < smallerDimension; ++currentRow)
			{
				// If there are no more non-zero rows below, end the algorithm
				if (!matrix.NonzeroBelow(currentRow))
				{
					break;
				}

				// Find the pivot of the current row
				int pivot = upper.Pivot(currentRow);

				// If it's not the one we expect...
				if (pivot != expectedPivot)
				{
					// Try to find a row below which contains such pivot
					int rowWithPivot = upper.RowWithPivot(expectedPivot, currentRow + 1);

					// If none was found
					if (rowWithPivot == -1)
					{
						// Increase the expected pivot by 1
						++expectedPivot;
					}
					else
					{
						// Otherwise swap rows with it
						upper.SwapRows(currentRow, rowWithPivot);
						lower.SwapRows(currentRow, rowWithPivot);
					}

					// Decrement currentRow to go through this row once again
					// And end this iteration
					--currentRow;
					continue;
				}

				// If we got here it means that current row has the proper pivot				

				// For each row below...
				for (int rowBelow = currentRow + 1; rowBelow < upper.Rows(); ++rowBelow)
				{
					// Make a scalar which is a quation of the elements in the pivot column: rowBelow/CurrentRow
					Fraction scalar = upper[rowBelow, pivot] / upper[currentRow, pivot];

					// Subtract each row so that entries on the left and below the pivot will be 0
					upper.SubtractRowFromRow(currentRow, rowBelow, scalar);
					lower[rowBelow, currentRow] = new Fraction(scalar);
				}

				// Increase the pivot (we're moving to the right)
				++expectedPivot;
			}

			// Finally, fill the main diagonal the lower matrix with 1s
			for (int i = 0; i < lower.Rows(); ++i)
			{
				lower[i, i] = new Fraction(1);
			}
		}

		#endregion

		#region Row manipulation

		/// <summary>
		/// Multiplies each entry in the given row by the provided value.
		/// If the given row is not valid, throws an exception
		/// </summary>
		/// <param name="matrix">Matrix to work on (MODIFIED)</param>
		/// <param name="row">Row to multiply. Indexing starts at 0</param>
		/// <param name="value">Value to multiply by</param>
		public static void MultiplyRow(this Fraction[,] matrix, int row, Fraction value)
		{
			// Check if the given row is valid
			if (row < 0 || row >= matrix.Rows())
			{
				throw new Exception("Invalid row");
			}

			// Multiply each entry in the row by value
			for (int i = 0; i < matrix.Columns(); ++i)
			{
				matrix[row, i] *= value;
			}
		}

		/// <summary>
		/// Divides each entry in the given row by the provided value.
		/// If the given row is not valid, throws an exception
		/// </summary>
		/// <param name="matrix">Matrix to work on (MODIFIED)</param>
		/// <param name="row">Row to divide. Indexing starts at 0</param>
		/// <param name="value">Value to divide by</param>
		public static void DivideRow(this Fraction[,] matrix, int row, Fraction value)
		{
			// Copy the value
			Fraction copy = new Fraction(value);

			// Flip the copy
			copy.Flip();

			// Multiply the row by flipped copy
			matrix.MultiplyRow(row, copy);
		}

		/// <summary>
		/// Adds one row to another. If scalar is null, it will be treated as '1' (meaning no impact).
		/// If any of the rows is invalid, an exception will be thrown
		/// </summary>
		/// <param name="matrix">Matrix to operate on (MODIFIED)</param>
		/// <param name="rowToAdd">Row to be added</param>
		/// <param name="rowToAddTo">Row to add to</param>
		/// <param name="scalar">Scalar by which each added value will be multiplied. If it's null, it will be treated as '1' (meaning no impact)</param>
		public static void AddRowToRow(this Fraction[,] matrix, int rowToAdd, int rowToAddTo, Fraction scalar = null)
		{
			// Check if the given row is valid
			if (rowToAdd < 0 || rowToAdd >= matrix.Rows() || rowToAddTo < 0 || rowToAddTo >= matrix.Rows())
			{
				throw new Exception("Invalid row(s)");
			}

			// If scalar is null, initialize it as 1
			if (scalar == null)
			{
				scalar = new Fraction(1);
			}

			// Add the value to each entry in the row
			for (int i = 0; i < matrix.Columns(); ++i)
			{
				matrix[rowToAddTo, i] += scalar * matrix[rowToAdd, i];
			}
		}

		/// <summary>
		/// Subtracts one row from another. If scalar is null, it will be treated as '1' (meaning no impact).
		/// If any of the rows is invalid, an exception will be thrown
		/// </summary>
		/// <param name="matrix">Matrix to operate on (MODIFIED)</param>
		/// <param name="rowToAdd">Row to be subtracted</param>
		/// <param name="rowToAddTo">Row to subtract from</param>
		/// <param name="scalar">Scalar by which each subtracted value will be multiplied. If it's null, it will be treated as '1' (meaning no impact)</param>
		public static void SubtractRowFromRow(this Fraction[,] matrix, int rowToSubtract, int rowToSubtractFrom, Fraction scalar = null)
		{
			// If scalar is null, initialize it as 1
			if (scalar == null)
			{
				scalar = new Fraction(1);
			}

			// Change the sign
			Fraction copy = new Fraction(scalar * -1);

			// Add the negated copy
			matrix.AddRowToRow(rowToSubtract, rowToSubtractFrom, copy);
		}

		/// <summary>
		/// Divides each row by the value on the diagonal so that each entry on the diagonal is either 1 or 0
		/// <para/>
		/// If an entry on the diagonal is 0, does nothing to the row.
		/// If the matrix isn't square, does nothing
		/// </summary>
		/// <param name="matrix">Matrix to work on (MODIFIED)</param>
		/// <param name="augmented">extension matrix on which identical operations will be performed (MODIFIED)</param>
		public static void DivideRowsByDiagonalEntries(this Fraction[,] matrix, Fraction[,] augmented = null)
		{
			// Check if passed matrix is square
			if (!matrix.IsSquare())
			{
				return;
			}

			int length = matrix.Rows();

			// For each row
			for (int row = 0; row < length; ++row)
			{
				// Find the divisor on the diagonal
				Fraction divisor = matrix[row, row];

				// If the diagonal entry is equal to 0, continue to the next row
				if (divisor == 0)
				{
					continue;
				}

				// Divide by it
				matrix.DivideRow(row, divisor);
				augmented?.DivideRow(row, divisor);
			}
		}

		/// <summary>
		/// Divides each row by the value on the diagonal so that each entry on the diagonal is either 1 or 0
		/// <para/>
		/// If an entry on the diagonal is 0, does nothing to the row.
		/// If the matrix isn't square, does nothing
		/// </summary>
		/// <param name="matrix">Matrix to work on (MODIFIED)</param>
		/// <param name="augmented">extension matrix on which identical operations will be performed (MODIFIED)</param>
		public static void DivideRowsByPivots(this Fraction[,] matrix, Fraction[,] augmented = null)
		{
			// Check if passed matrix is square
			if (!matrix.IsSquare())
			{
				return;
			}

			int length = matrix.Rows();

			// For each row
			for (int row = 0; row < length; ++row)
			{
				// Find the pivot index
				int pivotIndex = matrix.Pivot(row);

				// If it wasn't -1 (means we found it)
				if (pivotIndex != -1)
				{
					// Find the divisor
					Fraction divisor = matrix[row, pivotIndex];

					// If it's equal to 0, continue to the next row
					if (divisor == 0)
					{
						continue;
					}

					// Divide by it
					matrix.DivideRow(row, divisor);
					augmented?.DivideRow(row, divisor);
				}
			}
		}

		#endregion

		#region Row swapping

		/// <summary>
		/// Swaps the rows so that elements of row1 are located in row2 and elements of row2 are located in row1
		/// <para/>
		/// If the provided rows aren't valid, throws an exception
		/// </summary>
		/// <param name="matrix">Matrix to swap rows in (MODIFIED)</param>
		/// <param name="row1">Indexing starts at 0</param>
		/// <param name="row2">Indexing starts at 0</param>
		public static void SwapRows(this Fraction[,] matrix, int row1, int row2)
		{
			// Check if the provided rows are correct
			if (row1 < 0 || row2 < 0 || row1 >= matrix.Rows() || row2 >= matrix.Rows())
			{
				throw new Exception("Invalid row(s)");
			}

			// For each column...
			for (int i = 0; i < matrix.Columns(); ++i)
			{
				Fraction temp = matrix[row1, i];
				matrix[row1, i] = matrix[row2, i];
				matrix[row2, i] = temp;
			}
		}

		/// <summary>
		/// Swaps the given row with the first found nonzero row starting at the bottom.
		/// If it reaches the given row, it won't swap (i.e. it won't swap with rows above the given row)
		/// </summary>
		/// <param name="matrix">Matrix to work on</param>
		/// <param name="row">Row to swap (indexing starts at 0)</param>
		/// <returns>Zero-based index of the found row, if none was found it returns -1</returns>
		public static int SwapRowWithFirstNonzero(this Fraction[,] matrix, int row)
		{
			// For each row starting at the bottom and below the given row...
			for (int i = matrix.Rows() - 1; i > row; --i)
			{
				// If it's nonzero...
				if (matrix.IsRowNonzero(i))
				{
					// Swap
					matrix.SwapRows(row, i);

					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// Swaps the given row with the first row below that has non-zero entry in the pos-th column
		/// </summary>
		/// <param name="matrix">Matrix to work on</param>
		/// <param name="row">Row to swap (indexing starts at 0)</param>
		/// <param name="pos">Position which is supposed to be nonzero in the found row (indexing starts at 0)</param>
		/// <returns>True if such row has been found, false otherwise</returns>
		private static bool SwapRowWithNonzeroPosition(this Fraction[,] matrix, int row, int pos)
		{
			// If the given arguments are incorrect
			if (row >= matrix.Rows() || pos >= matrix.Columns())
			{
				return false;
			}

			// For each row below...
			for (int i = row + 1; i < matrix.Rows(); ++i)
			{
				// If the pos in that row is nonzero...
				if (matrix[i, pos] != 0)
				{
					// Swap the rows
					matrix.SwapRows(row, i);
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Swaps rows so that all zero rows are on bottom
		/// </summary>
		/// <param name="matrix">Matrix to work on</param>
		/// <param name="augmented">Optional matrix, if provided identical operations will be performed on it</param>
		private static void PositionZeroRowsOnBottom(this Fraction[,] matrix, Fraction[,] augmented = null)
		{
			// If the augmented matrix has too few rows, don't do anything to it
			if (augmented != null && augmented.Rows() < matrix.Rows())
			{
				augmented = null;
			}

			// For each row
			for (int i = 0; i < matrix.Rows(); ++i)
			{
				// If that row is zero row...
				if (!matrix.IsRowNonzero(i))
				{
					// Swap it
					int swappedRow = matrix.SwapRowWithFirstNonzero(i);

					if (swappedRow == -1)
					{
						// If none rows to swap with were found, it means that all nonzer rows are already on top
						return;
					}
					else
					{
						// Otherwise, do the same swap to augmented
						augmented?.SwapRows(i, swappedRow);
					}
				}
			}
		}

		/// <summary>
		/// Positions rows for the triangular matrix so that for each row i,j,
		/// where i is smaller than j, i's leading number has smaller index than j's
		/// </summary>
		/// <param name="matrix">Matrix to sort</param>
		/// <param name="augmented">Additional matrix for each exact swaps will be performed</param>
		private static void SortRowsInTriangular(this Fraction[,] matrix, Fraction[,] augmented = null)
		{
			// If the augmented matrix has too few rows, don't do anything to it
			if (augmented != null && augmented.Rows() < matrix.Rows())
			{
				augmented = null;
			}

			// For each row...
			for (int i = 0; i < matrix.Rows(); ++i)
			{
				// If the entry on the main diagonal is non-zero, it means this row is in the correct position
				if (matrix[i, i] != 0)
				{
					continue;
				}

				int currentLeading = matrix.PositionOfLeadingNumber(i);

				for (int j = i + 1; j < matrix.Rows(); ++j)
				{
					int jLeading = matrix.PositionOfLeadingNumber(j);
					if (jLeading != -1 && jLeading < currentLeading)
					{
						matrix.SwapRows(i, j);
						augmented?.SwapRows(i, j);
						--i;
						break;
					}
				}

			}

		}

		#endregion

		#region Matrix information

		/// <summary>
		/// Number of rows this array has
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		public static int Rows(this Fraction[,] array) => array.GetLength(0);

		/// <summary>
		/// Number of columns this array has
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		public static int Columns(this Fraction[,] array) => array.GetLength(1);

		/// <summary>
		/// True if the array is square, false otherwise
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		public static bool IsSquare(this Fraction[,] array) => array.Rows() == array.Columns();

		/// <summary>
		/// Returns 
		/// </summary>
		/// <param name="matrix">Matrix to check</param>
		/// <returns></returns>
		public static int NonzeroRows(this Fraction[,] matrix)
		{
			int nonzeroRows = 0;

			// For each row...
			for (int i = 0; i < matrix.Rows(); ++i)
			{
				// If the row is nonzero, increase the number of found rows
				if (matrix.IsRowNonzero(i))
				{
					++nonzeroRows;
				}
			}

			return nonzeroRows;
		}

		/// <summary>
		/// Returns the product (result of multiplication) of elements on the main diagonal.
		/// Requires a square matrix. If the matrix isn't square, returns null
		/// </summary>
		/// <param name="Matrix">Square matrix to use in calculations</param>
		/// <returns></returns>
		public static Fraction ProductOfDiagonal(this Fraction[,] matrix)
		{
			// Check if the matrix is square
			if (!matrix.IsSquare())
			{
				return null;
			}

			// Initialize the result as 1
			Fraction result = new Fraction(1);

			// Multiply it by every entry on the diagonal
			for (int i = 0; i < matrix.Rows(); ++i)
			{
				result *= matrix[i, i];
			}

			return result;
		}
		
		/// <summary>
		/// Returns the zero-based position of the leading number in the given row.
		/// (i. e. the position of the first non-zero entry in this row).
		/// If the row is zeros only, returns -1
		/// </summary>
		/// <param name="matrix">Matrix to work on</param>
		/// <param name="row">Row to check (indexing starts at 0)</param>
		/// <returns></returns>
		private static int PositionOfLeadingNumber(this Fraction[,] matrix, int row)
		{
			// If the given argument is incorrect
			if (row >= matrix.Rows())
			{
				return -1;
			}

			// For each element...
			for (int i = 0; i < matrix.Columns(); ++i)
			{
				// If it's nonzero, return its index
				if (matrix[row, i] != 0)
				{
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// Find the zero-based index of the row which has leading number on
		/// position pos or is the row with the least offset to the right of that position
		/// </summary>
		/// <param name="matrix">Matrix to work on</param>
		/// <param name="pos">Position of the leading number to look for (zero based)</param>
		/// <param name="searchBelow">Searches below the specified row</param>
		/// <returns>Found row or -1 if there was a problem</returns>
		private static int FindRowWithClosestLeadingNumber(this Fraction[,] matrix, int pos, int searchBelow = -1)
		{
			// If the argument was invalid
			if (searchBelow < -1)
			{
				return -1;
			}

			// Each consequent iteration is looking for position one place further than the specified
			for (int i = pos; i < matrix.Columns(); ++i)
			{
				// For each row below the specified row
				for (int j = searchBelow + 1; j <= matrix.Rows(); ++j)
				{
					// If it has the LeadingNumber on the deisred position, return this number
					if (matrix.PositionOfLeadingNumber(j) == i)
					{
						return j;
					}
				}
			}

			return -1;
		}

		/// <summary>
		/// Returns true if the given row is nonzero (contains at least 1 element not equal to 0).
		/// If the given row is invalid, throws an exception
		/// </summary>
		/// <param name="matrix">Matrix to check</param>
		/// <param name="row">Row to check. Indexing starts at 0</param>
		/// <returns></returns>
		public static bool IsRowNonzero(this Fraction[,] matrix, int row)
		{
			if (row < 0 || row >= matrix.Rows())
			{
				throw new Exception("Invalid row");
			}

			// For each element in that row...
			for (int i = 0; i < matrix.Columns(); ++i)
			{
				// If it's nonzero...
				if (matrix[row, i] != 0)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Checks if there is a nonzero row below the specified row
		/// </summary>
		/// <param name="matrix">Matrix to check</param>
		/// <param name="row">Search will be conducted below this row</param>
		/// <returns>True if there is a nonzer row below the specified row</returns>
		private static bool NonzeroBelow(this Fraction[,] matrix, int row)
		{
			// For each row below specified...
			for (int i = row + 1; i < matrix.Rows(); ++i)
			{
				// If it's a nonzero row...
				if (matrix.IsRowNonzero(i))
				{
					return true;
				}
			}

			// If none were found, return false
			return false;
		}

		#endregion

		#region Pivot

		/// <summary>
		/// Returns the zero-based index of the row with the given pivot
		/// If none was found, returns -1
		/// </summary>
		/// <param name="matrix">Matrix to work in</param>
		/// <param name="pivot">Pivot position to look for</param>
		/// <param name="searchFrom">Search begins in this row</param>
		/// <returns>Found row, -1 otherwise (if nothing was found)</returns>
		private static int RowWithPivot(this Fraction[,] matrix, int pivot, int searchFrom = 0)
		{
			// For each row starting at the speicified row...
			for (int i = searchFrom; i < matrix.Rows(); ++i)
			{
				// If the pivots match...
				if (matrix.Pivot(i) == pivot)
				{
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// Returns the position of the pivot element in the given row (or -1 if not found)
		/// </summary>
		/// <param name="matrix">Matrix to search</param>
		/// <param name="row">Row to search</param>
		/// <returns></returns>
		private static int Pivot(this Fraction[,] matrix, int row)
		{
			if (row < 0 || row >= matrix.Rows())
			{
				throw new Exception();
			}

			// For each element in that row...
			for (int i = 0; i < matrix.Columns(); ++i)
			{
				// If it's non-zero then it's the pivot
				if (matrix[row, i] != 0)
				{
					return i;
				}
			}

			return -1;
		}

		#endregion
	}


}
