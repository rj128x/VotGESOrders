using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VotGESOrders.Web.Logging;
using VotGESOrders.Web.ADONETEntities;
using System.ServiceModel.DomainServices.Server;

namespace VotGESOrders.Web.Models
{
	public class OrderObjectContext
	{

		public void RegisterChangeOrderObject(OrderObject newObj) {
			Logger.info("Пользователь изменил оборудование", Logger.LoggerSource.objectsContext);
			try {
				VotGESOrdersEntities context=new VotGESOrdersEntities();

				IQueryable<OrderObjects> objectsDB=(from o in context.OrderObjects where o.objectID == newObj.ObjectID select  o);
				OrderObjects objDB=null;
				if (objectsDB.Count()==0) {
					Logger.info("===Новое оборудование", Logger.LoggerSource.objectsContext);
					objDB = new OrderObjects();
					context.OrderObjects.AddObject(objDB);
				} else {
					objDB = objectsDB.First();
				}
				objDB.objectName = newObj.ObjectName;
				objDB.parentID = newObj.ParentObjectID;
				objDB.showInFullName = newObj.ShowInFullName;

				context.SaveChanges();

				newObj.ObjectID = objDB.objectID;
				newObj.ObjectName = objDB.objectName;
				OrderObject.init();
				newObj.FullName = OrderObject.getByID(newObj.ObjectID).getFullName();
				Logger.info("===Сохранено", Logger.LoggerSource.objectsContext);

			} catch (Exception e) {
				Logger.error(String.Format("===Ошибка при изменении оборудования: {0}", e), Logger.LoggerSource.objectsContext);
				throw new DomainException("Ошибка при изменении/создании оборудования");
			}
		}

		protected void deleteOrderObject(VotGESOrdersEntities context,OrderObjects obj) {
			IQueryable<OrderObjects>childs=(from o in context.OrderObjects where o.parentID == obj.objectID select o);
			foreach (OrderObjects child in childs) {
				deleteOrderObject(context, child);				
			}
			context.DeleteObject(obj);
		}

		public void RegisterDeleteOrderObject(OrderObject newObj) {
			Logger.info("Пользователь удалил оборудование", Logger.LoggerSource.objectsContext);
			try {
				VotGESOrdersEntities context=new VotGESOrdersEntities();
				VotGESOrders.Web.ADONETEntities.Orders orderDB=new Orders();
				OrderObjects objDB=(from o in context.OrderObjects where o.objectID == newObj.ObjectID select o).First();

				deleteOrderObject(context, objDB);
				//context.OrderObjects.DeleteObject(objDB);
				
				
				context.SaveChanges();
				OrderObject.init();
				Logger.info("===Сохранено", Logger.LoggerSource.objectsContext);

			} catch (Exception e) {
				Logger.error(String.Format("===Ошибка при удалении оборудования: {0}", e), Logger.LoggerSource.objectsContext);
				throw new DomainException("Ошибка при удалении оборудования, возможно на оборудование (или дочернее оборудование) ссылаются заявки");
			}
		}
	}

}