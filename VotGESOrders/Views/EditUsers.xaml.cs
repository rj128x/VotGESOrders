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
using System.ComponentModel;

namespace VotGESOrders
{
	public partial class EditUsers : Page,INotifyPropertyChanged
	{
		protected static int UserID=-2;
		public event PropertyChangedEventHandler PropertyChanged;

		public void NotifyChanged(string propName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
			
		}

		protected OrdersUser CurrentUser;
		protected bool isNew=false;
		
		public EditUsers() {			
			InitializeComponent();
			gridUsers.ItemsSource = OrdersContext.Current.Context.OrdersUsers;
		}


		protected void submit(SubmitOperation oper) {
			if (oper.HasError) {
				GlobalStatus.Current.Status = "Ошибка при выполнении операции на сервере: " + oper.Error.Message;
				MessageBox.Show(oper.Error.Message, "Ошибка при выполнении операции на сервере", MessageBoxButton.OK);
				refresh();
				Logger.info(oper.Error.ToString());
				oper.MarkErrorAsHandled();
			} else {
				GlobalStatus.Current.Status = "Готово";
			}
			userForm.CurrentItem = null;
			gridUsers.SelectedItem = null;
		}

		protected void cancelEdit() {
			if (CurrentUser != null) {
				userForm.CancelEdit();
				OrdersContext.Current.Context.RejectChanges();
			}
		}

		protected void refreshSelObject() {
			OrdersUser selectedUser=gridUsers.SelectedItem as OrdersUser;
			cancelEdit();
			if (selectedUser != null) {
				CurrentUser = selectedUser;				
			}
		}

		protected void saveEdit() {
			if (CurrentUser != null) {
				if (isNew) {
					OrdersContext.Current.Context.OrdersUsers.Attach(CurrentUser);
				}
				userForm.CommitEdit();
				OrdersContext.Current.Context.RegisterChangeUser(CurrentUser);
				OrdersContext.Current.Context.SubmitChanges(submit,null);
			}
		}


		private void btnChangeItem_Click(object sender, RoutedEventArgs e) {
			refreshSelObject();
			if (CurrentUser != null) {
				isNew = false;
				userForm.CurrentItem = CurrentUser;
				Login.IsEnabled = isNew;
			}
		}

		private void btnDeleteItem_Click(object sender, RoutedEventArgs e) {
			refreshSelObject();
			if (CurrentUser != null) {
				OrdersContext.Current.Context.RegisterDeleteUser(CurrentUser);

				OrdersContext.Current.Context.SubmitChanges(submit, null);
				OrdersContext.Current.Context.OrdersUsers.Detach(CurrentUser);

			}
		}

		private void btnAddItem_Click(object sender, RoutedEventArgs e) {
			cancelEdit();
			OrdersUser newUser=new OrdersUser();
			newUser.UserID = UserID--;
			isNew = true;
			CurrentUser=newUser;
			userForm.CurrentItem = CurrentUser;
			Login.IsEnabled = isNew;
		}

		private void btnSave_Click(object sender, RoutedEventArgs e) {
			saveEdit();
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e) {
			cancelEdit(); 
			CurrentUser = null;
			userForm.CurrentItem = null;
		}

		private void refresh() {
			OrdersContext.Current.Context.OrdersUsers.Clear();
			OrdersContext.Current.Context.Load(OrdersContext.Current.Context.LoadOrdersUsersQuery(), LoadBehavior.RefreshCurrent,
				oper => {
					userForm.CurrentItem = null;
					gridUsers.SelectedItem = null;
				}, null);
		}

		private void btnRefresh_Click(object sender, RoutedEventArgs e) {
			refresh();
		}


	}
}