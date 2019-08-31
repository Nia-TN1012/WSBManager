using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Windows.Storage;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace WSBManager.Models {

	/// <summary>
	/// The list of Windows Sandbox configuration model.
	/// </summary>
	[XmlRoot( "WSBConfigList" )]
	public class WSBManagerModel : INotifyPropertyChanged {

		/// <summary>
		/// The list of Windows Sandbox configurations.
		/// </summary>
		[XmlArray( "Configurations" ), XmlArrayItem( "Configuration" )]
		public List<WSBConfigManagerModel> WSBConfigCollection { get; private set; }

		public void Load( TextReader textReader ) {
			WSBConfigCollection.Clear();
			using( var xr = XmlReader.Create( textReader ) ) {
				var serializer = new XmlSerializer( typeof( WSBManagerModel ) );
				foreach( var item in ( ( WSBManagerModel )serializer.Deserialize( xr ) ).WSBConfigCollection ) {
					WSBConfigCollection.Add( item );
				}

				LoadCongiugurationListCompleted?.Invoke( this, null );
			}
		}

		public TextWriter Save( TextWriter textWriter ) {
			using( var xw = XmlWriter.Create( textWriter ) ) {
				var serializer = new XmlSerializer( typeof( WSBManagerModel ) );
				serializer.Serialize( xw, this );
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
