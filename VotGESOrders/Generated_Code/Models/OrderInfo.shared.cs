using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace VotGESOrders.Web.Models
{
	
	public class OrderInfo
	{
		public static Dictionary<OrderTypeEnum,string> OrderTypes=new Dictionary<OrderTypeEnum, string>();
		public static Dictionary<OrderTypeEnum,string> OrderTypesShort=new Dictionary<OrderTypeEnum, string>();
		public static Dictionary<OrderStateEnum,string> OrderStates=new Dictionary<OrderStateEnum, string>();
		public static NumberFormatInfo NFI;


		static OrderInfo() {
			NFI = new NumberFormatInfo();
			NFI.NumberDecimalSeparator = "/";
			//NFI.NumberDecimalDigits = 3;			
			

			OrderTypes.Add(OrderTypeEnum.crash, "Аварийная");
			OrderTypes.Add(OrderTypeEnum.pl, "Плановая");
			OrderTypes.Add(OrderTypeEnum.npl, "Неплановая");
			OrderTypes.Add(OrderTypeEnum.no, "Неотложная");

			OrderTypesShort.Add(OrderTypeEnum.crash, "АВ");
			OrderTypesShort.Add(OrderTypeEnum.pl, "ПЛН");
			OrderTypesShort.Add(OrderTypeEnum.npl, "НПЛ");
			OrderTypesShort.Add(OrderTypeEnum.no, "НО");

			OrderStates.Add(OrderStateEnum.accepted,"Разрешена");
			OrderStates.Add(OrderStateEnum.banned,"Отклонена");
			OrderStates.Add(OrderStateEnum.closed,"Работы завершены");
			OrderStates.Add(OrderStateEnum.created,"Создана");
			OrderStates.Add(OrderStateEnum.extended,"Продлена");
			OrderStates.Add(OrderStateEnum.askExtended,"Заявка на продление");
			OrderStates.Add(OrderStateEnum.opened,"Открыта");
			OrderStates.Add(OrderStateEnum.canceled,"Снята");
			OrderStates.Add(OrderStateEnum.completed,"Закрыта");
			OrderStates.Add(OrderStateEnum.completedWithoutEnter, "Закрыта без ввода");
		}

		
	}
}