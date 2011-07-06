using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VotGESOrders.Web.Controllers
{
	[HandleError]
	public class HomeController : Controller
	{
		public ActionResult Index() {
			ViewData["Message"] = "Добро пожаловать в ASP.NET MVC!";

			return View("VotGESOrders");
		}

		public ActionResult About() {
			return View();
		}
	}
}
