using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

namespace VotGESOrders.Converters
{
	public class HeightConverter : IValueConverter
	{
		#region IValueConverter Members
		/// <summary>
		/// Convert a Date into DD/MMM/YYYY HH:mm format
		/// </summary>
		/// <param name="value">object: value</param>
		/// <param name="targetType">Type: targetType</param>
		/// <param name="parameter">object: parameter</param>
		/// <param name="culture">System.Globalization.CultureInfo: culture</param>
		/// <returns>System.Object</returns>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			try {
				bool val=(Boolean)value;
				if (!val)
					return new GridLength(0);
				else return new GridLength(1,GridUnitType.Auto);
			} catch {
				return new GridLength(1, GridUnitType.Auto);
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			try{
				return DateTime.Parse(value.ToString());
			}catch {
				return DateTime.Now;
			}
		}

		#endregion
	}
}
