using System;
using System.Numerics;
using Windows.UI.Xaml.Data;

namespace Matrix_Augmented
{
	/// <summary>
	/// Converter for converting <see cref="BigInteger"/> to <see cref="string"/> and backwards in Numerator
	/// </summary>
	public class NumeratorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if ((BigInteger)value == 0)
				return string.Empty;

			return value.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			if (string.IsNullOrWhiteSpace((string)value))
			{
				return (BigInteger)0;
			}

			return BigInteger.Parse((string)value);
		}
	}

	/// <summary>
	/// Converter for converting <see cref="BigInteger"/> to <see cref="string"/> and backwards in Numerator
	/// </summary>
	public class DenominatorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if ((BigInteger)value == 1)
				return string.Empty;

			return value.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			if (string.IsNullOrWhiteSpace((string)value))
			{
				return (BigInteger)1;
			}

			return BigInteger.Parse((string)value);
		}
	}

	/// <summary>
	/// Converter for converting <see cref="double"/> to <see cref="string"/>
	/// </summary>
	public class DecimalConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if ((double)value == 0)
				return string.Empty;

			double v = (double)value;

			// Round this to the 16th place after the decimal point
			return Math.Round(v * Math.Pow(10, 16)) / Math.Pow(10, 16);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			if (string.IsNullOrWhiteSpace((string)value))
			{
				return 0;
			}
			double result = double.Parse((string)value);
			return double.Parse((string)value);
		}
	}
}
