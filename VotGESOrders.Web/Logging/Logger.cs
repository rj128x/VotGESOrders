using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net.Layout;
using log4net.Appender;
using log4net.Config;
using log4net;

namespace VotGESOrders.Web.Logging
{
	public static class Logger
	{
		public static log4net.ILog logger;
		static Logger() {

		}
		public static void init(string path, string name) {
			string fileName=String.Format("{0}/{1}_{2}.txt", path, name, DateTime.Now.ToShortDateString().Replace(":", "_").Replace("/", "_").Replace(".", "_"));
			PatternLayout layout = new PatternLayout(@"[%d] %-10p %m%n");
			FileAppender appender=new FileAppender();			
			appender.Layout = layout;
			appender.File = fileName;
			appender.AppendToFile = true;
			BasicConfigurator.Configure(appender);
			appender.ActivateOptions();
			logger = LogManager.GetLogger(name);
		}

		public static string createMessage(string message, string source="") {
			try {
				string user=HttpContext.Current.User.Identity.IsAuthenticated ? HttpContext.Current.User.Identity.Name : "Anonimous";
				string ip= HttpContext.Current.Request.UserHostAddress;
				return String.Format("{0,-30} {1,-15} {2,-20} {3}", user, ip, source, message);
			}catch{
				return String.Format("{0,-30} {1,-15} {2,-20} {3}", "", "", source, message);;
			}
		}

		public static void info(string str, string source) {			
			logger.Info(createMessage(str,source));
		}

		public static void error(string str, string source) {
			logger.Error(createMessage(str,source));
		}

		public static void debug(string str, string source) {
			logger.Debug(createMessage(str,source));
		}

	}
}