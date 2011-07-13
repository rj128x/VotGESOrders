
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
		public IQueryable<Order> LoadOrders() {
			return context.Orders;
		}
		
		[Query(HasSideEffects=true)]
		public IQueryable<Order> GetFilteredOrdersFromXML(string xml) {
			Logger.info("Сервис: Получение списка заказов (GetFilteredOrdersFromXML)");
			OrderFilter filter=XMLStringSerializer.Deserialize<OrderFilter>(xml);
			return context.getOrders(filter);
		}

		public IQueryable<OrdersUser> LoadOrdersUsers() {
			Logger.info("Сервис: Получение списка пользователей (LoadUsers)");
			return OrdersUser.getAllUsers().AsQueryable();
		}

		public IQueryable<OrderObject> LoadOrderObjects() {
			return OrderObject.getAllObjects().AsQueryable();
		}

		public void RegisterNew(Order order,Guid guid) {
			Logger.info("Сервис: создание заявки");
			context.RegisterOrder(order, guid);
		}

		public void UpdateOrder(Order order) {
			Logger.info("Сервис: Update " + order.OrderNumber.ToString());
			//context.UpdateOrder(order);
		}

		

		public void RegisterChangeOrder(Order order,Guid guid) {
			Logger.info("Сервис: изменение заявки " + order.OrderNumber.ToString());
			context.ChangeOrder(order, guid);
		}

		public void RegisterAcceptOrder(Order order,Guid guid) {
			Logger.info("Сервис: разрешение заявки");
			context.AcceptOrder(order, guid);
		}

		public void RegisterBanOrder(Order order, Guid guid) {
			Logger.info("Сервис: запрет заявки " + order.OrderNumber.ToString());
			context.BanOrder(order, guid);
		}

		public void RegisterCancelOrder(Order order, Guid guid) {
			Logger.info("Сервис: снятие заявки " + order.OrderNumber.ToString());
			context.CancelOrder(order, guid);
		}

		public void RegisterOpenOrder(Order order, Guid guid) {
			Logger.info("Сервис: открытие заявки " + order.OrderNumber.ToString());
			context.OpenOrder(order, guid);
		}

		public void RegisterCloseOrder(Order order, Guid guid) {
			Logger.info("Сервис: закрытие заявки " + order.OrderNumber.ToString());
			context.CloseOrder(order, guid);
		}

		public void RegisterCompleteOrder(Order order, Guid guid) {
			Logger.info("Сервис: ввод оборудования " + order.OrderNumber.ToString());
			context.CompleteOrder(order, guid);
		}

		public void ReloadOrder(Order order, Guid guid) {
			Logger.info("Сервис: обновление заявки" + order.OrderNumber.ToString());
			context.ReloadOrder(order);
		}

		public bool ExistsChanges(DateTime lastUpdate, Guid guid) {			
			bool exist= LastUpdate.IsChanged(lastUpdate,guid);
			//Logger.info(String.Format("Проверка изменений {0}, {1} : {2}",lastUpdate,guid,exist));
			return exist;
		}

		public void UpdateOrderObject(OrderObject obj) {
			Logger.info("Сервис: Update " + obj.ObjectName);
			//context.UpdateOrder(order);
		}


		public void RegisterChangeObject(OrderObject newObject) {
			Logger.info("Сервис: изменение оборудования " + newObject.ObjectName);
			objContext.RegisterChangeOrderObject(newObject);
		}

		public void RegisterDeleteObject(OrderObject newObject) {
			Logger.info("Сервис: удаление оборудования " + newObject.ObjectName);
			objContext.RegisterDeleteOrderObject(newObject);
		}

		public void UpdateOrdersUser(OrdersUser obj) {
			Logger.info("Сервис: Update " + obj.FullName);
			//context.UpdateOrder(order);
		}


		public void RegisterChangeUser(OrdersUser newObject) {
			Logger.info("Сервис: изменение пользователя " + newObject.FullName);
			usrContext.RegisterChangeUser(newObject);
		}

		public void RegisterDeleteUser(OrdersUser newObject) {
			Logger.info("Сервис: удаление пользователя " + newObject.FullName);
			usrContext.RegisterDeleteUser(newObject);
		}

	}
}


