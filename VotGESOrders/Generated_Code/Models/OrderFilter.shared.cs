using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml.Serialization;


namespace VotGESOrders.Web.Models
{
	public enum FilterDateType { create, accept, ban, cancel, planStart, planStop, faktStart, faktStop, faktEnter }
	public enum FilterUserType { create, accept, ban, cancel, open, close, enter }
	public enum OrderFilterEnum { defaultFilter, active, userFilter }
	public class OrderFilter : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public void NotifyChanged(string propName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}

		public static Dictionary<FilterDateType, string> DateFilterTypes;
		public static Dictionary<FilterUserType,string> UserFilterTypes;
		public static Dictionary<OrderFilterEnum, string> FilterTypes;

		static OrderFilter() {
			DateFilterTypes = new Dictionary<FilterDateType, string>();
			DateFilterTypes.Add(FilterDateType.create, "Дата создания");
			DateFilterTypes.Add(FilterDateType.accept, "Дата разрешения");
			DateFilterTypes.Add(FilterDateType.ban, "Дата отклонения");
			DateFilterTypes.Add(FilterDateType.cancel, "Дата снятия");
			DateFilterTypes.Add(FilterDateType.planStart, "Плановое начало");
			DateFilterTypes.Add(FilterDateType.planStop, "Плановое окончание");
			DateFilterTypes.Add(FilterDateType.faktStart, "Фактическое начало");
			DateFilterTypes.Add(FilterDateType.faktStop, "Разрешение наввод");
			DateFilterTypes.Add(FilterDateType.faktEnter, "Закрытие заявки");

			UserFilterTypes = new Dictionary<FilterUserType, string>();
			UserFilterTypes.Add(FilterUserType.accept, "Разрешил");
			UserFilterTypes.Add(FilterUserType.ban, "Запретил");
			UserFilterTypes.Add(FilterUserType.cancel, "Снял");
			UserFilterTypes.Add(FilterUserType.create, "Создал");
			UserFilterTypes.Add(FilterUserType.close, "Разрешил ввод");
			UserFilterTypes.Add(FilterUserType.enter, "Закрыл");
			UserFilterTypes.Add(FilterUserType.open, "Открыл");

			FilterTypes = new Dictionary<OrderFilterEnum, string>();
			FilterTypes.Add(OrderFilterEnum.defaultFilter, "Активные заявки/Недавно измененные");
			FilterTypes.Add(OrderFilterEnum.active, "Только активные заявки");
			FilterTypes.Add(OrderFilterEnum.userFilter, "Ручная настройка");

		}

		public OrderFilter() {
			DateStart = DateTime.Now.Date.AddDays(-1).Date;
			DateEnd = DateTime.Now.Date.AddDays(1).Date;
			SelectedUsers = new ObservableCollection<OrdersUser>();
			SelectedObjects = new ObservableCollection<OrderObject>();
			ShowAllStates = true;
			ShowAllNumbers = true;
			ShowAllTypes = true;
			ShowAllUsers = true;
			ShowAllObjects = true;
			ShowAllObjectIDs = true;
			ShowAllOrderTexts = true;
			OrderText = "";
			OrderObject = "";
			FilterDate = FilterDateType.create;
			ShowChildObjects = false;
			ShowRelatedOrders = false;
			ShowExpiredOnly = false;
			FilterType = OrderFilterEnum.defaultFilter;
			FilterUser = FilterUserType.create;
		}

		[XmlIgnore]
		public ObservableCollection<OrdersUser> SelectedUsers { get; set; }
		[XmlIgnore]
		public string SelectedUsersJoinStr {
			get {
				return String.Join("~", from u in SelectedUsers select u.UserID);
			}
		}

		[XmlIgnore]
		public ObservableCollection<OrderObject> SelectedObjects { get; set; }
		[XmlIgnore]
		public string SelectedObjectsJoinStr {
			get {
				return String.Join("~", from o in SelectedObjects select o.ObjectID);
			}
		}


		private bool showOrdersCreated;
		[XmlAttribute]
		public bool ShowOrdersCreated {
			get { return showOrdersCreated; }
			set { showOrdersCreated = value; NotifyChanged("ShowOrdersCreated"); }
		}
		private bool showOrdersAccepted;
		[XmlAttribute]
		public bool ShowOrdersAccepted {
			get { return showOrdersAccepted; }
			set { showOrdersAccepted = value; NotifyChanged("ShowOrdersAccepted"); }
		}
		private bool showOrdersBanned;
		[XmlAttribute]
		public bool ShowOrdersBanned {
			get { return showOrdersBanned; }
			set { showOrdersBanned = value; NotifyChanged("showOrdersBanned"); }
		}
		private bool showOrdersOpened;
		[XmlAttribute]
		public bool ShowOrdersOpened {
			get { return showOrdersOpened; }
			set { showOrdersOpened = value; NotifyChanged("ShowOrdersOpened"); }
		}
		private bool showOrdersCanceled;
		[XmlAttribute]
		public bool ShowOrdersCanceled {
			get { return showOrdersCanceled; }
			set { showOrdersCanceled = value; NotifyChanged("ShowOrdersCanceled"); }
		}
		private bool showOrdersClosed;
		[XmlAttribute]
		public bool ShowOrdersClosed {
			get { return showOrdersClosed; }
			set { showOrdersClosed = value; NotifyChanged("ShowOrdersClosed"); }
		}
		private bool showOrdersCompleted;
		[XmlAttribute]
		public bool ShowOrdersCompleted {
			get { return showOrdersCompleted; }
			set { showOrdersCompleted = value; NotifyChanged("ShowOrdersCompleted"); }
		}
		private bool showOrdersExtended;
		[XmlAttribute]
		public bool ShowOrdersExtended {
			get { return showOrdersExtended; }
			set { showOrdersExtended = value; NotifyChanged("ShowOrdersExtended"); }
		}
		private bool showOrdersAskExtended;
		[XmlAttribute]
		public bool ShowOrdersAskExtended {
			get { return showOrdersAskExtended; }
			set { showOrdersAskExtended = value; NotifyChanged("ShowOrdersAskExtended"); }
		}
		private bool showOrdersIsExtend;
		[XmlAttribute]
		public bool ShowOrdersIsExtend {
			get { return showOrdersIsExtend; }
			set { showOrdersIsExtend = value; NotifyChanged("ShowOrdersIsExtend"); }
		}

		private bool showOrdersAV;
		[XmlAttribute]
		public bool ShowOrdersAV {
			get { return showOrdersAV; }
			set { showOrdersAV = value; NotifyChanged("ShowOrdersAV"); }
		}
		private bool showOrdersNO;
		[XmlAttribute]
		public bool ShowOrdersNO {
			get { return showOrdersNO; }
			set { showOrdersNO = value; NotifyChanged("ShowOrdersNO"); }
		}
		private bool showOrdersNPL;
		[XmlAttribute]
		public bool ShowOrdersNPL {
			get { return showOrdersNPL; }
			set { showOrdersNPL = value; NotifyChanged("ShowOrdersNPL"); }
		}
		private bool showOrdersPL;
		[XmlAttribute]
		public bool ShowOrdersPL {
			get { return showOrdersPL; }
			set { showOrdersPL = value; NotifyChanged("ShowOrdersPL"); }
		}

		private bool showAllStates;
		[XmlAttribute]
		public bool ShowAllStates {
			get { return showAllStates; }
			set { showAllStates = value; NotifyChanged("ShowAllStates"); }
		}
		private bool showAllNumbers;
		[XmlAttribute]
		public bool ShowAllNumbers {
			get { return showAllNumbers; }
			set { showAllNumbers = value; NotifyChanged("ShowAllNumbers"); }
		}
		private bool showAllTypes;
		[XmlAttribute]
		public bool ShowAllTypes {
			get { return showAllTypes; }
			set { showAllTypes = value; NotifyChanged("ShowAllTypes"); }
		}
		private bool showAllTime;
		[XmlAttribute]
		public bool ShowAllTime {
			get { return showAllTime; }
			set { showAllTime = value; NotifyChanged("ShowAllTime"); }
		}
		private bool showAllObjects;
		[XmlAttribute]
		public bool ShowAllObjects {
			get { return showAllObjects; }
			set { showAllObjects = value; NotifyChanged("ShowAllObjects"); }
		}
		private bool showAllObjectIDs;
		[XmlAttribute]
		public bool ShowAllObjectIDs {
			get { return showAllObjectIDs; }
			set { showAllObjectIDs = value; NotifyChanged("ShowAllObjectIDs"); }
		}
		private bool showAllUsers;
		[XmlAttribute]
		public bool ShowAllUsers {
			get { return showAllUsers; }
			set { showAllUsers = value; NotifyChanged("ShowAllUsers"); }
		}
		private bool showAllOrderTexts;
		[XmlAttribute]
		public bool ShowAllOrderTexts {
			get { return showAllOrderTexts; }
			set { showAllOrderTexts = value; NotifyChanged("ShowAllOrderTexts"); }
		}

		private bool showExpiredOnly;
		public bool ShowExpiredOnly {
			get { return showExpiredOnly; }
			set { showExpiredOnly = value; NotifyChanged("ShowExpiredOnly"); }
		}


		private int startNumber;
		[XmlAttribute]
		public int StartNumber {
			get { return startNumber; }
			set { startNumber = value; NotifyChanged("StartNumber"); }
		}
		private int stopNumber;
		[XmlAttribute]
		public int StopNumber {
			get { return stopNumber; }
			set { stopNumber = value; NotifyChanged("StopNumber"); }
		}

		private string orderObject;
		[XmlAttribute]
		public string OrderObject {
			get { return orderObject; }
			set { orderObject = value; NotifyChanged("OrderObject"); }
		}
		private string orderText;
		[XmlAttribute]
		public string OrderText {
			get { return orderText; }
			set { orderText = value; NotifyChanged("OrderText"); }
		}

		private DateTime dateStart;
		[XmlAttribute]
		public DateTime DateStart {
			get { return dateStart; }
			set { dateStart = value; NotifyChanged("DateStart"); }
		}
		private DateTime dateEnd;
		[XmlAttribute]
		public DateTime DateEnd {
			get { return dateEnd; }
			set { dateEnd = value; NotifyChanged("DateEnd"); }
		}

		private String selectedUsersStr;
		[XmlAttribute]
		public String SelectedUsersStr {
			get { return selectedUsersStr; }
			set { selectedUsersStr = value; NotifyChanged("SelectedUsersStr"); }
		}

		private String selectedObjectsStr;
		[XmlAttribute]
		public String SelectedObjectsStr {
			get { return selectedObjectsStr; }
			set { selectedObjectsStr = value; NotifyChanged("SelectedObjectsStr"); }
		}


		private bool showRelatedOrders;
		[XmlAttribute]
		public bool ShowRelatedOrders {
			get { return showRelatedOrders; }
			set { showRelatedOrders = value; NotifyChanged("ShowRelatedOrders"); }
		}
		private bool showChildObjects;
		[XmlAttribute]
		public bool ShowChildObjects {
			get { return showChildObjects; }
			set { showChildObjects = value; NotifyChanged("ShowChildObjects"); }
		}

		private FilterDateType filterDate;
		[XmlAttribute]
		public FilterDateType FilterDate {
			get { return filterDate; }
			set { filterDate = value; NotifyChanged("FilterDate"); }
		}

		private FilterUserType filterUser;
		[XmlAttribute]
		public FilterUserType FilterUser {
			get { return filterUser; }
			set { filterUser = value; NotifyChanged("FilterUser"); }
		}
		private OrderFilterEnum filterType;
		[XmlAttribute]
		public OrderFilterEnum FilterType {
			get { return filterType; }
			set { filterType = value; NotifyChanged("FilterType"); }
		}

		[XmlIgnore]
		public List<String> OrderStates {
			get {
				List<String> orderStates=new List<string>();
				if (ShowOrdersCreated)
					orderStates.Add(OrderStateEnum.created.ToString());
				if (ShowOrdersAccepted)
					orderStates.Add(OrderStateEnum.accepted.ToString());
				if (ShowOrdersBanned)
					orderStates.Add(OrderStateEnum.banned.ToString());
				if (ShowOrdersOpened)
					orderStates.Add(OrderStateEnum.opened.ToString());
				if (ShowOrdersCanceled)
					orderStates.Add(OrderStateEnum.canceled.ToString());
				if (ShowOrdersClosed)
					orderStates.Add(OrderStateEnum.closed.ToString());
				if (ShowOrdersCompleted)
					orderStates.Add(OrderStateEnum.completed.ToString());
				if (ShowOrdersExtended)
					orderStates.Add(OrderStateEnum.extended.ToString());
				if (ShowOrdersAskExtended)
					orderStates.Add(OrderStateEnum.askExtended.ToString());

				return orderStates;
			}
		}

		[XmlIgnore]
		public List<String> OrderTypes {
			get {
				List<String> orderTypes=new List<string>();
				if (ShowOrdersAV)
					orderTypes.Add("АВ");
				if (ShowOrdersNO)
					orderTypes.Add("НО");
				if (ShowOrdersNPL)
					orderTypes.Add("НПЛ");
				if (ShowOrdersPL)
					orderTypes.Add("ПЛ");

				return orderTypes;
			}
		}

		[XmlIgnore]
		public List<int> SelectedUsersArray {
			get {
				try {
					List<String> lst=new List<String>(SelectedUsersStr.Split('~'));
					List<int> ids=new List<int>();
					foreach (string id in lst) {
						ids.Add(Int32.Parse(id));
					}
					return ids;
				} catch {
					return new List<int>();
				}
			}
		}

		[XmlIgnore]
		public List<int> SelectedObjectsArray {
			get {
				try {
					List<String> lst=new List<String>(SelectedObjectsStr.Split('~'));
					List<int> ids=new List<int>();
					foreach (string id in lst) {
						ids.Add(Int32.Parse(id));
					}
					return ids;
				} catch {
					return new List<int>();
				}
			}
		}




	}
}