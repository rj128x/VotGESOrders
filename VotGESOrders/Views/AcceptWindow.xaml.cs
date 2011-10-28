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
	public enum AcceptResult { accept, ban,cancel, comment }
	public partial class AcceptWindow : ChildWindow
	{
		public Order CurrentOrder { get; set; }
		public AcceptResult Result { get; set; }
		public bool isCancelWindow { get; set; }
		public bool isCommentWindow { get; set; }
		public AcceptWindow() {
			InitializeComponent();
			orderForm.AutoCommit = false;
			orderForm.AutoEdit = false;
			this.HasCloseButton = false;
		}


		private void CancelButton_Click(object sender, RoutedEventArgs e) {
			orderForm.CancelEdit();
			OrdersContext.Current.Context.RejectChanges();
			this.DialogResult = false;
		}

		private void BanButton_Click(object sender, RoutedEventArgs e) {
			if (MessageBox.Show("Вы уверены что хотите отклонить заявку", "ОТКАЗ ЗАЯВКИ", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
				Result = AcceptResult.ban;
				CurrentOrder.OrderState = OrderStateEnum.banned;
				bool ok=orderForm.ValidateItem();
				if (ok) {
					orderForm.CommitEdit();
					if (!CurrentOrder.HasValidationErrors) {
						OrderOperations.Current.ApplyAccept(CurrentOrder, Result);
						this.DialogResult = true;
					}
				}
			}
		}

		private void AcceptButton_Click(object sender, RoutedEventArgs e) {
			Result = AcceptResult.accept;
			CurrentOrder.OrderState = OrderStateEnum.accepted;
			bool ok=orderForm.ValidateItem();
			if (ok) {
				orderForm.CommitEdit();
				if (!CurrentOrder.HasValidationErrors) {
					OrderOperations.Current.ApplyAccept(CurrentOrder, Result);
					this.DialogResult = true;
				}
			}
		}

		protected override void OnOpened() {
			base.OnOpened();
			CurrentOrder.NewComment = "";
			orderForm.CurrentItem = CurrentOrder;
			orderForm.BeginEdit();
			if (isCancelWindow) {
				CancelOrderButton.Visibility = System.Windows.Visibility.Visible;
				AcceptButton.Visibility = System.Windows.Visibility.Collapsed;
				BanButton.Visibility = System.Windows.Visibility.Collapsed;
				CommentOrderButton.Visibility = System.Windows.Visibility.Collapsed;
				btnCancelNoWork.Visibility = System.Windows.Visibility.Visible;
				btnGI.Visibility = System.Windows.Visibility.Collapsed;
				btnAgreeText.Visibility = System.Windows.Visibility.Collapsed;
				btnNotAgreeText.Visibility = System.Windows.Visibility.Collapsed;
				Title = String.Format("Снять заявку №{0} от {1}", CurrentOrder.OrderNumber.ToString(OrderInfo.NFI), CurrentOrder.OrderDateCreate.ToShortDateString());
			} else if (isCommentWindow) {
				CancelOrderButton.Visibility = System.Windows.Visibility.Collapsed;
				AcceptButton.Visibility = System.Windows.Visibility.Collapsed;
				BanButton.Visibility = System.Windows.Visibility.Collapsed;
				CommentOrderButton.Visibility = System.Windows.Visibility.Visible;
				btnCancelNoWork.Visibility = System.Windows.Visibility.Collapsed;
				btnGI.Visibility = System.Windows.Visibility.Collapsed;
				btnAgreeText.Visibility = System.Windows.Visibility.Visible;
				btnNotAgreeText.Visibility = System.Windows.Visibility.Visible;
				Title = String.Format("Комментировать заявку №{0} от {1}", CurrentOrder.OrderNumber.ToString(OrderInfo.NFI), CurrentOrder.OrderDateCreate.ToShortDateString());
			} else {
				CancelOrderButton.Visibility = System.Windows.Visibility.Collapsed;
				AcceptButton.Visibility = System.Windows.Visibility.Visible;
				BanButton.Visibility = System.Windows.Visibility.Visible;
				CommentOrderButton.Visibility = System.Windows.Visibility.Collapsed;
				btnCancelNoWork.Visibility = System.Windows.Visibility.Collapsed;
				btnGI.Visibility = System.Windows.Visibility.Visible;
				btnAgreeText.Visibility = System.Windows.Visibility.Collapsed;
				btnNotAgreeText.Visibility = System.Windows.Visibility.Collapsed;
				Title = String.Format("Разрешить заявку №{0} от {1}", CurrentOrder.OrderNumber.ToString(OrderInfo.NFI), CurrentOrder.OrderDateCreate.ToShortDateString());
			}
		}

		private void CancelOrderButton_Click(object sender, RoutedEventArgs e) {
			Result = AcceptResult.cancel;
			CurrentOrder.OrderState = OrderStateEnum.canceled;
			bool ok=orderForm.ValidateItem();
			if (ok) {
				orderForm.CommitEdit();
				if (!CurrentOrder.HasValidationErrors) {
					OrderOperations.Current.ApplyAccept(CurrentOrder, Result);
					this.DialogResult = true;
				}
			}
			/*orderForm.CommitEdit();
			OrderOperations.Current.ApplyAccept(CurrentOrder, Result);*/
			this.DialogResult = true;
		}


		private void btnGI_Click(object sender, RoutedEventArgs e) {
			CurrentOrder.NewComment = "Главный инженер (по телефону)";
		}

		private void btnCancelNoWork_Click(object sender, RoutedEventArgs e) {
			CurrentOrder.NewComment = "Работы не проводились";
		}

		private void CommentOrderButton_Click(object sender, RoutedEventArgs e) {
			Result = AcceptResult.comment;
			CurrentOrder.OrderState = OrderStateEnum.accepted;
			bool ok=orderForm.ValidateItem();			
			if (ok) {
				orderForm.CommitEdit();
				if (!CurrentOrder.HasValidationErrors) {
					OrderOperations.Current.ApplyAccept(CurrentOrder, Result);
					this.DialogResult = true;
				}
			}
		}

		private void btnAgreeText_Click(object sender, RoutedEventArgs e) {
			CurrentOrder.NewComment = "Согласовано";
		}

		private void btnNotAgreeText_Click(object sender, RoutedEventArgs e) {
			CurrentOrder.NewComment = "Не согласовано";
		}
	}
}

