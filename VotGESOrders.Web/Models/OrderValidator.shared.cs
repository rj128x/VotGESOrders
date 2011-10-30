using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace VotGESOrders.Web.Models
{
	public static class OrderValidator
	{
		
		public static ValidationResult ValidatePlanStartDate(DateTime date, ValidationContext context) {
			Order order=context.ObjectInstance as Order;
			if (order.OrderOperation!=OrderOperationEnum.create) {
				return ValidationResult.Success;
			}
			if (date < DateTime.Now.AddMinutes(0) && !order.OrderIsExtend && !order.OrderIsFixErrorEnter &&
				order.OrderType != OrderTypeEnum.crash && order.OrderType != OrderTypeEnum.no)
				return new ValidationResult(String.Format("Плановая дата начала({0}) раньше текущей даты", date));
			if (order.OrderIsFixErrorEnter) {
				if (order.ParentOrder != null && order.ParentOrder.FaktStopDate > date) 
					return new ValidationResult(String.Format("Фактический отказ оборудования ({0}) раньше даты разрешения на ввод (родительская заявка)({1})", date, order.ParentOrder.FaktStopDate));				
			}
			if ((order.OrderType == OrderTypeEnum.crash || order.OrderType == OrderTypeEnum.no) && !order.OrderIsExtend && date > DateTime.Now.AddMinutes(0))
				return new ValidationResult(String.Format("Фактический вывод оборудования ({0}) позже текущей даты", date));
			return ValidationResult.Success;
		}

		public static ValidationResult ValidatePlanStopDate(DateTime date, ValidationContext context) {
			Order order=context.ObjectInstance as Order;
			if (order.OrderOperation != OrderOperationEnum.create) {
				return ValidationResult.Success;
			}
			if (date < order.PlanStartDate)
				return new ValidationResult(String.Format("Плановая дата завершения({0}) раньше даты планового начала({1})", date, order.PlanStartDate));
			if (date < DateTime.Now.AddMinutes(0))
				return new ValidationResult(String.Format("Плановая дата окончания({0}) раньше текущей даты", date));
			return ValidationResult.Success;
		}

		public static ValidationResult ValidateFaktStartDate(DateTime? date, ValidationContext context) {
			Order order=context.ObjectInstance as Order;
			if (order.OrderOperation != OrderOperationEnum.open) {
				return ValidationResult.Success;
			}
			if (date.Value.AddHours(6) < order.PlanStartDate)
				return new ValidationResult(String.Format("Дата начала работ({0}) раньше даты планового начала({1}) больше 6 часов", date, order.PlanStartDate));
			if (date.Value > order.PlanStopDate)
				return new ValidationResult(String.Format("Дата начала работ({0}) позже планового окончания({1})", date, order.PlanStopDate));
			if (date < order.OrderDateReview && !order.OrderIsExtend && !order.OrderIsFixErrorEnter && order.OrderType != OrderTypeEnum.crash && order.OrderType!=OrderTypeEnum.no)
				return new ValidationResult(String.Format("Дата начала работ({0}) раньше даты разрешения({1})", date, order.OrderDateReview));
			if (date > DateTime.Now.AddMinutes(0) && !order.OrderIsExtend) {
				return new ValidationResult(String.Format("Дата начала работ({0}) позже текущей даты", date));
			}
			return ValidationResult.Success;
		}

		public static ValidationResult ValidateFaktStopDate(DateTime? date, ValidationContext context) {
			Order order=context.ObjectInstance as Order;
			if (order.OrderOperation != OrderOperationEnum.close) {
				return ValidationResult.Success;
			}
			if (date < order.FaktStartDate)
				return new ValidationResult(String.Format("Дата разрешения на ввод({0}) раньше даты фактического начала({1})", date, order.FaktStartDate));

			if (date > DateTime.Now.AddMinutes(0) && !order.OrderExtended && !order.OrderAskExtended) {
				return new ValidationResult(String.Format("Дата разрешения на ввод ({0}) позже текущей даты", date));
			}
			return ValidationResult.Success;
		}

		public static ValidationResult ValidateFaktCompleteDate(DateTime? date, ValidationContext context) {
			Order order=context.ObjectInstance as Order;
			if (order.OrderOperation != OrderOperationEnum.complete) {
				return ValidationResult.Success;
			}
			if (date < order.FaktStopDate)
				return new ValidationResult(String.Format("Дата закрытия({0}) раньше даты разрешения на ввод({1})", date, order.FaktStopDate));

			if (date > DateTime.Now.AddMinutes(0) && !order.OrderExtended && !order.OrderAskExtended && order.OrderState != OrderStateEnum.completedWithoutEnter) {
				return new ValidationResult(String.Format("Дата закрытия ({0}) позже текущей даты", date));
			}
			
			return ValidationResult.Success;
		}

	}
}