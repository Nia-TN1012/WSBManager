using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

using WSBManager.Models;

namespace WSBManager.Common {
	public class WSBManagerModelIO {

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
		private static readonly SemaphoreSlim semaphore = new SemaphoreSlim( 1, 1 );

		public static async Task<bool> LoadAsync( WSBManagerModel model ) => 
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
					return true;
				}
				catch( Exception e ) {
					return false;
				}
				finally {
					semaphore.Release();
				}
			} );

		public static async Task<bool> SaveAsync( WSBManagerModel model ) =>
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
					return true;
				}
				catch( Exception ) {
					return false;
				}
				finally {
					semaphore.Release();
				}
			} );

	}
}
