using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VotGESOrders.Web.Services;
using VotGESOrders.Web.Logging;
using VotGESOrders.Web.ADONETEntities;
using System.Data.Objects;
using System.ServiceModel.DomainServices.Server;

namespace VotGESOrders.Web.Models
{
	public class OrdersContext
	{
		protected List<Order> orders;		

		public OrdersContext() {
			CurrentUser = OrdersUser.loadFromCache(HttpContext.Current.User.Identity.Name);
			AutoUser = OrdersUser.loadFromCache("auto");
		}

		public IQueryable<Order> getOrders(OrderFilter filter) {
			switch (filter.FilterType) {
				case OrderFilterEnum.userFilter:
					return getOrdersUserFilter(filter);
				case OrderFilterEnum.active:
					return OrdersActive;
				default:
					return Orders;
			}

		}

		#region Queries
		public IQueryable<Order> Orders {
			get {
				try {
					Logger.info("Получение списка заказов (по умолчанию)", Logger.LoggerSource.ordersContext);
					OrdersUser currentUser=OrdersUser.loadFromCache(HttpContext.Current.User.Identity.Name);
					VotGESOrdersEntities context=new VotGESOrdersEntities();
					DateTime lastDate=DateTime.Now.AddDays(-1);

					List<string> states=new List<string>();
					states.Add(OrderStateEnum.accepted.ToString());
					states.Add(OrderStateEnum.created.ToString());
					states.Add(OrderStateEnum.closed.ToString());
					states.Add(OrderStateEnum.opened.ToString());
					states.Add(OrderStateEnum.askExtended.ToString());

					//IQueryable<Orders> orders=context.Orders.Where(order => (!order.faktStopDate.HasValue)||(order.faktStopDate.HasValue&&order.faktStopDate.Value>lastDate));
					IQueryable<Orders> orders=context.Orders.Where(order => states.Contains(order.orderState) ||
						(order.orderLastUpdate > lastDate));
					List<Order> resultOrders=new List<Order>();
					foreach (Orders orderDB in orders) {
						resultOrders.Add(new Order(orderDB, currentUser, false, null));
					}					
					return resultOrders.AsQueryable();
				} catch (Exception e) {
					Logger.error("===Ошибка при получении списка заказов (по умолчанию)" + e.ToString(), Logger.LoggerSource.ordersContext);
					throw new DomainException(String.Format("Ошибка при получении списка заказов (по умолчанию)"));
				}
			}
		}

		public IQueryable<Order> OrdersActive {
			get {
				try {
					Logger.info("Получение списка активных заказов", Logger.LoggerSource.ordersContext);
					OrdersUser currentUser=OrdersUser.loadFromCache(HttpContext.Current.User.Identity.Name);
					VotGESOrdersEntities context=new VotGESOrdersEntities();

					List<string> states=new List<string>();
					states.Add(OrderStateEnum.accepted.ToString());
					states.Add(OrderStateEnum.created.ToString());
					states.Add(OrderStateEnum.closed.ToString());
					states.Add(OrderStateEnum.opened.ToString());
					states.Add(OrderStateEnum.askExtended.ToString());

					//IQueryable<Orders> orders=context.Orders.Where(order => (!order.faktStopDate.HasValue)||(order.faktStopDate.HasValue&&order.faktStopDate.Value>lastDate));
					IQueryable<Orders> orders=
					from Orders o in context.Orders
					where
						states.Contains(o.orderState)
					select o;
					List<Order> resultOrders=new List<Order>();
					foreach (Orders orderDB in orders) {
						resultOrders.Add(new Order(orderDB, currentUser,false,null));
					}
					return resultOrders.AsQueryable();
				} catch (Exception e) {
					Logger.error("===Ошибка при получении списка заказов (активные)" + e.ToString(), Logger.LoggerSource.ordersContext);
					throw new DomainException(String.Format("Ошибка при получении списка заказов (активные)"));
				}

			}
		}

		public IQueryable<Order> OrdersActiveExpired {
			get {
				try {
					Logger.info("Получение списка просроченных заказов", Logger.LoggerSource.ordersContext);
					OrdersUser currentUser=new OrdersUser();
					VotGESOrdersEntities context=new VotGESOrdersEntities();

					List<string> states1=new List<string>();
					List<string> states2=new List<string>();
					states1.Add(OrderStateEnum.accepted.ToString());
					states1.Add(OrderStateEnum.created.ToString());
					states2.Add(OrderStateEnum.closed.ToString());
					states2.Add(OrderStateEnum.opened.ToString());
					states2.Add(OrderStateEnum.askExtended.ToString());




					//IQueryable<Orders> orders=context.Orders.Where(order => (!order.faktStopDate.HasValue)||(order.faktStopDate.HasValue&&order.faktStopDate.Value>lastDate));
					IQueryable<Orders> orders=
					from Orders o in context.Orders
					where
						(states1.Contains(o.orderState) && o.planStartDate < DateTime.Now) ||
						(states2.Contains(o.orderState) && o.planStopDate < DateTime.Now)
					select o;
					List<Order> resultOrders=new List<Order>();
					foreach (Orders orderDB in orders) {
						resultOrders.Add(new Order(orderDB, currentUser, false, null));
					}
					return resultOrders.AsQueryable();
				} catch (Exception e) {
					Logger.error("===Ошибка при получении списка заказов (просроченных)" + e.ToString(), Logger.LoggerSource.ordersContext);
					throw new DomainException(String.Format("Ошибка при получении списка заказов (просроченных)"));
				}

			}
		}


		public IQueryable<Order> getOrdersUserFilter(OrderFilter filter) {
			Logger.info("Получение списка заказов (фильтр)", Logger.LoggerSource.ordersContext);
			try {
				OrdersUser currentUser=OrdersUser.loadFromCache(HttpContext.Current.User.Identity.Name);
				VotGESOrdersEntities context=new VotGESOrdersEntities();

				List<String> states=filter.OrderStates;
				List<int> users=filter.SelectedUsersArray;
				List<String> orderTypes=filter.OrderTypes;

				List<int> ids=filter.SelectedObjectsArray;
				if (!filter.ShowAllObjectIDs && filter.ShowChildObjects) {
					List<OrderObject> objects=new List<OrderObject>();
					foreach (int id in ids) {
						objects.Add(VotGESOrders.Web.Models.OrderObject.getByID(id));
					}
					foreach (OrderObject obj in objects) {
						obj.appendObjectIDSChildIDS(ids);
					}
				}

				List<int> objectsByName=new List<int>();
				if (!filter.ShowAllObjects) {
					objectsByName = OrderObject.getObjectIDSByFullName(filter.OrderObject);
				}

				IQueryable<Orders> orders= 
				from o in context.Orders
				where
					(filter.ShowAllStates || states.Contains(o.orderState))
					&& (filter.ShowAllTime ||
						(filter.FilterDate == FilterDateType.create && o.orderDateCreate >= filter.DateStart && o.orderDateCreate <= filter.DateEnd) ||
						(filter.FilterDate == FilterDateType.review && o.orderDateReview >= filter.DateStart && o.orderDateReview <= filter.DateEnd) ||
						(filter.FilterDate == FilterDateType.cancel && o.orderDateCancel >= filter.DateStart && o.orderDateCancel <= filter.DateEnd) ||
						(filter.FilterDate == FilterDateType.faktStart && o.faktStartDate >= filter.DateStart && o.faktStartDate <= filter.DateEnd) ||
						(filter.FilterDate == FilterDateType.faktStop && o.faktStopDate >= filter.DateStart && o.faktStopDate <= filter.DateEnd) ||
						(filter.FilterDate == FilterDateType.faktEnter && o.faktCompleteDate >= filter.DateStart && o.faktCompleteDate <= filter.DateEnd) ||
						(filter.FilterDate == FilterDateType.planStart && o.planStartDate >= filter.DateStart && o.planStartDate <= filter.DateEnd) ||
						(filter.FilterDate == FilterDateType.planStop && o.planStopDate >= filter.DateStart && o.planStopDate <= filter.DateEnd))
					&& (filter.ShowAllUsers ||
						(filter.FilterUser == FilterUserType.create && users.Contains(o.userCreateOrderID)) ||
						(filter.FilterUser == FilterUserType.review && users.Contains(o.userReviewOrderID.Value)) ||
						(filter.FilterUser == FilterUserType.cancel && users.Contains(o.userCancelOrderID.Value)) ||
						(filter.FilterUser == FilterUserType.open && users.Contains(o.userOpenOrderID.Value)) ||
						(filter.FilterUser == FilterUserType.close && users.Contains(o.userCloseOrderID.Value)) ||
						(filter.FilterUser == FilterUserType.complete && users.Contains(o.userCompleteOrderID.Value)))
					&& (filter.ShowAllObjectIDs || ids.Contains(o.orderObjectID))
					&& (filter.ShowAllTypes || orderTypes.Contains(o.orderType))
					&& (filter.ShowAllObjects || objectsByName.Contains(o.orderObjectID) || o.orderObjectAddInfo.Contains(filter.OrderObject))
					&& (filter.ShowAllOrderTexts || o.orderText.Contains(filter.OrderText))
					&& (filter.ShowAllNumbers ||
						(o.orderNumber >= filter.StartNumber || filter.StartNumber == 0) &&
						(o.orderNumber <= filter.StopNumber || filter.StopNumber == 0))
					&& (!filter.ShowExpiredOnly ||
						 o.orderClosed && o.planStopDate < o.faktStopDate ||
						 o.orderCompleted && o.planStopDate < o.faktCompleteDate)
				select o;

				List<Order> resultOrders=new List<Order>();
				foreach (Orders orderDB in orders) {
					resultOrders.Add(new Order(orderDB, currentUser, false, null));
				}

				if (filter.ShowRelatedOrders) {
					IEnumerable<double> floorNumbers=from Order o in resultOrders select Math.Floor(o.OrderNumber);
					IEnumerable<double> numbers=from Order o in resultOrders select o.OrderNumber;
					IQueryable<Orders> relOrders=from o in context.Orders where floorNumbers.Contains(Math.Floor(o.orderNumber)) && !numbers.Contains(o.orderNumber) select o;
					foreach (Orders orderDB in relOrders) {
						resultOrders.Add(new Order(orderDB, currentUser, false, null));
					}
				}


				return resultOrders.AsQueryable();
			} catch (Exception e) {
				Logger.error("===Ошибка при получении списка заказов (фильтр) " + e.ToString(), Logger.LoggerSource.ordersContext);
				throw new DomainException(String.Format("Ошибка при получении списка заказов (фильтр)"));
			}
		}

		#endregion

		private OrdersUser currentUser;
		public OrdersUser CurrentUser {
			get {
				return currentUser;
			}
			set {
				currentUser = value;
			}
		}

		private OrdersUser autoUser;
		public OrdersUser AutoUser {
			get {
				return autoUser;
			}
			set {
				autoUser = value;
			}
		}

		protected void writeOrderToOrderDB(Order order, Orders orderDB) {
			orderDB.orderText = order.OrderText;
			orderDB.agreeText = order.AgreeText;
			orderDB.createText = order.CreateText;
			orderDB.planStartDate = order.PlanStartDate;
			orderDB.planStopDate = order.PlanStopDate;
			orderDB.orderType = order.OrderType.ToString();
			orderDB.orderObjectID = order.SelOrderObjectID;
			orderDB.orderObjectAddInfo = order.OrderObjectAddInfo;
			orderDB.readyTime = order.ReadyTime;
			orderDB.agreeUsersIDS = order.AgreeUsersIDSText;
		}


		public void RegisterOrder(Order order, Guid guid) {
			Logger.info("Пользователь создал/изменил заявку", Logger.LoggerSource.ordersContext);
			try {
				VotGESOrdersEntities context=new VotGESOrdersEntities();

				VotGESOrders.Web.ADONETEntities.Orders orderDB=null;
				bool isNew=false;
				try {
					orderDB = context.Orders.First(o => o.orderNumber == order.OrderNumber);
					if (orderDB == null) 
						isNew = true;
				}catch{
					isNew=true;
				}

				if (!isNew) {
					Order tempOrder=new Order(orderDB, currentUser, false, null);
					tempOrder.checkPremissions(orderDB, currentUser);
					if (!tempOrder.AllowChangeOrder) {
						throw new DomainException("Нельзя изменить заявку");
					}
				} else {
					orderDB=new Orders();
					orderDB.userCreateOrderID = currentUser.UserID;

					double maxNumber=1;
					double newNumber=1;
					try {
						if (order.OrderIsExtend || order.OrderIsFixErrorEnter) {
							maxNumber = context.Orders.Where(o => Math.Floor(o.orderNumber) == Math.Floor(order.ParentOrderNumber)).Max(o => o.orderNumber);
							newNumber = maxNumber + 0.01;
						} else {
							maxNumber = context.Orders.Max(o => o.orderNumber);
							maxNumber = Math.Floor(maxNumber);
							newNumber = maxNumber + 1;
						}
					} catch { }
					orderDB.orderNumber = newNumber;
					orderDB.orderDateCreate = DateTime.Now;
				}
				orderDB.orderLastUpdate = DateTime.Now;				
				orderDB.orderCreated = true;
				orderDB.orderState = OrderStateEnum.created.ToString();

				orderDB.orderReviewed = false;
				orderDB.orderOpened = false;
				orderDB.reviewText = null;
				orderDB.openText = null;
				orderDB.orderDateReview = null;
				orderDB.orderDateOpen = null;
				orderDB.faktStartDate = null;
				orderDB.userOpenOrderID = null;
				orderDB.userReviewOrderID = null;

				writeOrderToOrderDB(order, orderDB);

				if ((order.OrderType == OrderTypeEnum.crash || order.OrderType==OrderTypeEnum.no) && !order.OrderIsExtend) {
					Logger.info("===Аварийная/неотложная заявка", Logger.LoggerSource.ordersContext);
					orderDB.orderReviewed = true;
					orderDB.orderOpened = true;
					orderDB.orderDateReview = DateTime.Now;
					orderDB.orderDateOpen = DateTime.Now;
					orderDB.userReviewOrderID = AutoUser.UserID;
					orderDB.userOpenOrderID = AutoUser.UserID;
					orderDB.reviewText = order.OrderType == OrderTypeEnum.crash?"Аварийная заявка":"Неотложная заявка";
					orderDB.openText = order.OrderType == OrderTypeEnum.crash ? "Аварийная заявка" : "Неотложная заявка";
					orderDB.faktStartDate = order.PlanStartDate;
					orderDB.orderState = OrderStateEnum.opened.ToString();
				}						

				if (order.OrderIsExtend) {
					Logger.info("===Продленная заявка", Logger.LoggerSource.ordersContext);
					Orders parentOrderDB=context.Orders.Where(o => o.orderNumber == order.ParentOrderNumber).First();

					Order parentOrder=new Order(parentOrderDB, currentUser, false, null);
					parentOrder.checkPremissions(parentOrderDB, currentUser);

					if (!parentOrder.AllowExtendOrder && isNew) {
						throw new DomainException("Заявка уже продлена");
					}

					if (isNew) {
						parentOrderDB.orderDateComplete = DateTime.Now;
						parentOrderDB.userCompleteOrderID = currentUser.UserID;
					}

					orderDB.userCreateOrderID = parentOrderDB.userCreateOrderID;
					
					parentOrderDB.orderLastUpdate = DateTime.Now;
					parentOrderDB.orderAskExtended = true;
					parentOrderDB.orderState = OrderStateEnum.askExtended.ToString();
					parentOrderDB.orderCompleted = true;					
					parentOrderDB.completeText = order.CreateText;
					parentOrderDB.faktCompleteDate = order.PlanStartDate;					

					orderDB.orderIsExtend = true;

					orderDB.parentOrderNumber = order.ParentOrderNumber;
					parentOrderDB.childOrderNumber = orderDB.orderNumber;

					if (isNew) {
						MailContext.sendMail(String.Format("Заявка №{0}. Продление заявки ({1})", order.ParentOrderNumber.ToString(OrderInfo.NFI), CurrentUser.FullName),
							new Order(parentOrderDB, currentUser, false, null), false, false);
					}
				}

				if (order.OrderIsFixErrorEnter) {
					Logger.info("===Заявка закрыта без ввода оборудования", Logger.LoggerSource.ordersContext);
					Orders parentOrderDB=context.Orders.Where(o => o.orderNumber == order.ParentOrderNumber).First();

					Order parentOrder=new Order(parentOrderDB, currentUser, false, null);
					parentOrder.checkPremissions(parentOrderDB, currentUser);

					if (!parentOrder.AllowCompleteWithoutEnterOrder&&isNew) {
						throw new DomainException("Заявка уже закрыта");
					}

					orderDB.userCreateOrderID = parentOrderDB.userCreateOrderID;

					if (isNew) {
						parentOrderDB.orderDateComplete = DateTime.Now;
						parentOrderDB.userCompleteOrderID = currentUser.UserID;
					}

					parentOrderDB.orderLastUpdate = DateTime.Now;
					parentOrderDB.orderCompletedWithoutEnter = true;
					parentOrderDB.orderCompleted = true;					
					parentOrderDB.completeText = order.CreateText;
					parentOrderDB.orderState = OrderStateEnum.completedWithoutEnter.ToString();
					parentOrderDB.faktCompleteDate = order.PlanStartDate;
					orderDB.orderIsFixErrorEnter = true;

					orderDB.parentOrderNumber = order.ParentOrderNumber;
					parentOrderDB.childOrderNumber = orderDB.orderNumber;

					if (isNew) {
						MailContext.sendMail(String.Format("Заявка №{0}. Заявка закрыта без ввода оборудования ({1})",
							order.ParentOrderNumber.ToString(OrderInfo.NFI), CurrentUser.FullName),
							new Order(parentOrderDB, currentUser, false, null), false, false);
					}
				}

				if (isNew) {
					context.Orders.AddObject(orderDB);
				}
				context.SaveChanges();

				Logger.info("===Сохранено", Logger.LoggerSource.ordersContext);		

				LastUpdate.save(guid);
				order.refreshOrderFromDB(orderDB, currentUser,false,null);
				MailContext.sendMail(String.Format(isNew ? "Заявка №{0}. Создана новая заявка ({1})" : "Заявка №{0}. Заявка изменена ({1})", 
					order.OrderNumber.ToString(OrderInfo.NFI), CurrentUser.FullName), 
					order, true, false);
			} catch (Exception e) {
				Logger.error(String.Format("===Ошибка при создании/изменении заявки: {0}", e), Logger.LoggerSource.ordersContext);
				if (e is DomainException) {
					throw e;
				}
				throw new DomainException("Ошибка при создании/изменении заявки");
			}
		}

		public void ChangeOrder(Order order, Guid guid) {
			Logger.info("Пользователь изменил заявку №" + order.OrderNumber.ToString(OrderInfo.NFI), Logger.LoggerSource.ordersContext);
			try {
				RegisterOrder(order, guid);
			} catch (Exception e) {
				Logger.error(String.Format("===Ошибка при изменении заявки №{1}: {0}", e, order.OrderNumber.ToString(OrderInfo.NFI)), Logger.LoggerSource.ordersContext);
				if (e is DomainException) {
					throw e;
				}
				throw new DomainException(String.Format("Ошибка при изменении заявки №{0}", order.OrderNumber.ToString(OrderInfo.NFI)));
			}
		}


		public void AcceptOrder(Order order, Guid guid) {
			Logger.info("Пользователь разрешил заявку №" + order.OrderNumber.ToString(OrderInfo.NFI), Logger.LoggerSource.ordersContext);
			try {
				VotGESOrdersEntities context=new VotGESOrdersEntities();
				Orders orderDB=context.Orders.First(o => o.orderNumber == order.OrderNumber);
				order.checkPremissions(orderDB, currentUser);
				if (order.AllowReviewOrder) {
					orderDB.orderLastUpdate = DateTime.Now;
					orderDB.orderDateReview = DateTime.Now;
					orderDB.userReviewOrderID = currentUser.UserID;
					orderDB.reviewText = order.ReviewText;
					orderDB.orderReviewed = true;
					
					orderDB.orderState = OrderStateEnum.accepted.ToString();

					if (order.OrderIsExtend) {
						Logger.info("===Продленная заявка", Logger.LoggerSource.ordersContext);
						Orders parentOrderDB=context.Orders.Where(o => o.orderNumber == order.ParentOrderNumber).First();
						
						orderDB.orderOpened = true;
						orderDB.openText = "Оборудование выведено. Заявка продлена";
						orderDB.userOpenOrderID = AutoUser.UserID;
						orderDB.orderState = OrderStateEnum.opened.ToString();
						orderDB.orderDateOpen = DateTime.Now;
						orderDB.faktStartDate = parentOrderDB.planStopDate;
						parentOrderDB.orderLastUpdate = DateTime.Now;
						parentOrderDB.orderExtended = true;
						parentOrderDB.orderAskExtended = false;
						parentOrderDB.orderState = OrderStateEnum.extended.ToString();
						MailContext.sendMail(String.Format("Заявка №{0}. Заявка продлена ({1})", order.ParentOrderNumber.ToString(OrderInfo.NFI), CurrentUser.FullName), 
							new Order(parentOrderDB, currentUser, false, null), false, false);
					}
					context.SaveChanges();
					LastUpdate.save(guid);
					Logger.info("===Заявка разрешена. Заявка №" + order.OrderNumber.ToString(OrderInfo.NFI), Logger.LoggerSource.ordersContext);
				} else {
					throw new DomainException("Нельзя разрешить заявку");
				}
				order.refreshOrderFromDB(orderDB, currentUser, false, null);
				MailContext.sendMail(String.Format("Заявка №{0}. Заявка разрешена ({1})", orderDB.orderNumber.ToString(OrderInfo.NFI), CurrentUser.FullName), 
					order, false, false);
			} catch (Exception e) {
				Logger.error(String.Format("===Ошибка при разрешении заявки №{1}: {0}", e, order.OrderNumber.ToString(OrderInfo.NFI)), Logger.LoggerSource.ordersContext);
				if (e is DomainException) {
					throw e;
				}
				throw new DomainException(String.Format("Ошибка при разрешении заявки №{0}", order.OrderNumber.ToString(OrderInfo.NFI)));
			}
		}

		public void BanOrder(Order order, Guid guid) {
			Logger.info("Пользователь запретил заявку  №" + order.OrderNumber.ToString(OrderInfo.NFI), Logger.LoggerSource.ordersContext);
			try {
				VotGESOrdersEntities context=new VotGESOrdersEntities();
				Orders orderDB=context.Orders.First(o => o.orderNumber == order.OrderNumber);
				order.checkPremissions(orderDB, currentUser);
				if (order.AllowReviewOrder) {
					orderDB.orderLastUpdate = DateTime.Now;
					orderDB.orderDateReview = DateTime.Now;
					orderDB.userReviewOrderID = currentUser.UserID;
					orderDB.reviewText = order.ReviewText;
					orderDB.orderReviewed = true;
					orderDB.orderState = OrderStateEnum.banned.ToString();

					if (order.OrderIsExtend) {
						Logger.info("===Продленная заявка", Logger.LoggerSource.ordersContext);
						Orders parentOrderDB=context.Orders.Include("Users").Where(o => o.orderNumber == order.ParentOrderNumber).First();
						parentOrderDB.orderLastUpdate = DateTime.Now;
						parentOrderDB.orderExtended = false;
						parentOrderDB.orderAskExtended = false;
						parentOrderDB.orderOpened = true;
						parentOrderDB.orderState = OrderStateEnum.opened.ToString();
						parentOrderDB.childOrderNumber = null;

						parentOrderDB.orderCompleted = false;
						parentOrderDB.orderDateComplete = null;
						parentOrderDB.completeText = null;
						parentOrderDB.faktCompleteDate = null;
						parentOrderDB.userCompleteOrderID = null;

						MailContext.sendMail(String.Format("Заявка №{0}. Продление заявки отклонено ({1})", order.ParentOrderNumber.ToString(OrderInfo.NFI),CurrentUser.FullName), 
							new Order(parentOrderDB, currentUser, false, null), false, false);
					}

					context.SaveChanges();
					LastUpdate.save(guid);
					Logger.info("===Заявка запрещена. Заявка №" + order.OrderNumber.ToString(OrderInfo.NFI), Logger.LoggerSource.ordersContext);

				} else {
					throw new DomainException("Нельзя отклонить заявку");
				}
				order.refreshOrderFromDB(orderDB, currentUser, false, null);
				MailContext.sendMail(String.Format("Заявка №{0}. Заявка отклонена ({1})", orderDB.orderNumber.ToString(OrderInfo.NFI), CurrentUser.FullName), 
					order, false, false);
			} catch (Exception e) {
				Logger.error(String.Format("===Ошибка при запрещении заявки №{1}: {0}", e, order.OrderNumber.ToString(OrderInfo.NFI)), Logger.LoggerSource.ordersContext);
				if (e is DomainException) {
					throw e;
				}
				throw new DomainException(String.Format("Ошибка при запрете заявки №{0}", order.OrderNumber.ToString(OrderInfo.NFI)));
			}
		}


		public void RejectReviewOrder(Order order, Guid guid) {
			Logger.info("Пользователь отменил рассмотрение заявки  №" + order.OrderNumber.ToString(OrderInfo.NFI), Logger.LoggerSource.ordersContext);
			try {
				VotGESOrdersEntities context=new VotGESOrdersEntities();
				Orders orderDB=context.Orders.First(o => o.orderNumber == order.OrderNumber);
				order.checkPremissions(orderDB, currentUser);
				if (order.AllowRejectReviewOrder) {
					orderDB.orderLastUpdate = DateTime.Now;
					orderDB.orderDateReview = null;
					orderDB.orderDateOpen = null;
					orderDB.faktStartDate = null;
					orderDB.userOpenOrderID = null;
					orderDB.orderOpened = false;
					orderDB.userReviewOrderID = null;
					orderDB.reviewText = null;
					orderDB.orderReviewed = false;
					orderDB.orderState = OrderStateEnum.created.ToString();

					if (order.OrderIsExtend) {
						Logger.info("===Продленная заявка", Logger.LoggerSource.ordersContext);
						Orders parentOrderDB=context.Orders.Include("Users").Where(o => o.orderNumber == order.ParentOrderNumber).First();
						parentOrderDB.orderLastUpdate = DateTime.Now;
						parentOrderDB.orderExtended = false;
						parentOrderDB.orderAskExtended = true;
						parentOrderDB.orderState = OrderStateEnum.askExtended.ToString();
						addComment(parentOrderDB, "Отмена рассмотрения заявки на продление");
						
						MailContext.sendMail(String.Format("Заявка №{0}. Отмена рассмотрения заявки на продление ({1})", 
							order.ParentOrderNumber.ToString(OrderInfo.NFI), CurrentUser.FullName),
							new Order(parentOrderDB, currentUser, false, null), false, false);
					}
					addComment(orderDB, "Отмена рассмотрения заявки");
					context.SaveChanges();
					LastUpdate.save(guid);
					Logger.info("===Заявка. Отмена рассмотрения. Заявка №" + order.OrderNumber.ToString(OrderInfo.NFI), Logger.LoggerSource.ordersContext);

				} else {
					throw new DomainException("Нельзя отменить рассмотрение заявки");
				}
				order.refreshOrderFromDB(orderDB, currentUser, false, null);
				MailContext.sendMail(String.Format("Заявка №{0}. Отмена рассмотрения заявки ({1})", orderDB.orderNumber.ToString(OrderInfo.NFI), CurrentUser.FullName),
					order, true, false);
			} catch (Exception e) {
				Logger.error(String.Format("===Ошибка при отмене рассмотрения заявки №{1}: {0}", e, order.OrderNumber.ToString(OrderInfo.NFI)), Logger.LoggerSource.ordersContext);
				if (e is DomainException) {
					throw e;
				}
				throw new DomainException(String.Format("Ошибка при отмене рассмотрения заявки №{0}", order.OrderNumber.ToString(OrderInfo.NFI)));
			}
		}


		public void OpenOrder(Order order, Guid guid) {
			Logger.info("Пользователь открыл заявку  №" + order.OrderNumber.ToString(OrderInfo.NFI), Logger.LoggerSource.ordersContext);
			try {
				VotGESOrdersEntities context=new VotGESOrdersEntities();
				Orders orderDB=context.Orders.First(o => o.orderNumber == order.OrderNumber);
				order.checkPremissions(orderDB, currentUser);
				if (order.AllowOpenOrder) {
					orderDB.orderLastUpdate = DateTime.Now;
					orderDB.orderDateOpen = DateTime.Now;
					orderDB.faktStartDate = order.FaktStartDate;
					orderDB.userOpenOrderID = currentUser.UserID;
					orderDB.openText = order.OpenText;
					orderDB.orderOpened = true;
					orderDB.orderState = OrderStateEnum.opened.ToString();

					context.SaveChanges();
					LastUpdate.save(guid);
					Logger.info("===Заявка открыта. Заявка №" + order.OrderNumber.ToString(OrderInfo.NFI), Logger.LoggerSource.ordersContext);
				} else {
					throw new DomainException("Нельзя открыть заявку");
				}
				order.refreshOrderFromDB(orderDB, currentUser, false, null);
				MailContext.sendMail(String.Format("Заявка №{0}. Заявка открыта ({1})", orderDB.orderNumber.ToString(OrderInfo.NFI), CurrentUser.FullName), 
					order, false, false);
			} catch (Exception e) {
				Logger.error(String.Format("===Ошибка при открытии заявки №{1}: {0}", e, order.OrderNumber.ToString(OrderInfo.NFI)), Logger.LoggerSource.ordersContext);
				if (e is DomainException) {
					throw e;
				}
				throw new DomainException(String.Format("Ошибка при открытии заявки №{0}", order.OrderNumber.ToString(OrderInfo.NFI)));
			}
		}

		public void RejectOpenOrder(Order order, Guid guid) {
			Logger.info("Пользователь отменил открытие заявки  №" + order.OrderNumber.ToString(OrderInfo.NFI), Logger.LoggerSource.ordersContext);
			try {
				VotGESOrdersEntities context=new VotGESOrdersEntities();
				Orders orderDB=context.Orders.First(o => o.orderNumber == order.OrderNumber);
				order.checkPremissions(orderDB, currentUser);
				if (order.AllowRejectOpenOrder) {
					orderDB.orderLastUpdate = DateTime.Now;
					orderDB.orderDateOpen = null;
					orderDB.faktStartDate = null;
					orderDB.userOpenOrderID = null;
					orderDB.openText = null;
					orderDB.orderOpened = false;
					orderDB.orderState = OrderStateEnum.accepted.ToString();
					addComment(orderDB, "Отмена открытия заявки");
					context.SaveChanges();
					LastUpdate.save(guid);
					Logger.info("===Заявка. отмена открытия. Заявка №" + order.OrderNumber.ToString(OrderInfo.NFI), Logger.LoggerSource.ordersContext);
				} else {
					throw new DomainException("Нельзя отменить открытие заявки");
				}
				order.refreshOrderFromDB(orderDB, currentUser, false, null);
				MailContext.sendMail(String.Format("Заявка №{0}. Отмена открытия заявки ({1})", orderDB.orderNumber.ToString(OrderInfo.NFI), CurrentUser.FullName),
					order, false, false);
			} catch (Exception e) {
				Logger.error(String.Format("===Ошибка при отмене открытия заявки №{1}: {0}", e, order.OrderNumber.ToString(OrderInfo.NFI)), Logger.LoggerSource.ordersContext);
				if (e is DomainException) {
					throw e;
				}
				throw new DomainException(String.Format("Ошибка при отмене открытия заявки №{0}", order.OrderNumber.ToString(OrderInfo.NFI)));
			}
		}

		public void CloseOrder(Order order, Guid guid) {
			Logger.info("Пользователь разрешил ввод оборудования. Заявка  №" + order.OrderNumber.ToString(OrderInfo.NFI), Logger.LoggerSource.ordersContext);
			try {
				VotGESOrdersEntities context=new VotGESOrdersEntities();
				Orders orderDB=context.Orders.First(o => o.orderNumber == order.OrderNumber);
				order.checkPremissions(orderDB, currentUser);
				if (order.AllowCloseOrder) {
					orderDB.orderLastUpdate = DateTime.Now;
					orderDB.orderDateClose = DateTime.Now;
					orderDB.faktStopDate = order.FaktStopDate;
					orderDB.orderClosed = true;
					orderDB.orderState = OrderStateEnum.closed.ToString();
					orderDB.closeText = order.CloseText;
					orderDB.userCloseOrderID = currentUser.UserID;

					context.SaveChanges();
					LastUpdate.save(guid);
					Logger.info("===Разрешен ввод оборудования. Заявка №" + order.OrderNumber.ToString(OrderInfo.NFI), Logger.LoggerSource.ordersContext);
				} else {
					throw new DomainException("Нельзя разрешить ввод оборудования");
				}
				order.refreshOrderFromDB(orderDB, currentUser, false, null);
				MailContext.sendMail(String.Format("Заявка №{0}. Разрешен ввод оборудования ({1})", orderDB.orderNumber.ToString(OrderInfo.NFI), CurrentUser.FullName), 
					order, false, false);
			} catch (Exception e) {
				Logger.error(String.Format("===Ошибка при разрешении ввода. Заявка №{1}: {0}", e, order.OrderNumber.ToString(OrderInfo.NFI)), Logger.LoggerSource.ordersContext);
				if (e is DomainException) {
					throw e;
				}
				throw new DomainException(String.Format("Ошибка при разрешении ввода. Заявка №{0}", order.OrderNumber.ToString(OrderInfo.NFI)));
			}
		}

		public void RejectCloseOrder(Order order, Guid guid) {
			Logger.info("Пользователь отменил разрешение на ввод оборудования. Заявка  №" + order.OrderNumber.ToString(OrderInfo.NFI), Logger.LoggerSource.ordersContext);
			try {
				VotGESOrdersEntities context=new VotGESOrdersEntities();
				Orders orderDB=context.Orders.First(o => o.orderNumber == order.OrderNumber);
				order.checkPremissions(orderDB, currentUser);
				if (order.AllowRejectCloseOrder) {
					orderDB.orderLastUpdate = DateTime.Now;
					orderDB.orderDateClose = null;
					orderDB.faktStopDate = null;
					orderDB.orderClosed = false;
					orderDB.orderState = OrderStateEnum.opened.ToString();
					orderDB.closeText = null;
					orderDB.userCloseOrderID = null;
					addComment(orderDB, "Отмена разрешения на ввод");

					context.SaveChanges();
					LastUpdate.save(guid);
					Logger.info("===Отменено разрешение на ввод оборудования. Заявка №" + order.OrderNumber.ToString(OrderInfo.NFI), Logger.LoggerSource.ordersContext);
				} else {
					throw new DomainException("Нельзя отменить разрешение на ввод оборудования");
				}
				order.refreshOrderFromDB(orderDB, currentUser, false, null);
				MailContext.sendMail(String.Format("Заявка №{0}. Отмена разрешения на ввод оборудования ({1})", orderDB.orderNumber.ToString(OrderInfo.NFI), CurrentUser.FullName),
					order, false, false);
			} catch (Exception e) {
				Logger.error(String.Format("===Ошибка при отмне разрешения на ввода. Заявка №{1}: {0}", e, order.OrderNumber.ToString(OrderInfo.NFI)), Logger.LoggerSource.ordersContext);
				if (e is DomainException) {
					throw e;
				}
				throw new DomainException(String.Format("Ошибка при отмене разрешения на ввод. Заявка №{0}", order.OrderNumber.ToString(OrderInfo.NFI)));
			}
		}

		public void CancelOrder(Order order, Guid guid) {
			Logger.info("Пользователь снял заявку №" + order.OrderNumber.ToString(OrderInfo.NFI), Logger.LoggerSource.ordersContext);
			try {
				VotGESOrdersEntities context=new VotGESOrdersEntities();
				Orders orderDB=context.Orders.First(o => o.orderNumber == order.OrderNumber);
				order.checkPremissions(orderDB, currentUser);
				if (order.AllowCancelOrder) {
					orderDB.orderLastUpdate = DateTime.Now;
					orderDB.orderDateCancel = DateTime.Now;
					orderDB.userCancelOrderID = currentUser.UserID;
					orderDB.cancelText = order.CancelText;
					orderDB.orderCanceled = true;
					orderDB.orderState = OrderStateEnum.canceled.ToString();

					if (order.OrderIsExtend) {
						Logger.info("===Продленная заявка", Logger.LoggerSource.ordersContext);
						Orders parentOrderDB=context.Orders.Include("Users").Where(o => o.orderNumber == order.ParentOrderNumber).First();
						parentOrderDB.orderLastUpdate = DateTime.Now;
						parentOrderDB.orderExtended = false;
						parentOrderDB.orderAskExtended = false;
						parentOrderDB.orderState = OrderStateEnum.opened.ToString();
						parentOrderDB.childOrderNumber = null;

						parentOrderDB.orderCompleted = false;
						parentOrderDB.orderDateComplete = null;
						parentOrderDB.completeText = null;
						parentOrderDB.faktCompleteDate = null;
						parentOrderDB.userCompleteOrderID = null;

						MailContext.sendMail(String.Format("Заявка №{0}. Снята заявка на продление ({1})", order.ParentOrderNumber.ToString(OrderInfo.NFI),CurrentUser.FullName), 
							new Order(parentOrderDB, currentUser, false, null), false, false);
					}

					context.SaveChanges();
					LastUpdate.save(guid);
					Logger.info("===Заявка. Снята заявка №" + order.OrderNumber.ToString(OrderInfo.NFI), Logger.LoggerSource.ordersContext);
				} else {
					throw new DomainException("Нельзя снять заявку");
				}
				order.refreshOrderFromDB(orderDB, currentUser, false, null);
				MailContext.sendMail(String.Format("Заявка №{0}. Заявка снята ({1})", orderDB.orderNumber.ToString(OrderInfo.NFI), CurrentUser.FullName), 
					order, false, false);
			} catch (Exception e) {
				Logger.error(String.Format("===Ошибка при снятии заявки №{1}: {0}", e, order.OrderNumber.ToString(OrderInfo.NFI)), Logger.LoggerSource.ordersContext);
				if (e is DomainException) {
					throw e;
				}
				throw new DomainException(String.Format("Ошибка при снятии заявки №{0}", order.OrderNumber.ToString(OrderInfo.NFI)));
			}
		}

		public void RejectCancelOrder(Order order, Guid guid) {
			Logger.info("Пользователь отменил снятие заявки №" + order.OrderNumber.ToString(OrderInfo.NFI), Logger.LoggerSource.ordersContext);
			try {
				VotGESOrdersEntities context=new VotGESOrdersEntities();
				Orders orderDB=context.Orders.First(o => o.orderNumber == order.OrderNumber);
				order.checkPremissions(orderDB, currentUser);
				if (order.AllowRejectCancelOrder) {
					orderDB.orderLastUpdate = DateTime.Now;
					orderDB.orderDateCancel = null;
					orderDB.userCancelOrderID = null;
					orderDB.cancelText = null;
					orderDB.orderCanceled = false;
					orderDB.orderState = order.OrderReviewed?OrderStateEnum.accepted.ToString():OrderStateEnum.created.ToString();
					addComment(orderDB, "Отмена снятия заявки");
					context.SaveChanges();
					LastUpdate.save(guid);
					Logger.info("===Заявка. Отмена снятия. Заявка №" + order.OrderNumber.ToString(OrderInfo.NFI), Logger.LoggerSource.ordersContext);
				} else {
					throw new DomainException("Нельзя отменить снятие заявки");
				}
				order.refreshOrderFromDB(orderDB, currentUser, false, null);
				MailContext.sendMail(String.Format("Заявка №{0}. Отмена снятия ({1})", orderDB.orderNumber.ToString(OrderInfo.NFI), CurrentUser.FullName),
					order, false, false);
			} catch (Exception e) {
				Logger.error(String.Format("===Ошибка при отмене снятия заявки №{1}: {0}", e, order.OrderNumber.ToString(OrderInfo.NFI)), Logger.LoggerSource.ordersContext);
				if (e is DomainException) {
					throw e;
				}
				throw new DomainException(String.Format("Ошибка при отмене снятия заявки №{0}", order.OrderNumber.ToString(OrderInfo.NFI)));
			}
		}

		public void CompleteOrder(Order order, Guid guid) {
			Logger.info("Пользователь ввел оборудование. Заявка №" + order.OrderNumber.ToString(OrderInfo.NFI), Logger.LoggerSource.ordersContext);
			try {
				VotGESOrdersEntities context=new VotGESOrdersEntities();
				Orders orderDB=context.Orders.First(o => o.orderNumber == order.OrderNumber);
				order.checkPremissions(orderDB, currentUser);
				if (order.AllowCompleteOrder) {
					orderDB.orderLastUpdate = DateTime.Now;
					orderDB.orderDateComplete = DateTime.Now;
					//orderDB.faktStopDate = order.FaktStopDate;
					orderDB.orderCompleted = true;
					orderDB.orderState = OrderStateEnum.completed.ToString();
					orderDB.completeText = order.CompleteText;
					orderDB.userCompleteOrderID = currentUser.UserID;
					orderDB.faktCompleteDate = order.FaktCompleteDate;
					context.SaveChanges();
					LastUpdate.save(guid);
					Logger.info("===Оборудование введено. Заявка №" + order.OrderNumber.ToString(OrderInfo.NFI), Logger.LoggerSource.ordersContext);
				} else {
					throw new DomainException("Нельзя закрыть заявку");
				}
				order.refreshOrderFromDB(orderDB, currentUser, false, null);
				MailContext.sendMail(String.Format("Заявка №{0}. Заявка закрыта ({1})", orderDB.orderNumber.ToString(OrderInfo.NFI), CurrentUser.FullName), 
					order, false, false);
			} catch (Exception e) {
				Logger.error(String.Format("===Ошибка при вводе оборудования №{1}: {0}", e, order.OrderNumber.ToString(OrderInfo.NFI)), Logger.LoggerSource.ordersContext);
				if (e is DomainException) {
					throw e;
				}
				throw new DomainException(String.Format("Ошибка при вводе оборудования по заявке №{0}", order.OrderNumber.ToString(OrderInfo.NFI)));
			}
		}

		public void RejectCompleteOrder(Order order, Guid guid) {
			Logger.info("Пользователь отменил ввод оборудования. Заявка №" + order.OrderNumber.ToString(OrderInfo.NFI), Logger.LoggerSource.ordersContext);
			try {
				VotGESOrdersEntities context=new VotGESOrdersEntities();
				Orders orderDB=context.Orders.First(o => o.orderNumber == order.OrderNumber);
				order.checkPremissions(orderDB, currentUser);
				if (order.AllowRejectCompleteOrder) {
					orderDB.orderLastUpdate = DateTime.Now;
					orderDB.orderDateComplete = null;
					orderDB.orderCompleted = false;
					orderDB.orderState = OrderStateEnum.closed.ToString();
					orderDB.completeText = null;
					orderDB.userCompleteOrderID = null;
					orderDB.faktCompleteDate = null;
					addComment(orderDB, "Отмена ввода оборудования");
					context.SaveChanges();
					LastUpdate.save(guid);
					Logger.info("===Отмена ввода оборудования. Заявка №" + order.OrderNumber.ToString(OrderInfo.NFI), Logger.LoggerSource.ordersContext);
				} else {
					throw new DomainException("Нельзя отменить ввод оборудования");
				}
				order.refreshOrderFromDB(orderDB, currentUser, false, null);
				MailContext.sendMail(String.Format("Заявка №{0}. Отмена ввода оборудования ({1})", orderDB.orderNumber.ToString(OrderInfo.NFI), CurrentUser.FullName),
					order, false, false);
			} catch (Exception e) {
				Logger.error(String.Format("===Ошибка при отмене ввода оборудования по заявке №{1}: {0}", e, order.OrderNumber.ToString(OrderInfo.NFI)), Logger.LoggerSource.ordersContext);
				if (e is DomainException) {
					throw e;
				}
				throw new DomainException(String.Format("Ошибка при отмене ввода оборудования по заявке №{0}", order.OrderNumber.ToString(OrderInfo.NFI)));
			}
		}

		public void RegisterEditOrder(Order order, Guid guid) {
			Logger.info("Пользователь отредактировал заявку. Заявка №" + order.OrderNumber.ToString(OrderInfo.NFI), Logger.LoggerSource.ordersContext);
			try {
				VotGESOrdersEntities context=new VotGESOrdersEntities();
				Orders orderDB=context.Orders.First(o => o.orderNumber == order.OrderNumber);
				order.checkPremissions(orderDB, currentUser);
				if (order.AllowEditOrder) {
					orderDB.orderLastUpdate = DateTime.Now;

					writeOrderToOrderDB(order, orderDB);

					if (orderDB.orderReviewed) {
						orderDB.reviewText = order.ReviewText;
						orderDB.orderDateReview = order.OrderDateReview;
					}
					if (orderDB.orderOpened) {
						orderDB.faktStartDate = order.FaktStartDate;
						orderDB.openText = order.OpenText;					
					}
					if (orderDB.orderClosed) {
						orderDB.faktStopDate = order.FaktStopDate;
						orderDB.closeText = order.CloseText;
					}
					if (orderDB.orderCompleted) {
						orderDB.faktCompleteDate = order.FaktCompleteDate;
						orderDB.completeText = order.CompleteText;
					}
					if (orderDB.orderCanceled) {						
						orderDB.cancelText = order.CancelText;
					}

					orderDB.commentsText = order.CommentsText;
					addComment(orderDB, "Ручное редактирование заявки");
					
					checkOrder(orderDB);
					context.SaveChanges();
					LastUpdate.save(guid);
					Logger.info("===Заявка отредактирована. Заявка №" + order.OrderNumber.ToString(OrderInfo.NFI), Logger.LoggerSource.ordersContext);
				} else {
					throw new DomainException("Нельзя отменить ввод оборудования");
				}
				order.refreshOrderFromDB(orderDB, currentUser, false, null);
				MailContext.sendMail(String.Format("Заявка №{0}. Заявка отредактирована ({1})", orderDB.orderNumber.ToString(OrderInfo.NFI), CurrentUser.FullName),
					order, false, false);
			} catch (Exception e) {
				Logger.error(String.Format("===Ошибка приредактировании заявки №{1}: {0}", e, order.OrderNumber.ToString(OrderInfo.NFI)), Logger.LoggerSource.ordersContext);
				if (e is DomainException) {
					throw e;
				}
				throw new DomainException(String.Format("Ошибка при редактировании заявки №{0}", order.OrderNumber.ToString(OrderInfo.NFI)));
			}
		}

		public void RegisterAddComment(Order order, String commentText, Guid guid) {
			Logger.info("Пользователь добавил комментарий к заявке. Заявка №" + order.OrderNumber.ToString(OrderInfo.NFI), Logger.LoggerSource.ordersContext);
			try {
				VotGESOrdersEntities context=new VotGESOrdersEntities();
				Orders orderDB=context.Orders.First(o => o.orderNumber == order.OrderNumber);
				order.checkPremissions(orderDB, currentUser);

				addComment(orderDB, commentText);

				context.SaveChanges();
				LastUpdate.save(guid);
				Logger.info("===Добавлен комментарий. Заявка №" + order.OrderNumber.ToString(OrderInfo.NFI), Logger.LoggerSource.ordersContext);
				
				order.refreshOrderFromDB(orderDB, currentUser, false, null);
				MailContext.sendMail(String.Format("Заявка №{0}. Новый комментарий ({1})", orderDB.orderNumber.ToString(OrderInfo.NFI), CurrentUser.FullName),
					order, true, false);
			} catch (Exception e) {
				Logger.error(String.Format("===Ошибка при комментировании заявки №{1}: {0}", e, order.OrderNumber.ToString(OrderInfo.NFI)), Logger.LoggerSource.ordersContext);
				if (e is DomainException) {
					throw e;
				}
				throw new DomainException(String.Format("Ошибка при комментировании заявки №{0}", order.OrderNumber.ToString(OrderInfo.NFI)));
			}
		}

		private void addComment(Orders orderDB, string commentText) {
			if (!String.IsNullOrEmpty(orderDB.commentsText))
				orderDB.commentsText = "\n" + orderDB.commentsText;
			orderDB.commentsText = String.Format("{1,20}[{0}] : {2}", DateTime.Now.ToString("dd.MM.yy HH:mm"), CurrentUser.FullName, commentText) + orderDB.commentsText;
		}


		protected void checkOrder(Orders orderDB) {
			if (!orderDB.orderReviewed) {
				orderDB.orderDateReview = null;
				orderDB.userReviewOrderID = null;
				orderDB.reviewText = null;
			}
			if (!orderDB.orderOpened) {
				orderDB.orderDateOpen = null;
				orderDB.faktStartDate = null;
				orderDB.userOpenOrderID = null;
				orderDB.openText = null;
			}
			if (!orderDB.orderClosed) {
				orderDB.orderDateClose = null;
				orderDB.faktStopDate = null;
				orderDB.userCloseOrderID = null;
				orderDB.closeText = null;
			}
			if (!orderDB.orderCompleted) {
				orderDB.orderDateComplete = null;
				orderDB.faktCompleteDate = null;
				orderDB.userCompleteOrderID = null;
				orderDB.completeText = null;
			}
			if (!orderDB.orderCanceled) {
				orderDB.orderDateCancel = null;				
				orderDB.userCancelOrderID = null;
				orderDB.cancelText = null;
			}
		}
		public void ReloadOrder(Order order) {
			Logger.info("Пользователь обновляет заявку №" + order.OrderNumber.ToString(OrderInfo.NFI), Logger.LoggerSource.ordersContext);
			try {
				VotGESOrdersEntities context=new VotGESOrdersEntities();
				Orders orderDB=context.Orders.First(o => o.orderNumber == order.OrderNumber);
				Logger.info("===Заявка обновлена. Заявка №" + order.OrderNumber.ToString(OrderInfo.NFI), Logger.LoggerSource.ordersContext);
				order.refreshOrderFromDB(orderDB, currentUser, false, null);
			} catch (Exception e) {
				Logger.error(String.Format("===Ошибка при обновлении заявки №{1}: {0}", e, order.OrderNumber.ToString(OrderInfo.NFI)), Logger.LoggerSource.ordersContext);
				throw new DomainException(String.Format("Ошибка при обновлении заявки №{0}", order.OrderNumber.ToString(OrderInfo.NFI)));
			}
		}


	}
}