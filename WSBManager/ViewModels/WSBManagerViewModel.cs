using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using Windows.Storage;
using System.Runtime.CompilerServices;
using WSBManager.Models;
using System.IO;
using System.Windows.Input;
using Windows.Storage.Provider;
using Windows.System;

namespace WSBManager.ViewModels
{

	/// <summary>
	/// A delegate for using Import file picker.
	/// </summary>
	public delegate void ImportFilePickerAction(Action<StorageFile, Action> fileSelectedCallback, Action canceledCallback = null);
	/// <summary>
	/// A delegate for using Export file picker.
	/// </summary>
	public delegate void ExportFilePickerAction(string name, Action<StorageFile, Action> fileSelectedCallback, Action canceledCallback = null);
	/// <summary>
	/// A delegate for using confirm delete item.
	/// </summary>
	public delegate void DeleteConfirmAction(string name, Action confirmedCallback, Action canceledCallback = null);

	/// <summary>
	/// WSB Manager View Model
	/// </summary>
	class WSBManagerViewModel : INotifyPropertyChanged
	{

		/// <summary>
		///	The application's temporary folder.
		/// </summary>
		private static readonly StorageFolder tempFolder = ApplicationData.Current.TemporaryFolder;

		/// <summary>
		/// Model
		/// </summary>
		private readonly WSBManagerModel model;

		/// <summary>
		/// Gets the sandbox configuration list.
		/// </summary>
		public IEnumerable<WSBConfigManagerModel> Items => model?.WSBConfigCollection;

		/// <summary>
		/// Creates a new instance of the <see cref="WSBManagerViewModel"/> class.
		/// </summary>
		public WSBManagerViewModel()
		{
			model = (App.Current as App)?.Model;
			if (model == null)
			{
				throw new Exception($"Failed to get reference of model instance on the {GetType()} class.");
			}

			model.PropertyChanged += (sender, e) => PropertyChanged?.Invoke(sender, e);
		}

		/// <summary>
		/// Initializes model.
		/// </summary>
		/// <returns>true: Success / false: Failed</returns>
		public async Task<bool> InitializeModelAsync() => await model.LoadAsync();

		/// <summary>
		///	A event handler which fire when the property changed.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		///	Notifies the property change that corresponds to the specified property name.
		/// </summary>
		/// <param name="propertyName">Property name</param>
		private void NotifyPropertyChanged([CallerMemberName] string propertyName = null) =>
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		/// <summary>
		/// A event handler which fire when sandbox launch action completed.
		/// </summary>
		public event EventHandler<(bool success, string name)> LaunchSandboxCompleted;
		/// <summary>
		/// A event handler which fire when pick a file to import.
		/// </summary>
		public event ImportFilePickerAction ImportSandboxConfingAction;
		/// <summary>
		/// A event handler which fire when pick a file to export.
		/// </summary>
		public event ExportFilePickerAction ExportSandboxConfingAction;
		/// <summary>
		/// A event handler which fire when confirm delete item.
		/// </summary>
		public event DeleteConfirmAction DeleteSandboxConfigAction;

		#region Commands

		private ICommand launchSandbox;
		/// <summary>
		/// A command which launch sandbox.
		/// </summary>
		public ICommand LaunchSandbox =>
			launchSandbox ?? (launchSandbox = new LaunchSandboxCommand(this));

		private ICommand importSandboxConfig;
		/// <summary>
		/// A command which import sandbox configration.
		/// </summary>
		public ICommand ImportSandboxConfig =>
			importSandboxConfig ?? (importSandboxConfig = new ImportSandboxConfigCommand(this));

		private ICommand exportSandboxConfig;
		/// <summary>
		/// A command which export sandbox configration.
		/// </summary>
		public ICommand ExportSandboxConfig =>
			exportSandboxConfig ?? (exportSandboxConfig = new ExportSandboxConfigCommand(this));

		private ICommand moveUpSandboxConfig;
		/// <summary>
		/// A command which move up item.
		/// </summary>
		public ICommand MoveUpSandboxConfig =>
			moveUpSandboxConfig ?? (moveUpSandboxConfig = new MoveUpSandboxConfigCommand(this));

		private ICommand moveDownSandboxConfig;
		/// <summary>
		/// A command which move down item.
		/// </summary>
		public ICommand MoveDownSandboxConfig =>
			moveDownSandboxConfig ?? (moveDownSandboxConfig = new MoveDownSandboxConfigCommand(this));

		private ICommand deleteSandboxConfig;
		/// <summary>
		/// A command which delete sandbox configration.
		/// </summary>
		public ICommand DeleteSandboxConfig =>
			deleteSandboxConfig ?? (deleteSandboxConfig = new DeleteSandboxConfigCommand(this));

		/// <summary>
		/// Launch sandbox command.
		/// </summary>
		private class LaunchSandboxCommand : ICommand
		{

			/// <summary>
			/// The reference of <see cref="WSBManagerViewModel"/>.
			/// </summary>
			private readonly WSBManagerViewModel viewModel;

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="_viewModel">The reference of <see cref="WSBManagerViewModel"/></param>
			internal LaunchSandboxCommand(WSBManagerViewModel _viewModel)
			{
				viewModel = _viewModel;
				viewModel.PropertyChanged += (sender, e) => CanExecuteChanged?.Invoke(sender, e);
			}

			/// <summary>
			///	Can or not execute the command.
			/// </summary>
			/// <param name="parameter">Parameter ( Not using )</param>
			/// <returns>Always returns true</returns>
			public bool CanExecute(object parameter) => true;

			/// <summary>
			/// A event handler which fire when CanExecute() changed.
			/// </summary>
			public event EventHandler CanExecuteChanged;

			/// <summary>
			/// Executes the command to launch sandbox from specified item.
			/// </summary>
			/// <param name="parameter">UUID</param>
			public async void Execute(object parameter)
			{
				if (parameter is string uuid)
				{
					if (viewModel.model.WSBConfigCollection.FirstOrDefault(item => item.UUID == uuid) is WSBConfigManagerModel launchModel)
					{
						try
						{
							var tempFile = await tempFolder.CreateFileAsync($"{launchModel.Name}_{launchModel.UUID}.wsb", CreationCollisionOption.ReplaceExisting);
							CachedFileManager.DeferUpdates(tempFile);
							using (var s = await tempFile.OpenStreamForWriteAsync())
							using (var sw = new StreamWriter(s))
							{
								((WSBConfigModel)launchModel).Export(sw);
							}
							var status = await CachedFileManager.CompleteUpdatesAsync(tempFile);
							if (status == FileUpdateStatus.Complete)
							{
								var options = new LauncherOptions
								{
									DisplayApplicationPicker = true,
								};
								bool success = await Launcher.LaunchFileAsync(tempFile, options);
								if (success)
								{
									launchModel.UpdateLastLaunchedAt();
								}
								viewModel.LaunchSandboxCompleted?.Invoke(this, (success, launchModel.Name));
							}
						}
						catch (Exception)
						{
							viewModel.LaunchSandboxCompleted?.Invoke(this, (false, launchModel.Name));
						}
					}
				}
			}
		}

		private class ImportSandboxConfigCommand : ICommand
		{

			private readonly WSBManagerViewModel viewModel;

			internal ImportSandboxConfigCommand(WSBManagerViewModel _viewModel)
			{
				viewModel = _viewModel;
				viewModel.PropertyChanged += (sender, e) => CanExecuteChanged?.Invoke(sender, e);
			}

			public bool CanExecute(object parameter) => true;

			public event EventHandler CanExecuteChanged;

			public void Execute(object parameter)
			{
				viewModel.ImportSandboxConfingAction?.Invoke(
					async (file, exportErrorCallback) => {
						try
						{
							using (var s = await file.OpenStreamForReadAsync())
							using (var sr = new StreamReader(s))
							{
								var importModel = WSBConfigModel.Import(sr);
								var importModel2 = new WSBConfigManagerModel(importModel)
								{
									Name = Path.GetFileNameWithoutExtension(file.Name)
								};
								viewModel.model.WSBConfigCollection.Add(importModel2);
							}
						}
						catch (Exception e)
						{
							exportErrorCallback?.Invoke();
						}
					}
				);
			}
		}

		private class ExportSandboxConfigCommand : ICommand
		{

			private readonly WSBManagerViewModel viewModel;

			internal ExportSandboxConfigCommand(WSBManagerViewModel _viewModel)
			{
				viewModel = _viewModel;
				viewModel.PropertyChanged += (sender, e) => CanExecuteChanged?.Invoke(sender, e);
			}

			public bool CanExecute(object parameter) => true;

			public event EventHandler CanExecuteChanged;

			public void Execute(object parameter)
			{
				if (parameter is string uuid)
				{
					if (viewModel.model.WSBConfigCollection.FirstOrDefault(item => item.UUID == uuid) is WSBConfigManagerModel exportModel)
					{
						viewModel.ExportSandboxConfingAction?.Invoke(
							exportModel.Name,
							async (file, exportErrorCallback) => {
								try
								{
									CachedFileManager.DeferUpdates(file);
									using (var s = await file.OpenStreamForWriteAsync())
									using (var sw = new StreamWriter(s))
									{
										((WSBConfigModel)exportModel).Export(sw);
									}
									var status = await CachedFileManager.CompleteUpdatesAsync(file);
									if (status != FileUpdateStatus.Complete)
									{
										exportErrorCallback?.Invoke();
									}
								}
								catch (Exception)
								{
									exportErrorCallback?.Invoke();
								}
							}
						);
					}
				}
			}
		}

		private class MoveUpSandboxConfigCommand : ICommand
		{

			private readonly WSBManagerViewModel viewModel;

			internal MoveUpSandboxConfigCommand(WSBManagerViewModel _viewModel)
			{
				viewModel = _viewModel;
				viewModel.PropertyChanged += (sender, e) => CanExecuteChanged?.Invoke(sender, e);
			}

			public bool CanExecute(object parameter) => viewModel.model.WSBConfigCollection.Any();

			public event EventHandler CanExecuteChanged;

			public void Execute(object parameter)
			{
				if (parameter is string uuid)
				{
					viewModel.model.MoveUp(uuid);
				}
			}
		}

		private class MoveDownSandboxConfigCommand : ICommand
		{

			private readonly WSBManagerViewModel viewModel;

			internal MoveDownSandboxConfigCommand(WSBManagerViewModel _viewModel)
			{
				viewModel = _viewModel;
				viewModel.PropertyChanged += (sender, e) => CanExecuteChanged?.Invoke(sender, e);
			}

			public bool CanExecute(object parameter) => viewModel.model.WSBConfigCollection.Any();

			public event EventHandler CanExecuteChanged;

			public void Execute(object parameter)
			{
				if (parameter is string uuid)
				{
					viewModel.model.MoveDown(uuid);
				}
			}
		}

		private class DeleteSandboxConfigCommand : ICommand
		{

			private readonly WSBManagerViewModel viewModel;

			internal DeleteSandboxConfigCommand(WSBManagerViewModel _viewModel)
			{
				viewModel = _viewModel;
				viewModel.PropertyChanged += (sender, e) => CanExecuteChanged?.Invoke(sender, e);
			}

			public bool CanExecute(object parameter) => true;

			public event EventHandler CanExecuteChanged;

			public void Execute(object parameter)
			{
				if (parameter is string uuid)
				{
					if (viewModel.model.WSBConfigCollection.FirstOrDefault(item => item.UUID == uuid) is WSBConfigManagerModel deleteModel)
					{
						viewModel.DeleteSandboxConfigAction?.Invoke(
							deleteModel.Name,
							() => viewModel.model.WSBConfigCollection.Remove(deleteModel)
						);
					}
				}
			}
		}

		#endregion
	}
}
