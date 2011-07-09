using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VotGESOrders.Web.Logging;
using VotGESOrders.Web.ADONETEntities;
using System.ServiceModel.DomainServices.Server;

namespace VotGESOrders.Web.Models
{
	public class OrdersUserContext
	{

		public void RegisterChangeUser(OrdersUser newUser) {
			Logger.info("Пользователь изменил пользователя");
			try {
				VotGESOrdersEntities context=new VotGESOrdersEntities();

				IQueryable<Users> users=(from u in context.Users where u.userID == newUser.UserID select u);
				Users user=null;
				if (users.Count()==0) {
					Logger.info("Новый пользователь");
					user = new Users();
					user.name = newUser.Name;
					context.Users.AddObject(user);
				} else {
					user = users.First();
				}

				user.fullName = newUser.FullName;
				user.allowAcceptOrder = newUser.AllowAcceptOrder;
				user.allowCancelOrder = newUser.AllowCancelOrder;
				user.allowCloseOrder = newUser.AllowCloseOrder;
				user.allowCreateOrder = newUser.AllowCreateOrder;
				user.allowEditOrders = newUser.AllowEditOrders;
				user.allowEditUsers = newUser.AllowEditUsers;
				user.allowEditTree = newUser.AllowEditTree;
				user.allowEnterOrder = newUser.AllowEnterOrder;
				user.allowExtendOrder = newUser.AllowExtendOrder;
				user.allowOpenOrder = newUser.AllowOpenOrder;
				
				context.SaveChanges();

				newUser.UserID = user.userID;
				OrdersUser.init();
				
				Logger.info("Сохранено");

			} catch (Exception e) {
				Logger.error(String.Format("Ошибка при изменении пользователя: {0}", e));
				throw new DomainException("Ошибка при изменении пользователя");
			}
		}


		public void RegisterDeleteUser(OrdersUser newUser) {
			Logger.info("Пользователь удалил пользователя");
			try {
				VotGESOrdersEntities context=new VotGESOrdersEntities();

				IQueryable<Users> users=(from u in context.Users where u.name.ToLower() == newUser.Name.ToLower() select u);
				Users user=null;
				user = users.First();

				context.DeleteObject(user);

				context.SaveChanges();
				OrdersUser.init();
				Logger.info("Сохранено");

			} catch (Exception e) {
				Logger.error(String.Format("Ошибка при удалении пользователя: {0}", e));
				throw new DomainException("Ошибка при удалении пользователя. Возможно на пользователя ссылаются заявки");
			}
		}
	}

}