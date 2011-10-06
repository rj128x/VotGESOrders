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
	public class ExpiredTimeConverter : IValueConverter
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
		/// 
		protected string getTimeStr(double time) {
			time = Math.Abs(time);
			return String.Format("{0}:{1:00}", Math.Floor(time), Math.Round((time - Math.Floor(time)) * 60));
		}

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			double? time=(double?)value;
			string param=parameter.ToString();
			if (param == "fakt") {
				if (time.HasValue) {
					if (time.Value > 0) {
						return "";
						return String.Format("{0}", getTimeStr(time.Value));
					} else if (time.Value < -2) {
						return String.Format("-{0}", getTimeStr(time.Value));
					} else {
						return "";
					}
				} else {
					return "";
				}
			} else if (param == "plan") {
				if (time.HasValue) {
					if (time.Value > 2) {
						return "";
					}
					else if (time.Value > 0) {
						return String.Format("{0}", getTimeStr(time.Value));
					} else if (time.Value < 0) {						
						return String.Format("-{0}", getTimeStr(time.Value));
					} else {
						return "";
					}
				} else {
					return "";
				}
			}
			return "";
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
