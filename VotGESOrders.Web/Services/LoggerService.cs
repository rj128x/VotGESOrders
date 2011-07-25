
namespace VotGESOrders.Web.Services
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Linq;
	using System.ServiceModel.DomainServices.Hosting;
	using System.ServiceModel.DomainServices.Server;
	using VotGESOrders.Web.Logging;


	// TODO: Create methods containing your application logic.
	[EnableClientAccess()]
	public class LoggerService : DomainService
	{
		public void info(string message) {
			Logger.info(String.Format("{0}", message), Logger.LoggerSource.client);
		}

		public void error(string message) {
			Logger.error(String.Format("{0}", message), Logger.LoggerSource.client);
		}

		public void debug(string message) {
			Logger.debug(String.Format("{0}", message), Logger.LoggerSource.client);
		}

		
	}
}


