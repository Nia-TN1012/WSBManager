using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using Windows.Storage;
using System.Runtime.CompilerServices;

using WSBManager.Common;
using WSBManager.Models;
using System.IO;
using System.Windows.Input;
using Windows.Storage.Provider;
using System.Diagnostics;
using Windows.System;

namespace WSBManager.ViewModels {

	public delegate void ImportFilePickerAction( Action<StorageFile, Action> fileSelectedCallback, Action canceledCallback = null );
	public delegate void ExportFilePickerAction( string name, Action<StorageFile, Action> fileSelectedCallback, Action canceledCallback = null );
	public delegate void DeleteConfirmAction( string name, Action confirmedCallback, Action canceledCallback = null );

	class WSBManagerViewModel : INotifyPropertyChanged {

		/// <summary>
		///	The application's temporary folder.
		/// </summary>
		private static readonly StorageFolder tempFolder = ApplicationData.Current.TemporaryFolder;

		private readonly WSBManagerModel model;

		public IEnumerable<WSBConfigManagerModel> Items => model?.WSBConfigCollection;

		public WSBManagerViewModel() {
			model = ( App.Current as App )?.Model;
			if( model == null ) {
				throw new Exception( $"Failed to get reference of model instance on the {GetType()} class." );
			}

			model.PropertyChanged += ( sender, e ) => PropertyChanged?.Invoke( sender, e );
		}

		public async Task<bool> InitializeModelAsync() => await model.LoadAsync();


		/// <summary>
		///	The event handler to be generated after the property changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		///	Notifies the property change that corresponds to the specified property name.
		/// </summary>
		/// <param name="propertyName">Property name</param>
		private void NotifyPropertyChanged( [CallerMemberName]string propertyName = null ) =>
			PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );

		public event EventHandler<( bool success, string name )> LaunchSandboxCompleted;

		public event ImportFilePickerAction ImportSandboxConfingAction;

		public event ExportFilePickerAction ExportSandboxConfingAction;

		public event DeleteConfirmAction DeleteSandboxConfigAction;

		private ICommand launchSandbox;
		public ICommand LaunchSandbox =>
			launchSandbox ?? ( launchSandbox = new LaunchSandboxCommand( this ) );

		private ICommand importSandboxConfig;
		public ICommand ImportSandboxConfig =>
			importSandboxConfig ?? ( importSandboxConfig = new ImportSandboxConfigCommand( this ) );

		private ICommand exportSandboxConfig;
		public ICommand ExportSandboxConfig =>
			exportSandboxConfig ?? ( exportSandboxConfig = new ExportSandboxConfigCommand( this ) );

		private ICommand moveUpSandboxConfig;
		public ICommand MoveUpSandboxConfig =>
			moveUpSandboxConfig ?? ( moveUpSandboxConfig = new MoveUpSandboxConfigCommand( this ) );

		private ICommand moveDownSandboxConfig;
		public ICommand MoveDownSandboxConfig =>
			moveDownSandboxConfig ?? ( moveDownSandboxConfig = new MoveDownSandboxConfigCommand( this ) );

		private ICommand deleteSandboxConfig;
		public ICommand DeleteSandboxConfig =>
			deleteSandboxConfig ?? ( deleteSandboxConfig = new DeleteSandboxConfigCommand( this ) );

		private class LaunchSandboxCommand : ICommand {
			private readonly WSBManagerViewModel viewModel;

			internal LaunchSandboxCommand( WSBManagerViewModel _viewModel ) {
				viewModel = _viewModel;
				viewModel.PropertyChanged += ( sender, e ) => CanExecuteChanged?.Invoke( sender, e );
			}

			public bool CanExecute( object parameter ) => true;

			public event EventHandler CanExecuteChanged;

			public async void Execute( object parameter ) {
				if( parameter is string uuid ) {
					if( viewModel.model.WSBConfigCollection.FirstOrDefault( item => item.UUID == uuid ) is WSBConfigManagerModel launchModel ) {
						try {
							var tempFile = await tempFolder.CreateFileAsync( $"{launchModel.Name}_{launchModel.UUID}.wsb", CreationCollisionOption.ReplaceExisting );
							CachedFileManager.DeferUpdates( tempFile );
							using( var s = await tempFile.OpenStreamForWriteAsync() )
							using( var sw = new StreamWriter( s ) ) {
								launchModel.Export( sw );
							}
							var status = await CachedFileManager.CompleteUpdatesAsync( tempFile );
							if( status == FileUpdateStatus.Complete ) {
								var options = new LauncherOptions {
									DisplayApplicationPicker = true
								};
								bool success = await Launcher.LaunchFileAsync( tempFile, options );
								if( success ) {
									launchModel.UpdateLastLaunchedAt();
								}
								viewModel.LaunchSandboxCompleted?.Invoke( this, ( success, launchModel.Name ) );
							}
						}
						catch( Exception ) {
							viewModel.LaunchSandboxCompleted?.Invoke( this, ( false, launchModel.Name ) );
						}
					}
				}
			}
		}

		private class ImportSandboxConfigCommand : ICommand {

			private readonly WSBManagerViewModel viewModel;

			internal ImportSandboxConfigCommand( WSBManagerViewModel _viewModel ) {
				viewModel = _viewModel;
				viewModel.PropertyChanged += ( sender, e ) => CanExecuteChanged?.Invoke( sender, e );
			}

			public bool CanExecute( object parameter ) => true;

			public event EventHandler CanExecuteChanged;

			public void Execute( object parameter ) {
				viewModel.ImportSandboxConfingAction?.Invoke(
					async ( file, exportErrorCallback ) => {
						try {
							using( var s = await file.OpenStreamForReadAsync() )
							using( var sr = new StreamReader( s ) ) {
								var importModel = WSBConfigManagerModel.Import( sr );
								if( string.IsNullOrEmpty( importModel.Name ) ) {
									importModel.Name = Path.GetFileNameWithoutExtension( file.Name );
								}
								viewModel.model.WSBConfigCollection.Add( importModel );
							}
						}
						catch( Exception ) {
							exportErrorCallback?.Invoke();
						}
					}
				);
			}
		}

		private class ExportSandboxConfigCommand : ICommand {

			private readonly WSBManagerViewModel viewModel;

			internal ExportSandboxConfigCommand( WSBManagerViewModel _viewModel ) {
				viewModel = _viewModel;
				viewModel.PropertyChanged += ( sender, e ) => CanExecuteChanged?.Invoke( sender, e );
			}

			public bool CanExecute( object parameter ) => true;

			public event EventHandler CanExecuteChanged;

			public void Execute( object parameter ) {
				if( parameter is string uuid ) {
					if( viewModel.model.WSBConfigCollection.FirstOrDefault( item => item.UUID == uuid ) is WSBConfigManagerModel exportModel ) {
						viewModel.ExportSandboxConfingAction?.Invoke(
							exportModel.Name,
							async ( file, exportErrorCallback ) => {
								try {
									CachedFileManager.DeferUpdates( file );
									using( var s = await file.OpenStreamForWriteAsync() )
									using( var sw = new StreamWriter( s ) ) {
										exportModel.Export( sw );
									}
									var status = await CachedFileManager.CompleteUpdatesAsync( file );
									if( status != FileUpdateStatus.Complete ) {
										exportErrorCallback?.Invoke();
									}
								}
								catch( Exception ) {
									exportErrorCallback?.Invoke();
								}
							}
						);
					}
				}
			}
		}

		private class MoveUpSandboxConfigCommand : ICommand {

			private readonly WSBManagerViewModel viewModel;

			internal MoveUpSandboxConfigCommand( WSBManagerViewModel _viewModel ) {
				viewModel = _viewModel;
				viewModel.PropertyChanged += ( sender, e ) => CanExecuteChanged?.Invoke( sender, e );
			}

			public bool CanExecute( object parameter ) => viewModel.model.WSBConfigCollection.Any();

			public event EventHandler CanExecuteChanged;

			public void Execute( object parameter ) {
				if( parameter is string uuid ) {
					viewModel.model.MoveUp( uuid );
				}
			}
		}

		private class MoveDownSandboxConfigCommand : ICommand {

			private readonly WSBManagerViewModel viewModel;

			internal MoveDownSandboxConfigCommand( WSBManagerViewModel _viewModel ) {
				viewModel = _viewModel;
				viewModel.PropertyChanged += ( sender, e ) => CanExecuteChanged?.Invoke( sender, e );
			}

			public bool CanExecute( object parameter ) => viewModel.model.WSBConfigCollection.Any();

			public event EventHandler CanExecuteChanged;

			public void Execute( object parameter ) {
				if( parameter is string uuid ) {
					viewModel.model.MoveDown( uuid );
				}
			}
		}

		private class DeleteSandboxConfigCommand : ICommand {

			private readonly WSBManagerViewModel viewModel;

			internal DeleteSandboxConfigCommand( WSBManagerViewModel _viewModel ) {
				viewModel = _viewModel;
				viewModel.PropertyChanged += ( sender, e ) => CanExecuteChanged?.Invoke( sender, e );
			}

			public bool CanExecute( object parameter ) => true;

			public event EventHandler CanExecuteChanged;

			public void Execute( object parameter ) {
				if( parameter is string uuid ) {
					if( viewModel.model.WSBConfigCollection.FirstOrDefault( item => item.UUID == uuid ) is WSBConfigManagerModel deleteModel ) {
						viewModel.DeleteSandboxConfigAction?.Invoke(
							deleteModel.Name,
							() => viewModel.model.WSBConfigCollection.Remove( deleteModel )
						);
					}
				}
			}
		}
	}
}
