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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VotGESOrders.Web.Services;
using VotGESOrders.Logging;
using VotGESOrders.Web.Models;
using VotGESOrders.Views;
using System.Windows.Data;
using System.Threading;
using System.Windows.Threading;
using System.ServiceModel.DomainServices.Client;
using System.Windows.Printing;

namespace VotGESOrders
{
	public partial class Home : Page
	{
		DispatcherTimer timerExistChanges;
		public Home() {
			
			InitializeComponent();
			pnlButtons.DataContext = WebContext.Current.User;

			OrdersContext.Current.Context.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(context_PropertyChanged);
			OrdersContext.Current.View = new PagedCollectionView(OrdersContext.Current.Context.Orders);
			OrdersContext.Current.View.SortDescriptions.Add(new System.ComponentModel.SortDescription("OrderNumber", System.ComponentModel.ListSortDirection.Descending));
			OrdersContext.Current.View.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(View_CollectionChanged);
			ordersGridControl.ordersGrid.ItemsSource = OrdersContext.Current.View;
			ordersGridControl.ordersGrid.MouseLeftButtonUp += new MouseButtonEventHandler(ordersGrid_MouseLeftButtonUp);
			//context.Orders.OrderByDescending(o => o.OrderDateCreate);			

			timerExistChanges = new DispatcherTimer();
			timerExistChanges.Tick += new EventHandler(timerExistChanges_Tick);
			timerExistChanges.Interval = new TimeSpan(0, 0, 10);
			timerExistChanges.Start();			


			cntrlOrder.Visibility = System.Windows.Visibility.Collapsed;
			cntrlFilter.DataContext = OrdersContext.Current.Filter;
			cmbFilterType.ItemsSource = OrderFilter.FilterTypes;
			cmbFilterType.DataContext = OrdersContext.Current.Filter;
		}

		void View_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			if (OrderOperations.Current.CurrentOrder == null) {
				cntrlOrder.Visibility = System.Windows.Visibility.Collapsed;
			}
			
		}

		void timerExistChanges_Tick(object sender, EventArgs e) {
				InvokeOperation<bool> oper=
					OrdersContext.Current.Context.ExistsChanges(OrdersContext.Current.LastUpdate, OrdersContext.Current.SessionGUID);				
				oper.Completed += new EventHandler(oper_Completed);
		}

		void oper_Completed(object sender, EventArgs e) {
			InvokeOperation<bool> oper=sender as InvokeOperation<bool>;
			if (OrdersContext.Current.LastUpdate.AddMinutes(10) < DateTime.Now) {
				GlobalStatus.Current.NeedRefresh= true;
			}
			if (oper.Value || GlobalStatus.Current.NeedRefresh) {
				if (GlobalStatus.Current.CanRefresh) {
					OrdersContext.Current.RefreshOrders(true);
				} else {
					GlobalStatus.Current.NeedRefresh = true;
				}
			}
			
		}

		void context_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			OrdersContext.Current.View.Refresh();
			//ordersGrid.UpdateLayout();
		}



		// Выполняется, когда пользователь переходит на эту страницу.
		protected override void OnNavigatedTo(NavigationEventArgs e) {
		}


		private void btnCreateOrder_Click(object sender, RoutedEventArgs e) {
			OrderOperations.Current.initCreate();
		}

		private void btnRefresh_Click(object sender, RoutedEventArgs e) {
			//OrdersContext.Context.Load(OrdersContext.Context.LoadOrdersQuery(), System.ServiceModel.DomainServices.Client.LoadBehavior.RefreshCurrent, false);
			OrdersContext.Current.RefreshOrders(true);
			OrdersContext.Current.View.Refresh();
		}


		private void btnVisFilter_Click(object sender, RoutedEventArgs e) {
			if (cntrlFilter.Visibility == System.Windows.Visibility.Visible) {
				cntrlFilter.Visibility = System.Windows.Visibility.Collapsed;
			} else {
				cntrlFilter.Visibility = System.Windows.Visibility.Visible;
			}
		}

		private void ordersGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			Order order=ordersGridControl.ordersGrid.SelectedItem as Order;
			if (order != null) {
				cntrlOrder.DataContext = order;
				if (OrderOperations.Current.CurrentOrder == null) {
					cntrlOrder.Visibility = System.Windows.Visibility.Visible;
				}
				OrderOperations.Current.CurrentOrder = order;
			}
		}
				

		private void btnVisDetails_Click(object sender, RoutedEventArgs e) {
			if ((cntrlOrder.Visibility == System.Windows.Visibility.Collapsed) && (ordersGridControl.ordersGrid.SelectedItem != null)) {
				cntrlOrder.Visibility = System.Windows.Visibility.Visible;
			} else {
				cntrlOrder.Visibility = System.Windows.Visibility.Collapsed;
			}
		}


		private void btnPrint_Click(object sender, RoutedEventArgs e) {
			PrintDocument multidoc = new PrintDocument();
			int index = 0;


			multidoc.PrintPage += (s, arg) => {
				StackPanel host = new StackPanel();
				bool isFirst=true;
				while (index < OrdersContext.Current.View.Count) {
					//OrderPrintControl cntrl = new OrderPrintControl();
					OrderPrintBriefControl cntrl = new OrderPrintBriefControl();
					cntrl.DataContext = OrdersContext.Current.View.GetItemAt(index); ;
					cntrl.UpdateLayout();

					host.Children.Add(cntrl);
					//cntrl.updateVisibility();
					if (!isFirst) {
						cntrl.hideHeader();
						cntrl.InvalidateArrange();						
					}
					isFirst = false;
					//host.UpdateLayout();

					host.Measure(new Size(arg.PrintableArea.Width, double.PositiveInfinity));

					if (host.DesiredSize.Height > arg.PrintableArea.Height && host.Children.Count > 1) {
						host.Children.Remove(cntrl);
						arg.HasMorePages = true;
						break;
					}
					index++;
				}
				arg.PageVisual = host;

			};

			multidoc.BeginPrint += (s, arg) => {
				GlobalStatus.Current.Status = "Печать списка заказов";
			};

			multidoc.EndPrint += (s, arg) => {
				GlobalStatus.Current.Status = "Готово";
			};

			multidoc.Print("Список какой-то фигни");



		}

		void doc_PrintPage(object sender, PrintPageEventArgs e) {
			OrderControl cntrl=new OrderControl();
			cntrl.DataContext = OrderOperations.Current.CurrentOrder;
			e.PageVisual = cntrl;
		}

		private void btnFullScreen_Click(object sender, RoutedEventArgs e) {
			Application.Current.Host.Content.IsFullScreen = !Application.Current.Host.Content.IsFullScreen;
		}

		private void cmbFilterType_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			btnVisFilter.Visibility=OrdersContext.Current.Filter.FilterType==OrderFilterEnum.userFilter?
				System.Windows.Visibility.Visible:
				System.Windows.Visibility.Collapsed;
			cntrlFilter.Visibility = OrdersContext.Current.Filter.FilterType == OrderFilterEnum.userFilter ?
				System.Windows.Visibility.Visible :
				System.Windows.Visibility.Collapsed;
		}


	}
}