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
	public enum OrderOperation { open, close, enter }
	public partial class OrderDateOperationWindow : ChildWindow
	{
		public OrderOperation Operation { get; set; }
		public Order CurrentOrder;
		public OrderDateOperationWindow() {
			InitializeComponent();
		}


		private void OKButton_Click(object sender, RoutedEventArgs e) {
			bool ok=orderForm.ValidateItem();
			orderForm.CommitEdit();
			/*foreach (ValidationResult vr in CurrentOrder.ValidationErrors) {
				Logger.logMessage(vr.ErrorMessage);
			}*/
			if (!CurrentOrder.HasValidationErrors && ok) {
				OrderOperations.Current.ApplyDataOperation(CurrentOrder, Operation);
				this.DialogResult = true;
			}
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e) {
			orderForm.CancelEdit();
			OrdersContext.Current.Context.RejectChanges();
			this.DialogResult = false;
		}

		protected override void OnOpened() {
			base.OnOpened();
			orderForm.CancelEdit();

			switch (Operation) {
				case OrderOperation.close:
					Title = String.Format("Закрытие заявки №{0} от {1}", CurrentOrder.OrderNumber, CurrentOrder.OrderDateCreate.ToShortDateString());
					CurrentOrder.FaktStopDate = DateTime.Now;
					CurrentOrder.CloseText = "Работы завершены. Оборудование можно вводить в работу";
					
					FaktStopDate.Visibility = System.Windows.Visibility.Visible;
					FaktStartDate.Visibility = System.Windows.Visibility.Collapsed;
					FaktEnterDate.Visibility = System.Windows.Visibility.Collapsed;
					
					break;
				case OrderOperation.enter:
					Title = String.Format("Ввод оборудования. заявка №{0} от {1}", CurrentOrder.OrderNumber, CurrentOrder.OrderDateCreate.ToShortDateString());
					CurrentOrder.FaktEnterDate = DateTime.Now;
					CurrentOrder.EnterText = "Оборудование введено в работу";
					
					FaktStopDate.Visibility = System.Windows.Visibility.Collapsed;
					FaktStartDate.Visibility = System.Windows.Visibility.Collapsed;
					FaktEnterDate.Visibility = System.Windows.Visibility.Visible;

					break;
				case OrderOperation.open:
					Title = String.Format("Открытие заявки №{0} от {1}", CurrentOrder.OrderNumber, CurrentOrder.OrderDateCreate.ToShortDateString());
					if (!CurrentOrder.OrderIsExtend) {
						CurrentOrder.FaktStartDate = DateTime.Now;
					}
					CurrentOrder.OpenText = "Оборудование выведено. Можно начинать работу";
	
					FaktStopDate.Visibility = System.Windows.Visibility.Collapsed;
					FaktStartDate.Visibility = System.Windows.Visibility.Visible;
					FaktEnterDate.Visibility = System.Windows.Visibility.Collapsed;

					break;

			}
			orderForm.CurrentItem = CurrentOrder;
		}



	}
}

