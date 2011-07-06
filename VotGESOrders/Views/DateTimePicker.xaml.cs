using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace VotGESOrders.Views
{
	public partial class DateTimePicker : UserControl, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public void NotifyChanged(string propName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}

				
		public DateTimePicker() {
			InitializeComponent();
			/*DatePicker.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>(DatePicker_SelectedDateChanged);
			TimePicker.ValueChanged += new RoutedPropertyChangedEventHandler<DateTime?>(TimePicker_ValueChanged);*/
			LayoutRoot.DataContext = this;

		}

		private bool enabled;
		public bool Enabled {
			get { return enabled; }
			set {
				enabled = value;
				NotifyChanged("Enabled");
			}
		}

		public DateTime? SelectedDateTime {
			get {
				return (DateTime?)GetValue(SelectedDateTimeProperty);
			}
			set {
				SetValue(SelectedDateTimeProperty, value);
			}
		}

		private DateTime? selDate;
		public DateTime? SelDate {
			get { return selDate; }
			set { 
				selDate = value;
				NotifyChanged("SelDate");
				try {
					SelectedDateTime = selDate.Value.Date.AddHours(selTime.Value.Hour).AddMinutes(selTime.Value.Minute);
				} catch { }
			}
		}

		private DateTime? selTime;
		public DateTime? SelTime {
			get { return selTime; }
			set { 
				selTime = value;
				NotifyChanged("SelTime");
				try {
					SelectedDateTime = selDate.Value.Date.AddHours(selTime.Value.Hour).AddMinutes(selTime.Value.Minute);
				} catch { }
			}
		}

		public static readonly DependencyProperty SelectedDateTimeProperty =
			 DependencyProperty.Register("SelectedDateTime",
			 typeof(DateTime?),
			 typeof(DateTimePicker),
			 new PropertyMetadata(null, new PropertyChangedCallback(SelectedDateTimeChanged)));

		public static readonly DependencyProperty EnabledProperty =
			 DependencyProperty.Register("Enabled",
			 typeof(bool),
			 typeof(DateTimePicker),
			 new PropertyMetadata(true));

		private static void SelectedDateTimeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
			DateTimePicker me = sender as DateTimePicker;
			
			if (me != null) {
				me.SelTime = (DateTime?)e.NewValue;
				me.SelDate = (DateTime?)e.NewValue;
			}
		}

		/*private void TimePicker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e) {
			if (TimePicker.Value.HasValue &&
				dt.HasValue &&
				(dt.Value.Minute != TimePicker.Value.Value.Minute ||
				dt.Value.Hour != TimePicker.Value.Value.Hour)) {
				SelectedDateTime = dt.Value.AddHours(TimePicker.Value.Value.Hour).AddMinutes(TimePicker.Value.Value.Minute);
				SelectedDateTime = dt;
			}
		}

		private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e) {
			if (DatePicker.SelectedDate.HasValue && dt.HasValue && DatePicker.SelectedDate.Value.Date != dt.Value.Date) {
				dt = DatePicker.SelectedDate.Value.AddHours(dt.Value.Hour).AddMinutes(dt.Value.Minute);
				SelectedDateTime = dt;
			}
		}*/
	}
}
