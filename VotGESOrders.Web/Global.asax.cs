﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using VotGESOrders.Web.Logging;
using VotGESOrders.Web.Models;
using System.Configuration;

namespace VotGESOrders.Web
{
	// Примечание: Инструкции по включению классического режима IIS6 или IIS7 
	// см. по ссылке http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{
		
		public static void RegisterRoutes(RouteCollection routes) {
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				 "Default", // Имя маршрута
				 "{controller}/{action}/{id}", // URL-адрес с параметрами
				 new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Параметры по умолчанию
			);

		}

		protected void Application_Start() {
			Logger.init(Server.MapPath("/logs/"), "orders");
			OrderObject.init();
			AreaRegistration.RegisterAllAreas();

			Logger.info(System.Configuration.ConfigurationManager.AppSettings["smtpServer"], Logger.LoggerSource.client);
			RegisterRoutes(RouteTable.Routes);

			

		}
	}
}