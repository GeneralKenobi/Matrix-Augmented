using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_Augmented.Core
{
	/// <summary>
	/// Class designed for computing equations
	/// It can either be a Matrix (then Scalar is null)
	/// or a Scalar (then Fields are null)
	/// </summary>
	public class CalcMatrix : Matrix
	{

		#region Public Properties

		/// <summary>
		/// Bool denoting if this is a scalar (true) or a Matrix (false)
		/// </summary>
		public bool IsScalar => Scalar != null;

		/// <summary>
		/// Scalar, if it's a matrix it's value is null
		/// </summary>
		public Fraction Scalar { get; set; } = null;

		/// <summary>
		/// ID used to identify this Matrix during calculations
		/// Range: 1111 - 9999
		/// </summary>
		public int CalcID { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="copy"></param>
		public CalcMatrix(CalcMatrix original)
		{
			if(original.IsScalar)
			{
				Scalar = original.Scalar;
				Fields = null;
			}
			else
			{
				Fields = original.Fields.Copy();
				Scalar = null;
			}
		}

		/// <summary>
		/// Constructor for the scalar type
		/// </summary>
		/// <param name="calcID"> ID used to identify this Matrix during calculations. Range: 1111 - 9999</param>
		/// <param name="scalar">Scalar this object represents</param>
		public CalcMatrix(int calcID, Fraction scalar)
		{
			CalcID = calcID;
			ID = '\0';
			Fields = null;
			Scalar = new Fraction(scalar);
		}

		/// <summary>
		/// Constructor for the scalar type
		/// </summary>
		/// <param name="calcID"> ID used to identify this Matrix during calculations. Range: 1111 - 9999</param>
		/// <param name="scalar">Scalar this object represents</param>
		public CalcMatrix(char id, int calcID, Fraction scalar)
		{
			CalcID = calcID;
			ID = id;
			Fields = null;
			Scalar = new Fraction(scalar);
		}

		/// <summary>
		/// Constructor for the Matrix type
		/// </summary>
		/// <param name="calcID"> ID used to identify this Matrix during calculations. Range: 1111 - 9999</param>
		/// <param name="fields">2d <see cref="Fraction"/> array this object represents</param>
		public CalcMatrix(int calcID, Fraction[,] fields) : base('\0', fields)
		{
			CalcID = calcID;
		}

		/// <summary>
		/// Constructor for the Matrix type
		/// </summary>
		/// <param name="calcID"> ID used to identify this Matrix during calculations. Range: 1111 - 9999</param>
		/// <param name="fields">2d <see cref="Fraction"/> array this object represents</param>
		public CalcMatrix(char id, int calcID, Fraction[,] fields) : base(id, fields)
		{				
			CalcID = calcID;
		}

		#endregion

		#region Operators

		/// <summary>
		/// Returns null if operation was illegal (adding scalar to a matrix)
		/// </summary>
		/// <param name="m1"></param>
		/// <param name="m2"></param>
		/// <returns></returns>
		public static CalcMatrix operator +(CalcMatrix m1, CalcMatrix m2)
		{
			// If both are scalars...
			if(m1.IsScalar && m2.IsScalar)
			{
				// Return new scalar
				return new CalcMatrix(0, m1.Scalar + m2.Scalar);
			}

			// If both are matrices...
			if((!m1.IsScalar) && (!m2.IsScalar))
			{
				// Return new matrix
				return new CalcMatrix(0, (Matrix)m1 + (Matrix)m2);
			}

			// Otherwise return null because we can't add a number to a matrix
			return null;	
		}

		/// <summary>
		/// Returns null if operation was illegal (subtracting scalar from a matrix)
		/// </summary>
		/// <param name="m1"></param>
		/// <param name="m2"></param>
		/// <returns></returns>
		public static CalcMatrix operator -(CalcMatrix m1, CalcMatrix m2)
		{
			// If both are scalars...
			if (m1.IsScalar && m2.IsScalar)
			{
				// Return new scalar
				return new CalcMatrix(0, m1.Scalar - m2.Scalar);
			}

			// If both are matrices...
			if ((!m1.IsScalar) && (!m2.IsScalar))
			{
				// Return new matrix
				return new CalcMatrix(0, (Matrix)m1 - (Matrix)m2);
			}

			// Otherwise return null because we can't add a number to a matrix
			return null;
		}

		public static CalcMatrix operator *(CalcMatrix m1, CalcMatrix m2)
		{
			// If both are scalars...
			if (m1.IsScalar && m2.IsScalar)
			{
				// Return new scalar
				return new CalcMatrix(0, m1.Scalar * m2.Scalar);
			}

			// If both are matrices...
			if ((!m1.IsScalar) && (!m2.IsScalar))
			{
				// Return new matrix
				return new CalcMatrix(0, (Matrix)m2 * (Matrix)m1);
			}

			// If only m1 is a scalar...
			if(m1.IsScalar)
			{
				return new CalcMatrix(0, m1.Scalar * m2);
			}

			// The last option is that m2 is a scalar
			return new CalcMatrix(0, m2.Scalar * m1);			
		}

		/// <summary>
		/// Returns null if operation was illegal (dividing number by matrix or matrix by matrix)
		/// </summary>
		/// <param name="m1"></param>
		/// <param name="m2"></param>
		/// <returns></returns>
		public static CalcMatrix operator /(CalcMatrix m1, CalcMatrix m2)
		{
			// If both are scalars...
			if (m1.IsScalar && m2.IsScalar)
			{
				// Return new scalar
				return new CalcMatrix(0, m1.Scalar / m2.Scalar);
			}

			// The last option is that m2 is a scalar and we divide matrix by a value
			if (m2.IsScalar)
			{
				return new CalcMatrix(0, m1 / m2.Scalar);
			}

			// All other options are illegal
			return null;
		}

		#endregion
	}
}
