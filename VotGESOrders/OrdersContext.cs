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
using VotGESOrders.Web.Services;
using VotGESOrders.Web.Models;
using System.ComponentModel;
using VotGESOrders.Logging;
using System.Windows.Data;
using VotGESOrders.Views;
using System.ServiceModel.DomainServices.Client;

namespace VotGESOrders
{
	public class OrdersContext : INotifyPropertyChanged
	{		
		public event PropertyChangedEventHandler PropertyChanged;

		public void NotifyChanged(string propName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}

		protected OrdersContext() {
			SessionGUID = Guid.NewGuid();
		}

		private DateTime lastUpdate;
		public DateTime LastUpdate {
			get {
				return lastUpdate;
			}
			protected set {
				lastUpdate = value;
				NotifyChanged("LastUpdate");
			}
		}

		public Guid SessionGUID { get; protected set; }
		
		protected static OrdersContext ordersContext;
		public static OrdersContext Current {
			get {
				return ordersContext;
			}
		}

		protected OrdersDomainContext context;
		public OrdersDomainContext Context {
			get {
				return context;
			}
		}


		protected PagedCollectionView view;
		public PagedCollectionView View {
			get {
				return view;
			}
			set {
				view = value;
			}
		}

		
		protected OrderFilter filter;
		public OrderFilter Filter {
			get {
				return filter;
			}
			set {
				filter = value;
			}
		}

		protected void loadData() {
			context = new OrdersDomainContext();
			context.Load(context.LoadOrdersUsersQuery());
			context.Load(context.LoadOrderObjectsQuery());

			filter = new OrderFilter();
			context.Load(context.LoadOrdersQuery());

			LastUpdate = DateTime.Now;
		}

		public void SubmitChangesCallbackError() {
			context.SubmitChanges(submit, null);
		}

		protected void submit(SubmitOperation oper) {
			if (oper.HasError) {				
				GlobalStatus.Current.Status = "Ошибка при выполнении операции на сервере";
				Logger.info(oper.Error.ToString());
				oper.MarkErrorAsHandled();
			} else {
				GlobalStatus.Current.Status = "Готово";
			}
		}
		

		protected void RefreshOrdersFilterXML(bool clear) {
			if (clear) {
				context.Orders.Clear();
			}
			filter.SelectedUsersStr = filter.SelectedUsersJoinStr;
			filter.SelectedObjectsStr = filter.SelectedObjectsJoinStr;
			string xml=XMLStringSerializer.Serialize<OrderFilter>(filter);
			context.Load(
				context.GetFilteredOrdersFromXMLQuery(xml),
				System.ServiceModel.DomainServices.Client.LoadBehavior.RefreshCurrent, true);
			LastUpdate = DateTime.Now;

		}

		public void RefreshOrders(bool clear){
			OrderOperations.Current.CurrentOrder = null;
			RefreshOrdersFilterXML(clear);			
		}

		public static void init() {
			ordersContext = new OrdersContext();
			ordersContext.loadData();
		}



	}
}
