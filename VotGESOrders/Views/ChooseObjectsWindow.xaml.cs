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
using System.Collections.ObjectModel;

namespace VotGESOrders.Views
{
	public partial class ChooseObjectsWindow : ChildWindow
	{
		public OrderFilter CurrentFilter{get;set;}
		protected List<OrderObject> prevSel;
		public ChooseObjectsWindow() {
			InitializeComponent();
			treeObjects.ItemsSource=from OrderObject o in OrdersContext.Current.Context.OrderObjects where o.ObjectID==0 select o;
			
		}

		protected override void OnOpened() {
			LayoutRoot.DataContext = CurrentFilter;
			prevSel = CurrentFilter.SelectedObjects.ToList();
		}

		private void OKButton_Click(object sender, RoutedEventArgs e) {
			this.DialogResult = true;
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e) {
			CurrentFilter.SelectedObjects.Clear();
			foreach (OrderObject obj in prevSel) {
				CurrentFilter.SelectedObjects.Add(obj);
			}
			this.DialogResult = false;
		}

		private void ListBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			OrderObject obj= lstSelectedObjects.SelectedItem as OrderObject;
			if (obj != null) {
				CurrentFilter.SelectedObjects.Remove(obj);
			}
		}

		private void treeObjects_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
			OrderObject obj= treeObjects.SelectedItem as OrderObject;
			if ((obj!=null)&&(!CurrentFilter.SelectedObjects.Contains(obj))){
				CurrentFilter.SelectedObjects.Add(obj);
			}
		}
	}
}

