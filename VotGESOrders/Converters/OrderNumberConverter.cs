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
using VotGESOrders.Logging;
using VotGESOrders.Web.Models;

namespace VotGESOrders.Converters
{
	public class OrderNumberConverter : IValueConverter
	{
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			try {
				double val=(double)value;
				return val > 0 ? val.ToString(OrderInfo.NFI) : "";
			}catch{
				return "";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			try {
				string val=value.ToString();
				return val.Length!=0 ? Double.Parse(val) : 0;
			} catch {
				return 0;
			}
		}

		#endregion
	}
}
