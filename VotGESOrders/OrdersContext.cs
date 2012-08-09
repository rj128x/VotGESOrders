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

		private bool readyUsers;
		private bool readyObjects;
		private bool readyOrders;
		private bool readyAll;
		protected void loadData() {
			readyObjects = false;
			readyUsers = false;
			readyOrders = true;
			readyAll = false;

			LoadOperation loadUsersOper=context.Load(context.LoadOrdersUsersQuery());
			loadUsersOper.Completed += new EventHandler(loadUsersOper_Completed);
			
			LoadOperation loadObjectsOper=context.Load(context.LoadOrderObjectsQuery());
			loadObjectsOper.Completed += new EventHandler(loadObjectsOper_Completed);

			LoadOperation loadOrdersOper=context.Load(context.LoadOrdersQuery(SessionGUID));
			loadOrdersOper.Completed += new EventHandler(loadOrdersOper_Completed);
		}

		void loadUsersOper_Completed(object sender, EventArgs e) {
			Logger.info("Получен список пользователей");
			readyUsers = true;
			loadOrders();
		}

		void loadObjectsOper_Completed(object sender, EventArgs e) {
			Logger.info("Получен список оборудования");
			readyObjects = true;
			loadOrders();
		}

		void loadOrdersOper_Completed(object sender, EventArgs e) {
			Logger.info("Получен список заявок");
			readyOrders = true;			
			loadOrders();
		}

		public delegate void DelegateLoadedAllData();
		public DelegateLoadedAllData FinishLoadingOrdersEvent=null;

		private void loadOrders() {
			if (readyUsers && readyObjects&&readyOrders&&!readyAll) {
				OrderOperations.Current.CreateWindows();
				readyAll = true;
				view = new PagedCollectionView(context.Orders);
				view.SortDescriptions.Add(new System.ComponentModel.SortDescription("OrderState", System.ComponentModel.ListSortDirection.Ascending));
				view.SortDescriptions.Add(new System.ComponentModel.SortDescription("OrderNumber", System.ComponentModel.ListSortDirection.Descending));
				LastUpdate = DateTime.Now;
				if (FinishLoadingOrdersEvent != null) {
					FinishLoadingOrdersEvent();
				}
			}
		}

		

		public void SubmitChangesCallbackError() {
			context.SubmitChanges(submit, null);
		}

		protected void submit(SubmitOperation oper) {
			if (oper.HasError) {				
				GlobalStatus.Current.Status = "Ошибка при выполнении операции на сервере: "+oper.Error.Message;
				MessageBox.Show(oper.Error.Message, "Ошибка при выполнении операции на сервере",MessageBoxButton.OK);
				RefreshOrders(true);
				Logger.info(oper.Error.ToString());
				oper.MarkErrorAsHandled();
			} else {
				GlobalStatus.Current.Status = "Готово";
			}
		}
		

		protected void RefreshOrdersFilterXML(bool clear, bool sendMail) {
			if (clear) {
				context.Orders.Clear();
			}
			filter.SelectedUsersStr = filter.getSelectedUsersJoinStr();
			filter.SelectedObjectsStr = filter.getSelectedObjectsJoinStr();
			string xml=XMLStringSerializer.Serialize<OrderFilter>(filter);
			if (!sendMail) {
				context.Load(
					context.GetFilteredOrdersFromXMLQuery(xml, OrdersContext.Current.SessionGUID),
					System.ServiceModel.DomainServices.Client.LoadBehavior.RefreshCurrent, true);
			} else {
				context.Load(
					context.GetFilteredOrdersFromXMLToMailQuery(xml, OrdersContext.Current.SessionGUID),
					System.ServiceModel.DomainServices.Client.LoadBehavior.RefreshCurrent, true);
			}
			LastUpdate = DateTime.Now;

		}

		public void RefreshOrders(bool clear){
			OrderOperations.Current.CurrentOrder = null;
			RefreshOrdersFilterXML(clear,false);			
		}

		public void SendMail(bool clear) {
			OrderOperations.Current.CurrentOrder = null;
			RefreshOrdersFilterXML(clear,true);
		}

		public static void init() {			
			ordersContext = new OrdersContext();
			ordersContext.context = new OrdersDomainContext();
			ordersContext.filter = new OrderFilter();
			GlobalStatus.Current.init();
			ordersContext.loadData();
		}


	}
}
