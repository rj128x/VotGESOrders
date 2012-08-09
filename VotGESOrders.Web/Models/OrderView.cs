using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VotGESOrders.Web.Models
{
	public class OrderView
	{
		public static string getOrderHTML(Order order, bool showStyle=true) {			
			string style=showStyle?"<Style>table {border-collapse: collapse;} td{text-align:center;} td.comments{text-align:left;} td, th {border-width: 1px;	border-style: solid;	border-color: #BBBBFF;	padding-left: 3px;	padding-right: 3px;}</Style>":"";
			string htmlNumber = String.Format("Заявка {0} №{1} от {2}", order.OrderTypeShortName, order.OrderNumber.ToString(OrderInfo.NFI), order.OrderDateCreate.ToString("dd.MM.yy"));
			string htmlState=String.Format("Состояние: {0}", order.OrderStateStr);
			string htmlReady=String.Format("Ав.готовность: {0}", order.ReadyTime);
			string htmlFirstTRTable=String.Format("<table width='100%'><tr><th>{0}</th><th>{1}</th><th>{2}</th></tr></table>", htmlNumber, htmlReady, htmlState);
			string htmlInfoTable=String.Format("<table width='100%'><tr><th colspan='3'>Информация о заявке</th></tr><tr><th width='30%'>Оборудование</th><th  width='30%'>Текст заявки</th><th width='30%'>Согласовано</th></tr><tr><td width='30%'>{0}</td><td width='30%'>{1}</td><td width='30%'>{2}</td></tr></table>",
				order.FullOrderObjectInfo, order.OrderText, order.AgreeText);

			string htmlExtend=order.OrderHasParentOrder ? "СТАРАЯ ЗАЯВКА №" + order.ParentOrderNumber.ToString(OrderInfo.NFI) : "";
			string htmlExtended=order.OrderHasChildOrder ? "НОВАЯ ЗАЯВКА №" + order.ChildOrderNumber.ToString(OrderInfo.NFI) : "";			
			htmlExtend += order.OrderExtended ? "<br/>" + htmlExtended : "";
			if (htmlExtend.Length > 0)
				htmlExtend += "<br/>";

			string htmlDatesTable = 
				String.Format("<table width='100%'><tr><th colspan='3'>Сроки заявки</th></tr><tr><th>&nbsp;</th><th>Начало</th><th>Окончание</th></tr><tr><th>План</th><td>{0}</td><td>{1}</td></tr><tr><th>Факт</th><td>{2}</td><td>{4}</td></tr><tr><th>Разрешение на ввод</th><td colspan='2'>{3}</td></tr></table>",
				order.PlanStartDate.ToString("dd.MM.yy HH:mm"), order.PlanStopDate.ToString("dd.MM.yy HH:mm"),
				order.FaktStartDate.HasValue ? order.FaktStartDate.Value.ToString("dd.MM.yy HH:mm") : "&nbsp;",
				order.FaktStopDate.HasValue ? order.FaktStopDate.Value.ToString("dd.MM.yy HH:mm") : "&nbsp;",
				order.FaktCompleteDate.HasValue ? order.FaktCompleteDate.Value.ToString("dd.MM.yy HH:mm") : "&nbsp;");

			string formatTR="<tr><th>{0}</th><td>{1}</td><td>{2}</td><td>{3}</td></tr>";
			string htmlCreateTR=String.Format(formatTR, "Создал", order.UserCreateOrder.FullName, order.OrderDateCreate.ToString("dd.MM.yy HH:mm"), order.CreateText);
			string htmlAcceptTR=order.OrderReviewed ? String.Format(formatTR, "Рассмотрел", order.UserReviewOrder.FullName, order.OrderDateReview.Value.ToString("dd.MM.yy HH:mm"), order.ReviewText) : "";
			string htmlOpenTR=order.OrderOpened ? String.Format(formatTR, "Открыл", order.UserOpenOrder.FullName, order.OrderDateOpen.Value.ToString("dd.MM.yy HH:mm"), order.OpenText) : "";
			string htmlCloseTR=order.OrderClosed ? String.Format(formatTR, "Разрешил ввод", order.UserCloseOrder.FullName, order.OrderDateClose.Value.ToString("dd.MM.yy HH:mm"), order.CloseText) : "";
			string htmlEnterTR=order.OrderCompleted ? String.Format(formatTR, "Завершил", order.UserCompleteOrder.FullName, order.OrderDateComplete.Value.ToString("dd.MM.yy HH:mm"), order.CompleteText) : "";
			string htmlCancelTR=order.OrderCanceled ? String.Format(formatTR, "Снял", order.UserCancelOrder.FullName, order.OrderDateCancel.Value.ToString("dd.MM.yy HH:mm"), order.CancelText) : "";
			string htmlCommentsTR=!String.IsNullOrEmpty(order.CommentsText) ? 
				String.Format("<tr><td colspan='4' class='comments'>{0}</td></tr>", order.CommentsText.Replace(" ","&nbsp;").Replace("\n","<br/>")) : "";

			string htmlOper="<tr><th>&nbsp;</th><th>Автор</th><th>Дата</th><th>Комментарий</th></tr>";
			string htmlOperTable=String.Format("<table  width='100%'><tr><th colspan='4'>Операции над заявкой</th></tr> {0}{1}{2}{3}{4}{5}{6}{7}</table>",
				htmlOper, htmlCreateTR, htmlAcceptTR, htmlOpenTR, htmlCloseTR, htmlEnterTR, htmlCancelTR, htmlCommentsTR);

			string fullTable=String.Format("<table width='100%'><tr><td colspan='2'>{0}</td></tr><tr><td colspan='2'>{1}</td></tr><tr><td>{2}</td><td>{3}</td></tr></table>",
				htmlFirstTRTable, htmlInfoTable, htmlExtend + htmlDatesTable, htmlOperTable);
			return style+fullTable;
		}
	}
}