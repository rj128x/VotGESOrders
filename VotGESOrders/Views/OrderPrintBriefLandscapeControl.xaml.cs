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
	public partial class OrderPrintBriefLandscapeControl : UserControl
	{


		public OrderPrintBriefLandscapeControl() {
			InitializeComponent();

		}

		public void showHeader() {
			grdOrders.RowDefinitions[0].Height = new GridLength(20);
		}

	
		
	}
}
