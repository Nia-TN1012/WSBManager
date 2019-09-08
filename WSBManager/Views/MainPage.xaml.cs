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
using Windows.UI.Core;
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

		private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();

		public MainPage() {
			this.InitializeComponent();
			wsbManagerViewModel.LaunchSandboxCompleted += OnLaunchSandboxCompleted;
			wsbManagerViewModel.ImportSandboxConfingAction += ImportSandboxConfing;
			wsbManagerViewModel.ExportSandboxConfingAction += ExportSandboxConfing;
			wsbManagerViewModel.DeleteSandboxConfigAction += DeleteSandboxConfig;
		}

		protected override async void OnNavigatedTo( NavigationEventArgs e ) {
			base.OnNavigatedTo( e );

			if( e.NavigationMode == NavigationMode.New ) {
				if( !await wsbManagerViewModel.InitializeModelAsync() ) {
					var errorMessageDialog = new MessageDialog( resourceLoader.GetString( "FailedToLoadWSBConfigListDialog" ), resourceLoader.GetString( "DialogTitleError" ) );
					await errorMessageDialog.ShowAsync();
				}
			}
		}

		private async void OnLaunchSandboxCompleted( object sender, ( bool success, string name ) e ) {
			if( !e.success ) {
				var errorMessageDialog = new MessageDialog( string.Format( resourceLoader.GetString( "FailedLaunchSandboxDialog" ), e.name ), resourceLoader.GetString( "DialogTitleError" ) );
				await errorMessageDialog.ShowAsync();
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
						var errorMessageDialog = new MessageDialog( string.Format( resourceLoader.GetString( "FailedToImportDialog" ), importFile.Path ), resourceLoader.GetString( "DialogTitleError" ) );
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
			exportPicker.FileTypeChoices.Add( resourceLoader.GetString( "WSBFileExtention" ), new List<string> { ".wsb" } );

			if( ( await exportPicker.PickSaveFileAsync() ) is StorageFile exportFile ) {
				fileSelectedCallback?.Invoke(
					exportFile,
					async () => {
						var errorMessageDialog = new MessageDialog( string.Format( resourceLoader.GetString( "FailedToExportDialog" ), exportFile.Path ), resourceLoader.GetString( "DialogTitleError" ) );
						await errorMessageDialog.ShowAsync();
					}
				);
			}

		}

		private async void DeleteSandboxConfig(string name, Action confirmedCallback, Action canceledCallback = null ) {
			var confirmMessageDialog = new MessageDialog( string.Format( resourceLoader.GetString( "DeleteSandboxConfigurationDialog" ), name ), resourceLoader.GetString( "DialogTitleConfirm" ) );
			confirmMessageDialog.Commands.Add( new UICommand( resourceLoader.GetString( "DialogButtonYes" ), command => confirmedCallback?.Invoke() ) );
			confirmMessageDialog.Commands.Add( new UICommand( resourceLoader.GetString( "DialogButtonNo" ) ) );
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

		private void AutoSuggestBox_TextChanged( AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args ) {
			if( args.Reason == AutoSuggestionBoxTextChangeReason.UserInput ) {
				sender.ItemsSource = wsbManagerViewModel.Items.Where( item => item.Name.ToLower().StartsWith( sender.Text.ToLower() ) ).Take( 10 );
			}
		}

		private void AutoSuggestBox_SuggestionChosen( AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args ) {
			if( args.SelectedItem is WSBConfigManagerModel selected ) {
				sender.Text = selected.Name;
			}
		}

		private void AutoSuggestBox_QuerySubmitted( AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args ) {
			if( args.ChosenSuggestion is WSBConfigManagerModel selected ) {
				SandboxListView.SelectedItem = selected;
			}
			else if( wsbManagerViewModel.Items.FirstOrDefault( item => item.Name == sender.Text ) is WSBConfigManagerModel hit ) {
				SandboxListView.SelectedItem = hit;
			}
		}

		private void ToAboutMenuItem_Click( object sender, RoutedEventArgs e ) {
			this.Frame.Navigate( typeof( About ) );
		}
	}
}
