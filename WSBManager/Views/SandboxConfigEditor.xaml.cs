using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
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
	public sealed partial class SandboxConfigEditor : Page {

		SandboxConfigEditorViewModel sandboxConfigEditorViewModel;

		public SandboxConfigEditor() {
			this.InitializeComponent();
		}

		protected override void OnNavigatedTo( NavigationEventArgs e ) {
			base.OnNavigatedTo( e );

			if( e.Parameter is int selected ) {
				sandboxConfigEditorViewModel = new SandboxConfigEditorViewModel( selected );
			}
			else {
				sandboxConfigEditorViewModel = new SandboxConfigEditorViewModel();
			}
			this.DataContext = sandboxConfigEditorViewModel;
		}

		protected override void OnNavigatedFrom( NavigationEventArgs e ) {
			base.OnNavigatedFrom( e );

			this.DataContext = null;
		}

		private void BackButton_Click( object sender, RoutedEventArgs e ) {
			if( this.Frame.CanGoBack ) {
				this.Frame.GoBack();
			}
		}

		private async void SaveButton_Click( object sender, RoutedEventArgs e ) {
			var result = sandboxConfigEditorViewModel.Validate();
			if( result.result != MappedFolderValidateResult.OK ) {
				var loader = ResourceLoader.GetForCurrentView();
				var message = loader.GetString( result.result.ToString() );
				var dialog = new MessageDialog(
					string.Format( $"{message}:\r\n\r\n{string.Join( "\r\n", result.validateFailedHostFolders )}" ),
					loader.GetString( "MappedFolderValidationFailedTitle" )
				);
				await dialog.ShowAsync();

				return;
			}

			sandboxConfigEditorViewModel.Save();
			if( this.Frame.CanGoBack ) {
				this.Frame.GoBack();
			}
		}

		private async void ReferenceButton_Click( object sender, RoutedEventArgs e ) {
			var folderPicker = new FolderPicker {
				SuggestedStartLocation = PickerLocationId.Desktop
			};
			folderPicker.FileTypeFilter.Add( "*" );
			if( ( await folderPicker.PickSingleFolderAsync() ) is StorageFolder folder ) {
				var button = ( Button )sender;
				button.Tag = folder.Path;
			}

		}
	}
}
