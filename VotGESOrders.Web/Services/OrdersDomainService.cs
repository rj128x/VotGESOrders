
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
			Logger.info("Получение списка заказов (GetFilteredOrdersFromXML)","Сервис");
			OrderFilter filter=XMLStringSerializer.Deserialize<OrderFilter>(xml);
			LastUpdate.saveUpdate(guid);
			return context.getOrders(filter);
		}

		public IQueryable<OrdersUser> LoadOrdersUsers() {
			Logger.info("Получение списка пользователей (LoadUsers)", "Сервис");
			return OrdersUser.getAllUsers().AsQueryable();
		}

		public IQueryable<OrderObject> LoadOrderObjects() {
			return OrderObject.getAllObjects().AsQueryable();
		}

		public void RegisterNew(Order order,Guid guid) {
			Logger.info("Ссоздание заявки", "Сервис");
			context.RegisterOrder(order, guid);
		}

		public void UpdateOrder(Order order) {
			//Logger.info("Сервис: Update " + order.OrderNumber.ToString());
			//context.UpdateOrder(order);
		}

		

		public void RegisterChangeOrder(Order order,Guid guid) {
			Logger.info("изменение заявки " + order.OrderNumber.ToString(), "Сервис");
			context.ChangeOrder(order, guid);
		}

		public void RegisterAcceptOrder(Order order,Guid guid) {
			Logger.info("разрешение заявки " + order.OrderNumber.ToString(), "Сервис");
			context.AcceptOrder(order, guid);
		}

		public void RegisterBanOrder(Order order, Guid guid) {
			Logger.info("запрет заявки " + order.OrderNumber.ToString(), "Сервис");
			context.BanOrder(order, guid);
		}

		public void RegisterCancelOrder(Order order, Guid guid) {
			Logger.info("снятие заявки " + order.OrderNumber.ToString(), "Сервис");
			context.CancelOrder(order, guid);
		}

		public void RegisterOpenOrder(Order order, Guid guid) {
			Logger.info("открытие заявки " + order.OrderNumber.ToString(), "Сервис");
			context.OpenOrder(order, guid);
		}

		public void RegisterCloseOrder(Order order, Guid guid) {
			Logger.info("разрешение ввода " + order.OrderNumber.ToString(), "Сервис");
			context.CloseOrder(order, guid);
		}

		public void RegisterCompleteOrder(Order order, Guid guid) {
			Logger.info("ввод оборудования " + order.OrderNumber.ToString(), "Сервис");
			context.CompleteOrder(order, guid);
		}


		public void ReloadOrder(Order order, Guid guid) {
			Logger.info("обновление заявки" + order.OrderNumber.ToString(), "Сервис");
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
			Logger.info("изменение оборудования " + newObject.ObjectName, "Сервис");
			objContext.RegisterChangeOrderObject(newObject);
		}

		public void RegisterDeleteObject(OrderObject newObject) {
			Logger.info("удаление оборудования " + newObject.ObjectName, "Сервис");
			objContext.RegisterDeleteOrderObject(newObject);
		}

		public void UpdateOrdersUser(OrdersUser obj) {
			//Logger.info("Сервис: Update " + obj.FullName);
			//context.UpdateOrder(order);
		}


		public void RegisterChangeUser(OrdersUser newObject) {
			Logger.info("изменение пользователя " + newObject.FullName, "Сервис");
			usrContext.RegisterChangeUser(newObject);
		}

		public void RegisterDeleteUser(OrdersUser newObject) {
			Logger.info("удаление пользователя " + newObject.FullName, "Сервис");
			usrContext.RegisterDeleteUser(newObject);
		}

	}
}


