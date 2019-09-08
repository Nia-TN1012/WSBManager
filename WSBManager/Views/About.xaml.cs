using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace WSBManager.Views {
	public sealed partial class About : Page {
		public About() {
			this.InitializeComponent();
		}

		protected override void OnNavigatedTo( NavigationEventArgs e ) {
			base.OnNavigatedTo( e );

			SystemNavigationManager.GetForCurrentView().BackRequested += About_BackRequested;
		}

		protected override void OnNavigatingFrom( NavigatingCancelEventArgs e ) {
			base.OnNavigatingFrom( e );

			SystemNavigationManager.GetForCurrentView().BackRequested -= About_BackRequested;
		}

		private void About_BackRequested( object sender, BackRequestedEventArgs e ) {
			if( this.Frame.CanGoBack ) {
				this.Frame.GoBack();
				e.Handled = true;
			}
		}

		private void BackButton_Click( object sender, RoutedEventArgs e ) {
			if( this.Frame.CanGoBack ) {
				this.Frame.GoBack();
			}
		}
	}
}
