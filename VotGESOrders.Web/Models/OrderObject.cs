using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using VotGESOrders.Web.ADONETEntities;
using VotGESOrders.Web.Logging;
using System.ServiceModel.DomainServices.Server;

namespace VotGESOrders.Web.Models
{
	public class OrderObject
	{
		
		public string ObjectName { get;  set; }
		public string FullName { get; set; }

		[Key]
		public int ObjectID{get; set;}
		public int ParentObjectID{get; set;}


		private OrderObject parentObject;
		[Include]
		[Association("Order_OrderObject1", "ParentObjectID", "ObjectID")]
		public OrderObject ParentObject {
			get {
				return parentObject;
			}
			set {
				parentObject = value;
				ParentObjectID = value==null?0:value.ObjectID;
			}
		}

		[Include]
		[Association("Order_OrderObject2", "ObjectID", "ParentObjectID")]
		public List<OrderObject> ChildObjects { get;  set; }



		protected static VotGESOrdersEntities context;
		protected static Dictionary<int,OrderObject> allObjects;

		public static void init() {
			Logger.info("Чтение списка объектов из БД");
			allObjects = new Dictionary<int, OrderObject>();
			context = new VotGESOrdersEntities();

			VotGESOrdersEntities ctx = new VotGESOrdersEntities();
			IQueryable<OrderObjects> dbObjects=from oo in ctx.OrderObjects orderby oo.objectName select oo ;
			foreach (OrderObjects dbObject in dbObjects) {
				allObjects.Add(dbObject.objectID, getFromDB(dbObject));
			}
			Logger.info("Чтение родительских объектов из БД");
			createParentObjects();
			Logger.info("создание дерева");
			createTree();
			createNames();
			Logger.info("Чтение списка объектов из БД завершено");
		}

		static OrderObject() {
			
		}

		protected static void createParentObjects() {
			foreach (KeyValuePair<int,OrderObject> de in allObjects) {
				OrderObject obj=de.Value;
				int parentID=obj.ParentObjectID;
				if ((parentID!=0)&&(allObjects.ContainsKey(parentID))) {
					obj.ParentObject = allObjects[parentID];
				} else {
					obj.ParentObject = null;
				}
					
			}
		}

		protected static void createTree() {
			foreach (KeyValuePair<int,OrderObject> de in allObjects) {
				OrderObject obj=de.Value;
				obj.ChildObjects = new List<OrderObject>((from o in allObjects.Values where o.ParentObjectID == obj.ObjectID select o));
			}
		}

		protected static void createNames() {
			foreach (KeyValuePair<int,OrderObject> de in allObjects) {
				OrderObject obj=de.Value;
				
				OrderObject parent=obj.ParentObject;
				List<string> names=new List<string>();
				names.Add(obj.ObjectName);
				while (parent != null) {
					names.Add(parent.ObjectName);
					parent = parent.ParentObject;
				}
				names.Reverse();
				obj.FullName = String.Join(" => ", names); ;
			}
		}

		public static List<int> getObjectIDSByFullName(string fullName) {
			return new List<int>(from OrderObject o in allObjects.Values where o.FullName.Contains(fullName) select o.ObjectID);
		}

		public void appendObjectIDSChildIDS(List<int> ObjectIDS) {
			foreach (OrderObject obj in ChildObjects) {
				if (!ObjectIDS.Contains(obj.ObjectID)) {
					ObjectIDS.Add(obj.ObjectID);
					obj.appendObjectIDSChildIDS(ObjectIDS);
				}
			}
		}

		public static IQueryable<OrderObject> getAllObjects() {
			return allObjects.Values.AsQueryable();
		}


		public static OrderObject getFromDB(OrderObjects objectDB) {
			try {
				OrderObject obj=new OrderObject();
				obj.ObjectName=objectDB.objectName;
				obj.ObjectID = objectDB.objectID;
				obj.ParentObjectID = objectDB.parentID;
				return obj;
			} catch (Exception e) {
				Logger.error(String.Format("Ошибка при получении информации об оборудовании: {0}", e));
			}
			return null;
		}

		public static OrderObject getByID(int id) {
			if (allObjects.ContainsKey(id)) {
				return allObjects[id];
			} else {
				return null;
			}
		}

		public override string ToString() {
			return string.Format("ID: {0}, ParentID: {1}, Name: {2}",
				ObjectID,ParentObjectID,ObjectName);
		}
	}
}