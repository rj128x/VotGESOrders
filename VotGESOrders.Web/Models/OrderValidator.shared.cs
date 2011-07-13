﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace VotGESOrders.Web.Models
{
	public static class OrderValidator
	{

		public static ValidationResult ValidatePlanStartDate(DateTime date, ValidationContext context) {
			return ValidationResult.Success;
			Order order=context.ObjectInstance as Order;
			if (order.OrderState != OrderStateEnum.created) {
				return ValidationResult.Success;
			}
			if (date < order.OrderDateCreate && !order.OrderIsExtend)
				return new ValidationResult(String.Format("Плановая дата начала({0}) раньше даты создания({1})",date,order.OrderDateCreate));
			return ValidationResult.Success;
		}

		public static ValidationResult ValidatePlanStopDate(DateTime date, ValidationContext context) {
			return ValidationResult.Success;
			Order order=context.ObjectInstance as Order;
			if (order.OrderState != OrderStateEnum.created) {
				return ValidationResult.Success;
			}
			if (date < order.PlanStartDate)
				return new ValidationResult(String.Format("Плановая дата завершения({0}) раньше даты планового начала({1})", date, order.PlanStartDate));
			return ValidationResult.Success;
		}

		public static ValidationResult ValidateFaktStartDate(DateTime? date, ValidationContext context) {
			return ValidationResult.Success;
			Order order=context.ObjectInstance as Order;
			if (order.OrderState != OrderStateEnum.opened) {
				return ValidationResult.Success;
			}
			if (date < order.PlanStartDate)
				return new ValidationResult(String.Format("Дата начала работ({0}) раньше даты планового начала({1})", date, order.PlanStartDate));
			if (date < order.OrderDateAccept&&!order.OrderIsExtend)
				return new ValidationResult(String.Format("Дата начала работ({0}) раньше даты разрешения({1})", date, order.OrderDateAccept));
			return ValidationResult.Success;
		}

		public static ValidationResult ValidateFaktStopDate(DateTime? date, ValidationContext context) {
			return ValidationResult.Success;
			Order order=context.ObjectInstance as Order;
			if (order.OrderState != OrderStateEnum.closed) {
				return ValidationResult.Success;
			}
			if (date < order.FaktStartDate)
				return new ValidationResult(String.Format("Дата разрешения на ввод({0}) раньше даты фактического начала({1})", date, order.FaktStartDate));
			return ValidationResult.Success;
		}

		public static ValidationResult ValidateFaktCompleteDate(DateTime? date, ValidationContext context) {
			return ValidationResult.Success;
			Order order=context.ObjectInstance as Order;
			if (order.OrderState != OrderStateEnum.completed) {
				return ValidationResult.Success;
			}
			if (date < order.FaktStopDate)
				return new ValidationResult(String.Format("Дата закрытия({0}) раньше даты разрешения на ввод({1})", date, order.FaktStopDate));
			return ValidationResult.Success;
		}

		public static ValidationResult ValidateFutureDate(DateTime? date, ValidationContext context) {
			return ValidationResult.Success;
			Order order=context.ObjectInstance as Order;
			if (!date.HasValue) 
				return ValidationResult.Success;
			if (date > DateTime.Now) {
				return new ValidationResult(String.Format("Дата закрытия({0}) позже текущей даты", date));
			}
			return ValidationResult.Success;
		}
	}
}