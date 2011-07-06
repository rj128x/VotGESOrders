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

		public static void save(Guid guid) {
			LastChanges = DateTime.Now;
			LastUpdateGUID = guid;
		}

		public static bool IsChanged(DateTime lastUpdate, Guid lastGUID) {
			//Logger.info(String.Format("Проверка изменений {0}, {1} : {2}, {3}", lastUpdate, lastGUID, LastChanges,LastUpdateGUID));
			if (LastUpdateGUID.HasValue && LastChanges.HasValue) {
				return lastUpdate < LastChanges.Value && LastUpdateGUID!=lastGUID;
			} else {
				return false;
			}
		}
	}
}