﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using VotGESOrders.Web.Services;
using System.ServiceModel.DomainServices.Client.ApplicationServices;
using VotGESOrders.Logging;
using VotGESOrders.Web.Models;


namespace VotGESOrders
{
	public partial class App : Application
	{
		public App() {
			this.Startup += this.Application_Startup;
			this.UnhandledException += this.Application_UnhandledException;
			WebContext webcontext = new WebContext();
			webcontext.Authentication = new System.ServiceModel.DomainServices.Client.ApplicationServices.WindowsAuthentication();

			LoggerContext context=new LoggerContext();
			Logging.Logger.init(context);
			OrdersContext.init();
									
			this.ApplicationLifetimeObjects.Add(webcontext);

			WebContext.Current.Authentication.LoadUser(OnLoadUser_Completed, null);
			InitializeComponent();
		}

		private void Application_Startup(object sender, StartupEventArgs e) {
			//this.RootVisual = new MainPage();
		}

		private void OnLoadUser_Completed(LoadUserOperation operation) {			
			Logger.info("Пользователь авторизовался в клиенте");
			this.RootVisual = new MainPage();
		}
		

		private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e) {
			// Если приложение выполняется вне отладчика, воспользуйтесь для сообщения об исключении
			// элементом управления ChildWindow.
			if (!System.Diagnostics.Debugger.IsAttached) {
				// ПРИМЕЧАНИЕ. Это позволит приложению выполняться после того, как исключение было выдано,
				// но не было обработано. 
				// Для рабочих приложений такую обработку ошибок следует заменить на код, 
				// оповещающий веб-сайт об ошибке и останавливающий работу приложения.

				e.Handled = true;
								
				if (e.ExceptionObject.GetType()==typeof(System.ServiceModel.DomainServices.Client.DomainOperationException)){
					GlobalStatus.Current.Status="Ошибка:";
					Exception inner=e.ExceptionObject.InnerException;
					if (inner!=null){
						if (inner.GetType() == typeof(System.ServiceModel.CommunicationException)) {
							GlobalStatus.Current.Status += "Соединение с сервером";
						}
						if (inner.GetType() == typeof(VotGESOrders.Web.Models.OrderException)) {
							ChildWindow errorWin = new ErrorWindow(inner);
							errorWin.Show();
						}
					} else {
						ChildWindow errorWin = new ErrorWindow(e.ExceptionObject);
						Logger.logMessage(e.ExceptionObject.ToString());
						errorWin.Show();
					}
				}
				else{
					ChildWindow errorWin = new ErrorWindow(e.ExceptionObject);
					Logger.logMessage(e.ExceptionObject.ToString());
					errorWin.Show();
				}				
				
				

				
				
			}
		}
	}
}