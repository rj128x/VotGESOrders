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
		protected static string smtpServer;
		protected static string smtpUser;
		protected static string smtpPassword;
		protected static string smtpDomain;
		protected static int smtpPort;
		protected static string smtpFrom;

		static MailContext() {
			smtpServer = System.Configuration.ConfigurationManager.AppSettings["smtpServer"];
			smtpUser = System.Configuration.ConfigurationManager.AppSettings["smtpUser"];
			smtpPassword = System.Configuration.ConfigurationManager.AppSettings["smtpPassword"];
			smtpDomain = System.Configuration.ConfigurationManager.AppSettings["smtpDomain"];
			smtpFrom = System.Configuration.ConfigurationManager.AppSettings["smtpFrom"];
			Int32.TryParse(System.Configuration.ConfigurationManager.AppSettings["smtpPort"],out smtpPort);


		}
		public static void sendMail(string header, Order order, bool isNewOrder, bool onlyAuthor, Order prevOrder=null) {
			if (HttpContext.Current.Request.Url.Port == 8071 || HttpContext.Current.Request.Url.Port == 8090)
				return;
			try {
				IQueryable users=OrdersUser.getAllUsers();
				List<string> mailToList=new List<string>();

				foreach (OrdersUser user in users) {
					if (
						user.SendAllAgreeMail && order.AgreeUsers.Contains(user) && !mailToList.Contains(user.Mail) && !onlyAuthor ||
						user.SendAllMail && !mailToList.Contains(user.Mail) ||
						user.SendCreateMail && order.UserCreateOrderID == user.UserID && !mailToList.Contains(user.Mail)||
						onlyAuthor && order.UserCreateOrderID == user.UserID && !mailToList.Contains(user.Mail)||
						isNewOrder && (user.SendAllCreateMail || user.SendAgreeMail && order.AgreeUsers.Contains(user)) && !mailToList.Contains(user.Mail) && !onlyAuthor
						) {
						if (user.Mails.Count>0) {
							foreach (string mail in user.Mails) {
								if (!String.IsNullOrEmpty(mail)) {
									mailToList.Add(mail);
								}
							}
						}
					}
				}

				string message=OrderView.getOrderHTML(order);
				if (prevOrder!=null) {
					message += "<hr/>" + OrderView.getOrderHTML(prevOrder,false);
				}
				message += String.Format("<h3><a href='{0}'>Перейти к списку заявок</a></h3>", String.Format("http://{0}:{1}", HttpContext.Current.Request.Url.Host, HttpContext.Current.Request.Url.Port));
				
				if (mailToList.Count > 0) {
					//SendMailLocal("mx-votges-021.corp.gidroogk.com", 25, "", "", "", "SR-VOTGES-INT@votges.rushydro.ru", mailToList, header, message, true);
					SendMailLocal(smtpServer, smtpPort, smtpUser, smtpPassword, smtpDomain, smtpFrom, mailToList, header, message, true);
				}
			} catch (Exception e) {
				Logger.error(String.Format("Ошибка при отправке почты: {0}", e.ToString()), Logger.LoggerSource.server);
			}
		}

		public static void sendOrdersList(string header, List<Order> orders) {
			try {
				IQueryable users=OrdersUser.getAllUsers();
				List<string> mailToList=new List<string>();

				OrdersUser CurrentUser = OrdersUser.loadFromCache(HttpContext.Current.User.Identity.Name);
				foreach (string mail in CurrentUser.Mails) {
					if (!String.IsNullOrEmpty(mail)) {
						mailToList.Add(mail);
					}
				}

				bool isFirst=true;
				if (mailToList.Count > 0) {
					string message="";
					foreach (Order order in orders) {
						message += OrderView.getOrderHTML(order, isFirst) + "<hr/>";
						isFirst = false;
					}
					//SendMailLocal("mx-votges-021.corp.gidroogk.com", 25, "", "", "", "SR-VOTGES-INT@votges.rushydro.ru", mailToList, header, message,true);
					SendMailLocal(smtpServer, smtpPort, smtpUser, smtpPassword, smtpDomain, smtpFrom, mailToList, header, message, true);
				}				
			} catch (Exception e) {
				Logger.error(String.Format("Ошибка при отправке почты: {0}", e.ToString()), Logger.LoggerSource.server);
			}
		}


		private static bool SendMailLocal(string smtp_server, int port, string mail_user, string mail_password, string domain, string mail_from, List<string> mailToList, string subject, string message, bool is_html) {

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
				client.Credentials = new System.Net.NetworkCredential(mail_user, mail_password, domain);
			}
			// Отправляем письмо
			client.Send(mess);
			return true;
		}





	}
}