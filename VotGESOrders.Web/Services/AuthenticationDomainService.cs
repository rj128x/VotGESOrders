using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.DomainServices.Hosting;
using System.ServiceModel.DomainServices.Server;
using System.ServiceModel.DomainServices.Server.ApplicationServices;
using VotGESOrders.Web.ADONETEntities;
using VotGESOrders.Web.Logging;
using VotGESOrders.Web.Models;


namespace VotGESOrders.Web.Services
{
	[EnableClientAccess]
	public class AuthenticationDomainService : AuthenticationBase<User>
	{
		protected override User GetAuthenticatedUser(System.Security.Principal.IPrincipal principal) {
			User user=base.GetAuthenticatedUser(principal);
			user.loadFromDB(user.Name);
			Logger.info(String.Format("Пользователь авторизовался в системе: {0}", user));

			return user;
		}
	}

	public class User : UserBase
	{
		// NOTE: Profile properties can be added here 
		// To enable profiles, edit the appropriate section of web.config file.

		// public string MyProfileProperty { get; set; }

		protected OrdersUser ordersUser;

		public void loadFromDB(string userName) {
			ordersUser = OrdersUser.loadFromCache(userName);
			FullName = ordersUser.FullName;
			AllowCreateOrder = ordersUser.AllowCreateOrder;
			AllowCreateCrashOrder = ordersUser.AllowCreateCrashOrder;
			AllowEditTree = ordersUser.AllowEditTree;
			AllowEditOrders = ordersUser.AllowEditOrders;
			AllowEditUsers = ordersUser.AllowEditUsers;
			UserID = ordersUser.UserID;
		}

		[ProfileUsage(IsExcluded = true)]
		public string FullName { get; set; }

		[ProfileUsage(IsExcluded = true)]
		public bool AllowCreateOrder { get; set; }
		[ProfileUsage(IsExcluded = true)]
		public bool AllowCreateCrashOrder { get; set; }
		[ProfileUsage(IsExcluded = true)]
		public bool AllowEditTree { get; set; }
		[ProfileUsage(IsExcluded = true)]
		public bool AllowEditOrders { get; set; }
		[ProfileUsage(IsExcluded = true)]
		public bool AllowEditUsers { get; set; }
		[ProfileUsage(IsExcluded = true)]
		public int UserID { get; set; }


		public override string ToString() {
			return ordersUser.ToString();
		}
	}
}


