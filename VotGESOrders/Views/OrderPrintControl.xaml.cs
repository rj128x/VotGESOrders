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
using VotGESOrders.Web.Models;

namespace VotGESOrders.Views
{
	public partial class OrderPrintControl : UserControl
	{


		public OrderPrintControl() {
			InitializeComponent();

		}

		public void updateVisibility() {
			Order order=DataContext as Order;
			grdComments.RowDefinitions[2].Height = order.OrderReviewed ? new GridLength(1, GridUnitType.Auto) : new GridLength(0);
			grdComments.RowDefinitions[3].Height = order.OrderOpened ? new GridLength(1, GridUnitType.Auto) : new GridLength(0);
			grdComments.RowDefinitions[4].Height = order.OrderClosed ? new GridLength(1, GridUnitType.Auto) : new GridLength(0);
			grdComments.RowDefinitions[5].Height = order.OrderCompleted ? new GridLength(1, GridUnitType.Auto) : new GridLength(0);
			grdComments.RowDefinitions[6].Height = order.OrderCanceled ? new GridLength(1, GridUnitType.Auto) : new GridLength(0);
			

		}

	
		
	}
}
