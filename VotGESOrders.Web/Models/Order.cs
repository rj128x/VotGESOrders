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
	public enum OrderStateEnum { created, accepted, banned, opened, canceled, closed, completed, completedWithoutEnter, extended, askExtended }
	public enum OrderTypeEnum { pl, npl, no, crash }
	

	/*[CustomValidation(typeof(OrderValidator), "Validate")]*/
	public class Order : INotifyPropertyChanged
	{
		public static string OrderCommentsDelim="=======================================";
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyChanged(string propName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}



		private double parentOrderNumber;
		public double ParentOrderNumber {
			get { return parentOrderNumber; }
			set { 
				parentOrderNumber = value;
			}
		}
		
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

		private double childOrderNumber;
		public double ChildOrderNumber {
			get { return childOrderNumber; }
			set { 
				childOrderNumber = value;
			}
		}
		
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


		private double orderNumber;
		[Key]
		public double OrderNumber {
			get { return orderNumber; }
			set { 
				orderNumber = value;
			}
		}

		


		private OrderTypeEnum orderType;
		public OrderTypeEnum OrderType {
			get { return orderType; }
			set { 
				orderType = value;
				OrderTypeShortName = OrderInfo.OrderTypesShort[orderType];
				OrderTypeName = OrderInfo.OrderTypes[orderType];
			}
		}

		private string orderTypeShortName;
		public string OrderTypeShortName {
			get { return orderTypeShortName; }
			set { orderTypeShortName = value; }
		}

		private string orderTypeName;
		public string OrderTypeName {
			get { return orderTypeName; }
			set { orderTypeName = value; }
		}


		public int UserCreateOrderID { get; set; }
		private OrdersUser userCreateOrder;
		[Include]
		[Association("Order_UserCreate", "UserCreateOrderID", "UserID")]
		public OrdersUser UserCreateOrder {
			get { return userCreateOrder; }
			set {
				userCreateOrder = value;
				UserCreateOrderID = value.UserID;
			}
		}

		public int UserReviewOrderID { get; set; }
		private OrdersUser userReviewOrder;
		[Include]
		[Association("Order_UserReview", "UserReviewOrderID", "UserID")]
		public OrdersUser UserReviewOrder {
			get { return userReviewOrder; }
			set {
				userReviewOrder = value;
				UserReviewOrderID = value.UserID;
			}
		}
				

		public int UserCloseOrderID { get; set; }
		private OrdersUser userCloseOrder;
		[Include]
		[Association("Order_UserClose", "UserCloseOrderID", "UserID")]
		public OrdersUser UserCloseOrder {
			get { return userCloseOrder; }
			set {
				userCloseOrder = value;
				UserCloseOrderID = value.UserID;
			}
		}

		public int UserCancelOrderID { get; set; }
		private OrdersUser userCancelOrder;
		[Include]
		[Association("Order_UserCancel", "UserCancelOrderID", "UserID")]
		public OrdersUser UserCancelOrder {
			get { return userCancelOrder; }
			set {
				userCancelOrder = value;
				UserCancelOrderID = value.UserID;
			}
		}

		public int UserCompleteOrderID { get; set; }
		private OrdersUser userCompleteOrder;
		[Include]
		[Association("Order_UserEnter", "UserCompleteOrderID", "UserID")]
		public OrdersUser UserCompleteOrder {
			get { return userCompleteOrder; }
			set {
				userCompleteOrder = value;
				UserCompleteOrderID = value.UserID;
			}
		}

		public int UserOpenOrderID { get; set; }
		private OrdersUser userOpenOrder;
		[Include]
		[Association("Order_UserOpen", "UserOpenOrderID", "UserID")]
		public OrdersUser UserOpenOrder {
			get { return userOpenOrder; }
			set {
				userOpenOrder = value;
				UserOpenOrderID = value.UserID;
			}
		}

		private DateTime orderDateCreate;
		public DateTime OrderDateCreate {
			get { return orderDateCreate; }
			set { orderDateCreate = value; }
		}

		private DateTime? orderDateReview;
		public DateTime? OrderDateReview {
			get { return orderDateReview; }
			set { orderDateReview = value; }
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

		private DateTime? orderDateComplete;
		public DateTime? OrderDateComplete {
			get { return orderDateComplete; }
			set { orderDateComplete = value; }
		}


		private DateTime planStartDate;		
		[CustomValidation(typeof(OrderValidator), "ValidatePlanStartDate", ErrorMessage = "Ошибка")]
		[Display(ShortName="Плановое начало")]
		public DateTime PlanStartDate {
			get { return planStartDate; }
			set { planStartDate = value; }
		}


		private DateTime planStopDate;
		[CustomValidation(typeof(OrderValidator), "ValidatePlanStopDate", ErrorMessage = "Ошибка")]
		[Display(ShortName = "Плановое окончание")]
		public DateTime PlanStopDate {
			get { return planStopDate; }
			set { planStopDate = value; }
		}


		private DateTime? faktStartDate;
		[CustomValidation(typeof(OrderValidator), "ValidateFaktStartDate", ErrorMessage = "Ошибка")]
		[Display(ShortName = "Фактическое начало")]
		public DateTime? FaktStartDate {
			get { return faktStartDate; }
			set { faktStartDate = value; }
		}


		private DateTime? faktStopDate;
		[CustomValidation(typeof(OrderValidator), "ValidateFaktStopDate", ErrorMessage = "Ошибка")]
		[Display(ShortName = "Разрешение на ввод")]
		public DateTime? FaktStopDate {
			get { return faktStopDate; }
			set { faktStopDate = value; }
		}

		private DateTime? faktCompleteDate;
		[CustomValidation(typeof(OrderValidator), "ValidateFaktCompleteDate", ErrorMessage = "Ошибка")]
		[Display(ShortName = "Ввод в работу")]
		public DateTime? FaktCompleteDate {
			get { return faktCompleteDate; }
			set { faktCompleteDate = value; }
		}


		private string orderText;
		[RegularExpression(".{5,}", ErrorMessage = "Текст заявки - Минимум 5 символов")]
		[Display(Description = "Введите текст заявки (минимум 5 символов)",ShortName="Текст заявки")]
		[StringLength(250, ErrorMessage = "Текст заявки - Максимум 250 символов")]
		[Required(ErrorMessage = "Текст заявки - обязательное поле")]
		public string OrderText {
			get { return orderText; }
			set { orderText = value; }
		}

		private string agreeText;
		[RegularExpression(".{1,}", ErrorMessage = "Согласование")]
		[Display(Description = "С кем согласована заявка", ShortName = "Согласование")]
		[StringLength(250, ErrorMessage = "Согласование - Максимум 250 символов")]
		[Required(ErrorMessage = "Согласование - обязательное поле")]
		public string AgreeText {
			get { return agreeText; }
			set { agreeText = value; }
		}

		
		private List<OrdersUser> agreeUsers;
		public List<OrdersUser> AgreeUsers {
			get { return agreeUsers; }
			set { agreeUsers = value; }
		}

		private Dictionary<int,string>agreeUsersDict;
		[DataMember]
		public Dictionary<int, string> AgreeUsersDict {
			get { return agreeUsersDict; }
			set { agreeUsersDict = value; }
		}

		private string agreeUsersIDSText;
		public string AgreeUsersIDSText {
			get { return agreeUsersIDSText; }
			set { agreeUsersIDSText = value; }
		}

		private void refreshAgreeUsers() {
			AgreeUsers = new List<OrdersUser>();
			AgreeUsersDict = new Dictionary<int, string>();
			try {
				string[] ids=AgreeUsersIDSText.Split(';');
				foreach (string id in ids) {
					try {
						OrdersUser user=OrdersUser.loadFromCache(Int32.Parse(id));
						AgreeUsers.Add(user);
						AgreeUsersDict.Add(user.UserID, user.FullName);
					} catch { }
				}
			} catch { }
		}



		private string createText;
		[Display(Description = "Дополнительный комментарий к заявке", ShortName = "Комментарий")]
		[StringLength(250, ErrorMessage = "Комментарий - Максимум 250 символов")]
		public string CreateText {
			get { return createText; }
			set { createText = value; }
		}


		private string reviewText;
		[Display(Description = "Комментарий к разрешению (не обязательно)", ShortName = "Комментарий")]
		[StringLength(250, ErrorMessage = "Комментарий - Максимум 250 символов")]
		public string ReviewText {
			get { return reviewText; }
			set { reviewText = value; }
		}
		

		private string openText;
		[Display(Description = "Комментарий к выводу оборудования (не обязательно)", ShortName = "Комментарий")]
		[StringLength(250, ErrorMessage = "Комментарий - Максимум 250 символов")]
		public string OpenText {
			get { return openText; }
			set { openText = value; }
		}


		private string closeText;
		[Display(Description = "Комментарий к разрешению на ввод (не обязательно)", ShortName = "Комментарий")]
		[StringLength(250, ErrorMessage = "Комментарий - Максимум 250 символов")]
		public string CloseText {
			get { return closeText; }
			set { closeText = value; }
		}

		private string cancelText;
		[Display(Description = "Комментарий к отмене заявки (не обязательно)", ShortName = "Комментарий")]
		[StringLength(250, ErrorMessage = "Комментарий - Максимум 250 символов")]
		public string CancelText {
			get { return cancelText; }
			set { cancelText = value; }
		}

		private string completeText;
		[Display(Description = "Комментарий к закрытию заявки (не обязательно)", ShortName = "Комментарий")]
		[StringLength(250, ErrorMessage = "Комментарий - Максимум 250 символов")]
		public string CompleteText {
			get { return completeText; }
			set { completeText = value; }
		}
		
		public string FullOrderObjectInfo { get; set; }


		private string orderObjectAddInfo;
		[Display(Description = "Детализация оборудования (если отсутствует в дереве)", ShortName = "Оборудование")]
		[StringLength(100, ErrorMessage = "Оборудование - Максимум 100 символов")]
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
		[Display(Description = "Объект оборудования (выбирается из дерева)", ShortName = "Оборудование")]
		[Required(ErrorMessage = "Оборудование - Обязательное поле")]
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


		private string readyTime;
		[Display(Description = "Аварийная готовность", ShortName = "Аварийная готовность")]
		[StringLength(50, ErrorMessage = "Аварийная готовность - Максимум 50 символов")]
		[RegularExpression(".{5,}", ErrorMessage = "Аварийная готовность - Минимум 5 символов")]
		[Required(ErrorMessage = "Аварийная готовность - Обязательное поле")]
		public string ReadyTime {
			get { return readyTime; }
			set {	readyTime = value;}
		}

		private bool orderCreated;
		public bool OrderCreated {
			get { return orderCreated; }
			set { orderCreated = value; }
		}

		private bool orderReviewed;
		public bool OrderReviewed {
			get { return orderReviewed; }
			set { orderReviewed = value; }
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

		private bool orderCompleted;
		public bool OrderCompleted {
			get { return orderCompleted; }
			set { orderCompleted = value; }
		}

		private bool orderCompletedWithoutEnter;
		public bool OrderCompletedWithoutEnter {
			get { return orderCompletedWithoutEnter; }
			set { orderCompletedWithoutEnter = value; }
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

		private bool orderIsFixErrorEnter;
		public bool OrderIsFixErrorEnter {
			get { return orderIsFixErrorEnter; }
			set { orderIsFixErrorEnter = value; }
		}

		private bool orderHasChildOrder;
		public bool OrderHasChildOrder {
			get { return orderHasChildOrder; }
			set { orderHasChildOrder = value; }
		}


		private bool orderHasParentOrder;
		public bool OrderHasParentOrder {
			get { return orderHasParentOrder; }
			set { orderHasParentOrder = value; }
		}

		private OrderStateEnum orderState;
		public OrderStateEnum OrderState {
			get { return orderState; }
			set { 
				orderState = value;
				orderStateStr = OrderInfo.OrderStates[orderState];
			}
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

		private bool? orderIsExpriredComplete;
		public bool? OrderIsExpriredComplete {
			get { return orderIsExpriredComplete; }
			set { orderIsExpriredComplete = value; }
		}

		private bool? orderIsExpiredOpen;
		public bool? OrderIsExpiredOpen {
			get { return orderIsExpiredOpen; }
			set { orderIsExpiredOpen = value; }
		}

		private bool? orderIsExpiredReglament;
		public bool? OrderIsExpiredReglament {
			get { return orderIsExpiredReglament; }
			set { orderIsExpiredReglament = value; }
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

		private double? expiredCompleteHours;
		public double? ExpiredCompleteHours {
			get { return expiredCompleteHours; }
			set { expiredCompleteHours = value; }
		}

		private double? expiredReglamentHours;
		public double? ExpiredReglamentHours {
			get { return expiredReglamentHours; }
			set { expiredReglamentHours = value; }
		}

		private String commentsText;
		public String CommentsText {
			get { return commentsText; }
			set { 
				commentsText = value;
				CommentsTextBrief = commentsText;
				
			}
		}

		private String commentsTextBrief;
		public String CommentsTextBrief {
			get { return commentsTextBrief; }
			set { 
				commentsTextBrief = value;
				if (!String.IsNullOrEmpty(commentsTextBrief)) {
					commentsTextBrief=commentsTextBrief.Replace(OrderCommentsDelim + "\n", "\n");
					while (commentsTextBrief.IndexOf("=\n") > 1)
						commentsTextBrief=commentsTextBrief.Replace("=\n", "\n"); 
					commentsTextBrief=commentsTextBrief.Replace("\n\n", "\n");					
				}
			}
		}
				

		public Order() {
			OrderNumber = -1;
			OrderDateCreate = DateTime.Now;
			UserCreateOrderID = OrdersUser.loadFromCache(HttpContext.Current.User.Identity.Name).UserID;
			ParentOrderNumber = 0;
			ChildOrderNumber = 0;
			OrderState = OrderStateEnum.created;
			OrderCreated = true;			
		}

		public void refreshOrderFromDB(Orders dbOrder, OrdersUser currentUser, bool readRelated, List<Order> listOrders) {
			if (listOrders != null) {
				if (!listOrders.Contains(this)) {
					listOrders.Add(this);
				}
			}
			
			checkPremissions(dbOrder, currentUser);

			SelOrderObject = OrderObject.getByID(dbOrder.orderObjectID);

			OrderNumber = dbOrder.orderNumber;
			OrderType = (OrderTypeEnum)Enum.Parse(typeof(OrderTypeEnum), dbOrder.orderType, true);
			ReadyTime = dbOrder.readyTime;

			CreateText = dbOrder.createText;
			ReviewText = dbOrder.reviewText;
			OpenText = dbOrder.openText;
			CloseText = dbOrder.closeText;
			CompleteText = dbOrder.completeText;
			CancelText = dbOrder.cancelText;
			OrderObjectAddInfo = dbOrder.orderObjectAddInfo;

			OrderText = dbOrder.orderText;
			AgreeText = dbOrder.agreeText;
			AgreeUsersIDSText = dbOrder.agreeUsersIDS;
			refreshAgreeUsers();
			
			FaktStartDate = dbOrder.faktStartDate;
			FaktStopDate = dbOrder.faktStopDate;
			FaktCompleteDate = dbOrder.faktCompleteDate;
			PlanStartDate = dbOrder.planStartDate;
			PlanStopDate = dbOrder.planStopDate;

			OrderDateReview = dbOrder.orderDateReview;
			OrderDateClose = dbOrder.orderDateClose;
			OrderDateOpen = dbOrder.orderDateOpen;
			OrderDateCreate = dbOrder.orderDateCreate;
			OrderDateComplete = dbOrder.orderDateComplete;
			OrderDateCancel = dbOrder.orderDateCancel;

			ExpiredReglamentHours = dbOrder.expiredReglamentHours;
			ExpiredOpenHours = dbOrder.expiredOpenHours;
			ExpiredCompleteHours = dbOrder.expiredCompleteHours;
			ExpiredCloseHours = dbOrder.expiredCloseHours;

			OrderIsExpiredClose = ExpiredCloseHours.HasValue && ExpiredCloseHours.Value < 0;
			OrderIsExpiredOpen = ExpiredOpenHours.HasValue && ExpiredOpenHours.Value < 0;
			OrderIsExpriredComplete = ExpiredCompleteHours.HasValue && ExpiredCompleteHours.Value < 0;
			OrderIsExpiredReglament = ExpiredReglamentHours.HasValue && ExpiredReglamentHours.Value < 0;


			CommentsText = dbOrder.commentsText;
			HasComments = !String.IsNullOrEmpty(CommentsText) || !String.IsNullOrEmpty(CreateText);


			UserCreateOrder = OrdersUser.loadFromCache(dbOrder.userCreateOrderID);

			if (dbOrder.userReviewOrderID != null) {
				UserReviewOrder = OrdersUser.loadFromCache(dbOrder.userReviewOrderID.Value);
			}
			if (dbOrder.userCloseOrderID != null) {
				UserCloseOrder = OrdersUser.loadFromCache(dbOrder.userCloseOrderID.Value);
			}
			if (dbOrder.userCancelOrderID != null) {
				UserCancelOrder = OrdersUser.loadFromCache(dbOrder.userCancelOrderID.Value);
			}
			if (dbOrder.userOpenOrderID != null) {
				UserOpenOrder = OrdersUser.loadFromCache(dbOrder.userOpenOrderID.Value);
			}
			if (dbOrder.userCompleteOrderID != null) {
				UserCompleteOrder = OrdersUser.loadFromCache(dbOrder.userCompleteOrderID.Value);
			}

			if (OrderExtended || OrderAskExtended ||OrderCompletedWithoutEnter) {
				if (readRelated) {
					ChildOrder = GetOrder(dbOrder.childOrderNumber.Value, currentUser, readRelated, listOrders);
				} else {
					ChildOrderNumber = dbOrder.childOrderNumber.Value;
				}
			} else {
				ChildOrderNumber = 0;
			}

			if (OrderIsExtend||OrderIsFixErrorEnter) {
				if (readRelated) {
					ParentOrder = GetOrder(dbOrder.parentOrderNumber.Value, currentUser, readRelated, listOrders);
				} else {
					ParentOrderNumber = dbOrder.parentOrderNumber.Value;
				}
			} else {
				ParentOrderNumber = 0;
			}
			OrderHasChildOrder = ChildOrderNumber > 0;
			OrderHasParentOrder = ParentOrderNumber > 0;
			checkTimeToOpen();	
		}



		public void checkPremissions(Orders dbOrder, OrdersUser currentUser) {
			OrderCreated = dbOrder.orderCreated;
			OrderReviewed = dbOrder.orderReviewed;
			OrderOpened = dbOrder.orderOpened;
			OrderClosed = dbOrder.orderClosed;
			OrderCanceled = dbOrder.orderCanceled;
			OrderCompleted = dbOrder.orderCompleted;
			OrderCompletedWithoutEnter = dbOrder.orderCompletedWithoutEnter;
			OrderExtended = dbOrder.orderExtended;
			OrderAskExtended = dbOrder.orderAskExtended;
			OrderIsExtend = dbOrder.orderIsExtend;
			OrderIsFixErrorEnter = dbOrder.orderIsFixErrorEnter;
						
			OrderState = (OrderStateEnum)Enum.Parse(typeof(OrderStateEnum), dbOrder.orderState, true);
			OrderType = (OrderTypeEnum)Enum.Parse(typeof(OrderTypeEnum), dbOrder.orderType, true);

			int creator=dbOrder.userCreateOrderID;
			AllowReviewOrder = currentUser.AllowReviewOrder && OrderState == OrderStateEnum.created;
			AllowOpenOrder = currentUser.AllowChangeOrder && OrderState == OrderStateEnum.accepted;
			AllowCloseOrder = (currentUser.UserID == creator && OrderState == OrderStateEnum.opened) ||
				currentUser.AllowChangeOrder && OrderState == OrderStateEnum.opened;
			AllowCompleteWithoutEnterOrder = currentUser.AllowChangeOrder && currentUser.AllowCreateCrashOrder && OrderState == OrderStateEnum.closed;
			AllowCompleteOrder = currentUser.AllowChangeOrder && OrderState == OrderStateEnum.closed;
			AllowChangeOrder = (currentUser.UserID == creator || currentUser.AllowChangeOrder) && OrderState == OrderStateEnum.created||
				(currentUser.AllowEditOrders && (OrderType==OrderTypeEnum.no||OrderType==OrderTypeEnum.crash)&&OrderState==OrderStateEnum.opened);
			AllowExtendOrder = (currentUser.AllowChangeOrder || currentUser.UserID == creator) && OrderState == OrderStateEnum.opened;
			AllowCancelOrder = ((currentUser.UserID == creator || currentUser.AllowChangeOrder) && OrderState == OrderStateEnum.created) ||
				(currentUser.AllowChangeOrder && (OrderState == OrderStateEnum.accepted));


			string[] ids= dbOrder.agreeUsersIDS.Split(';');
			AllowCommentOrder = true;

			AllowRejectReviewOrder = (currentUser.AllowEditOrders||currentUser.AllowReviewOrder) && 
				(OrderState == OrderStateEnum.accepted || OrderState == OrderStateEnum.banned && !OrderIsExtend || 
				OrderState==OrderStateEnum.opened && OrderIsExtend);
			AllowRejectOpenOrder = currentUser.AllowEditOrders && OrderState == OrderStateEnum.opened && !OrderIsExtend && !OrderIsFixErrorEnter && 
				!OrderExtended && !OrderAskExtended &&
				OrderType != OrderTypeEnum.crash && OrderType != OrderTypeEnum.no;
			AllowRejectCloseOrder = currentUser.AllowEditOrders && OrderState == OrderStateEnum.closed;
			AllowRejectCancelOrder = currentUser.AllowEditOrders && OrderState == OrderStateEnum.canceled && !OrderIsExtend;
			AllowRejectCompleteOrder = currentUser.AllowEditOrders && OrderState == OrderStateEnum.completed;
			AllowEditOrder = currentUser.AllowEditOrders;
		}

		private void checkTimeToOpen(){
			double koef=10000000.0 * 60.0 * 60.0;
			timeToOpen = null;
			timeToClose = null;
			timeToEnter = null;
			if (OrderState == OrderStateEnum.accepted || OrderState == OrderStateEnum.created) {
				timeToOpen = (PlanStartDate.Ticks - DateTime.Now.Ticks) / koef;
				timeToClose = (PlanStopDate.Ticks - DateTime.Now.Ticks) / koef;
				timeToEnter = (PlanStopDate.Ticks - DateTime.Now.Ticks) / koef;
			}
			if (OrderState == OrderStateEnum.opened) {
				timeToClose = (PlanStopDate.Ticks - DateTime.Now.Ticks) / koef;
				timeToEnter = (PlanStopDate.Ticks - DateTime.Now.Ticks) / koef;
			}
			if (OrderState == OrderStateEnum.closed) {
				timeToEnter = (PlanStopDate.Ticks - DateTime.Now.Ticks) / koef;
			}
		}

		public static void writeExpired(Orders orderDB) {
			double koef=10000000.0 * 60.0 * 60.0;

			if (orderDB.orderOpened&&orderDB.faktStartDate.HasValue) {
				orderDB.expiredOpenHours = (orderDB.planStartDate.Ticks - orderDB.faktStartDate.Value.Ticks) / koef;
			}

			if (orderDB.orderClosed && orderDB.faktStopDate.HasValue) {
				orderDB.expiredCloseHours = (orderDB.planStopDate.Ticks - orderDB.faktStopDate.Value.Ticks) / koef;
			}
			if (orderDB.orderCompleted && orderDB.faktCompleteDate.HasValue) {
				orderDB.expiredCompleteHours = (orderDB.planStopDate.Ticks - orderDB.faktCompleteDate.Value.Ticks) / koef;
			}


			DateTime needCreate=orderDB.orderDateCreate;
			if (!orderDB.orderIsExtend) {
				if (orderDB.orderType == OrderTypeEnum.npl.ToString() || orderDB.orderType == OrderTypeEnum.pl.ToString()) {
					if (orderDB.planStartDate.DayOfWeek == DayOfWeek.Monday) {
						needCreate = orderDB.planStartDate.AddDays(-3).Date.AddHours(15);
					} else if (orderDB.planStartDate.DayOfWeek == DayOfWeek.Sunday) {
						needCreate = orderDB.planStartDate.AddDays(-2).Date.AddHours(15);
					} else if (orderDB.planStartDate.DayOfWeek == DayOfWeek.Saturday) {
						needCreate = orderDB.planStartDate.AddDays(-1).Date.AddHours(15);
					} 
					else {
						needCreate = orderDB.planStartDate.AddDays(-1).Date.AddHours(15);
					}
				} else {
					needCreate = orderDB.planStartDate.AddHours(24);
				}
			}
			orderDB.expiredReglamentHours = (needCreate.Ticks - orderDB.orderDateCreate.Ticks) / koef;
		}


		public Order(Orders dbOrder, OrdersUser currentUser, bool readRelated, List<Order> listOrders) {
			refreshOrderFromDB(dbOrder, currentUser, readRelated,listOrders);			
		}


		public static Order GetOrder(double oNumber, OrdersUser currentUser, bool readRelated, List<Order> listOrders){
			if (listOrders != null) {
				try{
					return (from Order o in listOrders where o.OrderNumber == oNumber select o).First();
				}
				catch{}
			} 
			Order newOrder=new Order();
			VotGESOrdersEntities context=new VotGESOrdersEntities();
			Orders orderDB=context.Orders.Where(o => o.orderNumber == oNumber).First();
			newOrder.refreshOrderFromDB(orderDB, currentUser, readRelated, listOrders);			
			return newOrder;			
		}

		public bool HasComments { get; set; }

		public bool AllowCloseOrder { get; protected set; }
		public bool AllowOpenOrder { get; protected set; }
		public bool AllowChangeOrder { get; protected set; }
		public bool AllowReviewOrder { get; protected set; }
		public bool AllowCompleteOrder { get; protected set; }
		public bool AllowCompleteWithoutEnterOrder { get; protected set; }
		public bool AllowExtendOrder { get; protected set; }
		public bool AllowCancelOrder { get; protected set; }
		public bool AllowCommentOrder { get; protected set; }

		public bool AllowEditOrder { get; protected set; }
		public bool ManualEdit { get; set; }
		public bool AllowRejectReviewOrder { get; protected set; }
		public bool AllowRejectOpenOrder { get; protected set; }
		public bool AllowRejectCloseOrder { get; protected set; }
		public bool AllowRejectCompleteOrder { get; protected set; }
		public bool AllowRejectCancelOrder { get; protected set; }		

		[Display(Description = "Комментарий (не обязательно)")]
		public string NewComment { get; set; }

	}
}