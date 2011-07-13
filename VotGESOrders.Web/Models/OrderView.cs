using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VotGESOrders.Web.Models
{
	public class OrderView
	{
		public static string getOrderHTML(Order order) {
			string style="<Style>table {border-collapse: collapse;} td{text-align:center;} td, th {border-width: 1px;	border-style: solid;	border-color: #BBBBFF;	padding-left: 3px;	padding-right: 3px;}</Style>";
			string htmlNumber = String.Format("Заявка №{0} от {1}", order.OrderNumber, order.OrderDateCreate.ToString("dd.MM.yy"));
			string htmlState=String.Format("Состояние: {0}", order.OrderStateStr);
			string htmlFirstTRTable=String.Format("<table width='100%'><tr><th>{0}</th><th>{1}</th></tr></table>", htmlNumber, htmlState);
			string htmlInfoTable=String.Format("<table width='100%'><tr><th colspan='3'>Информация о заявке</th></tr><tr><th width='30%'>Оборудование</th><th  width='30%'>Текст заявки</th><th width='30%'>Согласовано</th></tr><tr><td width='30%'>{0}</td><td width='30%'>{1}</td><td width='30%'>{2}</td></tr></table>",
				order.SelOrderObjectText, order.OrderText, order.AgreeText);

			string htmlExtend=order.OrderIsExtend ? "ЗАЯВКА НА ПРОДЛЕНИЕ. СТАРАЯ ЗАЯВКА №" + order.ParentOrderNumber.ToString() : "";
			string htmlExtended=order.OrderExtended ? "ЗАЯВКА ПРОДЛЕНА. НОВАЯ ЗАЯВКА №" + order.ChildOrderNumber.ToString() : "";
			string htmlAskExtended=order.OrderAskExtended ? "ЗАЯВКА ОЖИДАЕТ РАЗРЕШЕНИЯ НА ПРОДЛЕНИЕ. НОВАЯ ЗАЯВКА №" + order.ChildOrderNumber.ToString() : "";
			htmlExtend += order.OrderExtended ? "<br/>" + htmlExtended : "";
			htmlExtend += order.OrderAskExtended ? "<br/>" + htmlAskExtended : "";
			if (htmlExtend.Length > 0)
				htmlExtend += "<br/>";

			string htmlDatesTable = 
				String.Format("<table width='100%'><tr><th colspan='3'>Сроки заявки</th></tr><tr><th>&nbsp;</th><th>Начало</th><th>Окончание</th></tr><tr><th>План</th><td>{0}</td><td>{1}</td></tr><tr><th>Факт</th><td>{2}</td><td>{3}</td></tr><tr><th>Разрешение на ввод</th><td colspan='2'>{4}</td></tr></table>",
				order.PlanStartDate.ToString("dd.MM.yy HH:mm"), order.PlanStopDate.ToString("dd.MM.yy HH:mm"),
				order.FaktStartDate.HasValue ? order.FaktStartDate.Value.ToString("dd.MM.yy HH:mm") : "&nbsp;",
				order.FaktStopDate.HasValue ? order.FaktStopDate.Value.ToString("dd.MM.yy HH:mm") : "&nbsp;",
				order.FaktCompleteDate.HasValue ? order.FaktCompleteDate.Value.ToString("dd.MM.yy HH:mm") : "&nbsp;");

			string formatTR="<tr><th>{0}</th><td>{1}</td><td>{2}</td><td>{3}</td></tr>";
			string htmlCreateTR=String.Format(formatTR, "Создал", order.UserCreateOrder.FullName, order.OrderDateCreate, order.CreateText);
			string htmlAcceptTR=order.OrderAccepted ? String.Format(formatTR, "Разрешил", order.UserAcceptOrder.FullName, order.OrderDateAccept.Value, order.AcceptText) : "";
			string htmlBanTR=order.OrderBanned ? String.Format(formatTR, "Отклонил", order.UserBanOrder.FullName, order.OrderDateBan.Value, order.BanText) : "";
			string htmlOpenTR=order.OrderOpened ? String.Format(formatTR, "Открыл", order.UserOpenOrder.FullName, order.OrderDateOpen.Value, order.OpenText) : "";
			string htmlCloseTR=order.OrderClosed ? String.Format(formatTR, "Разрешил ввод", order.UserCloseOrder.FullName, order.OrderDateClose.Value, order.CloseText) : "";
			string htmlEnterTR=order.OrderCompleted ? String.Format(formatTR, "Завершил", order.UserCompleteOrder.FullName, order.OrderDateComplete.Value, order.CompleteText) : "";
			string htmlCancelTR=order.OrderCanceled ? String.Format(formatTR, "Снял", order.UserCancelOrder.FullName, order.OrderDateCancel.Value, order.CancelText) : "";

			string htmlOper="<tr><th>&nbsp;</th><th>Автор</th><th>Дата</th><th>Комментарий</th></tr>";
			string htmlOperTable=String.Format("<table  width='100%'><tr><th colspan='4'>Операции над заявкой</th></tr> {0}{1}{2}{3}{4}{5}{6}{7}</table>",
				htmlOper, htmlCreateTR, htmlAcceptTR, htmlBanTR, htmlOpenTR, htmlCloseTR, htmlEnterTR, htmlCancelTR);

			string fullTable=String.Format("<table width='100%'><tr><td colspan='2'>{0}</td></tr><tr><td colspan='2'>{1}</td></tr><tr><td>{2}</td><td>{3}</td></tr></table>",
				htmlFirstTRTable, htmlInfoTable, htmlExtend + htmlDatesTable, htmlOperTable);
			return style+fullTable;
		}
	}
}