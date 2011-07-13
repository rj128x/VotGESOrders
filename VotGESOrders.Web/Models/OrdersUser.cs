using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VotGESOrders.Web.Logging;
using VotGESOrders.Web.ADONETEntities;
using System.ComponentModel.DataAnnotations;
using System.Data.Objects.DataClasses;
using System.Runtime.Serialization;
using System.Data;

namespace VotGESOrders.Web.Models
{
	public class OrdersUser
	{
		[Key]
		public int UserID { get; set; }
		public string Name { get; set; }
		public string FullName { get; set; }
		public string Mail { get; set; }
		public bool SendAllMail { get; set; }
		public bool SendAgreeMail { get; set; }
		public bool SendCreateMail { get; set; }
		public bool AllowCreateOrder { get; set; }
		public bool AllowOpenOrder { get; set; }
		public bool AllowAcceptOrder { get; set; }
		public bool AllowCloseOrder { get; set; }
		public bool AllowCompleteOrder { get; set; }
		public bool AllowExtendOrder { get; set; }
		public bool AllowCancelOrder { get; set; }
		public bool AllowEditTree { get; set; }
		public bool AllowEditOrders { get; set; }
		public bool AllowEditUsers { get; set; }
		public bool AllowAgreeOrders { get; set; }

		protected static VotGESOrdersEntities context;
		protected static List<OrdersUser> allUsers;
		static OrdersUser() {
			init();
		}

		public static void init() {
			allUsers = new List<OrdersUser>();
			context = new VotGESOrdersEntities();

			VotGESOrdersEntities ctx = new VotGESOrdersEntities();
			IQueryable<Users> dbUsers=from u in ctx.Users select u;
			foreach (Users dbUser in dbUsers) {
				allUsers.Add(getFromDB(dbUser));
			}
		}

		public static IQueryable<OrdersUser> getAllUsers() {
			return allUsers.AsQueryable();
		}

		public static OrdersUser loadFromCache(string userName) {
			//Logger.debug(String.Format("Получение краткой информации о пользователе из БД: {0}", userName));
			try {
				//Users userDB=context.Users.First(u => u.name.ToLower() == userName.ToLower());				
				//Users userDB =(from u in context.Users where u.name.ToLower()==userName.ToLower() select u).First();
				OrdersUser user=allUsers.AsQueryable().First(u => u.Name.ToLower() == userName.ToLower());
				return user;
			} catch (Exception e) {
				OrdersUser user=new OrdersUser();
				user.FullName = String.Format("{0} (нет в базе)", userName);
				user.Name = userName;
				user.UserID = -1;
				Logger.error(String.Format("Ошибка при получении краткой информации о пользователе из БД: {0}, {1}", userName, e));
				return user;
			}
		}

		public static OrdersUser loadFromCache(int userID) {
			//Logger.debug(String.Format("Получение краткой информации о пользователе из БД: {0}", userName));
			try {
				//Users userDB=context.Users.First(u => u.name.ToLower() == userName.ToLower());				
				//Users userDB =(from u in context.Users where u.name.ToLower()==userName.ToLower() select u).First();
				OrdersUser user=allUsers.AsQueryable().First(u => u.UserID == userID);
				return user;
			} catch (Exception e) {
				OrdersUser user=new OrdersUser();
				user.FullName = String.Format("{0} (нет в базе)", userID);
				user.Name = userID.ToString();
				user.UserID = -1;
				Logger.error(String.Format("Ошибка при получении краткой информации о пользователе из БД: {0}, {1}", userID, e));
				return user;
			}
		}

		public static OrdersUser getFromDB(Users userDB) {
			try {
				OrdersUser user=new OrdersUser();
				user.UserID = userDB.userID;
				user.Name = userDB.name;
				user.FullName = userDB.fullName;
				user.Mail = userDB.mail;
				user.SendAgreeMail = userDB.sendAgreeMail;
				user.SendAllMail = userDB.sendAllMail;
				user.SendCreateMail = userDB.sendCreateMail;
				user.AllowCreateOrder = userDB.allowCreateOrder;
				user.AllowOpenOrder = userDB.allowOpenOrder;
				user.AllowCloseOrder = userDB.allowCloseOrder;
				user.AllowAcceptOrder = userDB.allowAcceptOrder;
				user.AllowCompleteOrder = userDB.allowCompleteOrder;
				user.AllowExtendOrder = userDB.allowExtendOrder;
				user.AllowCancelOrder = userDB.allowCancelOrder;
				user.AllowEditTree = userDB.allowEditTree;
				user.AllowEditUsers = userDB.allowEditUsers;
				user.AllowEditOrders = userDB.allowEditOrders;
				user.AllowAgreeOrders = userDB.allowAgreeOrders;
				return user;
			} catch (Exception e) {
				Logger.error(String.Format("Ошибка при получении краткой информации о пользователе: {0}", e));
			}
			return null;
		}

		public override string ToString() {
			return string.Format("Name: {0}, FullName: {1}, AllowCreateOrder: {2}, AllowOpenOrder: {3}, AllowAcceptOrder: {4}, AllowCancelOrder: {5},AllowCompleteOrder: {6}, AllowExtendOrder: {7}",
				Name, FullName, AllowCreateOrder, AllowOpenOrder, AllowAcceptOrder, AllowCancelOrder, AllowCompleteOrder, AllowExtendOrder);
		}
	}
}