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
					Logger.info("Получение списка заказов (по умолчанию)");
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
						resultOrders.Add(new Order(orderDB, currentUser, false, false));
					}
					return resultOrders.AsQueryable();
				} catch (Exception e) {
					Logger.info("Ошибка при получении списка заказов (по умолчанию)"+ e.ToString());
					throw new DomainException(String.Format("Ошибка при получении списка заказов (по умолчанию)"));
				}
			}
		}

		public IQueryable<Order> OrdersActive {
			get {
				try {
					Logger.info("Получение списка активных заказов");
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
						resultOrders.Add(new Order(orderDB, currentUser,false,false));
					}
					return resultOrders.AsQueryable();
				} catch (Exception e) {
					Logger.info("Ошибка при получении списка заказов (активные)" + e.ToString());
					throw new DomainException(String.Format("Ошибка при получении списка заказов (активные)"));
				}

			}
		}


		public IQueryable<Order> getOrdersUserFilter(OrderFilter filter) {
			Logger.info("Получение списка заказов (фильтр)");
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
						(filter.FilterDate == FilterDateType.accept && o.orderDateAccept >= filter.DateStart && o.orderDateAccept <= filter.DateEnd) ||
						(filter.FilterDate == FilterDateType.ban && o.orderDateBan >= filter.DateStart && o.orderDateBan <= filter.DateEnd) ||
						(filter.FilterDate == FilterDateType.cancel && o.orderDateCancel >= filter.DateStart && o.orderDateCancel <= filter.DateEnd) ||
						(filter.FilterDate == FilterDateType.faktStart && o.faktStartDate >= filter.DateStart && o.faktStartDate <= filter.DateEnd) ||
						(filter.FilterDate == FilterDateType.faktStop && o.faktStopDate >= filter.DateStart && o.faktStopDate <= filter.DateEnd) ||
						(filter.FilterDate == FilterDateType.faktEnter && o.faktCompleteDate >= filter.DateStart && o.faktCompleteDate <= filter.DateEnd) ||
						(filter.FilterDate == FilterDateType.planStart && o.planStartDate >= filter.DateStart && o.planStartDate <= filter.DateEnd) ||
						(filter.FilterDate == FilterDateType.planStop && o.planStopDate >= filter.DateStart && o.planStopDate <= filter.DateEnd))
					&& (filter.ShowAllUsers ||
						(filter.FilterUser == FilterUserType.create && users.Contains(o.userCreateOrderID)) ||
						(filter.FilterUser == FilterUserType.accept && users.Contains(o.userAcceptOrderID.Value)) ||
						(filter.FilterUser == FilterUserType.ban && users.Contains(o.userBanOrderID.Value)) ||
						(filter.FilterUser == FilterUserType.cancel && users.Contains(o.userCancelOrderID.Value)) ||
						(filter.FilterUser == FilterUserType.open && users.Contains(o.userOpenOrderID.Value)) ||
						(filter.FilterUser == FilterUserType.close && users.Contains(o.userCloseOrderID.Value)) ||
						(filter.FilterUser == FilterUserType.enter && users.Contains(o.userCompleteOrderID.Value)))
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
					resultOrders.Add(new Order(orderDB, currentUser, filter.ShowRelatedOrders, filter.ShowRelatedOrders));
				}
				return resultOrders.AsQueryable();
			} catch (Exception e) {
				Logger.info("Ошибка при получении списка заказов (фильтр) "+e.ToString());
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

		protected void writeOrderToOrderDB(Order order, Orders orderDB) {
			orderDB.orderText = order.OrderText;
			orderDB.agreeText = order.AgreeText;
			orderDB.createText = order.CreateText;
			orderDB.planStartDate = order.PlanStartDate;
			orderDB.planStopDate = order.PlanStopDate;
			orderDB.orderType = order.OrderType;
			orderDB.orderObjectID = order.SelOrderObjectID;
			orderDB.orderObjectAddInfo = order.OrderObjectAddInfo;
			orderDB.readyTime = order.ReadyTime;
			orderDB.agreeUsersIDS = order.AgreeUsersIDSText;
		}


		public void RegisterOrder(Order order, Guid guid) {
			Logger.info("Пользователь создал заявку");
			try {
				VotGESOrdersEntities context=new VotGESOrdersEntities();
				VotGESOrders.Web.ADONETEntities.Orders orderDB=new Orders();
				orderDB.orderLastUpdate = DateTime.Now;
				orderDB.orderDateCreate = DateTime.Now;
				orderDB.orderCreated = true;
				orderDB.orderState = OrderStateEnum.created.ToString();
				orderDB.userCreateOrderID = currentUser.UserID;
				orderDB.orderNumber = 5;

				writeOrderToOrderDB(order, orderDB);

				context.Orders.AddObject(orderDB);
				context.SaveChanges();

				Logger.info("Сохранено");
				

				if (order.OrderIsExtend) {
					Logger.info("Продленная заявка");
					Orders parentOrderDB=context.Orders.Where(o => o.orderNumber == order.ParentOrderNumber).First();

					parentOrderDB.orderLastUpdate = DateTime.Now;
					parentOrderDB.orderAskExtended = true;
					parentOrderDB.orderState = OrderStateEnum.askExtended.ToString();

					orderDB.orderIsExtend = true;

					orderDB.parentOrderNumber = order.ParentOrderNumber;
					parentOrderDB.childOrderNumber = orderDB.orderNumber;
					context.SaveChanges();
					Logger.info("Сохранена дочерняя заявка");
					try {
						MailContext.sendMail("Продление заявки №" + order.ParentOrderNumber, new Order(parentOrderDB, currentUser, false, false));
					} catch (Exception e) { Logger.error("Ошибка приотправке почты " + e.ToString()); }
				}
				LastUpdate.save(guid);
				order.refreshOrderFromDB(orderDB, currentUser);
				MailContext.sendMail("Создана новая заявка", order);
			} catch (Exception e) {
				Logger.error(String.Format("Ошибка при создании заявки: {0}", e));
				throw new DomainException("Ошибка при создании заявки");
			}
		}

		public void ChangeOrder(Order order, Guid guid) {
			Logger.info("Пользователь изменил заявку №" + order.OrderNumber);
			try {
				VotGESOrdersEntities context=new VotGESOrdersEntities();
				Orders orderDB=context.Orders.First(o => o.orderNumber == order.OrderNumber);
				order.checkPremissions(orderDB, currentUser);
				if (order.AllowChangeOrder) {
					writeOrderToOrderDB(order, orderDB);
					orderDB.orderLastUpdate = DateTime.Now;
					context.SaveChanges();
					LastUpdate.save(guid);
					Logger.info("Изменения сохранены. Заявка №" + order.OrderNumber);
				} 
				order.refreshOrderFromDB(orderDB, currentUser);
				MailContext.sendMail("Изменена заявка №"+orderDB.orderNumber, order);
			} catch (Exception e) {
				Logger.error(String.Format("Ошибка при изменении заявки №{1}: {0}", e, order.OrderNumber));
				throw new DomainException(String.Format("Ошибка при изменении заявки №{0}", order.OrderNumber));
			}
		}


		public void AcceptOrder(Order order, Guid guid) {
			Logger.info("Пользователь разрешил заявку №" + order.OrderNumber);
			try {
				VotGESOrdersEntities context=new VotGESOrdersEntities();
				Orders orderDB=context.Orders.First(o => o.orderNumber == order.OrderNumber);
				order.checkPremissions(orderDB, currentUser);
				if (order.AllowAcceptOrder) {
					orderDB.orderLastUpdate = DateTime.Now;
					orderDB.orderDateAccept = DateTime.Now;
					orderDB.userAcceptOrderID = currentUser.UserID;
					orderDB.acceptText = order.AcceptText;
					orderDB.orderAccepted = true;
					orderDB.orderBanned = false;
					orderDB.orderState = OrderStateEnum.accepted.ToString();

					if (order.OrderIsExtend) {
						Logger.info("продленная заявка");
						Orders parentOrderDB=context.Orders.Where(o => o.orderNumber == order.ParentOrderNumber).First();
						parentOrderDB.orderLastUpdate = DateTime.Now;
						parentOrderDB.orderExtended = true;
						parentOrderDB.orderAskExtended = false;
						parentOrderDB.orderState = OrderStateEnum.extended.ToString();
						try {
							MailContext.sendMail("Продлена заявка №" + order.ParentOrderNumber, new Order(parentOrderDB, currentUser, false, false));
						} catch { }
					}
					context.SaveChanges();
					LastUpdate.save(guid);
					Logger.info("Заявка разрешена. Заявка №" + order.OrderNumber);
				}
				order.refreshOrderFromDB(orderDB, currentUser);
				MailContext.sendMail("Разрешена заявка №" + orderDB.orderNumber, order);
			} catch (Exception e) {
				Logger.error(String.Format("Ошибка при разрешении заявки №{1}: {0}", e, order.OrderNumber));
				throw new DomainException(String.Format("Ошибка при разрешении заявки №{0}", order.OrderNumber));
			}
		}

		public void BanOrder(Order order, Guid guid) {
			Logger.info("Пользователь запретил заявку  №" + order.OrderNumber);
			try {
				VotGESOrdersEntities context=new VotGESOrdersEntities();
				Orders orderDB=context.Orders.First(o => o.orderNumber == order.OrderNumber);
				order.checkPremissions(orderDB, currentUser);
				if (order.AllowAcceptOrder) {
					orderDB.orderLastUpdate = DateTime.Now;
					orderDB.orderDateBan = DateTime.Now;
					orderDB.userBanOrderID = currentUser.UserID;
					orderDB.banText = order.BanText;
					orderDB.orderBanned = true;
					orderDB.orderAccepted = false;
					orderDB.orderState = OrderStateEnum.banned.ToString();

					if (order.OrderIsExtend) {
						Logger.info("продленная заявка");
						Orders parentOrderDB=context.Orders.Include("Users").Where(o => o.orderNumber == order.ParentOrderNumber).First();
						parentOrderDB.orderLastUpdate = DateTime.Now;
						parentOrderDB.orderExtended = false;
						parentOrderDB.orderAskExtended = false;
						parentOrderDB.orderOpened = true;
						parentOrderDB.orderState = OrderStateEnum.opened.ToString();
						parentOrderDB.childOrderNumber = null;

						try {
							MailContext.sendMail("Продление заявки отклонено. Заявка №" + order.ParentOrderNumber, new Order(parentOrderDB, currentUser, false, false));
						} catch { }
					}

					context.SaveChanges();
					LastUpdate.save(guid);
					Logger.info("Заявка запрещена. Заявка №" + order.OrderNumber);

				}
				order.refreshOrderFromDB(orderDB, currentUser);
				MailContext.sendMail("Отклонена заявка №" + orderDB.orderNumber, order);
			} catch (Exception e) {
				Logger.error(String.Format("Ошибка при запрещении заявки №{1}: {0}", e, order.OrderNumber));
				throw new DomainException(String.Format("Ошибка при запрете заявки №{0}", order.OrderNumber));
			}
		}



		public void OpenOrder(Order order, Guid guid) {
			Logger.info("Пользователь открыл заявку  №" + order.OrderNumber);
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
					Logger.info("Заявка открыта. Заявка №" + order.OrderNumber);
				}
				order.refreshOrderFromDB(orderDB, currentUser);
				MailContext.sendMail("Открыта заявка №" + orderDB.orderNumber, order);
			} catch (Exception e) {
				Logger.error(String.Format("Ошибка при открытии заявки №{1}: {0}", e, order.OrderNumber));
				throw new DomainException(String.Format("Ошибка при открытии заявки №{0}", order.OrderNumber));
			}
		}

		public void CloseOrder(Order order, Guid guid) {
			Logger.info("Пользователь закрыл заявку  №" + order.OrderNumber);
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
					Logger.info("Заявка закрыта. Заявка №" + order.OrderNumber);
				}
				order.refreshOrderFromDB(orderDB, currentUser);
				MailContext.sendMail("Разрешен ввод оборудования. Заявка №" + orderDB.orderNumber, order);
			} catch (Exception e) {
				Logger.error(String.Format("Ошибка при закрытии заявки №{1}: {0}", e, order.OrderNumber));
				throw new DomainException(String.Format("Ошибка при закрытии заявки №{0}", order.OrderNumber));
			}
		}

		public void CancelOrder(Order order, Guid guid) {
			Logger.info("Пользователь снял заявку №" + order.OrderNumber);
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
						Logger.info("продленная заявка");
						Orders parentOrderDB=context.Orders.Include("Users").Where(o => o.orderNumber == order.ParentOrderNumber).First();
						parentOrderDB.orderLastUpdate = DateTime.Now;
						parentOrderDB.orderExtended = false;
						parentOrderDB.orderAskExtended = false;
						parentOrderDB.orderState = OrderStateEnum.opened.ToString();
						parentOrderDB.childOrderNumber = null;

						try {
							MailContext.sendMail("Снята заявка на продление. Заявка №" + order.ParentOrderNumber, new Order(parentOrderDB, currentUser, false, false));
						} catch { }
					}

					context.SaveChanges();
					LastUpdate.save(guid);
					Logger.info("Заявка снята. Заявка №" + order.OrderNumber);
				}
				order.refreshOrderFromDB(orderDB, currentUser);
				MailContext.sendMail("Снята заявка №" + orderDB.orderNumber, order);
			} catch (Exception e) {
				Logger.error(String.Format("Ошибка при снятии заявки №{1}: {0}", e, order.OrderNumber));
				throw new DomainException(String.Format("Ошибка при отмене заявки №{0}", order.OrderNumber));
			}
		}

		public void CompleteOrder(Order order, Guid guid) {
			Logger.info("Пользователь ввел оборудование. Заявка №" + order.OrderNumber);
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
					Logger.info("Оборудование введено. Заявка №" + order.OrderNumber);
				}
				order.refreshOrderFromDB(orderDB, currentUser);
				MailContext.sendMail("Завершена заявка №" + orderDB.orderNumber, order);
			} catch (Exception e) {
				Logger.error(String.Format("Ошибка при вводе оборудования №{1}: {0}", e, order.OrderNumber));
				throw new DomainException(String.Format("Ошибка при вводе оборудования по заявке №{0}", order.OrderNumber));
			}
		}

		public void ReloadOrder(Order order) {
			Logger.info("Пользователь обновляет заявку №" + order.OrderNumber);
			try {
				VotGESOrdersEntities context=new VotGESOrdersEntities();
				Orders orderDB=context.Orders.First(o => o.orderNumber == order.OrderNumber);
				Logger.info("Заявка обновлена. Заявка №" + order.OrderNumber);
				order.refreshOrderFromDB(orderDB, currentUser);
			} catch (Exception e) {
				Logger.error(String.Format("Ошибка при обновлении заявки №{1}: {0}", e, order.OrderNumber));
				throw new DomainException(String.Format("Ошибка при обновлении заявки №{0}", order.OrderNumber));
			}
		}


	}
}