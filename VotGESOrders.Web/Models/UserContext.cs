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

		private OrdersUser currentUser;
		public OrdersUser CurrentUser {
			get {
				return currentUser;
			}
			set {
				currentUser = value;
			}
		}
		public OrdersUserContext() {
			CurrentUser = OrdersUser.loadFromCache(HttpContext.Current.User.Identity.Name);
		}

		public void RegisterChangeUser(OrdersUser newUser) {
			Logger.info("Пользователь изменил пользователя", Logger.LoggerSource.usersContext);
			try {
				if (!CurrentUser.AllowEditUsers) {
					throw new DomainException("У вас нет прав редактировать пользователей");
				}
				VotGESOrdersEntities context=new VotGESOrdersEntities();

				IQueryable<Users> users=(from u in context.Users where u.userID == newUser.UserID select u);
				Users user=null;
				if (users.Count()==0) {
					Logger.info("===Новый пользователь", Logger.LoggerSource.usersContext);
					user = new Users();
					user.name = newUser.Name;
					context.Users.AddObject(user);
				} else {
					user = users.First();
				}

				user.fullName = newUser.FullName;
				user.mail = newUser.Mail;
				user.sendAgreeMail = newUser.SendAgreeMail;
				user.sendAllMail = newUser.SendAllMail;
				user.sendCreateMail = newUser.SendCreateMail;
				user.sendAllAgreeMail = newUser.SendAllAgreeMail;
				user.sendAllCreateMail = newUser.SendAllCreateMail;
				user.allowChangeOrder = newUser.AllowChangeOrder;
				user.allowCreateOrder = newUser.AllowCreateOrder;
				user.allowCreateCrashOrder = newUser.AllowCreateCrashOrder;
				user.allowEditUsers = newUser.AllowEditUsers;
				user.allowEditOrders = newUser.AllowEditOrders;
				user.allowEditTree = newUser.AllowEditTree;
				user.allowAgreeOrders = newUser.AllowAgreeOrders;
				user.allowReviewOrder = newUser.AllowReviewOrder;

				
				context.SaveChanges();

				newUser.UserID = user.userID;
				OrdersUser.init();

				Logger.info("===Сохранено", Logger.LoggerSource.usersContext);

			} catch (Exception e) {
				Logger.error(String.Format("===Ошибка при изменении пользователя: {0}", e), Logger.LoggerSource.usersContext);
				if (e is DomainException) {
					throw e;
				}
				throw new DomainException("Ошибка при изменении пользователя");
			}
		}


		public void RegisterDeleteUser(OrdersUser newUser) {
			Logger.info("Пользователь удалил пользователя", Logger.LoggerSource.usersContext);
			try {
				if (!CurrentUser.AllowEditUsers) {
					throw new DomainException("У вас нет прав редактировать пользователей");
				}
				VotGESOrdersEntities context=new VotGESOrdersEntities();

				IQueryable<Users> users=(from u in context.Users where u.name.ToLower() == newUser.Name.ToLower() select u);
				Users user=null;
				user = users.First();

				context.DeleteObject(user);

				context.SaveChanges();
				OrdersUser.init();
				Logger.info("===Сохранено", Logger.LoggerSource.usersContext);

			} catch (Exception e) {
				Logger.error(String.Format("===Ошибка при удалении пользователя: {0}", e), Logger.LoggerSource.usersContext);
				if (e is DomainException) {
					throw e;
				}
				throw new DomainException("Ошибка при удалении пользователя. Возможно на пользователя ссылаются заявки");
			}
		}
	}

}