using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VotGESOrders.Web.Models;
using System.Net.Mail;

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

		public ActionResult TestMail() {
			System.Net.Mail.MailMessage mess =	new System.Net.Mail.MailMessage();

			mess.From = new MailAddress("ChekunovaMV@votges.rushydro.ru");
			mess.Subject = "test"; mess.Body = "test message";
			mess.To.Add("ChekunovaMV@votges.rushydro.ru");
			mess.To.Add("rj128x@gmail.com");
			

			mess.SubjectEncoding = System.Text.Encoding.UTF8;
			mess.BodyEncoding = System.Text.Encoding.UTF8;
			mess.IsBodyHtml = true;
			System.Net.Mail.SmtpClient client =	new System.Net.Mail.SmtpClient("mx-votges-121.corp.gidroogk.com", 25);
			client.EnableSsl = true;
			client.Credentials = new System.Net.NetworkCredential("ChekunovaMV", "320204","CORP");			
			
			// Отправляем письмо
			client.Send(mess);
			return View();
		}

		public ActionResult About() {
			return View();
		}
	}
}
