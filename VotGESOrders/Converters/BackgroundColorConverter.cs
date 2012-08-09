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
	public class BackgroundColorConverter : IValueConverter
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
				Color color=Colors.Transparent;
				string param=parameter.ToString();
				if (param == "State") {
					OrderStateEnum state=(OrderStateEnum)value;					
					switch (state) {
						case OrderStateEnum.accepted:
							color = Color.FromArgb(255, 204, 255, 255);
							break;
						case OrderStateEnum.askExtended:
							color = Color.FromArgb(255, 255, 255, 153);
							break;
						case OrderStateEnum.banned:
							color = Colors.LightGray;
							break;
						case OrderStateEnum.canceled:
							color = Colors.LightGray;
							break;
						case OrderStateEnum.closed:
							color = Color.FromArgb(255, 204, 204, 255);
							break;
						case OrderStateEnum.created:
							color = Colors.White;
							break;
						case OrderStateEnum.completed:
							color = Colors.LightGray;
							break;
						case OrderStateEnum.completedWithoutEnter:
							color = Colors.LightGray;
							break;
						case OrderStateEnum.extended:
							color = Colors.LightGray;
							break;
						case OrderStateEnum.opened:
							color = Color.FromArgb(255, 204, 255, 204);
							break;
					}
				} else if (param == "type") {
					OrderTypeEnum type=(OrderTypeEnum)value;
					switch (type) {
						case OrderTypeEnum.crash:
							color = Colors.Red;
							break;
						case OrderTypeEnum.npl:
							color = Color.FromArgb(255, 255, 204, 204);
							break;
						case OrderTypeEnum.pl:
							color = Color.FromArgb(255, 153, 255, 204);
							break;
						case OrderTypeEnum.no:
							color = Colors.Orange;
							break;
					}
				} else if (param == "Time") {
					double? time=(double?)value;
					if (time.HasValue) {
						if (time.Value < 0)
							color = Colors.Red;
						else if (time.Value < 2)
							color = Colors.Orange;
						else if (time.Value < 8)
							color = Colors.Yellow;
					}
				} else if (param == "Expired") {
					double? time=(double?)value;
					color = Colors.Transparent;
					if (time.HasValue) {
						/*if (time.Value > -2)
							color = Color.FromArgb(255, 153, 255, 204);
						else */if (time.Value<-2)
							color = Color.FromArgb(255, 255, 204, 204);						
					}
				}
				return new SolidColorBrush(color);
			} catch {
				return new SolidColorBrush(Colors.Transparent);
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return null;
		}

		#endregion
	}
}
