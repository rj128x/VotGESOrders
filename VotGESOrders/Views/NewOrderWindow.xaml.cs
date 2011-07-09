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
using System.ComponentModel;
using VotGESOrders.Converters;
using System.Collections.ObjectModel;
using VotGESOrders.Logging;
using System.ComponentModel.DataAnnotations;

namespace VotGESOrders.Views
{

	public partial class NewOrderWindow : ChildWindow
	{
		public bool IsNewOrder { get; set; }
		public Order CurrentOrder { get; set; }
		public Order ParentOrder { get; set; }
		protected Dictionary<String, String> OrderTypes=new Dictionary<String, String>();

		public NewOrderWindow() {
			InitializeComponent();
			OrderTypes.Add("ПЛ","Плановая");
			OrderTypes.Add("НПЛ", "Неплановая");
			OrderTypes.Add("НО", "Неотложная");
			OrderTypes.Add("АВ", "Аварийная");
			cmbOrderTypes.ItemsSource = OrderTypes;
			treeObjects.ItemsSource = from OrderObject o in OrdersContext.Current.Context.OrderObjects where o.ObjectID == 0 select o;
			orderForm.AutoCommit = false;
			orderForm.AutoEdit = false;
			
			this.HasCloseButton = false;
		}


		private void OKButton_Click(object sender, RoutedEventArgs e) {
			orderForm.CommitEdit();

			if (!CurrentOrder.HasValidationErrors) {				
				OrderOperations.Current.ApplyCreate(CurrentOrder, IsNewOrder, ParentOrder);
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
			orderForm.CurrentItem = CurrentOrder;
			orderForm.BeginEdit();

			if (IsNewOrder && !CurrentOrder.OrderIsExtend) {
				CurrentOrder.PlanStartDate = DateTime.Now.Date.AddDays(1).AddHours(8);
				CurrentOrder.PlanStopDate = DateTime.Now.Date.AddDays(1).AddHours(17);

			}
			if (CurrentOrder.OrderIsExtend) {
				treeObjects.Visibility = System.Windows.Visibility.Collapsed;
			} else {
				treeObjects.Visibility = System.Windows.Visibility.Visible;
			}

			if (CurrentOrder.SelOrderObject != null) {
				TreeViewItem item = treeObjects.ItemContainerGenerator.ContainerFromItem(CurrentOrder.SelOrderObject) as TreeViewItem;
				if (item != null) {
					item.IsSelected = true;					
				}				
			}

			//treeObjects.IsEnabled = !CurrentOrder.OrderIsExtend;

			Title = IsNewOrder ? "Создание заявки" : String.Format("Заявка №{0} от {1}", CurrentOrder.OrderNumber, CurrentOrder.OrderDateCreate.ToShortDateString());

			cmbOrderTypes.IsEnabled = !CurrentOrder.OrderIsExtend;

			

		}
		

		private void treeObjects_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
			OrderObject obj=treeObjects.SelectedItem as OrderObject;
			if (obj != null) {
				//CurrentOrder.SelOrderObject = obj;
				CurrentOrder.SelOrderObjectID = obj.ObjectID;
				CurrentOrder.SelOrderObjectText = obj.FullName;
			}
		}

	}
}

