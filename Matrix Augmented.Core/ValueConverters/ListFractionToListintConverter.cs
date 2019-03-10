using System.Collections.Generic;
using System.Numerics;

namespace Matrix_Augmented.Core
{

	/// <summary>
	/// Provides conversion from a list of Fractions to a list of ints
	/// </summary>
	public static class ListFractionToListBigIntsConverter
	{
		/// <summary>
		/// Converts a list of <see cref="Fraction"/>s to a list of <see cref="BigInteger"/>s which are taken from these Fractions' denominators
		/// </summary>
		/// <param name="list">A list of Fractions to convert</param>
		/// <returns></returns>
		public static List<BigInteger> ConvertToDenominator(List<Fraction> list)
		{
			var result = new List<BigInteger>();

			list.ForEach(item => result.Add(item.Denominator));

			return result;
		}
		
		/// <summary>
		/// Converts a list of <see cref="Fraction"/>s to a list of <see cref="BigInteger"/>s which are taken from these Fractions' numerators
		/// </summary>
		/// <param name="list">A list of Fractions to convert</param>
		/// <returns></returns>
		public static List<BigInteger> ConvertToNumerator(List<Fraction> list)
		{
			var result = new List<BigInteger>();

			list.ForEach(item => result.Add(item.Numerator));

			return result;
		}
	}
}
