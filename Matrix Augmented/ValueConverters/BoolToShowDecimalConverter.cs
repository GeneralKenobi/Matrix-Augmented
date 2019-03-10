using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Matrix_Augmented
{
	/// <summary>
	/// Converter which returns strings: "Toggle to Decimal" for false and "Toggle to Simple" for true
	/// </summary>
	public class BoolToShowDecimalConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			string result = ((bool)value) ? "Simple" : "Decimal";
			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
