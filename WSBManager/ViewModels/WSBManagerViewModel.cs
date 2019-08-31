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

namespace WSBManager.ViewModels {

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
	}
}
