
namespace VotGESOrders.Web.Services
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Linq;
	using System.ServiceModel.DomainServices.Hosting;
	using System.ServiceModel.DomainServices.Server;
	using VotGESOrders.Web.Models;
	using VotGESOrders.Web.Logging;
	using System.Web;
	
	// TODO: Create methods containing your application logic.
	[EnableClientAccess()]
	public class OrdersDomainService : DomainService
	{
		private OrdersContext context= new OrdersContext();
		private OrderObjectContext objContext=new OrderObjectContext();
		private OrdersUserContext usrContext=new OrdersUserContext();
		public IQueryable<Order> LoadOrders(Guid guid) {
			LastUpdate.saveUpdate(guid);
			return context.Orders;			
		}
		
		[Query(HasSideEffects=true)]
		public IQueryable<Order> GetFilteredOrdersFromXML(string xml,Guid guid) {
			Logger.info("Получение списка заказов (GetFilteredOrdersFromXML)", Logger.LoggerSource.service);
			OrderFilter filter=XMLStringSerializer.Deserialize<OrderFilter>(xml);
			LastUpdate.saveUpdate(guid);
			return context.getOrders(filter);
		}

		[Query(HasSideEffects = true)]
		public IQueryable<Order> GetFilteredOrdersFromXMLToMail(string xml, Guid guid) {
			Logger.info("Получение списка заказов (GetFilteredOrdersFromXML) В почту", Logger.LoggerSource.service);
			OrderFilter filter=XMLStringSerializer.Deserialize<OrderFilter>(xml);
			IQueryable<Order> ordersQuery= context.getOrders(filter);
			List<Order> orders=ordersQuery.ToList();
			MailContext.sendOrdersList("Список заявкок", orders);
			return ordersQuery;
		}

		public IQueryable<OrdersUser> LoadOrdersUsers() {
			Logger.info("Получение списка пользователей (LoadUsers)", Logger.LoggerSource.service);
			return OrdersUser.getAllUsers().AsQueryable();
		}

		public IQueryable<OrderObject> LoadOrderObjects() {
			Logger.info("Получение списка объектов (LoadOrderObjects)", Logger.LoggerSource.service);
			return OrderObject.getAllObjects().AsQueryable();
		}

		public void RegisterNew(Order order,Guid guid) {
			Logger.info("Создание заявки", Logger.LoggerSource.service);
			context.RegisterOrder(order, guid);
		}

		public void UpdateOrder(Order order) {
			//Logger.info("Сервис: Update " + order.OrderNumber.ToString());
			//context.UpdateOrder(order);
		}

		

		public void RegisterChangeOrder(Order order,Guid guid) {
			Logger.info("изменение заявки " + order.OrderNumber.ToString(), Logger.LoggerSource.service);
			context.ChangeOrder(order, guid);
		}

		public void RegisterAcceptOrder(Order order,Guid guid) {
			Logger.info("разрешение заявки " + order.OrderNumber.ToString(), Logger.LoggerSource.service);
			context.AcceptOrder(order, guid);
		}

		public void RegisterBanOrder(Order order, Guid guid) {
			Logger.info("запрет заявки " + order.OrderNumber.ToString(), Logger.LoggerSource.service);
			context.BanOrder(order, guid);
		}

		public void RegisterCancelOrder(Order order, Guid guid) {
			Logger.info("снятие заявки " + order.OrderNumber.ToString(), Logger.LoggerSource.service);
			context.CancelOrder(order, guid);
		}

		public void RegisterOpenOrder(Order order, Guid guid) {
			Logger.info("открытие заявки " + order.OrderNumber.ToString(), Logger.LoggerSource.service);
			context.OpenOrder(order, guid);
		}

		public void RegisterCloseOrder(Order order, Guid guid) {
			Logger.info("разрешение ввода " + order.OrderNumber.ToString(), Logger.LoggerSource.service);
			context.CloseOrder(order, guid);
		}

		public void RegisterCompleteOrder(Order order, Guid guid) {
			Logger.info("ввод оборудования " + order.OrderNumber.ToString(), Logger.LoggerSource.service);
			context.CompleteOrder(order, guid);
		}

		public void RegisterRejectReviewOrder(Order order, Guid guid) {
			Logger.info("отмена рассмотрения заявки " + order.OrderNumber.ToString(), Logger.LoggerSource.service);
			context.RejectReviewOrder(order, guid);
		}

		public void RegisterRejectCancelOrder(Order order, Guid guid) {
			Logger.info("отмена снятия заявки " + order.OrderNumber.ToString(), Logger.LoggerSource.service);
			context.RejectCancelOrder(order, guid);
		}

		public void RegisterRejectOpenOrder(Order order, Guid guid) {
			Logger.info("отмена открытия заявки " + order.OrderNumber.ToString(), Logger.LoggerSource.service);
			context.RejectOpenOrder(order, guid);
		}

		public void RegisterRejectCloseOrder(Order order, Guid guid) {
			Logger.info("отмена разрешения на ввод заявки " + order.OrderNumber.ToString(), Logger.LoggerSource.service);
			context.RejectCloseOrder(order, guid);
		}

		public void RegisterRejectCompleteOrder(Order order, Guid guid) {
			Logger.info("отмена закрытия заявки " + order.OrderNumber.ToString(), Logger.LoggerSource.service);
			context.RejectCompleteOrder(order, guid);
		}

		public void RegisterEditOrder(Order order, Guid guid) {
			Logger.info("редактирование заявки " + order.OrderNumber.ToString(), Logger.LoggerSource.service);
			context.RegisterEditOrder(order, guid);
		}

		public void RegisterAddComment(Order order, string commentText, Guid guid) {
			Logger.info("комментирование заявки " + order.OrderNumber.ToString(), Logger.LoggerSource.service);
			context.RegisterAddComment(order, commentText, guid);
		}

		public void ReloadOrder(Order order, Guid guid) {
			Logger.info("обновление заявки" + order.OrderNumber.ToString(), Logger.LoggerSource.service);
			context.ReloadOrder(order);
		}

		public bool ExistsChanges(Guid guid) {			
			bool exist= LastUpdate.IsChanged(guid);
			//Logger.info(String.Format("Проверка изменений {0}, {1} : {2}",lastUpdate,guid,exist));
			return exist;
		}

		public void UpdateOrderObject(OrderObject obj) {
			//Logger.info("Сервис: Update " + obj.ObjectName);
			//context.UpdateOrder(order);
		}


		public void RegisterChangeObject(OrderObject newObject) {
			Logger.info("изменение оборудования " + newObject.ObjectName, Logger.LoggerSource.service);
			objContext.RegisterChangeOrderObject(newObject);
		}

		public void RegisterDeleteObject(OrderObject newObject) {
			Logger.info("удаление оборудования " + newObject.ObjectName, Logger.LoggerSource.service);
			objContext.RegisterDeleteOrderObject(newObject);
		}

		public void UpdateOrdersUser(OrdersUser obj) {
			//Logger.info("Сервис: Update " + obj.FullName);
			//context.UpdateOrder(order);
		}


		public void RegisterChangeUser(OrdersUser newObject) {
			Logger.info("изменение пользователя " + newObject.FullName, Logger.LoggerSource.service);
			usrContext.RegisterChangeUser(newObject);
		}

		public void RegisterDeleteUser(OrdersUser newObject) {
			Logger.info("удаление пользователя " + newObject.FullName, Logger.LoggerSource.service);
			usrContext.RegisterDeleteUser(newObject);
		}

	}
}


