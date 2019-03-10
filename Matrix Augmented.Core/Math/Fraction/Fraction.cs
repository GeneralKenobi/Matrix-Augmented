using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Matrix_Augmented.Core
{


	/// <summary>
	/// A class representing a fraction composed of a real numerator and a real denominator
	/// </summary>
	public class Fraction : INotifyPropertyChanged
	{
		#region Private Members

		BigInteger mNumerator;

		BigInteger mDenominator;

		#endregion

		#region Public Properties		

		public BigInteger Numerator
		{
			get => mNumerator;

			set
			{
				mNumerator = value;
				OnPropertyChanged(nameof(Numerator));
				OnPropertyChanged(nameof(Denominator));
				OnPropertyChanged(nameof(Decimal));
			}
		}

		public BigInteger Denominator
		{
			get => mDenominator;

			set
			{
				// If the denominator is to be set to 0...
				if (value == 0)
				{
					// Indicate it's an illegal situation
					throw new Exception("Can't assign 0 to denominator");
				}

				mDenominator = value;
				OnPropertyChanged(nameof(Numerator));
				OnPropertyChanged(nameof(Denominator));
				OnPropertyChanged(nameof(Decimal));
			}
		}
		
		/// <summary>
		/// Decimal value of the Fraction
		/// </summary>
		public double Decimal
		{
			get => Math.Floor(((double) Numerator / (double) Denominator) * 10000) / 10000;
			set
			{
				ConstructFromDouble(value);
				OnPropertyChanged(nameof(Decimal));
				OnPropertyChanged(nameof(Numerator));
				OnPropertyChanged(nameof(Denominator));
			}
		}

		#endregion

		#region Public Constructors
		
		/// <summary>
		/// Default constructor assigning 0 to Numerator and 1 to Denominator
		/// </summary>
		public Fraction()
		{
			Numerator = 0;
			Denominator = 1;			
		}

		/// <summary>
		/// Creates a fraction based on an <see cref="BigInteger"/>
		/// </summary>
		/// <param name="numerator"></param>
		public Fraction(BigInteger numerator)
		{
			Numerator = numerator;
			Denominator = 1;			
		}

		/// <summary>
		/// Creates a fraction based on an <see cref="int"/>
		/// </summary>
		/// <param name="numerator"></param>
		public Fraction(int numerator)
		{
			Numerator = numerator;
			Denominator = 1;
		}

		/// <summary>
		/// Creates a fraction based on a decimal fraction
		/// </summary>
		/// <param name="value"></param>
		public Fraction(double value)
		{
			ConstructFromDouble(value);
		}		

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="numerator">The value to assign to Numerator</param>
		/// <param name="denominator">The value to assign to Denominator</param>
		public Fraction(BigInteger numerator, BigInteger denominator)
		{
			Numerator = numerator;
			Denominator = denominator;

			Shorten();
			SortSigns();			
		}

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="numerator">The value to assign to Numerator</param>
		/// <param name="denominator">The value to assign to Denominator</param>
		public Fraction(int numerator, int denominator)
		{
			Numerator = numerator;
			Denominator = denominator;

			Shorten();
			SortSigns();
		}

		/// <summary>
		/// Copy constructor obtaining values from copy
		/// </summary>
		/// <param name="copy">Fraction to use as an original</param>
		public Fraction(Fraction copy)
		{
			this.Numerator = copy.Numerator;
			this.Denominator = copy.Denominator;			
		}

		#endregion

		#region Operators

		#region Scalar Multiplication and Division
		
		/// <summary>
		/// Scalar multiplication
		/// </summary>
		/// <param name="f">Fraction to multiply</param>
		/// <param name="x">Scalar to multiply by</param>
		/// <returns></returns>
		public static Fraction operator *(Fraction f, BigInteger x) =>		
			new Fraction(f.Numerator * x, f.Denominator);

		/// <summary>
		/// Scalar multiplication
		/// </summary>
		/// <param name="f">Fraction to multiply</param>
		/// <param name="x">Scalar to multiply by</param>
		/// <returns></returns>
		public static Fraction operator *(BigInteger x, Fraction f) =>		
			new Fraction(f.Numerator * x, f.Denominator);
		
		/// <summary>
		/// Scalar division
		/// </summary>
		/// <param name="f"></param>
		/// <param name="x"></param>
		/// <returns></returns>
		public static Fraction operator /(Fraction f, BigInteger x) =>		
			new Fraction(f / new Fraction(x, 1));
		
		/// <summary>
		/// Scalar division
		/// </summary>
		/// <param name="f"></param>
		/// <param name="x"></param>
		/// <returns></returns>
		public static Fraction operator /(BigInteger x, Fraction f) =>
			new Fraction(new Fraction(x, 1) / f);
		
		#endregion
		
		#region Addition and Subtraction of Integers
		
		public static Fraction operator +(Fraction f, BigInteger x) =>		
			new Fraction(f.Denominator * x + f.Numerator, f.Denominator);
		
		public static Fraction operator +(BigInteger x, Fraction f) =>		
			new Fraction(f.Denominator * x + f.Numerator, f.Denominator);
		
		public static Fraction operator -(Fraction f, BigInteger x) =>		
			new Fraction(f.Numerator - f.Denominator * x, f.Denominator);
		
		public static Fraction operator -(BigInteger x, Fraction f) =>		
			new Fraction(f.Denominator * x - f.Numerator, f.Denominator);
		
		#endregion
		
		#region Operations on 2 Fractions

		public static Fraction operator +(Fraction f1, Fraction f2)
		{
			// Create copies of the passed fractions
			// so as not to modify the passed values
			Fraction f1c = new Fraction(f1);
			Fraction f2c = new Fraction(f2);

			// Bring both fractions to a common denominator
			BringToCommonDenominator(ref f1c, ref f2c);

			// Add the numerators
			return new Fraction(f1c.Numerator + f2c.Numerator, f1c.Denominator);
		}
		
		public static Fraction operator -(Fraction f1, Fraction f2)
		{
			// Create copies of the passed fractions
			// so as not to modify the passed values
			Fraction f1c = new Fraction(f1);
			Fraction f2c = new Fraction(f2);

			// Bring both fractions to a common denominator
			BringToCommonDenominator(ref f1c, ref f2c);

			// Subtract the numerators
			return new Fraction(f1c.Numerator - f2c.Numerator, f1c.Denominator);
		}
		
		public static Fraction operator *(Fraction f1, Fraction f2)
		{
			// Create copies of the passed fractions
			// so as not to modify the passed values
			Fraction f1c = new Fraction(f1);
			Fraction f2c = new Fraction(f2);

			// Multiply the numerators and denumarators
			return new Fraction(f1c.Numerator * f2c.Numerator, f1c.Denominator * f2c.Denominator);
		}


		public static Fraction operator /(Fraction f1, Fraction f2)
		{
			// Throw exception if there's attempted division by 0
			if(f2.Numerator==0)
			{
				throw new Exception("Can't divide by 0");
			}
			// Create copies of the passed fractions
			// so as not to modify the passed values
			Fraction f1c = new Fraction(f1);
			Fraction f2c = new Fraction(f2);

			f2c.Flip();

			// Multiply the numerators and denumarators
			return new Fraction(f1c.Numerator * f2c.Numerator, f1c.Denominator * f2c.Denominator);
		}
		
		#endregion
		
		#region Equality Operators


		public static bool operator ==(Fraction f1, Fraction f2)
		{
			if(f1 is null)
			{
				if(f2 is null)
				{
					return true;
				}
				else
				{
					return false;
				}
			}

			if (f2 is null)
			{
				if (f1 is null)
				{
					return true;
				}
				else
				{
					return false;
				}
			}

			Fraction f1c = null;
			Fraction f2c = null;

			if (f1 != null)
			{
				f1c = new Fraction(f1);
				f1c.Shorten();
			}

			if (f2 != null)
			{
				f2c = new Fraction(f2);
				f2c.Shorten();
			}

			return f1c.Numerator == f2c.Numerator && f1c.Denominator == f2c.Denominator;
		}

		public static bool operator !=(Fraction f1, Fraction f2) => !(f1 == f2);
		
		public static bool operator ==(Fraction f, BigInteger x) => f == new Fraction(x);
		
		public static bool operator !=(Fraction f, BigInteger x) => !(f == new Fraction(x));
		
		public static bool operator ==(BigInteger x, Fraction f) => f == new Fraction(x);
		
		public static bool operator !=(BigInteger x, Fraction f) => !(f == new Fraction(x));
		
		
		#endregion
		
		#region Order operators
				
		public static bool operator >(Fraction f1, Fraction f2)
		{
			Fraction f1c = new Fraction(f1);
			Fraction f2c = new Fraction(f2);

			f1c.Shorten();
			f2c.Shorten();

			BringToCommonDenominator(ref f1c, ref f2c);

			return f1c.Numerator > f2c.Denominator;
		}

		public static bool operator <(Fraction f1, Fraction f2)
		{
			Fraction f1c = new Fraction(f1);
			Fraction f2c = new Fraction(f2);

			f1c.Shorten();
			f2c.Shorten();

			BringToCommonDenominator(ref f1c, ref f2c);

			return f1c.Numerator < f2c.Denominator;
		}

		public static bool operator >=(Fraction f1, Fraction f2) => !(f1 < f2);
		
		public static bool operator <=(Fraction f1, Fraction f2) => !(f1 > f2);
		
		public static bool operator >(Fraction f, BigInteger x) => f > new Fraction(x);
		
		public static bool operator <(Fraction f, BigInteger x) => f < new Fraction(x);
		
		public static bool operator >=(Fraction f, BigInteger x) => f >= new Fraction(x);
		
		public static bool operator <= (Fraction f, BigInteger x) => f <= new Fraction(x);

		public static bool operator >(BigInteger x, Fraction f) => new Fraction(x) > f;
		
		public static bool operator <(BigInteger x, Fraction f) => new Fraction(x) < f;

		public static bool operator >=(BigInteger x, Fraction f) => new Fraction(x) >= f;
		
		public static bool operator <=(BigInteger x, Fraction f) => new Fraction(x) <= f;
		
		#endregion
		
		#region Power
		
		public static Fraction operator ^(Fraction f, int x) =>
			new Fraction(BigInteger.Pow(f.Numerator, x), BigInteger.Pow(f.Denominator, x));
		
		#endregion
		
		#endregion
		
		#region Public Helpers

		/// <summary>
		/// Flips the fractions (ex: 3/5 -> 5/3)
		/// If the given fraction is 0, throws an exception
		/// </summary>
		public void Flip()
		{
			// Check if the fraction isn't 0
			if(Numerator==0)
			{
				throw new Exception("Can't flip a '0'");
			}

			var temp = Numerator;
			Numerator = Denominator;
			Denominator = temp;
		}

		#endregion
		
		#region Private Helpers
		
		/// <summary>
		/// Constructs this Fraction based on a decimal Fraction
		/// </summary>
		/// <param name="value"></param>
		private void ConstructFromDouble(double value)
		{			
			Denominator = 1;

			// Keep going until the precision is lost after the 8th decimal point 
			while (Math.Abs(value - Math.Round(value)) > Math.Pow(10, -8))
			{
				value *= 10;
				Denominator *= 10;
			}

			Numerator = (BigInteger)value;
			Shorten();
		}

		/// <summary>
		/// If there are minuses in both Numerator and Denominator, this method shortens them (removes)
		/// If the minus is located only in Denominator, this method moves it to the Numerator
		/// </summary>
		private void SortSigns()
		{
			// If both Numerator and Denumerator are smaller than 0
			// Or Numerator >= 0 and Denominator is smaller than 0...
			if ((Numerator < 0 && Denominator < 0) || (Numerator >= 0 && Denominator < 0))
			{
				// Get rid of the minuses
				// Or transfer the minus to the Numerator
				Numerator *= -1;
				Denominator *= -1;
			}
		}
		
		/// <summary>
		/// Shortens the fractions (ex: 2/4 -> 1/2)
		/// </summary>
		private void Shorten()
		{
			// If this fraction == 0...
			if (Numerator == 0)
			{
				// Set Denominator to 0 and return
				Denominator = 1;
				return;
			}


			// Greatest common divider of the numerator and denominator
			BigInteger divider = 1;

			do
			{
				// Calculate the divider
				divider = MathHelpers.GCD(Numerator, Denominator);

				// Divide the numerator and denumerator
				Numerator /= divider;
				Denominator /= divider;
			}
			// Keep going until the greatest common divider > 1
			// After that the fraction can't be shortened more
			while (divider > 1);
		}
		
		/// <summary>
		/// Finds the least common denominator for fractions f1 and f2
		/// Modifies the fractions to be described using this common denominator
		/// </summary>
		/// <param name="f1"></param>
		/// <param name="f2"></param>
		private static void BringToCommonDenominator(ref Fraction f1, ref Fraction f2)
		{
			// Find the least common denominator
			var leastCommonDenominator = MathHelpers.LCM(f1.Denominator, f2.Denominator);

			// Find the scalar to multiply by for f1
			var scalar = leastCommonDenominator / f1.Denominator;

			// Multiply by it
			f1.Numerator *= scalar;
			f1.Denominator *= scalar;

			// Do the same for f2
			scalar = leastCommonDenominator / f2.Denominator;
			f2.Numerator *= scalar;
			f2.Denominator *= scalar;
		}
		
		/// <summary>
		/// Finds the least common denominator for fractions inside the passed List
		/// Modifies the fractions to be described using this common denominator
		/// </summary>
		/// <param name="fractions">List containing all the fractions to modify</param>
		/// <returns></returns>
		private static void BringToCommonDenominator(ref List<Fraction> fractions)
		{
			// If there are less than 2 fractions passed...
			if(fractions.Count<2)
			{
				// Return as there is nothing to modify
				return;
			}
			

			// Find the least common denominator for the provided fractions
			var leastCommonDenominator = MathHelpers.LCM(ListFractionToListBigIntsConverter.ConvertToDenominator(fractions));
			
			fractions.ForEach(item =>
			{
				// Find the scalar to multiply by for each item
				var scalar = leastCommonDenominator / item.Denominator;

				// Multiply by it
				item.Numerator *= scalar;
				item.Denominator *= scalar;
			});
		}		
		
		#endregion
		
		#region Property Changed

		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// On property changed
		/// </summary>
		/// <param name="propertyName"></param>
		void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


		#endregion
		
		#region Public Methods

		/// <summary>
		/// Returns a string representation of this fraction
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Numerator.ToString() + '/' + Denominator.ToString();
		}

		#endregion
	}
}
