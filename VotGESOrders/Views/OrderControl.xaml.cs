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
	public partial class OrderControl : UserControl
	{
		

		public OrderControl() {
			InitializeComponent();
		}
		
		private void btnChange_Click(object sender, RoutedEventArgs e) {
			OrderOperations.Current.initChange();
		}

		private void btnAccept_Click(object sender, RoutedEventArgs e) {
			OrderOperations.Current.initAccept();
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e) {
			OrderOperations.Current.initCancel();
		}

		private void btnOpen_Click(object sender, RoutedEventArgs e) {
			OrderOperations.Current.initOpen();
		}

		private void btnClose_Click(object sender, RoutedEventArgs e) {
			OrderOperations.Current.initClose();
		}

		private void btnComplete_Click(object sender, RoutedEventArgs e) {
			OrderOperations.Current.initComplete();
		}

		private void btnExtend_Click(object sender, RoutedEventArgs e) {
			OrderOperations.Current.initExtend();
		}

		private void btnCompleteWithoutEnter_Click(object sender, RoutedEventArgs e) {
			OrderOperations.Current.initCompleteWithoutEnter();
		}

		private void btnRejectReview_Click(object sender, RoutedEventArgs e) {
			OrderOperations.Current.RejectReviewOrder();
		}

		private void btnRejectOpen_Click(object sender, RoutedEventArgs e) {
			OrderOperations.Current.RejectOpenOrder();
		}

		private void btnRejectCancel_Click(object sender, RoutedEventArgs e) {
			OrderOperations.Current.RejectCancelOrder();
		}

		private void btnRejectClose_Click(object sender, RoutedEventArgs e) {
			OrderOperations.Current.RejectCloseOrder();
		}

		private void btnRejectComplete_Click(object sender, RoutedEventArgs e) {
			OrderOperations.Current.RejectCompleteOrder();
		}

		private void btnEdit_Click(object sender, RoutedEventArgs e) {
			OrderOperations.Current.initEdit();
		}

		private void btnComment_Click(object sender, RoutedEventArgs e) {
			OrderOperations.Current.initComment();
		}		
		
	}
}
