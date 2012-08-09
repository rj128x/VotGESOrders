using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using VotGESOrders.Views;
using VotGESOrders.Web.Models;
using System.ServiceModel.DomainServices.Client;
using System.Collections.Generic;

namespace VotGESOrders
{
	public class OrderOperations
	{
		public static int OrderNumber=-2;
		protected static OrderOperations current;

		static OrderOperations(){
			current=new OrderOperations();
		}

		protected OrderOperations() {
			
		}

		public void CreateWindows() {
			newOrderWindow = new NewOrderWindow();
			acceptWindow = new AcceptWindow();
			dateOperationWindow = new OrderDateOperationWindow();
			editOrderWindow = new EditOrderWindow();
			newOrderWindow.Closed += new EventHandler(window_Closed);
			editOrderWindow.Closed += new EventHandler(window_Closed);
			acceptWindow.Closed += new EventHandler(window_Closed);
			dateOperationWindow.Closed += new EventHandler(window_Closed);
		}

		void window_Closed(object sender, EventArgs e) {
			GlobalStatus.Current.IsChangingOrder = false;
		}

		public static OrderOperations Current {
			get {
				return current;
			}
		}

		private NewOrderWindow newOrderWindow;
		private EditOrderWindow editOrderWindow;
		private AcceptWindow acceptWindow;
		private OrderDateOperationWindow dateOperationWindow;
		
		public Order CurrentOrder { get; set; }
		
		public void ApplyDataOperation(Order currentOrder,OrderOperation operation) {
				switch (operation) {
					case OrderOperation.open:
						OrdersContext.Current.Context.RegisterOpenOrder(currentOrder,OrdersContext.Current.SessionGUID);
						break;
					case OrderOperation.close:
						OrdersContext.Current.Context.RegisterCloseOrder(currentOrder, OrdersContext.Current.SessionGUID);
						break;
					case OrderOperation.complete:
						OrdersContext.Current.Context.RegisterCompleteOrder(currentOrder, OrdersContext.Current.SessionGUID);
						break;
				}
				if ((currentOrder.OrderIsExtend||currentOrder.OrderIsFixErrorEnter) && (currentOrder.ParentOrder != null)) {
					OrdersContext.Current.Context.ReloadOrder(currentOrder.ParentOrder, OrdersContext.Current.SessionGUID);
				}
				OrdersContext.Current.SubmitChangesCallbackError();
		}



		public void ApplyAccept(Order currentOrder,AcceptResult result) {
				switch (result) {
					case AcceptResult.accept:
						currentOrder.ReviewText = currentOrder.NewComment;
						OrdersContext.Current.Context.RegisterAcceptOrder(currentOrder, OrdersContext.Current.SessionGUID);
						break;
					case AcceptResult.ban:
						currentOrder.ReviewText = currentOrder.NewComment;
						OrdersContext.Current.Context.RegisterBanOrder(currentOrder, OrdersContext.Current.SessionGUID);
						break;
					case AcceptResult.cancel:
						currentOrder.CancelText = currentOrder.NewComment;
						OrdersContext.Current.Context.RegisterCancelOrder(currentOrder, OrdersContext.Current.SessionGUID);
						break;
					case AcceptResult.comment:
						OrdersContext.Current.Context.RegisterAddComment(currentOrder, currentOrder.NewComment, OrdersContext.Current.SessionGUID);
						break;
				}
				if ((currentOrder.OrderIsExtend || currentOrder.OrderIsFixErrorEnter) && (currentOrder.ParentOrder != null)) {
					OrdersContext.Current.Context.ReloadOrder(currentOrder.ParentOrder, OrdersContext.Current.SessionGUID);
				}
				OrdersContext.Current.SubmitChangesCallbackError();
		}

		public void ApplyCreate(Order currentOrder,bool isNew,Order parentOrder) {
				if (isNew) {
					OrdersContext.Current.Context.Orders.Attach(currentOrder);
					OrdersContext.Current.Context.RegisterNew(currentOrder, OrdersContext.Current.SessionGUID);

					if (currentOrder.OrderIsExtend) {
						parentOrder.ChildOrderNumber = currentOrder.OrderNumber;
						parentOrder.OrderAskExtended = true;
						OrdersContext.Current.Context.ReloadOrder(parentOrder, OrdersContext.Current.SessionGUID);
					} else if (currentOrder.OrderIsFixErrorEnter) {
						parentOrder.ChildOrderNumber = currentOrder.OrderNumber;
						OrdersContext.Current.Context.ReloadOrder(parentOrder, OrdersContext.Current.SessionGUID);
					}

					OrdersContext.Current.SubmitChangesCallbackError();
				} else {
					OrdersContext.Current.Context.RegisterChangeOrder(currentOrder, OrdersContext.Current.SessionGUID);
					OrdersContext.Current.SubmitChangesCallbackError();
				}
				//context.Orders.Detach(newOrderWindow.CurrentOrder);
		}

		public void ApplyEdit(Order currentOrder) {
			OrdersContext.Current.Context.RegisterEditOrder(currentOrder, OrdersContext.Current.SessionGUID);
			OrdersContext.Current.SubmitChangesCallbackError();
		}

		public void RejectReviewOrder() {
			GlobalStatus.Current.IsChangingOrder = true;
			CurrentOrder.OrderOperation = OrderOperationEnum.none;
			if (MessageBox.Show("Вы уверены что хотите отозвать рассмотрение заявки?", "ОТМЕНА!", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
				OrdersContext.Current.Context.RegisterRejectReviewOrder(CurrentOrder, OrdersContext.Current.SessionGUID);
				if ((CurrentOrder.OrderIsExtend || CurrentOrder.OrderIsFixErrorEnter) && (CurrentOrder.ParentOrder != null)) {
					OrdersContext.Current.Context.ReloadOrder(CurrentOrder.ParentOrder, OrdersContext.Current.SessionGUID);
				}
				OrdersContext.Current.SubmitChangesCallbackError();
			}
		}

		public void RejectOpenOrder() {
			GlobalStatus.Current.IsChangingOrder = true;
			CurrentOrder.OrderOperation = OrderOperationEnum.none;
			if (MessageBox.Show("Вы уверены что хотите отозвать открытие заявки?", "ОТМЕНА!", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
				OrdersContext.Current.Context.RegisterRejectOpenOrder(CurrentOrder, OrdersContext.Current.SessionGUID);
				if ((CurrentOrder.OrderIsExtend || CurrentOrder.OrderIsFixErrorEnter) && (CurrentOrder.ParentOrder != null)) {
					OrdersContext.Current.Context.ReloadOrder(CurrentOrder.ParentOrder, OrdersContext.Current.SessionGUID);
				}
				OrdersContext.Current.SubmitChangesCallbackError();
			}
		}

		public void RejectCloseOrder() {
			GlobalStatus.Current.IsChangingOrder = true;
			CurrentOrder.OrderOperation = OrderOperationEnum.none;
			if (MessageBox.Show("Вы уверены что хотите отозвать разрешение на ввод?", "ОТМЕНА!", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
				OrdersContext.Current.Context.RegisterRejectCloseOrder(CurrentOrder, OrdersContext.Current.SessionGUID);
				if ((CurrentOrder.OrderIsExtend || CurrentOrder.OrderIsFixErrorEnter) && (CurrentOrder.ParentOrder != null)) {
					OrdersContext.Current.Context.ReloadOrder(CurrentOrder.ParentOrder, OrdersContext.Current.SessionGUID);
				}
				OrdersContext.Current.SubmitChangesCallbackError();
			}
		}

		public void RejectCompleteOrder() {
			GlobalStatus.Current.IsChangingOrder = true;
			CurrentOrder.OrderOperation = OrderOperationEnum.none;
			if (MessageBox.Show("Вы уверены что хотите отозвать закрытие заявки?", "ОТМЕНА!", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
				OrdersContext.Current.Context.RegisterRejectCompleteOrder(CurrentOrder, OrdersContext.Current.SessionGUID);
				if ((CurrentOrder.OrderIsExtend || CurrentOrder.OrderIsFixErrorEnter) && (CurrentOrder.ParentOrder != null)) {
					OrdersContext.Current.Context.ReloadOrder(CurrentOrder.ParentOrder, OrdersContext.Current.SessionGUID);
				}
				OrdersContext.Current.SubmitChangesCallbackError();
			}
		}

		public void RejectCancelOrder() {
			GlobalStatus.Current.IsChangingOrder = true;
			CurrentOrder.OrderOperation = OrderOperationEnum.none;
			if (MessageBox.Show("Вы уверены что хотите отозвать снятие заявки?", "ОТМЕНА!", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
				OrdersContext.Current.Context.RegisterRejectCancelOrder(CurrentOrder, OrdersContext.Current.SessionGUID);
				if ((CurrentOrder.OrderIsExtend || CurrentOrder.OrderIsFixErrorEnter) && (CurrentOrder.ParentOrder != null)) {
					OrdersContext.Current.Context.ReloadOrder(CurrentOrder.ParentOrder, OrdersContext.Current.SessionGUID);
				}
				OrdersContext.Current.SubmitChangesCallbackError();
			}
		}

		public void initCreate() {
			GlobalStatus.Current.IsChangingOrder = true;
			Order newOrder=new Order();
			newOrder.OrderOperation = OrderOperationEnum.create;
			newOrder.OrderNumber = OrderNumber--;
			newOrder.UserCreateOrderID = WebContext.Current.User.UserID;
			newOrder.OrderDateCreate = DateTime.Now;
			newOrder.OrderIsExtend = false;
			newOrder.OrderIsFixErrorEnter = false;
			newOrder.OrderType = OrderTypeEnum.pl;
			newOrder.ReadyTime = "2 часа";
			newOrderWindow.CurrentOrder = newOrder;
			newOrderWindow.IsNewOrder = true;
			
			newOrderWindow.Show();
		}

		public void initChange() {
			GlobalStatus.Current.IsChangingOrder = true;
			newOrderWindow.CurrentOrder = CurrentOrder;
			CurrentOrder.OrderOperation = OrderOperationEnum.create;
			if (CurrentOrder.ParentOrder != null) {
				newOrderWindow.ParentOrder = CurrentOrder.ParentOrder;
			}
			newOrderWindow.IsNewOrder = false;
			newOrderWindow.Show();
		}

		public void initEdit() {
			GlobalStatus.Current.IsChangingOrder = true;
			CurrentOrder.OrderOperation = OrderOperationEnum.edit;
			editOrderWindow.CurrentOrder = CurrentOrder;
			editOrderWindow.Show();
		}

		public void initAccept() {
			GlobalStatus.Current.IsChangingOrder = true;
			CurrentOrder.OrderOperation = OrderOperationEnum.review;
			acceptWindow.CurrentOrder = CurrentOrder;
			acceptWindow.isCancelWindow = false;
			acceptWindow.isCommentWindow = false;
			acceptWindow.Show();
		}

		public void initCancel() {
			GlobalStatus.Current.IsChangingOrder = true;
			CurrentOrder.OrderOperation = OrderOperationEnum.cancel;
			acceptWindow.CurrentOrder = CurrentOrder;
			acceptWindow.isCancelWindow = true;
			acceptWindow.isCommentWindow = false;
			acceptWindow.Show();
		}

		public void initComment() {
			GlobalStatus.Current.IsChangingOrder = true;
			CurrentOrder.OrderOperation = OrderOperationEnum.comment;
			acceptWindow.CurrentOrder = CurrentOrder;
			acceptWindow.isCancelWindow = false;
			acceptWindow.isCommentWindow = true;
			acceptWindow.Show();
		}

		public void initOpen() {
			GlobalStatus.Current.IsChangingOrder = true;
			CurrentOrder.OrderOperation = OrderOperationEnum.open;
			dateOperationWindow.CurrentOrder = CurrentOrder;
			dateOperationWindow.Operation = OrderOperation.open;
			dateOperationWindow.Show();
		}

		public void initClose() {
			GlobalStatus.Current.IsChangingOrder = true;
			CurrentOrder.OrderOperation = OrderOperationEnum.close;
			dateOperationWindow.CurrentOrder = CurrentOrder;
			dateOperationWindow.Operation = OrderOperation.close;
			dateOperationWindow.Show();
		}

		public void initComplete() {
			GlobalStatus.Current.IsChangingOrder = true;
			CurrentOrder.OrderOperation = OrderOperationEnum.complete;
			dateOperationWindow.CurrentOrder = CurrentOrder;
			dateOperationWindow.Operation = OrderOperation.complete;
			dateOperationWindow.Show();
		}

	
		public void initExtend() {
			GlobalStatus.Current.IsChangingOrder = true;
			Order newOrder=new Order();
			newOrder.OrderOperation = OrderOperationEnum.create;
			newOrder.OrderNumber = OrderNumber--;
			newOrder.OrderType = CurrentOrder.OrderType;
			newOrder.OrderTypeName = CurrentOrder.OrderTypeName;
			newOrder.OrderTypeShortName = CurrentOrder.OrderTypeShortName;
			newOrder.ParentOrderNumber = CurrentOrder.OrderNumber;
			newOrder.ParentOrder = CurrentOrder;
			newOrder.UserCreateOrderID = WebContext.Current.User.UserID;
			newOrder.OrderIsExtend = true;
			newOrder.SelOrderObject = CurrentOrder.SelOrderObject;
			newOrder.SelOrderObjectText = CurrentOrder.SelOrderObjectText;
			newOrder.SelOrderObjectID = CurrentOrder.SelOrderObjectID;
			newOrder.OrderObjectAddInfo = CurrentOrder.OrderObjectAddInfo;
			newOrder.PlanStartDate = CurrentOrder.PlanStopDate;
			newOrder.PlanStopDate = CurrentOrder.PlanStopDate.AddDays(1);
			newOrder.OrderText = CurrentOrder.OrderText;
			newOrder.AgreeText = CurrentOrder.AgreeText;
			newOrder.AgreeUsersIDSText = CurrentOrder.AgreeUsersIDSText;
			newOrder.AgreeUsersDict = new Dictionary<int, string>();
			foreach (KeyValuePair<int,string> de in CurrentOrder.AgreeUsersDict){
				newOrder.AgreeUsersDict.Add(de.Key, de.Value);
			}
			newOrder.ReadyTime = CurrentOrder.ReadyTime;

			if (!String.IsNullOrEmpty(CurrentOrder.CreateText))
				newOrder.CreateText =  CurrentOrder.CreateText;
			else
				newOrder.CreateText = "Работы не завершены";
			newOrder.OrderDateCreate = DateTime.Now;


			newOrderWindow.CurrentOrder = newOrder;
			newOrderWindow.ParentOrder = CurrentOrder;
			newOrderWindow.IsNewOrder = true;
			newOrderWindow.Show();
		}

		public void initCompleteWithoutEnter() {
			GlobalStatus.Current.IsChangingOrder = true;
			Order newOrder=new Order();
			newOrder.OrderOperation = OrderOperationEnum.create;
			newOrder.OrderNumber = OrderNumber--;
			newOrder.OrderType = OrderTypeEnum.crash;
			newOrder.OrderTypeName = OrderInfo.OrderTypes[OrderTypeEnum.crash];
			newOrder.OrderTypeShortName = OrderInfo.OrderTypesShort[OrderTypeEnum.crash];
			newOrder.ParentOrderNumber = CurrentOrder.OrderNumber;
			newOrder.ParentOrder = CurrentOrder;
			newOrder.UserCreateOrderID = WebContext.Current.User.UserID;
			newOrder.OrderIsFixErrorEnter = true;
			newOrder.SelOrderObject = CurrentOrder.SelOrderObject;
			newOrder.SelOrderObjectText = CurrentOrder.SelOrderObjectText;
			newOrder.SelOrderObjectID = CurrentOrder.SelOrderObjectID;
			newOrder.OrderObjectAddInfo = CurrentOrder.OrderObjectAddInfo;
			newOrder.PlanStartDate = DateTime.Now;
			newOrder.PlanStopDate = CurrentOrder.PlanStopDate.AddDays(1);
			newOrder.OrderText = CurrentOrder.OrderText;
			newOrder.AgreeText = CurrentOrder.AgreeText;
			newOrder.AgreeUsersIDSText = CurrentOrder.AgreeUsersIDSText;
			newOrder.AgreeUsersDict = new Dictionary<int, string>();
			foreach (KeyValuePair<int,string> de in CurrentOrder.AgreeUsersDict) {
				newOrder.AgreeUsersDict.Add(de.Key, de.Value);
			}
			
			newOrder.ReadyTime = "Время заявки";

			if (!String.IsNullOrEmpty(CurrentOrder.CreateText))
				newOrder.CreateText = CurrentOrder.CreateText;
			else
				newOrder.CreateText = "Невозможность ввода оборудования";

			newOrder.OrderDateCreate = DateTime.Now;


			newOrderWindow.CurrentOrder = newOrder;
			newOrderWindow.ParentOrder = CurrentOrder;
			newOrderWindow.IsNewOrder = true;
			newOrderWindow.Show();
		}


	}
}
