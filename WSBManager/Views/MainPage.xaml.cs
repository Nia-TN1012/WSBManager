using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using WSBManager.Models;

namespace WSBManager.Views
{
	/// <summary>
	/// Main page
	/// </summary>
	public sealed partial class MainPage : Page
	{
		/// <summary>
		/// Resource loader
		/// </summary>
		private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();

		/// <summary>
		/// Constructor
		/// </summary>
		public MainPage()
		{
			this.InitializeComponent();
			wsbManagerViewModel.LaunchSandboxCompleted += OnLaunchSandboxCompleted;
			wsbManagerViewModel.ImportSandboxConfingAction += ImportSandboxConfing;
			wsbManagerViewModel.ExportSandboxConfingAction += ExportSandboxConfing;
			wsbManagerViewModel.DeleteSandboxConfigAction += DeleteSandboxConfig;
		}

		/// <summary>
		/// Invoked when navigate to this page.
		/// </summary>
		/// <param name="e">Event argsment</param>
		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			if (e.NavigationMode == NavigationMode.New)
			{
				if (!await wsbManagerViewModel.InitializeModelAsync())
				{
					var errorMessageDialog = new MessageDialog(resourceLoader.GetString("FailedToLoadWSBConfigListDialog"), resourceLoader.GetString("DialogTitleError"));
					await errorMessageDialog.ShowAsync();
				}
			}
		}

		/// <summary>
		/// Invoked when lanch sandbox action completed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e">
		///		<para>A tuple of execution success flag and sandbox name.</para>
		///		<para>success: A execution success flag ( true: Success / false: Failed )</para>
		///		<para>name: A sandbox name</para>
		/// </param>
		private async void OnLaunchSandboxCompleted(object sender, (bool success, string name) e)
		{
			if (!e.success)
			{
				var errorMessageDialog = new MessageDialog(string.Format(resourceLoader.GetString("FailedLaunchSandboxDialog"), e.name), resourceLoader.GetString("DialogTitleError"));
				await errorMessageDialog.ShowAsync();
			}
		}

		/// <summary>
		/// Selects the sandbox configuration file to import.
		/// </summary>
		/// <param name="fileSelectedCallback">Callback when selected a file</param>
		/// <param name="canceledCallback">Callback when canceled</param>
		private async void ImportSandboxConfing(Action<StorageFile, Action> fileSelectedCallback, Action canceledCallback = null)
		{
			var importPicker = new FileOpenPicker
			{
				SuggestedStartLocation = PickerLocationId.DocumentsLibrary
			};
			importPicker.FileTypeFilter.Add(".wsb");

			if ((await importPicker.PickSingleFileAsync()) is StorageFile importFile)
			{
				fileSelectedCallback?.Invoke(
					importFile,
					// On failed to export
					async () => {
						var errorMessageDialog = new MessageDialog(string.Format(resourceLoader.GetString("FailedToImportDialog"), importFile.Path), resourceLoader.GetString("DialogTitleError"));
						await errorMessageDialog.ShowAsync();
					}
				);
			}
		}

		/// <summary>
		/// Selects the sandbox configuration file to export.
		/// </summary>
		/// <param name="fileSelectedCallback">Callback when selected a file</param>
		/// <param name="canceledCallback">Callback when canceled</param>
		private async void ExportSandboxConfing(string name, Action<StorageFile, Action> fileSelectedCallback, Action canceledCallback = null)
		{
			var exportPicker = new FileSavePicker
			{
				SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
				SuggestedFileName = name
			};
			exportPicker.FileTypeChoices.Add(resourceLoader.GetString("WSBFileExtention"), new List<string> { ".wsb" });

			if ((await exportPicker.PickSaveFileAsync()) is StorageFile exportFile)
			{
				fileSelectedCallback?.Invoke(
					exportFile,
					// On failed to export
					async () => {
						var errorMessageDialog = new MessageDialog(string.Format(resourceLoader.GetString("FailedToExportDialog"), exportFile.Path), resourceLoader.GetString("DialogTitleError"));
						await errorMessageDialog.ShowAsync();
					}
				);
			}

		}

		/// <summary>
		/// Shows a confirmation dialog to delete the sandbox configuration item.
		/// </summary>
		/// <param name="name">Sandbox name</param>
		/// <param name="confirmedCallback">Callback when confirmed</param>
		/// <param name="canceledCallback">Callback when canceled</param>
		private async void DeleteSandboxConfig(string name, Action confirmedCallback, Action canceledCallback = null)
		{
			var confirmMessageDialog = new MessageDialog(string.Format(resourceLoader.GetString("DeleteSandboxConfigurationDialog"), name), resourceLoader.GetString("DialogTitleConfirm"));
			confirmMessageDialog.Commands.Add(new UICommand(resourceLoader.GetString("DialogButtonYes"), command => confirmedCallback?.Invoke()));
			confirmMessageDialog.Commands.Add(new UICommand(resourceLoader.GetString("DialogButtonNo")));
			await confirmMessageDialog.ShowAsync();

		}

		/// <summary>
		/// Invoked when pressed the Add Sandbox Configration button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AddButton_Click(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(SandboxConfigEditor));
		}

		/// <summary>
		/// Invoked when pressed the Edit Sandbox Configration button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void EditButton_Click(object sender, RoutedEventArgs e)
		{
			if (SandboxListView.SelectedIndex > -1)
			{
				this.Frame.Navigate(typeof(SandboxConfigEditor), SandboxListView.SelectedIndex);
			}
		}

		/// <summary>
		/// Invoked when changed the text in the auto-suggestion box.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
		{
			if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
			{
				sender.ItemsSource = wsbManagerViewModel.Items.Where(item => item.Name.ToLower().StartsWith(sender.Text.ToLower())).Take(10);
			}
		}

		/// <summary>
		/// Invoked when chosen a suggestion in the auto-suggestion box.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
		{
			if (args.SelectedItem is WSBConfigManagerModel selected)
			{
				sender.Text = selected.Name;
			}
		}

		/// <summary>
		/// Invoked when submitted query in the auto-suggestion box.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
		{
			if (args.ChosenSuggestion is WSBConfigManagerModel selected)
			{
				SandboxListView.SelectedItem = selected;
			}
			else if (wsbManagerViewModel.Items.FirstOrDefault(item => item.Name == sender.Text) is WSBConfigManagerModel hit)
			{
				SandboxListView.SelectedItem = hit;
			}
		}

		/// <summary>
		/// Invoked when pressed the About button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ToAboutMenuItem_Click(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(About));
		}

		/// <summary>
		/// Invoked when pressed the User Guide button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(UserGide));
		}
	}
}
