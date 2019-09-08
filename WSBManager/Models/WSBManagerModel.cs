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

		/// <summary>
		/// Root node name
		/// </summary>
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

		/// <summary>
		/// Constructor
		/// </summary>
		public WSBManagerModel() { }

		/// <summary>
		/// Loads a sandbox configuration list.
		/// </summary>
		/// <returns>true: Success / false : Failed</returns>
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

		/// <summary>
		/// Saves a sandbox configuration list.
		/// </summary>
		/// <returns>true: Success / false : Failed</returns>
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

		/// <summary>
		/// Finds the element by specified a condition.
		/// </summary>
		/// <param name="predicate">A condition</param>
		/// <returns>
		///		<para>A tuple of the element and the index.</para>
		///		<para>item: <see cref="WSBConfigManagerModel"/> element which found. or null if not found.</para>
		///		<para>index: The index of item. or -1 if not found.</para>
		/// </returns>
		public ( WSBConfigManagerModel item, int index ) FindItem( Func<WSBConfigManagerModel, bool> predicate ) {
			foreach( ( WSBConfigManagerModel item, int index ) in WSBConfigCollection.Select( ( item, i ) => ( item, i ) ) ) {
				if( predicate( item ) ) {
					return ( item, index );
				}
			}
			return ( null, -1 );
		}

		/// <summary>
		/// Moves up the item.
		/// </summary>
		/// <param name="uuid">UUID</param>
		public void MoveUp( string uuid ) {
			( WSBConfigManagerModel modeModel, int index ) = FindItem( item => item.UUID == uuid );
			if( modeModel != null ) {
				var moveTo = index == 0 ? WSBConfigCollection.Count - 1 : index - 1;
				WSBConfigCollection.Move( index, moveTo );
			}
		}

		/// <summary>
		/// Moves down the item.
		/// </summary>
		/// <param name="uuid">UUID</param>
		public void MoveDown( string uuid ) {
			(WSBConfigManagerModel modeModel, int index) = FindItem( item => item.UUID == uuid );
			if( modeModel != null ) {
				var moveTo = index == WSBConfigCollection.Count - 1 ? 0 : index + 1;
				WSBConfigCollection.Move( index, moveTo );
			}
		}

		/// <summary>
		/// A event handler which fire when Load configuration list completed.
		/// </summary>
		public event EventHandler LoadCongiugurationListCompleted;

		/// <summary>
		///	A event handler which fire when the property changed.
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
