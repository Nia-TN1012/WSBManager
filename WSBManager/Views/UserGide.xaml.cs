using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace WSBManager.Views
{
	public sealed partial class UserGide : Page
	{
		public UserGide()
		{
			this.InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			SystemNavigationManager.GetForCurrentView().BackRequested += UserGide_BackRequested; ;
		}
		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
		{
			base.OnNavigatingFrom(e);

			SystemNavigationManager.GetForCurrentView().BackRequested -= UserGide_BackRequested;
		}

		private void UserGide_BackRequested(object sender, BackRequestedEventArgs e)
		{
			if (this.Frame.CanGoBack)
			{
				this.Frame.GoBack();
				e.Handled = true;
			}
		}

		private void BackButton_Click(object sender, RoutedEventArgs e)
		{
			if (this.Frame.CanGoBack)
			{
				this.Frame.GoBack();
			}
		}
	}
}
