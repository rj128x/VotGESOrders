using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VotGESOrders.Web.Logging;
using System.Net.Mail;

namespace VotGESOrders.Web.Models
{
	public class MailContext
	{
		public static void sendMail(string header, List<Order> orders) {
			return;
			try {
				IQueryable users=OrdersUser.getAllUsers();
				List<string> mailToList=new List<string>();
				/*foreach (OrdersUser user in users) {
					string[]parts=user.Name.Split('\\');
					string name=parts[1];
					string mail=name + "@votges.rushydro.ru";
					mailToList.Add(mail);
				}*/
				mailToList.Add("ChekunovaMV@votges.rushydro.ru");
				string message="";
				foreach (Order order in orders) {
					message += OrderView.getOrderHTML(order) + "<br/>";
				}
				SendMailLocal("mx-votges-121.corp.gidroogk.com", 25, "", "", "ChekunovaMV@votges.rushydro.ru", mailToList, header, message, true);
			} catch (Exception e) {
				Logger.error(String.Format("Ошибка при отправке почты: {0}", e.ToString()));
			}
		}

		public static void sendMail(string header, Order order) {
			sendMail(header, new List<Order> { order });
		}

		public static void sendMail(string header, Order order1, Order order2) {
			sendMail(header, new List<Order> { order1, order2 });
		}



		private static bool SendMailLocal(string smtp_server, int port, string mail_user, string mail_password, string mail_from,  List<string> mailToList, string subject, string message, bool is_html) {

			System.Net.Mail.MailMessage mess =	new System.Net.Mail.MailMessage();

			mess.From = new MailAddress(mail_from);
			mess.Subject = subject; mess.Body = message;
			foreach (string mail in mailToList) {
				mess.To.Add(mail);
			}
			
			mess.SubjectEncoding = System.Text.Encoding.UTF8;
			mess.BodyEncoding = System.Text.Encoding.UTF8;
			mess.IsBodyHtml = is_html;
			System.Net.Mail.SmtpClient client =	new System.Net.Mail.SmtpClient(smtp_server, port);
			client.EnableSsl = true; 
			if (string.IsNullOrEmpty(mail_user)) {
				client.UseDefaultCredentials = true;
			} else {
				client.Credentials = new System.Net.NetworkCredential(mail_user, mail_password);
			}
			// Отправляем письмо
			client.Send(mess);
			return true;
		}





	}
}