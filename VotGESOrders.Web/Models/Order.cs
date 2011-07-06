using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VotGESOrders.Web.Services;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Server;
using VotGESOrders.Web.ADONETEntities;
using System.Collections.ObjectModel;
using System.Data.Objects.DataClasses;
using VotGESOrders.Web.Logging;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace VotGESOrders.Web.Models
{

	public enum OrderStateEnum { created, accepted, banned, opened, canceled, closed, entered, extended, askExtended }

	/*[CustomValidation(typeof(OrderValidator), "Validate")]*/
	public class Order : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyChanged(string propName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}

		public int ParentOrderNumber { get; set; }
		private Order parentOrder;
		[Include]
		[Association("Order_ParentOrder", "ParentOrderNumber", "OrderNumber")]
		public Order ParentOrder {
			get {
				return parentOrder;
			}
			set {
				parentOrder = value;
				ParentOrderNumber = value.OrderNumber;
			}
		}


		public int ChildOrderNumber { get; set; }
		private Order childOrder;
		[Include]
		[Association("Order_ChildOrder", "ChildOrderNumber", "OrderNumber")]
		public Order ChildOrder {
			get {
				return childOrder;
			}
			set {
				childOrder = value;
				ChildOrderNumber = value.OrderNumber;
			}
		}


		private int orderNumber;
		[Key]
		public int OrderNumber {
			get { return orderNumber; }
			set { orderNumber = value; }
		}


		private string orderType;
		public string OrderType {
			get { return orderType; }
			set { orderType = value; }
		}


		public string UserCreateOrderName { get; set; }
		private OrdersUser userCreateOrder;
		[Include]
		[Association("Order_UserCreate", "UserCreateOrderName", "Name")]
		public OrdersUser UserCreateOrder {
			get { return userCreateOrder; }
			set {
				userCreateOrder = value;
				UserCreateOrderName = value.Name;
			}
		}

		public string UserAcceptOrderName { get; set; }
		private OrdersUser userAcceptOrder;
		[Include]
		[Association("Order_UserAccept", "UserAcceptOrderName", "Name")]
		public OrdersUser UserAcceptOrder {
			get { return userAcceptOrder; }
			set {
				userAcceptOrder = value;
				UserAcceptOrderName = value.Name;
			}
		}

		public string UserBanOrderName { get; set; }
		private OrdersUser userBanOrder;
		[Include]
		[Association("Order_UserBan", "UserBanOrderName", "Name")]
		public OrdersUser UserBanOrder {
			get { return userBanOrder; }
			set {
				userBanOrder = value;
				UserBanOrderName = value.Name;
			}
		}

		public string UserAcceptBanOrderName { get; set; }
		private OrdersUser userAcceptBanOrder;
		[Include]
		[Association("Order_UserAcceptBan", "UserAcceptBanOrderName", "Name")]
		public OrdersUser UserAcceptBanOrder {
			get { return userAcceptBanOrder; }
			set {
				userAcceptBanOrder = value;
				UserAcceptBanOrderName = value.Name;
			}
		}

		public string UserCloseOrderName { get; set; }
		private OrdersUser userCloseOrder;
		[Include]
		[Association("Order_UserClose", "UserCloseOrderName", "Name")]
		public OrdersUser UserCloseOrder {
			get { return userCloseOrder; }
			set {
				userCloseOrder = value;
				UserCloseOrderName = value.Name;
			}
		}

		public string UserCancelOrderName { get; set; }
		private OrdersUser userCancelOrder;
		[Include]
		[Association("Order_UserCancel", "UserCancelOrderName", "Name")]
		public OrdersUser UserCancelOrder {
			get { return userCancelOrder; }
			set {
				userCancelOrder = value;
				UserCancelOrderName = value.Name;
			}
		}

		public string UserEnterOrderName { get; set; }
		private OrdersUser userEnterOrder;
		[Include]
		[Association("Order_UserEnter", "UserEnterOrderName", "Name")]
		public OrdersUser UserEnterOrder {
			get { return userEnterOrder; }
			set {
				userEnterOrder = value;
				UserEnterOrderName = value.Name;
			}
		}

		public string UserOpenOrderName { get; set; }
		private OrdersUser userOpenOrder;
		[Include]
		[Association("Order_UserOpen", "UserOpenOrderName", "Name")]
		public OrdersUser UserOpenOrder {
			get { return userOpenOrder; }
			set {
				userOpenOrder = value;
				UserOpenOrderName = value.Name;
			}
		}

		private DateTime orderDateCreate;
		public DateTime OrderDateCreate {
			get { return orderDateCreate; }
			set { orderDateCreate = value; }
		}

		private DateTime? orderDateAccept;
		public DateTime? OrderDateAccept {
			get { return orderDateAccept; }
			set { orderDateAccept = value; }
		}

		private DateTime? orderDateBan;
		public DateTime? OrderDateBan {
			get { return orderDateBan; }
			set { orderDateBan = value; }
		}

		private DateTime? orderDateOpen;
		public DateTime? OrderDateOpen {
			get { return orderDateOpen; }
			set { orderDateOpen = value; }
		}

		private DateTime? orderDateClose;
		public DateTime? OrderDateClose {
			get { return orderDateClose; }
			set { orderDateClose = value; }
		}

		private DateTime? orderDateCancel;
		public DateTime? OrderDateCancel {
			get { return orderDateCancel; }
			set { orderDateCancel = value; }
		}

		private DateTime? orderDateEnter;
		public DateTime? OrderDateEnter {
			get { return orderDateEnter; }
			set { orderDateEnter = value; }
		}


		private DateTime planStartDate;
		[CustomValidation(typeof(OrderValidator), "ValidatePlanStartDate", ErrorMessage = "Ошибка")]
		public DateTime PlanStartDate {
			get { return planStartDate; }
			set { planStartDate = value; }
		}


		private DateTime planStopDate;
		[CustomValidation(typeof(OrderValidator), "ValidatePlanStopDate", ErrorMessage = "Ошибка")]
		public DateTime PlanStopDate {
			get { return planStopDate; }
			set { planStopDate = value; }
		}


		private DateTime? faktStartDate;
		[CustomValidation(typeof(OrderValidator), "ValidateFaktStartDate", ErrorMessage = "Ошибка")]
		[CustomValidation(typeof(OrderValidator), "ValidateFutureDate", ErrorMessage = "Ошибка")]
		public DateTime? FaktStartDate {
			get { return faktStartDate; }
			set { faktStartDate = value; }
		}


		private DateTime? faktStopDate;
		[CustomValidation(typeof(OrderValidator), "ValidateFaktStopDate", ErrorMessage = "Ошибка")]
		[CustomValidation(typeof(OrderValidator), "ValidateFutureDate", ErrorMessage = "Ошибка")]
		public DateTime? FaktStopDate {
			get { return faktStopDate; }
			set { faktStopDate = value; }
		}

		private DateTime? faktEnterDate;
		[CustomValidation(typeof(OrderValidator), "ValidateFaktEnterDate", ErrorMessage = "Ошибка")]
		[CustomValidation(typeof(OrderValidator), "ValidateFutureDate", ErrorMessage = "Ошибка")]
		public DateTime? FaktEnterDate {
			get { return faktEnterDate; }
			set { faktEnterDate = value; }
		}


		private string orderText;
		[RegularExpression(".{5,}", ErrorMessage = "Введите текст заявки")]
		[Display(Description = "Введите текст заявки (минимум 5 символов)")]
		[Required(ErrorMessage = "Обязателен текст заявки")]
		public string OrderText {
			get { return orderText; }
			set { orderText = value; }
		}

		private string soglasText;
		[RegularExpression(".{5,}", ErrorMessage = "С кем согласованно")]
		[Display(Description = "С кем согласованна заявка (минимум 5 символов)")]
		[Required(ErrorMessage = "Обязательно поле согласования")]
		public string SoglasText {
			get { return soglasText; }
			set { soglasText = value; }
		}

		private string createText;
		[Display(Description = "Дополнительный комментарий к заявке")]
		public string CreateText {
			get { return createText; }
			set { createText = value; }
		}


		private string acceptText;
		[Display(Description = "Комментарий к разрешению (не обязательно)")]
		public string AcceptText {
			get { return acceptText; }
			set { acceptText = value; }
		}

		private string banText;
		[Display(Description = "Комментарий к запрету (не обязательно)")]
		public string BanText {
			get { return banText; }
			set { banText = value; }
		}


		private string openText;
		[Display(Description = "Комментарий к выводу оборудования (не обязательно)")]
		public string OpenText {
			get { return openText; }
			set { openText = value; }
		}


		private string closeText;
		[Display(Description = "Комментарий к разрешению на ввод (не обязательно)")]
		public string CloseText {
			get { return closeText; }
			set { closeText = value; }
		}

		private string cancelText;
		[Display(Description = "Комментарий к отмене заявки (не обязательно)")]
		public string CancelText {
			get { return cancelText; }
			set { cancelText = value; }
		}

		private string enterText;
		[Display(Description = "Комментарий к вводу в работу (не обязательно)")]
		public string EnterText {
			get { return enterText; }
			set { enterText = value; }
		}

		public string FullOrderObjectInfo { get; set; }


		private string orderObjectAddInfo;
		[Display(Description = "Детализация оборудования (если отсутствует в дереве)")]
		public string OrderObjectAddInfo {
			get { return orderObjectAddInfo; }
			set {
				orderObjectAddInfo = value;
				if (value.Length > 0) {
					FullOrderObjectInfo = String.Format("{0} [{1}]", SelOrderObjectText, orderObjectAddInfo);
				} else {
					FullOrderObjectInfo = SelOrderObjectText;
				}
			}
		}



		public int SelOrderObjectID { get; set; }

		private string selOrderObjectText;
		[RegularExpression(".{1,}", ErrorMessage = "Выберите оборудование")]
		[Display(Description = "Объект оборудования (выбирается из дерева)")]
		[Required(ErrorMessage = "Обязательное поле")]
		public string SelOrderObjectText {
			get { return selOrderObjectText; }
			set { selOrderObjectText = value; }
		}



		private OrderObject selOrderObject;
		[Include]
		[Association("Order_OrderObject", "SelOrderObjectID", "ObjectID")]
		public OrderObject SelOrderObject {
			get { return selOrderObject; }
			set {
				selOrderObject = value;
				SelOrderObjectID = value.ObjectID;
				SelOrderObjectText = value.FullName;
			}
		}

		private bool orderCreated;
		public bool OrderCreated {
			get { return orderCreated; }
			set { orderCreated = value; }
		}

		private bool orderAccepted;
		public bool OrderAccepted {
			get { return orderAccepted; }
			set { orderAccepted = value; }
		}

		private bool orderBanned;
		public bool OrderBanned {
			get { return orderBanned; }
			set { orderBanned = value; }
		}


		private bool orderOpened;
		public bool OrderOpened {
			get { return orderOpened; }
			set { orderOpened = value; }
		}

		private bool orderClosed;
		public bool OrderClosed {
			get { return orderClosed; }
			set { orderClosed = value; }
		}

		private bool orderCanceled;
		public bool OrderCanceled {
			get { return orderCanceled; }
			set { orderCanceled = value; }
		}

		private bool orderEntered;
		public bool OrderEntered {
			get { return orderEntered; }
			set { orderEntered = value; }
		}

		private bool orderExtended;
		public bool OrderExtended {
			get { return orderExtended; }
			set { orderExtended = value; }
		}

		private bool orderAskExtended;
		public bool OrderAskExtended {
			get { return orderAskExtended; }
			set { orderAskExtended = value; }
		}

		private bool orderIsExtend;
		public bool OrderIsExtend {
			get { return orderIsExtend; }
			set { orderIsExtend = value; }
		}

		private bool orderHasComments;

		public bool OrderHasComments {
			get { return orderHasComments; }
			set { orderHasComments = value; }
		}


		private OrderStateEnum orderState;
		public OrderStateEnum OrderState {
			get { return orderState; }
			set { orderState = value; }
		}

		private string orderStateStr;
		public string OrderStateStr {
			get { return orderStateStr; }
			set { orderStateStr = value; }
		}

		private double? timeToOpen;
		public double? TimeToOpen {
			get { return timeToOpen; }
			set { timeToOpen = value; }
		}

		private double? timeToClose;
		public double? TimeToClose {
			get { return timeToClose; }
			set { timeToClose = value; }
		}

		private double? timeToEnter;
		public double? TimeToEnter {
			get { return timeToEnter; }
			set { timeToEnter = value; }
		}

		private bool? orderIsExpiredClose;
		public bool? OrderIsExpiredClose {
			get { return orderIsExpiredClose; }
			set { orderIsExpiredClose = value; }
		}

		private bool? orderIsExpriredEnter;
		public bool? OrderIsExpriredEnter {
			get { return orderIsExpriredEnter; }
			set { orderIsExpriredEnter = value; }
		}

		private bool? orderIsExpiredOpen;
		public bool? OrderIsExpiredOpen {
			get { return orderIsExpiredOpen; }
			set { orderIsExpiredOpen = value; }
		}

		private double? expiredOpenHours;
		public double? ExpiredOpenHours {
			get { return expiredOpenHours; }
			set { expiredOpenHours = value; }
		}

		private double? expiredCloseHours;
		public double? ExpiredCloseHours {
			get { return expiredCloseHours; }
			set { expiredCloseHours = value; }
		}

		private double? expiredEnterHours;
		public double? ExpiredEnterHours {
			get { return expiredEnterHours; }
			set { expiredEnterHours = value; }
		}
				

		public Order() {
			OrderNumber = -1;
			OrderDateCreate = DateTime.Now;
			UserCreateOrderName = HttpContext.Current.User.Identity.Name;
			ParentOrderNumber = 0;
			ChildOrderNumber = 0;
			OrderState = OrderStateEnum.created;
			OrderCreated = true;
		}

		public void refreshOrderFromDB(Orders dbOrder, OrdersUser currentUser, bool readParent = true, bool readChild = true) {
			checkPremissions(dbOrder, currentUser);

			SelOrderObject = OrderObject.getByID(dbOrder.orderObjectID);

			OrderNumber = dbOrder.orderNumber;
			OrderType = dbOrder.orderType;

			CreateText = dbOrder.createText;
			AcceptText = dbOrder.acceptText;
			BanText = dbOrder.banText;
			OpenText = dbOrder.openText;
			CloseText = dbOrder.closeText;
			EnterText = dbOrder.enterText;
			CancelText = dbOrder.cancelText;
			OrderObjectAddInfo = dbOrder.orderObjectAddInfo;

			OrderText = dbOrder.orderText;
			SoglasText = dbOrder.soglasText;

			OrderHasComments = OrderAccepted || OrderBanned || OrderCanceled;

			FaktStartDate = dbOrder.faktStartDate;
			FaktStopDate = dbOrder.faktStopDate;
			FaktEnterDate = dbOrder.faktEnterDate;
			PlanStartDate = dbOrder.planStartDate;
			PlanStopDate = dbOrder.planStopDate;

			OrderDateAccept = dbOrder.orderDateAccept;
			OrderDateClose = dbOrder.orderDateClose;
			OrderDateOpen = dbOrder.orderDateOpen;
			OrderDateCreate = dbOrder.orderDateCreate;
			OrderDateBan = dbOrder.orderDateBan;
			OrderDateEnter = dbOrder.orderDateEnter;
			OrderDateCancel = dbOrder.orderDateCancel;


			if (dbOrder.userAcceptOrderName != null) {
				UserAcceptOrder = OrdersUser.loadFromCache(dbOrder.userAcceptOrderName);
				UserAcceptBanOrder = UserAcceptOrder;
			}
			if (dbOrder.userBanOrderName != null) {
				UserBanOrder = OrdersUser.loadFromCache(dbOrder.userBanOrderName);
				UserAcceptBanOrder = UserBanOrder;
			}
			if (dbOrder.userCreateOrderName != null) {
				UserCreateOrder = OrdersUser.loadFromCache(dbOrder.userCreateOrderName);
			}
			if (dbOrder.userCloseOrderName != null) {
				UserCloseOrder = OrdersUser.loadFromCache(dbOrder.userCloseOrderName);
			}
			if (dbOrder.userCancelOrderName != null) {
				UserCancelOrder = OrdersUser.loadFromCache(dbOrder.userCancelOrderName);
			}
			if (dbOrder.userOpenOrderName != null) {
				UserOpenOrder = OrdersUser.loadFromCache(dbOrder.userOpenOrderName);
			}
			if (dbOrder.userEnterOrderName != null) {
				UserEnterOrder = OrdersUser.loadFromCache(dbOrder.userEnterOrderName);
			}

			if ((OrderExtended) || (OrderAskExtended)) {
				if (readChild) {
					ChildOrder = new Order(dbOrder.childOrderNumber.Value, currentUser, false, true);
				} else {
					ChildOrderNumber = dbOrder.childOrderNumber.Value;
				}
			} else {
				ChildOrderNumber = 0;
			}

			if (OrderIsExtend) {
				if (readParent) {
					ParentOrder = new Order(dbOrder.parentOrderNumber.Value, currentUser, true, false);
				} else {
					ParentOrderNumber = dbOrder.parentOrderNumber.Value;
				}
			} else {
				ParentOrderNumber = 0;
			}
			checkExpired();

		}

		public void checkPremissions(Orders dbOrder, OrdersUser currentUser) {
			OrderCreated = dbOrder.orderCreated;
			OrderAccepted = dbOrder.orderAccepted;
			OrderOpened = dbOrder.orderOpened;
			OrderClosed = dbOrder.orderClosed;
			OrderCanceled = dbOrder.orderCanceled;
			OrderEntered = dbOrder.orderEntered;
			OrderBanned = dbOrder.orderBanned;
			OrderExtended = dbOrder.orderExtended;
			OrderAskExtended = dbOrder.orderAskExtended;
			OrderIsExtend = dbOrder.orderIsExtend;

			OrderState = (OrderStateEnum)Enum.Parse(typeof(OrderStateEnum), dbOrder.orderState, true);
			OrderStateStr = getOrderStateStr();

			string creator=dbOrder.userCreateOrderName.ToLower();
			AllowAcceptOrder = currentUser.AllowAcceptOrder && OrderState == OrderStateEnum.created;
			AllowBanOrder = currentUser.AllowAcceptOrder && OrderState == OrderStateEnum.created;
			AllowOpenOrder = currentUser.AllowOpenOrder && OrderState == OrderStateEnum.accepted;
			AllowCloseOrder = (currentUser.Name.ToLower() == creator && OrderState == OrderStateEnum.opened) ||
				currentUser.AllowCloseOrder && OrderState == OrderStateEnum.opened;
			AllowEnterOrder = currentUser.AllowEnterOrder && OrderState == OrderStateEnum.closed;
			AllowChangeOrder = currentUser.Name.ToLower() == creator && OrderState == OrderStateEnum.created;
			AllowExtendOrder = (currentUser.AllowExtendOrder || currentUser.Name.ToLower() == creator) && OrderState == OrderStateEnum.opened;
			AllowCancelOrder = (currentUser.Name.ToLower() == creator && OrderState == OrderStateEnum.created) ||
				(currentUser.AllowCancelOrder && (OrderState == OrderStateEnum.accepted));
		}

		private void checkExpired() {
			double koef=10000000.0 * 60.0 * 60.0;
			timeToOpen =  null;
			timeToClose = null;
			timeToEnter = null;
			OrderIsExpiredClose = null;
			OrderIsExpriredEnter = null;
			OrderIsExpiredOpen = null;

			if (OrderState == OrderStateEnum.accepted || OrderState == OrderStateEnum.created) {
				timeToOpen = (PlanStartDate.Ticks - DateTime.Now.Ticks) / koef;
				timeToClose = (PlanStopDate.Ticks - DateTime.Now.Ticks) / koef;
				timeToEnter = (PlanStopDate.Ticks - DateTime.Now.Ticks) / koef;
			}
			if (OrderState == OrderStateEnum.opened) {
				timeToClose = (PlanStopDate.Ticks - DateTime.Now.Ticks) / koef;
				timeToEnter = (PlanStopDate.Ticks - DateTime.Now.Ticks) / koef;
			}			
			if (OrderState==OrderStateEnum.closed){
				timeToEnter = (PlanStopDate.Ticks - DateTime.Now.Ticks) / koef;
			}

			if (OrderOpened) {
				OrderIsExpiredOpen = PlanStartDate < FaktStartDate;
				ExpiredOpenHours = (FaktStartDate.Value.Ticks - PlanStartDate.Ticks) / koef;
			}

			if (OrderClosed) {
				OrderIsExpiredClose = PlanStopDate < FaktStopDate;
				ExpiredCloseHours = (FaktStopDate.Value.Ticks - PlanStopDate.Ticks) / koef;
			}
			if (OrderEntered) {
				OrderIsExpriredEnter = PlanStopDate < FaktEnterDate;
				ExpiredEnterHours = (FaktEnterDate.Value.Ticks - PlanStopDate.Ticks) / koef;
			}
		}



		public Order(Orders dbOrder, OrdersUser currentUser, bool readParent = true, bool readChild = true) {
			refreshOrderFromDB(dbOrder, currentUser, readParent, readChild);
		}

		protected Order(int oNumber, OrdersUser currentUser, bool readParent = true, bool readChild = true) {
			VotGESOrdersEntities context=new VotGESOrdersEntities();
			Orders orderDB=context.Orders.Where(o => o.orderNumber == oNumber).First();
			refreshOrderFromDB(orderDB, currentUser, readParent, readChild);
		}



		private string getOrderStateStr() {
			string state="";
			switch (OrderState) {
				case OrderStateEnum.accepted:
					state = "Разрешена";
					break;
				case OrderStateEnum.banned:
					state = "Отклонена";
					break;
				case OrderStateEnum.closed:
					state = "Работы завершены";
					break;
				case OrderStateEnum.created:
					state = "Создана";
					break;
				case OrderStateEnum.entered:
					state = "Закрыта";
					break;
				case OrderStateEnum.extended:
					state = "Продлена";
					break;
				case OrderStateEnum.askExtended:
					state = "Заявка на продление";
					break;
				case OrderStateEnum.opened:
					state = "Открыта";
					break;
				case OrderStateEnum.canceled:
					state = "Снята";
					break;
			}
			return state;
		}

		public bool AllowCloseOrder { get; protected set; }
		public bool AllowOpenOrder { get; protected set; }
		public bool AllowChangeOrder { get; protected set; }
		public bool AllowAcceptOrder { get; protected set; }
		public bool AllowEnterOrder { get; protected set; }
		public bool AllowBanOrder { get; protected set; }
		public bool AllowExtendOrder { get; protected set; }
		public bool AllowCancelOrder { get; protected set; }

		[Display(Description = "Комментарий (не обязательно)")]
		public string NewComment { get; set; }

	}
}