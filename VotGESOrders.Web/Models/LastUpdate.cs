using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using VotGESOrders.Web.Logging;

namespace VotGESOrders.Web.Models
{
	public class LastUpdate
	{
		public static DateTime? LastChanges { get; protected set; }
		public static Guid? LastUpdateGUID { get; protected set; }

		public static Dictionary<Guid,DateTime> clients=new Dictionary<Guid, DateTime>();

		public static void save(Guid guid) {
			LastChanges = DateTime.Now;
			LastUpdateGUID = guid;
		}

		public static void saveUpdate(Guid guid) {
			if (!clients.ContainsKey(guid)) {
				clients.Add(guid, DateTime.Now);
			} else {
				clients[guid] = DateTime.Now;
			}
		}

		public static bool IsChanged(Guid guid) {			
			if (LastUpdateGUID.HasValue && LastChanges.HasValue) {
				return clients[guid] < LastChanges.Value && LastUpdateGUID != guid;
			} else {
				return false;
			}

		}
	}
}