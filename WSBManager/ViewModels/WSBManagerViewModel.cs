using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using Windows.Storage;
using System.Runtime.CompilerServices;

using WSBManager.Models;
using System.IO;
using System.Windows.Input;
using Windows.Storage.Provider;

namespace WSBManager.ViewModels {

	public delegate void ImportFilePickerAction( Action<StorageFile, Action> fileSelectedCallback, Action canceledCallback = null );
	public delegate void ExportFilePickerAction( string name, Action<StorageFile, Action> fileSelectedCallback, Action canceledCallback = null );
	public delegate void DeleteConfirmAction( string name, Action confirmedCallback, Action canceledCallback = null );

	class WSBManagerViewModel : INotifyPropertyChanged {

		WSBManagerModel model;

		/// <summary>
		///	The application's local folder.
		/// </summary>
		private static readonly StorageFolder localFolder = ApplicationData.Current.LocalFolder;

		/// <summary>
		///	Represents the file name of registered favorite list.
		/// </summary>
		private const string wsbConfigListFileName = "WSBConfigList.xml";

		/// <summary>
		///	Represents the lock object for async.
		/// </summary>
		private SemaphoreSlim semaphore = new SemaphoreSlim( 1, 1 );

		public IEnumerable<WSBConfigManagerModel> Items => model?.WSBConfigCollection;

		public WSBManagerViewModel() {
			model = ( App.Current as App )?.Model;
			if( model == null ) {
				throw new Exception( $"Failed to get reference of model instance on the {GetType()} class." );
			}

			model.PropertyChanged += ( sender, e ) => PropertyChanged?.Invoke( sender, e );
		}

		public async Task LoadAsync() {
			await Task.Run( async () => {
				await semaphore.WaitAsync().ConfigureAwait( false );
				try {
					var localFile = await localFolder.TryGetItemAsync( wsbConfigListFileName );
					if( localFile is IStorageFile storageFile ) {
						using( var s = await storageFile.OpenStreamForReadAsync() )
						using( var sr = new StreamReader( s ) ) {
							model.Load( sr );
						}
					}
				}
				finally {
					semaphore.Release();
				}
			} );
		}

		public async Task SaveAsync() {
			await Task.Run( async () => {
				await semaphore.WaitAsync().ConfigureAwait( false );
				try {
					var localFile = await localFolder.CreateFileAsync( wsbConfigListFileName, CreationCollisionOption.ReplaceExisting ); ;
					if( localFile is IStorageFile storageFile ) {
						using( var s = await storageFile.OpenStreamForWriteAsync() )
						using( var sw = new StreamWriter( s ) ) {
							model.Save( sw );
						}
					}
				}
				finally {
					semaphore.Release();
				}
			} );
		}

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

		public event ImportFilePickerAction ImportSandboxConfingAction;

		public event ExportFilePickerAction ExportSandboxConfingAction;

		public event DeleteConfirmAction DeleteSandboxConfigAction;

		private ICommand importSandboxConfig;
		public ICommand ImportSandboxConfig =>
			importSandboxConfig ?? ( importSandboxConfig = new ImportSandboxConfigCommand( this ) );

		private ICommand exportSandboxConfig;
		public ICommand ExportSandboxConfig =>
			exportSandboxConfig ?? ( exportSandboxConfig = new ExportSandboxConfigCommand( this ) );

		private ICommand deleteSandboxConfig;
		public ICommand DeleteSandboxConfig =>
			deleteSandboxConfig ?? ( deleteSandboxConfig = new DeleteSandboxConfigCommand( this ) );

		private class ImportSandboxConfigCommand : ICommand {

			private WSBManagerViewModel viewModel;

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
									importModel.Name = file.Name;
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

			private WSBManagerViewModel viewModel;

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

		private class DeleteSandboxConfigCommand : ICommand {

			private WSBManagerViewModel viewModel;

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
