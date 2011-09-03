using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VotGESOrders.Web.Models;

namespace VotGESOrders.Web.Controllers
{
	[HandleError]
	public class HomeController : Controller
	{
		public ActionResult Index() {
			ViewData["Message"] = "Добро пожаловать в ASP.NET MVC!";

			return View("VotGESOrders");
		}
				
		public ActionResult ProcessExpiredOrders() {
			OrdersContext context= new OrdersContext();
			IQueryable<Order> expiredOrders=context.OrdersActiveExpired;
			foreach (Order order in expiredOrders) {
				MailContext.sendMail("Информация о просроченной заявке", order, true);
				Logging.Logger.info(String.Format("Отправка письма о просроченной заявке №{0} - {1}", order.OrderNumber.ToString(OrderInfo.NFI), order.UserCreateOrder.Mail), Logging.Logger.LoggerSource.server);
			}
			return View();
		}

		public ActionResult About() {
			return View();
		}
	}
}
