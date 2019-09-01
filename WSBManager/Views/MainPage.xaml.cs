using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using WSBManager.Models;
using WSBManager.ViewModels;

namespace WSBManager.Views {
	/// <summary>
	/// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
	/// </summary>
	public sealed partial class MainPage : Page {
		public MainPage() {
			this.InitializeComponent();
		}

		private void AddButton_Click( object sender, RoutedEventArgs e ) {
			this.Frame.Navigate( typeof( SandboxConfigEditor ), -1 );
		}

		private void EditButton_Click( object sender, RoutedEventArgs e ) {
			if( SandboxListView.SelectedIndex > -1 ) {
				this.Frame.Navigate( typeof( SandboxConfigEditor ), SandboxListView.SelectedIndex );
			}
		}
	}
}
