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

	public partial class EditOrderWindow : ChildWindow
	{
		public Order CurrentOrder { get; set; }

		public EditOrderWindow() {
			InitializeComponent();
			if (!WebContext.Current.User.AllowCreateCrashOrder) {
				OrderInfo.OrderTypes.Remove(OrderTypeEnum.crash);
				OrderInfo.OrderTypes.Remove(OrderTypeEnum.no);
			}

			cmbOrderTypes.ItemsSource = OrderInfo.OrderTypes;			

			treeObjects.ItemsSource = from OrderObject o in OrdersContext.Current.Context.OrderObjects where o.ObjectID == 0 select o;
			lstUsers.ItemsSource = from OrdersUser u in OrdersContext.Current.Context.OrdersUsers where u.AllowAgreeOrders select u;
			cmbReadyTime.ItemsSource = new List<String> { "5 минут", "15 минут", "30 минут", "45 минут", "1 час", "2 часа", "3 часа", "4 часа", 
				"5 часов", "6 часов", "7 часов", "8 часов", "9 часов", "10 часов", "Время заявки" };
			orderForm.AutoCommit = false;
			orderForm.AutoEdit = false;
			
			this.HasCloseButton = false;
		}


		private void OKButton_Click(object sender, RoutedEventArgs e) {
			bool ok=orderForm.ValidateItem();
			if (ok) {
				orderForm.CommitEdit();
				if (!CurrentOrder.HasValidationErrors) {
					OrderOperations.Current.ApplyEdit(CurrentOrder);
					this.DialogResult = true;
				}
			}
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e) {
			orderForm.CancelEdit();
			OrdersContext.Current.Context.RejectChanges();
			orderForm.CurrentItem = null;
			this.DialogResult = false;
		}

		protected override void OnOpened() {
				base.OnOpened();
				//orderForm.CancelEdit();
				orderForm.CurrentItem = CurrentOrder;
				orderForm.BeginEdit();				

		}
		

		private void treeObjects_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
			OrderObject obj=treeObjects.SelectedItem as OrderObject;
			if (obj != null) {
				//CurrentOrder.SelOrderObject = obj;
				CurrentOrder.SelOrderObjectID = obj.ObjectID;
				CurrentOrder.SelOrderObjectText = obj.FullName;
			}
		}


		private void lstUsers_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			OrdersUser user=lstUsers.SelectedItem as OrdersUser;
			if ((CurrentOrder.AgreeText == null)||(CurrentOrder.AgreeUsersDict == null)) {
				CurrentOrder.AgreeText = "";
				CurrentOrder.AgreeUsersDict = new Dictionary<int, string>();
			}
			if (user != null) {
				if (CurrentOrder.AgreeUsersDict.Keys.Contains(user.UserID)) {
					CurrentOrder.AgreeUsersDict.Remove(user.UserID);
				} else {
					CurrentOrder.AgreeUsersDict.Add(user.UserID,user.FullName);
				}
				CurrentOrder.AgreeText = string.Join("; ", from string name in CurrentOrder.AgreeUsersDict.Values select name);
				CurrentOrder.AgreeUsersIDSText = string.Join(";", from int key in CurrentOrder.AgreeUsersDict.Keys select key.ToString());
			}
		}

	}
}

