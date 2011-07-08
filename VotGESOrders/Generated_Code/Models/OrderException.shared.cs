using System;
using System.Collections.Generic;
using System.Linq;

namespace VotGESOrders.Web.Models
{
	public enum ExceptionOperation{create,accept,ban,cancel,open,close,enter,extend,change}
	public class OrderException:Exception
	{
		public ExceptionOperation Operation { get; set; }
		public Order SourceOrder { get; set; }
		public OrderException(Order order, ExceptionOperation oper) {
			Operation = oper;
			SourceOrder = order;
		}

		public override string ToString() {
			string str="";
			switch (Operation) {
				case ExceptionOperation.accept:
					str = "Разрешение заявки";
					break;
				case ExceptionOperation.ban:
					str = "Запрет заявки";
					break;
				case ExceptionOperation.cancel:
					str = "Отмена заявки";
					break;
				case ExceptionOperation.close:
					str = "Разрешение на ввод оборудования";
					break;
				case ExceptionOperation.create:
					str = "Создание заявки";
					break;
				case ExceptionOperation.enter:
					str = "Ввод оборудования";
					break;
				case ExceptionOperation.extend:
					str = "Продление заявки";
					break;
				case ExceptionOperation.open:
					str = "Открытие заявки";
					break;
				case ExceptionOperation.change:
					str = "Изменение заявки";
					break;
			}
			str = String.Format("Ошибка:{0}. Заявка №{1}", str, SourceOrder.OrderNumber);
			return str;
		}
	}

}