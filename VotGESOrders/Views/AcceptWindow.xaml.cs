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
using System.Windows.Data;
using VotGESOrders.Web.Models;

namespace VotGESOrders.Views
{
	public enum AcceptResult { accept, ban,cancel }
	public partial class AcceptWindow : ChildWindow
	{
		public Order CurrentOrder { get; set; }
		public AcceptResult Result { get; set; }
		public bool isCancelWindow { get; set; }
		public AcceptWindow() {
			InitializeComponent();
		}


		private void CancelButton_Click(object sender, RoutedEventArgs e) {
			orderForm.CancelEdit();
			OrdersContext.Current.Context.RejectChanges();
			this.DialogResult = false;
		}

		private void BanButton_Click(object sender, RoutedEventArgs e) {
			Result = AcceptResult.ban;
			orderForm.CommitEdit();
			OrderOperations.Current.ApplyAccept(CurrentOrder, Result);
			this.DialogResult = true;
		}

		private void AcceptButton_Click(object sender, RoutedEventArgs e) {
			Result = AcceptResult.accept;
			orderForm.CommitEdit();
			OrderOperations.Current.ApplyAccept(CurrentOrder, Result);
			this.DialogResult = true;
		}

		protected override void OnOpened() {
			base.OnOpened();
			orderForm.CancelEdit();
			CurrentOrder.NewComment = "";
			orderForm.CurrentItem = CurrentOrder;

			if (isCancelWindow) {
				CancelOrderButton.Visibility = System.Windows.Visibility.Visible;
				AcceptButton.Visibility = System.Windows.Visibility.Collapsed;
				BanButton.Visibility = System.Windows.Visibility.Collapsed;
				btnCancelNoWork.Visibility = System.Windows.Visibility.Visible;
				btnGI.Visibility = System.Windows.Visibility.Collapsed;
				Title = String.Format("Снять заявку №{0} от {1}", CurrentOrder.OrderNumber, CurrentOrder.OrderDateCreate.ToShortDateString());
			} else {
				CancelOrderButton.Visibility = System.Windows.Visibility.Collapsed;
				AcceptButton.Visibility = System.Windows.Visibility.Visible;
				BanButton.Visibility = System.Windows.Visibility.Visible;
				btnCancelNoWork.Visibility = System.Windows.Visibility.Collapsed;
				btnGI.Visibility = System.Windows.Visibility.Visible;				
				Title = String.Format("Разрешить заявку №{0} от {1}", CurrentOrder.OrderNumber, CurrentOrder.OrderDateCreate.ToShortDateString());
			}
		}

		private void CancelOrderButton_Click(object sender, RoutedEventArgs e) {
			Result = AcceptResult.cancel;
			orderForm.CommitEdit();
			OrderOperations.Current.ApplyAccept(CurrentOrder, Result);
			this.DialogResult = true;
		}


		private void btnGI_Click(object sender, RoutedEventArgs e) {
			CurrentOrder.NewComment = "Главный инженер (по телефону)";
		}

		private void btnCancelNoWork_Click(object sender, RoutedEventArgs e) {
			CurrentOrder.NewComment = "Работы не проводились";
		}
	}
}

