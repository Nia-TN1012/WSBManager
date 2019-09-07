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

namespace WSBManager.Models {

	/// <summary>
	/// The list of Windows Sandbox configuration model.
	/// </summary>
	public class WSBManagerModel : INotifyPropertyChanged {

		public const string RootNodeName = "WSBConfigList";

		/// <summary>
		/// The list of Windows Sandbox configurations.
		/// </summary>
		public ObservableCollection<WSBConfigManagerModel> WSBConfigCollection { get; private set; } = new ObservableCollection<WSBConfigManagerModel>();

		public WSBManagerModel() { }

		public void Load( TextReader textReader ) {
			WSBConfigCollection.Clear();
			using( var xr = XmlReader.Create( textReader ) ) {
				var xElement = XElement.Load( xr );
				foreach( var configItem in xElement.Elements( WSBConfigModel.RootNodeName ) ) {
					WSBConfigCollection.Add( WSBConfigManagerModel.FromXElement( configItem ) );
				}

				LoadCongiugurationListCompleted?.Invoke( this, null );
			}
		}

		public TextWriter Save( TextWriter textWriter ) {
			using( var xw = XmlWriter.Create( textWriter ) ) {
				var xElement = new XElement( RootNodeName,
					WSBConfigCollection.Select( configItem => configItem.ToXElement() )
				);
				xElement.Save( xw );
				return textWriter;
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
