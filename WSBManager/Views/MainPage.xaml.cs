using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
	public sealed partial class MainPage : Page {
		public MainPage() {
			this.InitializeComponent();
			wsbManagerViewModel.ImportSandboxConfingAction += ImportSandboxConfing;
			wsbManagerViewModel.ExportSandboxConfingAction += ExportSandboxConfing;
			wsbManagerViewModel.DeleteSandboxConfigAction += DeleteSandboxConfig;
		}

		protected override async void OnNavigatedTo( NavigationEventArgs e ) {
			base.OnNavigatedTo( e );

			if( e.NavigationMode == NavigationMode.New ) {
				if( !await wsbManagerViewModel.LoadAsync() ) {
					var errorMessageDialog = new MessageDialog( $"Failed to load", "Error" );
					await errorMessageDialog.ShowAsync();
				}
			}
		}

		private async void ImportSandboxConfing( Action<StorageFile, Action> fileSelectedCallback, Action canceledCallback = null ) {
			var importPicker = new FileOpenPicker {
				SuggestedStartLocation = PickerLocationId.DocumentsLibrary
			};
			importPicker.FileTypeFilter.Add( ".wsb" );

			if( ( await importPicker.PickSingleFileAsync() ) is StorageFile importFile ) {
				fileSelectedCallback?.Invoke(
					importFile,
					async () => {
						var errorMessageDialog = new MessageDialog( $"Failed to import '{importFile.Path}'", "Error" );
						await errorMessageDialog.ShowAsync();
					}
				);
			}
		}

		private async void ExportSandboxConfing( string name, Action<StorageFile, Action> fileSelectedCallback, Action canceledCallback = null ) {
			var exportPicker = new FileSavePicker {
				SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
				SuggestedFileName = name
			};
			exportPicker.FileTypeChoices.Add( "wsb config file", new List<string> { ".wsb" } );

			if( ( await exportPicker.PickSaveFileAsync() ) is StorageFile exportFile ) {
				fileSelectedCallback?.Invoke(
					exportFile,
					async () => {
						var errorMessageDialog = new MessageDialog( $"Failed to export '{exportFile.Path}'", "Error" );
						await errorMessageDialog.ShowAsync();
					}
				);
			}

		}

		private async void DeleteSandboxConfig(string name, Action confirmedCallback, Action canceledCallback = null ) {
			var confirmMessageDialog = new MessageDialog( $"Are you sure you want to delete '{name}'", "Confirm" );
			confirmMessageDialog.Commands.Add( new UICommand( "Yes", command => confirmedCallback?.Invoke() ) );
			confirmMessageDialog.Commands.Add( new UICommand( "No" ) );
			await confirmMessageDialog.ShowAsync();

		}

		private void AddButton_Click( object sender, RoutedEventArgs e ) {
			this.Frame.Navigate( typeof( SandboxConfigEditor ) );
		}

		private void EditButton_Click( object sender, RoutedEventArgs e ) {
			if( SandboxListView.SelectedIndex > -1 ) {
				this.Frame.Navigate( typeof( SandboxConfigEditor ), SandboxListView.SelectedIndex );
			}
		}
	}
}
