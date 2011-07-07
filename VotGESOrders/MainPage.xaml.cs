using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VotGESOrders.Web.Services;
using System.ServiceModel.DomainServices.Client.ApplicationServices;
using VotGESOrders.Logging;

namespace VotGESOrders
{
	public partial class MainPage : UserControl
	{
		public MainPage() {
			Logger.info("Старт главной страницы");
			InitializeComponent();
			LoginName.DataContext = WebContext.Current.User;
			LinkEditTree.Visibility = WebContext.Current.User.AllowEditTree ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
			LinkEditUsers.Visibility = WebContext.Current.User.AllowEditUsers ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;	
		}

		// После перехода в фрейме убедиться, что выбрана кнопка HyperlinkButton, представляющая текущую страницу
		private void ContentFrame_Navigated(object sender, NavigationEventArgs e) {
			foreach (UIElement child in LinksStackPanel.Children) {
				HyperlinkButton hb = child as HyperlinkButton;
				if (hb != null && hb.NavigateUri != null) {
					if (hb.NavigateUri.ToString().Equals(e.Uri.ToString())) {
						VisualStateManager.GoToState(hb, "ActiveLink", true);
					} else {
						VisualStateManager.GoToState(hb, "InactiveLink", true);
					}
				}
			}
		}

		// Если во время навигации возникает ошибка, отобразить окно ошибки
		private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e) {
			e.Handled = true;
			ChildWindow errorWin = new ErrorWindow(e.Uri);
			errorWin.Show();
		}
	}
}