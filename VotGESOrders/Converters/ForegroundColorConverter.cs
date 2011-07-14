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
using VotGESOrders.Web.Models;
using VotGESOrders.Logging;

namespace VotGESOrders.Converters
{
	public class ForegroundColorConverter : IValueConverter
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
				OrderStateEnum state=(OrderStateEnum)value;
				Color color=Colors.White;
				switch (state) {
					case OrderStateEnum.accepted:
						color = Colors.Black;
						break;
					case OrderStateEnum.askExtended:
						color = Colors.Black;
						break;
					case OrderStateEnum.banned:
						color = Colors.Red;
						break;
					case OrderStateEnum.completedWithoutEnter:
						color = Colors.Red;
						break;
					case OrderStateEnum.canceled:
						color = Colors.Blue;
						break;
					case OrderStateEnum.closed:
						color = Colors.Black;
						break;
					case OrderStateEnum.created:
						color = Colors.Black;
						break;
					case OrderStateEnum.completed:
						color = Colors.Black;
						break;
					case OrderStateEnum.extended:
						color = Colors.Green;
						break;
					case OrderStateEnum.opened:
						color = Colors.Black;
						break;
				}
				return new SolidColorBrush(color);
			} catch {
				return new SolidColorBrush(Colors.White);
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return null;
		}

		#endregion
	}
}
