using System;
using System.Collections.Generic;
using System.Numerics;

namespace Matrix_Augmented.Core
{
    public static class MathHelpers
    {
		#region Greatest Common Divisor

		/// <summary>
		/// Returns the greatest commond divisor of numbers a and b
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static BigInteger GCD(BigInteger a, BigInteger b)
        {
            if (a == 0)
                return b;

            else
                return GCD(b % a, a);
        }		

		#endregion

		#region Least Common Multiple

		/// <summary>
		/// Returns the least common multiple of numbers a and b
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static BigInteger LCM(BigInteger a, BigInteger b)
        {
			if(a==b)
			{
				return a;
			}

            return (a * b) / GCD(a, b);
        }
		
		/// <summary>
		/// Returns the least common multiple of all numbers in list
		/// </summary>
		/// <param name="list">List containing the numbers</param>
		/// <returns></returns>
		public static BigInteger LCM(List<BigInteger> list)
		{
			// If there are less than 2 numbers in list...
			if (list.Count < 2)
			{
				switch(list.Count)
				{
					// Throw an exception if the list was empty
					case 0:
						{
							throw new Exception("Can't find least common multiple: list was empty");
						}

					// If the list contained only one item, return it
					case 1:
						{
							return list[0];
						}
				}				
			}

			// Find the least common multiple for the first 2 numbers
			var leastCommonMultiple = MathHelpers.LCM(list[0], list[1]);

			// Then find the least common multiple for the ith number and the value of the least common multiple for the previous numbers
			for (int i = 2; i < list.Count; ++i)
			{
				leastCommonMultiple = MathHelpers.LCM(leastCommonMultiple, list[i]);
			}

			return leastCommonMultiple;
		}

		#endregion		
	}
}
