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

		protected static int CurrentObjectNumber=-1;
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
				SelParentOrderObjectName = value == null ? "1 уровень" : value.FullName;
				NotifyChanged("SelParentOrderObject");
			}
		}

		protected bool isNew=false;
		public EditTree() {
			
			InitializeComponent();
			refreshTrees();
			txtParent.DataContext = this;
			OrdersContext.Current.Context.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Context_PropertyChanged);
		}

		void Context_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			if (e.PropertyName == "IsLoading") {
				if (!OrdersContext.Current.Context.IsLoading) {
					refreshTrees();
					objectForm.CurrentItem = null;
					treeObjects.ClearSelection();
				}
			}
			if (e.PropertyName == "IsSubmitting") {
				if (!OrdersContext.Current.Context.IsSubmitting) {
					objectForm.CurrentItem = null;
					treeObjects.ClearSelection();
				}
			}
		}


		protected void refreshTrees() {
			treeObjects.ItemsSource = (from OrderObject o in OrdersContext.Current.Context.OrderObjects where o.ParentObjectID==0 select o).ToList();			
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
				SelObject.ParentObjectID = SelParentOrderObject==null?0:SelParentOrderObject.ObjectID;
				if (isNew) {
					OrdersContext.Current.Context.OrderObjects.Attach(SelObject);
				}
				objectForm.CommitEdit();				
				OrdersContext.Current.Context.RegisterChangeObject(SelObject);
				OrdersContext.Current.SubmitChangesCallbackError();
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

				OrdersContext.Current.SubmitChangesCallbackError();
				deleteObject(SelObject);
				OrdersContext.Current.Context.OrderObjects.Detach(SelObject);

			}
		}

		private void btnAddItem_Click(object sender, RoutedEventArgs e) {
			refreshSelObject();	
			OrderObject newObj=new OrderObject();
			newObj.ObjectID = CurrentObjectNumber--;
			if (SelObject != null) {
				SelParentOrderObject = SelObject;
				newObj.ParentObjectID = SelObject.ObjectID;
				
			} else {
				SelParentOrderObject = null;
				newObj.ParentObjectID = 0;
			}
			objectForm.CurrentItem = newObj;
			isNew = true;
			SelObject = newObj;
		}

		private void btnFirstLevel_Click(object sender, RoutedEventArgs e) {
			if (SelObject != null) {
				SelParentOrderObject = null;
			}
		}

		private void btnRefresh_Click(object sender, RoutedEventArgs e) {
			OrdersContext.Current.Context.OrderObjects.Clear();
			OrdersContext.Current.Context.Load(OrdersContext.Current.Context.LoadOrderObjectsQuery(), LoadBehavior.RefreshCurrent, false);
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