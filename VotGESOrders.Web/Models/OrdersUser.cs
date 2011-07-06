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
		public string Name{get;protected set;}
		public string FullName{get;protected set;}
		public bool AllowCreateOrder { get; protected set; }
		public bool AllowOpenOrder { get; protected set; }
		public bool AllowAcceptOrder { get; protected set; }
		public bool AllowCloseOrder { get; protected set; }
		public bool AllowEnterOrder { get; protected set; }
		public bool AllowExtendOrder { get; protected set; }
		public bool AllowCancelOrder { get; protected set; }

		protected static VotGESOrdersEntities context;
		protected static List<OrdersUser> allUsers=new List<OrdersUser>();
		static OrdersUser(){
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
				user.FullName = String.Format("{0} (нет в базе)",user.Name);
				user.Name = userName;
				Logger.error(String.Format("Ошибка при получении краткой информации о пользователе из БД: {0}, {1}", userName, e));
				return user;
			}
		}

		public static OrdersUser getFromDB(Users userDB) {			
			try {
				OrdersUser user=new OrdersUser();
				user.Name = userDB.name;
				user.FullName = userDB.fullName;
				user.AllowCreateOrder = userDB.allowCreateOrder.Value;
				user.AllowOpenOrder = userDB.allowOpenOrder.Value;
				user.AllowCloseOrder = userDB.allowCloseOrder.Value;
				user.AllowAcceptOrder = userDB.allowAcceptOrder.Value;
				user.AllowEnterOrder = userDB.allowEnterOrder.Value;
				user.AllowExtendOrder = userDB.allowExtendOrder.Value;
				user.AllowCancelOrder = userDB.allowCancelOrder.Value;
				return user;
			} catch (Exception e) {
				Logger.error(String.Format("Ошибка при получении краткой информации о пользователе: {0}",e));
			}
			return null;
		}						
		
		public override string ToString() {
			return string.Format("Name: {0}, FullName: {1}, AllowCreateOrder: {2}, AllowOpenOrder: {3}, AllowAcceptOrder: {4}, AllowCancelOrder: {5},AllowEnterOrder: {6}, AllowExtendOrder: {7}",
				Name, FullName, AllowCreateOrder, AllowOpenOrder, AllowAcceptOrder,AllowCancelOrder, AllowEnterOrder, AllowExtendOrder);
		}
	}
}