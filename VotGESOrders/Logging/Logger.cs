using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using VotGESOrders.Web.Services;

namespace VotGESOrders.Logging
{
	public class Logger
	{
		private static Logger logger;
		private LoggerContext loggerContext;
		
		private Logger() {
		}

		static Logger(){
			logger = new Logger();
		}		

		public static void init(LoggerContext context) {
			logger.loggerContext = context;
		}

		public static void info(string message) {
			logger.loggerContext.info(message,
				oper => {
					if (oper.HasError) {
						oper.MarkErrorAsHandled();
					}
				}, null);
		}

		public static void showMessage(string message) {
			System.Windows.Browser.HtmlPage.Window.Invoke("showMessage", message);
		}

		public static void logMessage(string message) {
			System.Windows.Browser.HtmlPage.Window.Invoke("logMessage", message);
		}


	}
}
