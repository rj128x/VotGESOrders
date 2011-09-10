﻿using System;
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
					OrdersContext.Current.Context.ExistsChanges(OrdersContext.Current.SessionGUID);
				oper.Completed += new EventHandler(oper_Completed);									
		}

		void oper_Completed(object sender, EventArgs e) {
			InvokeOperation<bool> oper=sender as InvokeOperation<bool>;
			if (!oper.HasError) {
				if (OrdersContext.Current.LastUpdate.AddMinutes(10) < DateTime.Now) {
					GlobalStatus.Current.NeedRefresh = true;
				}
				if (oper.Value || GlobalStatus.Current.NeedRefresh) {
					if (GlobalStatus.Current.CanRefresh) {
						OrdersContext.Current.RefreshOrders(true);
					} else {
						GlobalStatus.Current.NeedRefresh = true;
					}
				}
			} else {
				GlobalStatus.Current.Status = "Ошибка при соединении с сервером";
				oper.MarkErrorAsHandled();
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
			if (OrderOperations.Current.CurrentOrder == null) {
				cntrlOrder.Visibility = System.Windows.Visibility.Collapsed;
			} else {
				if ((cntrlOrder.Visibility == System.Windows.Visibility.Collapsed) && (ordersGridControl.ordersGrid.SelectedItem != null)) {
					cntrlOrder.Visibility = System.Windows.Visibility.Visible;
				} else {
					cntrlOrder.Visibility = System.Windows.Visibility.Collapsed;
				}
			}
		}


		private List<StackPanel> getPrintPages(double width, double height) {
			List<StackPanel> pages=new List<StackPanel>();
			bool finish=false;
			int index=0;			
			while (!finish) {
				StackPanel host = new StackPanel();				
				bool isFirst=true;
				while (index < OrdersContext.Current.View.Count) {
					OrderPrintBriefLandscapeControl cntrl = new OrderPrintBriefLandscapeControl();
					cntrl.DataContext = OrdersContext.Current.View.GetItemAt(index); ;
					cntrl.UpdateLayout();
					host.Children.Add(cntrl);
					
					if (!isFirst) {
						cntrl.hideHeader();
						cntrl.InvalidateArrange();
					}
					isFirst = false;

					host.Measure(new Size(width, double.PositiveInfinity));

					if (host.DesiredSize.Height + 65 > height && host.Children.Count > 1) {
						host.Children.Remove(cntrl);
						break;
					}
					index++;
					finish = OrdersContext.Current.View.Count==index;					
				}
				pages.Add(host);
			}
			return pages;
		}

		private void btnPrint_Click(object sender, RoutedEventArgs e) {
			PrintDocument multidoc = new PrintDocument();
			int index = 0;
			List<StackPanel> pages=null;

			multidoc.PrintPage += (s, arg) => {
				double width = arg.PrintableArea.Width;
				double height=arg.PrintableArea.Height;
				bool rotate=false;

				Logger.logMessage(String.Format("{0}-{1}",width,height));

				if (width < height) {
					double temp=width;
					width = height;
					height = temp;
					rotate = true;
				}				

				Logger.logMessage(String.Format("{0}-{1}", width, height));

				if (pages == null) {
					pages = getPrintPages(width,height);
				}
				if (index < pages.Count) {
					StackPanel host=pages[index];

					Grid grid=new Grid();
					Grid layout=new Grid();
					layout.VerticalAlignment = System.Windows.VerticalAlignment.Top;
					layout.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
					layout.Width = width;
					layout.Height = width;
					layout.Children.Add(grid);
					grid.RowDefinitions.Add(new RowDefinition());
					grid.RowDefinitions.Add(new RowDefinition());
					grid.RowDefinitions.Add(new RowDefinition());
					grid.RowDefinitions[0].Height = new GridLength(30);
					grid.RowDefinitions[2].Height = new GridLength(35);
					grid.RowDefinitions[1].Height = new GridLength(height - 65);

					/*grid.ColumnDefinitions.Add(new ColumnDefinition());
					grid.ColumnDefinitions[0].Width = new GridLength(width);*/

					TextBlock header=new TextBlock();
					header.Text = String.Format("{0} на {1}", GlobalStatus.Current.HomeHeader,DateTime.Now.ToString("dd.MM.yy HH:mm"));
					header.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
					header.Margin = new Thickness(0, 0, 0, 10);
					header.FontSize = 13;
					grid.Children.Add(header);
					header.SetValue(Grid.RowProperty, 0);

					//host.Measure(new Size(width, double.PositiveInfinity));

					StackPanel footerPnl=new StackPanel();
					footerPnl.Width = width;
					footerPnl.Orientation = Orientation.Horizontal;
					
					TextBlock page=new TextBlock();
					page.Text = String.Format("Cтраница {0} из {1}", index+1, pages.Count); ;
					page.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
					page.FontSize = 12;
					page.Width = 200;
					footerPnl.Children.Add(page);

					TextBlock footer=new TextBlock();
					//footer.Text = String.Format(" Начальник ОС ________________/{0}/",DateTime.Now.ToString("dd.MM.yy"));
					footer.TextAlignment = TextAlignment.Right;
					footer.FontSize = 12;
					footer.Width = width - page.Width;
					footerPnl.Children.Add(footer);
					
					grid.Children.Add(footerPnl);
					footerPnl.SetValue(Grid.RowProperty, 2);
					footerPnl.Margin = new Thickness(0, 15, 0, 0);

					grid.Children.Add(host);
					host.SetValue(Grid.RowProperty, 1);
					if (rotate){
						CompositeTransform transform=new CompositeTransform() {							
							Rotation = 90,
							TranslateX=height
							
						};
						grid.RenderTransform = transform;
						
					}
					
					arg.PageVisual = layout;			
					
				}				
				index++;
				arg.HasMorePages = index < pages.Count;
			};

			multidoc.BeginPrint += (s, arg) => {
				GlobalStatus.Current.Status = "Печать списка заказов";
			};

			multidoc.EndPrint += (s, arg) => {
				GlobalStatus.Current.Status = "Готово";
			};

			multidoc.Print("Список заявок");



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