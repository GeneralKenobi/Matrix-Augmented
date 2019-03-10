using System;
using static System.Math;

namespace Matrix_Augmented.Core
{
	public class Matrix : BaseMatrix
	{		

		#region Public Properties		

		/// <summary>
		/// Number of rows this matrix has
		/// </summary>
		public int Rows => Fields.Rows();

		/// <summary>
		/// Number of columns this matrix has
		/// </summary>
		public int Columns => Fields.Columns();

		/// <summary>
		/// Operator accesing the given element in Fields.
		/// It's a shortened version of this.Fields[x,y].
		/// Indexing starts at 0.
		/// <para/>
		/// If the get argument is outside of array's range, it will return null.
		/// If the set argument is outside of array's range, nothing will happen.
		/// <para/>
		/// Example: this[x,y] is identical to this.Fields[x,y]
		/// </summary>
		/// <param name="row">The row index of the value (starts at 0)</param>
		/// <param name="column">The column index of the value (starts at 0)</param>
		/// <returns></returns>
		public Fraction this[int row, int column]
		{
			get
			{
				// If outside of the range of the array, return null
				if (row < 1 || column < 1 || row > Rows || column > Columns)
				{
					return null;
				}
				// Otherwise return the value
				else
				{
					return Fields[row, column];
				}

			}

			set
			{
				// If outside of the range of the array, do nothing
				if (!(row < 1 || column < 1 || row > Rows || column > Columns))
				{
					Fields[row, column] = value;
				}
			}
		}

		/// <summary>
		/// True if another row can be added
		/// </summary>
		public bool CanAddRow => Rows < Constants.MaxMatrixSize;

		/// <summary>
		/// True if another column can be added
		/// </summary>
		public bool CanAddColumn => Columns < Constants.MaxMatrixSize;

		/// <summary>
		/// True if the last row can be deleted
		/// </summary>
		public bool CanDeleteRow => Rows > 1;

		/// <summary>
		/// True if the last column can be deleted
		/// </summary>
		public bool CanDeleteColumn => Columns > 1;
		
		#endregion

		#region Constructors

		/// <summary>
		/// Default constructor for a 1x1 matrix with placeholder ID \0
		/// </summary>
		public Matrix()
		{
			Fields[0, 0] = new Fraction();
		}
		
		/// <summary>
		/// Default constructor for a 1x1 matrix with given ID
		/// </summary>
		/// <param name="id">ID to assign to this matrix</param>
		public Matrix(char id) : this()
		{
			ID = id;
		}
		
		/// <summary>
		/// Constructor with parameters.
		/// <para/>
		/// If either of the size parameters is either smaller than 1
		/// or bigger than the maximum size defined in Constants, an exception will be thrown
		/// </summary>
		/// <param name="id">ID to assign to this matrix</param>
		/// <param name="rows">Number of rows to create</param>
		/// <param name="columns">Number of columns to create</param>
		public Matrix(char id, int rows, int columns) : this (id)
		{
			Resize(rows, columns);
		}

		/// <summary>
		/// Constructor with parameters.
		/// <para/>
		/// If the array's size is bigger than the maximum size defined in Constants, an exception will be thrown
		/// </summary>
		/// <param name="id"></param>
		/// <param name="array"></param>
		public Matrix(char id, Fraction[,] array) : this(id)
		{
			if (array.Rows() > Constants.MaxMatrixSize || array.Columns() > Constants.MaxMatrixSize)
			{
				throw new Exception("Passed array was bigger than the maximum allowed size");
			}
			Fields = array;
		}
		
		#endregion		

		#region Public Transformation Methods
		
		/// <summary>
		/// Adds a new row to the matrix.
		/// If the array has already reached the maximum number of rows, method won't do anything
		/// </summary>
		public void AddRow()
		{
			// If it's possible, add another row
			if (CanAddRow)
			{
				Resize(Rows + 1, Columns);
			}			
		}

		/// <summary>
		/// Deletes a row from the matrix.
		/// If the matrix has only 1 row, it won't do anything
		/// </summary>
		public void DeleteRow()
		{
			// If it's possible delete the last row
			if (CanDeleteRow)
			{
				Resize(Rows - 1, Columns);
			}
		}

		/// <summary>
		/// Adds a new columns to the matrix.
		/// If the array has already reached the maximum number of columns, method won't do anything
		/// </summary>
		public void AddColumn()
		{
			// If it's possible add another column
			if(CanAddColumn)
			{
				Resize(Rows, Columns + 1);
			}
		}

		/// <summary>
		/// Deletes a column from the matrix.
		/// If the matrix has only 1 column, it won't do anything
		/// </summary>
		public void DeleteColumn()
		{
			// If it's possible, delete the last column
			if (CanDeleteColumn)
			{
				Resize(Rows, Columns - 1);
			}
		}

		#endregion
		
		#region Private Transformation Methods

		/// <summary>
		/// Resizes the matrix.
		/// Deletes all fields which fall outside of the new size.
		/// Sets all new fields to 0.
		/// <para/>
		/// It's recommended to let user resize the matrix using <see cref="AddRow"/>, <see cref="DeleteRow"/>,
		/// <see cref="AddColumn"/>, <see cref="DeleteColumn"/> rather than this method.
		/// <para/>
		/// If the size isn't valid (either argument is smaller than 1 or higher than the maximum size defined in Constants), exception will be thrown
		/// </summary>
		/// <param name="newRows">New number of rows</param>
		/// <param name="newColumns">New number of columns</param>
		private void Resize(int newRows, int newColumns)
		{
			// Check if provided arguments are valid
			if (newRows < 1 || newColumns < 1 || newRows > Constants.MaxMatrixSize || newColumns > Constants.MaxMatrixSize)
			{
				throw new Exception("New size is invalid");
			}

			// Save information for the resize event
			int oldRows = Rows;
			int oldColumns = Columns;

			// Indexes of the last position to copy to
			int copyToRows = Math.Min(Rows, newRows);
			int copyToColumns = Math.Min(Columns, newColumns);

			// Create new array
			Fraction[,] newTab = new Fraction[newRows, newColumns];

			// Fill retained spots with old values
			for (int i = 0; i < copyToRows; ++i)
			{
				for (int j = 0; j < copyToColumns; ++j)
				{
					newTab[i, j] = Fields[i, j];
				}
			}

			// Make sure there are no null entries in the new array
			newTab.RemoveNull();

			// Assign to matrix
			Fields = newTab;

			// Notify the SizeChanged event
			SizeChanged(this, new ResizeEventArgs(ID, oldRows, oldColumns, Rows, Columns));
		}

		#endregion
		
		#region Standard Operators
		
		/// <summary>
		/// Adds 2 matrices and returnes a Fraction array as a result
		/// </summary>
		/// <param name="m1"></param>
		/// <param name="m2"></param>
		/// <returns></returns>
		public static Fraction[,] operator +(Matrix m1, Matrix m2)
		{
			// If the matrices don't have the same dimensions, throw an exception
			if (m1.Rows != m2.Rows || m1.Columns != m2.Columns)
			{
				throw new Exception("Incompatible sizes for addition");
			}

			Fraction[,] result = new Fraction[m1.Rows, m1.Columns];
			result.RemoveNull();

			// For each row...
			for (int i = 0; i < m1.Rows; ++i)
			{
				// For each field in that row...
				for (int j = 0; j < m1.Columns; ++j)
				{
					result[i, j] = m1.Fields[i, j] + m2.Fields[i, j];					
				}
			}

			return result;
		}
		
		/// <summary>
		/// Subtracts 2 matrices and returnes a Fraction array as a result
		/// </summary>
		/// <param name="m1"></param>
		/// <param name="m2"></param>
		/// <returns></returns>
		public static Fraction[,] operator -(Matrix m1, Matrix m2)
		{
			// If the matrices don't have the same dimensions, throw an exception
			if (m1.Rows != m2.Rows || m1.Columns != m2.Columns)
			{
				throw new Exception("Incompatible sizes for subtraction");
			}

			Fraction[,] result = new Fraction[m1.Rows, m1.Columns];


			// For each row...
			for (int i = 0; i < m1.Rows; ++i)
			{
				// For each field in that row...
				for (int j = 0; j < m1.Columns; ++j)
				{
					result[i, j] = m1.Fields[i, j] - m2.Fields[i, j];
				}
			}

			return result;
		}
		
		/// <summary>
		/// Multiplies 2 matrices and returnes a Fraction array as a result
		/// </summary>
		/// <param name="m1"></param>
		/// <param name="m2"></param>
		/// <returns></returns>
		public static Fraction[,] operator *(Matrix m1, Matrix m2)
		{
			// [A x B] * [B x C] = [A x C]
			// Dimensions resulting from multiplication
			// Algorithm presented on https://www.mathsisfun.com/algebra/matrix-multiplying.html

			// If the matrices don't have the same dimensions, throw an exception
			if (m1.Columns != m2.Rows)
			{
				throw new Exception("Incompatible sizes for multiplication");
			}

			Fraction[,] result = new Fraction[m1.Rows, m2.Columns];

			// Initialize result with fractions equal to 0
			result.RemoveNull();

			// For each row...
			for(int i=0; i<m1.Rows; ++i)
			{
				// For each field in that row...
				for(int j=0; j<m2.Columns; ++j)
				{					
					for(int k=0; k<m1.Columns; ++k)
					{
						// Add to that field the result of every multiplication
						result[i, j] += m1.Fields[i, k] * m2.Fields[k, j];
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Multiplies the givem matrix by a scalar
		/// </summary>
		/// <param name="f">Scalar to multiply by</param>
		/// <param name="m">Matrix to be multiplied</param>
		/// <returns></returns>
		public static Fraction[,] operator *(Fraction f, Matrix m)
		{
			Fraction[,] result = new Fraction[m.Rows, m.Columns];

			// For each row...
			for (int i = 0; i < m.Rows; ++i)
			{
				// For each field in that row...
				for (int j = 0; j < m.Columns; ++j)
				{
					result[i, j] = f * m.Fields[i, j];
				}
			}

			return result;
		}

		/// <summary>
		/// Multiplies the givem matrix by a scalar
		/// </summary>
		/// <param name="f">Scalar to multiply by</param>
		/// <param name="m">Matrix to be multiplied</param>
		/// <returns></returns>
		public static Fraction[,] operator *(Matrix m, Fraction f)
		{
			Fraction[,] result = new Fraction[m.Rows, m.Columns];

			// For each row...
			for (int i = 0; i < m.Rows; ++i)
			{
				// For each field in that row...
				for (int j = 0; j < m.Columns; ++j)
				{
					result[i, j] = f * m.Fields[i, j];
				}
			}

			return result;
		}

		/// <summary>
		/// Divides the givem matrix by a scalar
		/// </summary>
		/// <param name="f">Scalar to divide by</param>
		/// <param name="m">Matrix to be divided</param>
		/// <returns></returns>
		public static Fraction[,] operator /(Matrix m, Fraction f)
		{
			f.Flip();
			return f * m;
		}


		#endregion

		#region Determinant

		/// <summary>
		/// Calculates the determinant of this matrix
		/// </summary>
		/// <param name="info">The information about this operation will be saved in this string</param>
		/// <returns>Determinant of this matrix</returns>
		public Fraction Determinant(out string info)
		{
			// Provide the information of this operation
			info = "Determinant(" + ID + ") =";

			// If the matrix isn't square, return null
			if (!Fields.IsSquare())
			{
				info = "Determinant can only be calculated for square matrices";
				return null;
			}		

			// Make a copy to work on
			Fraction[,] copy = Fields.Copy();

			// Transform it into a row-echelon form and save the number of row swaps
			int swaps = copy.RowEchelon(false);

			// If we performed an odd number of swaps during the row-echelon transform,
			// determinant has to change the sign
			int sign = swaps % 2 == 0 ? 1 : -1;

			// Determinant is the product of the entries on the main diagonal
			return copy.ProductOfDiagonal() * sign;
		}

		/// <summary>
		/// Calculates the determinant of this matrix
		/// </summary>
		/// <returns>Determinant of this matrix</returns>
		public Fraction Determinant()
		{
			// If the matrix isn't square, return null
			if (!Fields.IsSquare())
			{
				return null;
			}

			// If the matrix has only 1 entry, return it
			if (Rows == 1)
			{
				return Fields[0, 0];
			}

			// Make a copy to work on
			Fraction[,] copy = Fields.Copy();

			// Transform it into a row-echelon form and save the number of row swaps
			int swaps = copy.RowEchelon(false);

			// If we performed an odd number of swaps during the row-echelon transform,
			// determinant has to change the sign
			int sign = swaps % 2 == 0 ? 1 : -1;

			// Determinant is the product of the entries on the main diagonal
			return copy.ProductOfDiagonal() * sign;
		}

		#endregion

		#region Rank

		/// <summary>
		/// Returns the rank of this matrix
		/// </summary>
		/// <param name="info">The information about this operation will be saved in this string</param>
		/// <returns></returns>
		public int Rank(out string info)
		{
			// Provide information about the calculation
			info = "Rank(" + ID + ") =";

			// Make a copy to work on
			Fraction[,] copy = Fields.Copy();

			// Transform it to the row echelon form
			copy.RowEchelon();

			// Rank is the number of nonzero rows
			return copy.NonzeroRows();
		}

		/// <summary>
		/// Returns the rank of this matrix
		/// </summary>
		/// <returns></returns>
		public int Rank()
		{
			// Make a copy to work on
			Fraction[,] copy = Fields.Copy();

			// Transform it to the row echelon form
			copy.RowEchelon();

			// Rank is the number of nonzero rows
			return copy.NonzeroRows();
		}

		#endregion

		#region Transpose

		/// <summary>
		/// Returns a Fraction 2d array which is a transposition of this matrix
		/// </summary>
		/// <returns></returns>
		public Fraction[,] Transpose()
		{
			return Fields.Transpose();
		}

		#endregion

		#region Triangular

		/// <summary>
		/// Returns the triangular form of this matrix
		/// Or null if this matrix isn't square
		/// </summary>
		/// <returns></returns>
		public Fraction[,] Triangular()
		{
			// If this matrix isn't square, return null
			if(!Fields.IsSquare())
			{
				return null;
			}

			// Create a copy
			Fraction[,] copy = Fields.Copy();
			
			// Make it triangular
			copy.Triangular();

			// Return it
			return copy;
		}

		#endregion

		#region Row-Echelon

		/// <summary>
		/// Returns the row-echelon form of this matrix
		/// </summary>
		/// <returns></returns>
		public Fraction[,] RowEchelon()
		{
			Fraction[,] copy = Fields.Copy();

			copy.RowEchelon();

			return copy;
		}

		#endregion

		#region Inverse

		// Returns true if this matrix is invertible
		public bool IsInvertible()
		{
			// Matrix is invertible if its determinant is not equal to 0
			return Fields.IsSquare() && Determinant() != 0;
		}

		/// <summary>
		/// Returns a Fraction 2d array which is an inverse of this matrix
		/// </summary>
		/// <returns></returns>
		public Fraction[,] Inverse()
		{
			if (!Fields.IsSquare())
			{				
				return null;
			}

			// Make a copy to work on
			Fraction[,] copy = Fields.Copy();

			return copy.Inverse();
		}

		#endregion

		#region LUDecomposition

		/// <summary>
		/// Decomposes the current matrix into lower and upper triangular matrices
		/// so that this = lower*upper
		/// </summary>
		/// <param name="lower">This matrix will be the lower triangular matrix</param>
		/// <param name="upper">This matrix will be the upper triangular matrix</param>
		public void LUDecomposition(out Fraction[,] lower, out Fraction[,] upper)
		{
			// Perform the LUDecomposition
			Fields.LUDecompositionBis(out lower, out upper);
		}

		#endregion

		#region Public Events

		/// <summary>
		/// Event for when the size of this matrix has changed
		/// </summary>
		public event ResizeEventHandler SizeChanged;

		/// <summary>
		/// Method for when the size of this matrix has changed
		/// </summary>
		/// <param name="e"></param>
		private void OnSizeChanged(ResizeEventArgs e) => SizeChanged?.Invoke(this, e);

		#endregion

	}
}
