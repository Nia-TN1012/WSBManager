using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Windows.Storage;
using System.Runtime.CompilerServices;
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Data;
using System.Xml.Linq;
using System.Threading;

namespace WSBManager.Models {

	/// <summary>
	/// The list of Windows Sandbox configuration model.
	/// </summary>
	public class WSBManagerModel : INotifyPropertyChanged {

		public const string RootNodeName = "WSBConfigList";

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

		/// <summary>
		/// The list of Windows Sandbox configurations.
		/// </summary>
		public ObservableCollection<WSBConfigManagerModel> WSBConfigCollection { get; private set; } = new ObservableCollection<WSBConfigManagerModel>();

		public WSBManagerModel() { }

		public async Task<bool> LoadAsync() {
			await semaphore.WaitAsync().ConfigureAwait( false );
			try {
				var localFile = await localFolder.TryGetItemAsync( wsbConfigListFileName );
				if( localFile is IStorageFile storageFile ) {
					using( var s = await storageFile.OpenStreamForReadAsync() )
					using( var sr = new StreamReader( s ) ) {
						WSBConfigCollection.Clear();
						using( var xr = XmlReader.Create( sr ) ) {
							var xElement = XElement.Load( xr );
							foreach( var configItem in xElement.Elements( WSBConfigModel.RootNodeName ) ) {
								WSBConfigCollection.Add( WSBConfigManagerModel.FromXElement( configItem ) );
							}

							LoadCongiugurationListCompleted?.Invoke( this, null );
						}
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
		}

		public async Task<bool> SaveAsync() {
			await semaphore.WaitAsync().ConfigureAwait( false );
			try {
				var localFile = await localFolder.CreateFileAsync( wsbConfigListFileName, CreationCollisionOption.ReplaceExisting ); ;
				if( localFile is IStorageFile storageFile ) {
					using( var s = await storageFile.OpenStreamForWriteAsync() )
					using( var sw = new StreamWriter( s ) ) {
						using( var xw = XmlWriter.Create( sw ) ) {
							var xElement = new XElement( RootNodeName,
								WSBConfigCollection.Select( configItem => configItem.ToXElement( includeExtraMetada: true ) )
							);
							xElement.Save( xw );
						}
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
		}

		public ( WSBConfigManagerModel item, int index ) FindItem( Func<WSBConfigManagerModel, bool> predicate ) {
			foreach( ( WSBConfigManagerModel item, int index ) in WSBConfigCollection.Select( ( item, i ) => ( item, i ) ) ) {
				if( predicate( item ) ) {
					return ( item, index );
				}
			}
			return ( null, -1 );
		}

		public void MoveUp( string uuid ) {
			( WSBConfigManagerModel modeModel, int index ) = FindItem( item => item.UUID == uuid );
			if( modeModel != null ) {
				var moveTo = index == 0 ? WSBConfigCollection.Count - 1 : index - 1;
				WSBConfigCollection.Move( index, moveTo );
			}
		}

		public void MoveDown( string uuid ) {
			(WSBConfigManagerModel modeModel, int index) = FindItem( item => item.UUID == uuid );
			if( modeModel != null ) {
				var moveTo = index == WSBConfigCollection.Count - 1 ? 0 : index + 1;
				WSBConfigCollection.Move( index, moveTo );
			}
		}

		public event EventHandler LoadCongiugurationListCompleted;

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
