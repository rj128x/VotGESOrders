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
using System.ComponentModel;
using VotGESOrders.Web.Models;

namespace VotGESOrders
{
	public class GlobalStatus : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public void NotifyChanged(string propName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}

		public GlobalStatus() {
			
		}

		public void init() {
			try {
				OrdersContext.Current.Context.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Context_PropertyChanged);
				OrdersContext.Current.PropertyChanged += new PropertyChangedEventHandler(Current_PropertyChanged);
				LastUpdate = OrdersContext.Current.LastUpdate;
				HomeHeader = "Список заявок";
			} catch (Exception e){
				//Logging.Logger.logMessage(e.ToString());
			}
		}

		void Current_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == "LastUpdate") {
				LastUpdate = OrdersContext.Current.LastUpdate;
			}
		}



		void Context_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			
			if (e.PropertyName == "IsLoading") {
				Status = OrdersContext.Current.Context.IsLoading ? "Загрузка" : "Готово";				
			}
			if (e.PropertyName == "IsSubmitting") {
				Status = OrdersContext.Current.Context.IsSubmitting ? "Применение изменений" : "Готово";
			}
			
			IsBusy = OrdersContext.Current.Context.IsLoading || OrdersContext.Current.Context.IsSubmitting;
			HomeHeader = "Список заявок - " + OrderFilter.FilterTypes[OrdersContext.Current.Filter.FilterType];
		}

		protected string status;
		public string Status {
			get {
				return status;
			}
			set {
				status = value;
				NotifyChanged("Status");
			}
		}

		protected bool isBusy;
		public bool IsBusy {
			get {
				return isBusy;
			}
			set {
				isBusy = value;
				if (isBusy) {
					isChangingOrder = false;
				}
				NotifyChanged("IsBusy");
				CanRefresh = !IsBusy && !IsChangingOrder;
			}
		}

		protected bool isChangingOrder;
		public bool IsChangingOrder {
			get { return isChangingOrder; }
			set { 
				isChangingOrder = value;
				NotifyChanged("IsChangingOrder");
				CanRefresh = !IsBusy && !IsChangingOrder;
			}
		}

		protected bool isError;
		public bool IsError {
			get { return isError; }
			set {
				isError = value;
				NotifyChanged("IsError");
			}
		}

		private bool canRefresh;
		public bool CanRefresh {
			get { return canRefresh; }
			set { 
				canRefresh = value;
				NotifyChanged("CanRefresh");
			}
		}


		private bool needRefresh;
		public bool NeedRefresh {
			get { return needRefresh; }
			set { 
				needRefresh = value;
				NotifyChanged("NeedRefresh");
			}
		}

		private string homeHeader;
		public string HomeHeader {
			get { return homeHeader; }
			set { 
				homeHeader = value;
				NotifyChanged("HomeHeader");
			}
		}

		private DateTime lastUpdate;
		public DateTime LastUpdate {
			get {
				return lastUpdate;
			}
			protected set {
				lastUpdate = value;
				NotifyChanged("LastUpdate");
				NeedRefresh = false;
			}
		}

		public static GlobalStatus Current {
			get {
				return Application.Current.Resources["globalStatus"] as GlobalStatus;
			}
		}
	}
}
