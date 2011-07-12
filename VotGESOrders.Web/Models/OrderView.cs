﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VotGESOrders.Web.Models
{
	public class OrderView
	{
		public static string getOrderHTML(Order order) {
			string htmlNumber = String.Format("Заявка №{0} от {1}", order.OrderNumber, order.OrderDateCreate.ToString("dd.MM.yy"));
			string htmlState=String.Format("Состояние: {0}", order.OrderStateStr);
			string htmlFirstTRTable=String.Format("<table width='100%'><tr><th><td align='left'>{0}<th><th align='right'>{1}</th></tr></table>", htmlNumber, htmlState);
			string htmlInfoTable=String.Format("<table width='100%'><tr><th>Оборудование</th><th>Текст заявки</th><th>Согласовано</th></tr><tr><td>{0}</td><td>{1}</td><td>{2}</td></tr></table>",
				order.SelOrderObjectText, order.OrderText, order.SoglasText);

			string htmlExtend=order.OrderIsExtend ? "ЗАЯВКА НА ПРОДЛЕНИЕ. СТАРАЯ ЗАЯВКА №" + order.ParentOrderNumber.ToString() : "";
			string htmlExtended=order.OrderExtended ? "ЗАЯВКА ПРОДЛЕНА. НОВАЯ ЗАЯВКА №" + order.ChildOrder.ToString() : "";
			string htmlAskExtended=order.OrderAskExtended ? "ЗАЯВКА ОЖИДАЕТ РАЗРЕШЕНИЯ НА ПРОДЛЕНИЕ. НОВАЯ ЗАЯВКА №" + order.ChildOrder.ToString() : "";
			htmlExtend += order.OrderExtended ? "<br/>" + htmlExtended : "";
			htmlExtend += order.OrderAskExtended ? "<br/>" + htmlAskExtended : "";
			if (htmlExtend.Length > 0)
				htmlExtend += "<br/>";

			string htmlDatesTable = 
				String.Format("<table width='100%'><tr><th>&nbsp;</th><th>Начало</th><th>Окончание</th></tr><tr><th>План</th><td>{0}</td><td>{1}</td></tr><tr><th>Факт</th><td>{2}</td><td>{3}</td></tr><tr><th>Разрешение на ввод</th><td colspan='2'>{4}</td></tr></table>",
				order.PlanStartDate.ToString("dd.MM.yy HH:mm"), order.PlanStopDate.ToString("dd.MM.yy HH:mm"),
				order.FaktStartDate.HasValue ? order.FaktStartDate.Value.ToString("dd.MM.yy HH:mm") : "&nbsp;",
				order.FaktStopDate.HasValue ? order.FaktStopDate.Value.ToString("dd.MM.yy HH:mm") : "&nbsp;",
				order.FaktEnterDate.HasValue ? order.FaktEnterDate.Value.ToString("dd.MM.yy HH:mm") : "&nbsp;");

			string formatTR="<tr><th>{0}</th><td>{1}</td><td>{2}</td><td>{3}</td></tr>";
			string htmlCreateTR=String.Format(formatTR, "Создал", order.UserCreateOrder.FullName, order.OrderDateCreate, order.CreateText);
			string htmlAcceptTR=order.OrderAccepted ? String.Format(formatTR, "Разрешил", order.UserAcceptOrder.FullName, order.OrderDateAccept.Value, order.AcceptText) : "";
			string htmlBanTR=order.OrderBanned ? String.Format(formatTR, "Отклонил", order.UserBanOrder.FullName, order.OrderDateBan.Value, order.BanText) : "";
			string htmlOpenTR=order.OrderOpened ? String.Format(formatTR, "Открыл", order.UserOpenOrder.FullName, order.OrderDateOpen.Value, order.OpenText) : "";
			string htmlCloseTR=order.OrderClosed ? String.Format(formatTR, "Разрешил ввод", order.UserCloseOrder.FullName, order.OrderDateClose.Value, order.CloseText) : "";
			string htmlEnterTR=order.OrderEntered ? String.Format(formatTR, "Завершил", order.UserEnterOrder.FullName, order.OrderDateEnter.Value, order.EnterText) : "";
			string htmlCancelTR=order.OrderCanceled ? String.Format(formatTR, "Снял", order.UserCancelOrder.FullName, order.OrderDateCancel.Value, order.CancelText) : "";

			string htmlOper="<tr><th>&nbsp;</th><th>Автор</th><th>Дата</th><th>Комментарий</th></tr>";
			string htmlOperTable=String.Format("<table  width='100%'>{0}{1}{2}{3}{4}{5}{6}{7}</table>",
				htmlOper, htmlCreateTR, htmlAcceptTR, htmlBanTR, htmlOpenTR, htmlCloseTR, htmlEnterTR, htmlCancelTR);

			string fullTable=String.Format("<table width='100%'><tr><td colspan='2'>{0}</td></tr><tr><td colspan='2'>{1}</td></tr><tr><td>{2}</td><td>{3}</td></tr></table>",
				htmlFirstTRTable, htmlInfoTable, htmlExtend + htmlDatesTable, htmlOperTable);
			return fullTable;
		}
	}
}