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
using VotGESOrders.Converters;
using VotGESOrders.Logging;
using System.ComponentModel.DataAnnotations;

namespace VotGESOrders.Views
{
	public enum OrderOperation { open, close, complete }
	public partial class OrderDateOperationWindow : ChildWindow
	{
		public OrderOperation Operation { get; set; }
		public Order CurrentOrder;
		public OrderDateOperationWindow() {
			InitializeComponent();
			orderForm.AutoCommit = false;
			orderForm.AutoEdit = false;
			this.HasCloseButton = false;
		}


		private void OKButton_Click(object sender, RoutedEventArgs e) {
			bool ok=orderForm.ValidateItem();
			if (ok) {
				orderForm.CommitEdit();
				if (!CurrentOrder.HasValidationErrors) {
					OrderOperations.Current.ApplyDataOperation(CurrentOrder, Operation);
					this.DialogResult = true;
				}
			}
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e) {
			orderForm.CancelEdit();
			OrdersContext.Current.Context.RejectChanges();			
			this.DialogResult = false;
		}

		protected override void OnOpened() {			
			base.OnOpened();			
			orderForm.CurrentItem = CurrentOrder;
			orderForm.BeginEdit();

			switch (Operation) {
				case OrderOperation.close:
					Title = String.Format("Разрешение на ввод. Заявка №{0} от {1}", CurrentOrder.OrderNumber.ToString(OrderInfo.NFI), CurrentOrder.OrderDateCreate.ToShortDateString());
					CurrentOrder.OrderClosed = true;

					CurrentOrder.FaktStopDate = DateTime.Now;
					CurrentOrder.CloseText = "Работы завершены. Оборудование можно вводить в работу";

					if (WebContext.Current.User.UserID != CurrentOrder.UserCreateOrderID) {
						CurrentOrder.CloseText += "\n" + CurrentOrder.UserCreateOrder.FullName + " (по телефону)";
					}
					
					FaktStopDate.Visibility = System.Windows.Visibility.Visible;
					FaktStartDate.Visibility = System.Windows.Visibility.Collapsed;
					FaktCompleteDate.Visibility = System.Windows.Visibility.Collapsed;
					
					break;
				case OrderOperation.complete:
					Title = String.Format("Закрытие заявки. Заявка №{0} от {1}", CurrentOrder.OrderNumber.ToString(OrderInfo.NFI), CurrentOrder.OrderDateCreate.ToShortDateString());
					CurrentOrder.OrderCompleted = true;
					CurrentOrder.FaktCompleteDate = DateTime.Now;
					CurrentOrder.CompleteText = "Оборудование введено в работу";
					
					FaktStopDate.Visibility = System.Windows.Visibility.Collapsed;
					FaktStartDate.Visibility = System.Windows.Visibility.Collapsed;
					FaktCompleteDate.Visibility = System.Windows.Visibility.Visible;

					break;
				case OrderOperation.open:
					Title = String.Format("Открытие заявки №{0} от {1}", CurrentOrder.OrderNumber.ToString(OrderInfo.NFI), CurrentOrder.OrderDateCreate.ToShortDateString());
					CurrentOrder.OrderOpened = true;		
					if (!CurrentOrder.OrderIsExtend&&CurrentOrder.OrderType!=OrderTypeEnum.crash) {
						CurrentOrder.FaktStartDate = DateTime.Now;						
					} else {
						CurrentOrder.FaktStartDate = CurrentOrder.PlanStartDate;
					}
					if (CurrentOrder.OrderIsExtend) {
						CurrentOrder.OpenText = "Заявка продлена. Оборудование выведено.";
					} else if (CurrentOrder.OrderType == OrderTypeEnum.crash ) {
						CurrentOrder.OpenText = "Аварийная заявка. Оборудование выведено.";
					} else if (CurrentOrder.OrderType == OrderTypeEnum.no) {
						CurrentOrder.OpenText = "Неотложная заявка. Оборудование выведено.";
					} else {
						CurrentOrder.OpenText = "Оборудование выведено. Можно начинать работу";
					}
	
					FaktStopDate.Visibility = System.Windows.Visibility.Collapsed;
					FaktStartDate.Visibility = System.Windows.Visibility.Visible;
					FaktCompleteDate.Visibility = System.Windows.Visibility.Collapsed;


					bool canChange=(CurrentOrder.OrderType == OrderTypeEnum.crash || CurrentOrder.OrderIsExtend);
					FaktStartDatePicker.Enabled = !canChange;

					break;
			}
		}



	}
}

