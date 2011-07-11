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
	public partial class EditTree : Page,INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public void NotifyChanged(string propName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}

		protected static int CurrentObjectNumber=-2;
		public OrderObject SelObject { get; protected set; }

		private string selParentOrderObjectName;

		public string SelParentOrderObjectName {
			get { return selParentOrderObjectName; }
			set { selParentOrderObjectName = value; NotifyChanged("SelParentOrderObjectName"); }
		}

		private OrderObject selParentOrderObject;
		protected OrderObject SelParentOrderObject {
			get { return selParentOrderObject; }
			set { 
				selParentOrderObject = value;
				SelParentOrderObjectName = value==null?"1 уровень":value.FullName;
				NotifyChanged("SelParentOrderObject");
			}
		}

		protected bool isNew=false;
		public EditTree() {
			
			InitializeComponent();
			refreshTrees();
			txtParent.DataContext = this;
		}


		protected void submit(SubmitOperation oper) {
			if (oper.HasError) {
				oper.MarkErrorAsHandled();
				GlobalStatus.Current.Status = "Ошибка при выполнении операции на сервере: " + oper.Error.Message;
				MessageBox.Show(oper.Error.Message, "Ошибка при выполнении операции на сервере", MessageBoxButton.OK);
				Logger.info(oper.Error.ToString());
				refresh();							
			} else {
				GlobalStatus.Current.Status = "Готово";	
			}
			objectForm.CurrentItem = null;
			treeObjects.ClearSelection();
			
		}


		protected void refreshTrees() {
			OrderObject root=(from OrderObject o in OrdersContext.Current.Context.OrderObjects where o.ObjectID == 0 select o).First();
			treeObjects.ItemsSource = new List<OrderObject>{root};			
		}

		protected void cancelEdit() {
			if (SelObject != null) {
				objectForm.CancelEdit();
				OrdersContext.Current.Context.RejectChanges();
			}
		}

		protected void refreshSelObject(){
			OrderObject selectedObject=treeObjects.SelectedItem as OrderObject;
				cancelEdit();				
			if (selectedObject != null) {
				SelObject = selectedObject;
				SelParentOrderObject = selectedObject.ParentObject;
			}
			isNew = false;
		}

		protected void saveEdit() {
			if (SelObject != null) {
				SelObject.ParentObject = SelParentOrderObject;
				SelObject.ParentObjectID = SelParentOrderObject.ObjectID;
				if (isNew) {
					OrdersContext.Current.Context.OrderObjects.Attach(SelObject);
				}
				objectForm.CommitEdit();
				OrdersContext.Current.Context.RegisterChangeObject(SelObject);
				OrdersContext.Current.Context.SubmitChanges(submit, null);
			}
		}


		private void btnSave_Click(object sender, RoutedEventArgs e) {
			saveEdit();
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e) {
			cancelEdit();
			objectForm.CurrentItem = null;
			treeObjects.ClearSelection();
		}


		private void btnChangeItem_Click(object sender, RoutedEventArgs e) {
			refreshSelObject();			
			if (SelObject != null) {				
				objectForm.CurrentItem = SelObject;
			}
		}

		private void deleteObject(OrderObject obj) {
			foreach (OrderObject child in obj.ChildObjects) {
				OrdersContext.Current.Context.OrderObjects.Detach(child);
			}
		}

		private void btnDeleteItem_Click(object sender, RoutedEventArgs e) {
			refreshSelObject();	
			if (SelObject != null) {
				
				OrdersContext.Current.Context.RegisterDeleteObject(SelObject);

				OrdersContext.Current.Context.SubmitChanges(submit, null);
				deleteObject(SelObject);
				OrdersContext.Current.Context.OrderObjects.Detach(SelObject);

			}
		}

		private void btnAddItem_Click(object sender, RoutedEventArgs e) {
			refreshSelObject();	
			OrderObject newObj=new OrderObject();
			newObj.ObjectID = CurrentObjectNumber--;
			SelParentOrderObject = SelObject;
			newObj.ParentObjectID = SelObject.ObjectID;
			objectForm.CurrentItem = newObj;
			isNew = true;
			SelObject = newObj;
		}

		private void refresh() {
			OrdersContext.Current.Context.OrderObjects.Clear();
			OrdersContext.Current.Context.Load(OrdersContext.Current.Context.LoadOrderObjectsQuery(), LoadBehavior.RefreshCurrent,
				oper => { refreshTrees(); },null);
		}

		private void btnRefresh_Click(object sender, RoutedEventArgs e) {
			refresh();
		}

		private void treeObjects_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
			if (SelObject != null) {
				OrderObject selectedObject=treeObjects.SelectedItem as OrderObject;
				if ((selectedObject != null)&&(selectedObject!=SelObject)) {
					OrderObject parent=selectedObject.ParentObject;
					bool log=true;
					while (parent != null) {
						if (parent.ObjectID == SelObject.ObjectID) {
							log = false;
						}
						parent = parent.ParentObject;						
					}
					if (log) {
						SelParentOrderObject = selectedObject;
					}
				}

			}
		}

	}
}