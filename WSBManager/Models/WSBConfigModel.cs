using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Serialization;

using WSBManager.Configurations;

namespace WSBManager.Models {

	/// <summary>
	/// A folder on the host shared with the sandbox.
	/// </summary>
	[XmlRoot( "MappedFolder" )]
	public class MappedFolder {

		/// <summary>
		/// The host folder path.
		/// </summary>
		[XmlElement( "HostFolder" )]
		public string HostFolder { get; set; } = null;

		/// <summary>
		/// The flag of whether to make it read-only on the sandbox.
		/// </summary>
		[XmlElement( "ReadOnly" )]
		public bool ReadOnly { get; set; } = false;

	}

	/// <summary>
	/// A login command which will be invoked automatically after the container logs on.
	/// </summary>
	[XmlRoot( "LoginCommand" )]
	public class LoginCommand {
		/// <summary>
		/// A command.
		/// </summary>
		[XmlElement( "Command" )]
		public string Command { get; set; } = null;

		public LoginCommand() {}

		public LoginCommand( LoginCommand loginCommand ) {
			Command = loginCommand.Command;
		}
	}

	/// <summary>
	/// Represents Windows Sandbox configuration model.
	/// </summary>
	[XmlRoot( "Configuration" )]
	public class WSBConfigModel : INotifyPropertyChanged {

		#region Properties

		/// <summary>
		/// Enables or disables GPU sharing.
		/// </summary>
		[XmlElement( "VGpu" )]
		public VGpu VGpu { get; set; } = VGpu.Default;

		/// <summary>
		/// Enables or disables networking in the sandbox.
		/// </summary>
		[XmlElement( "Network" )]
		public Network Network { get; set; } = Network.Default;

		/// <summary>
		/// Folders on the host shared with the sandbox.
		/// </summary>
		[XmlArray( "MappedFolders" ), XmlArrayItem( "MappedFolder" )]
		public List<MappedFolder> MappedFolders { get; protected set; } = new List<MappedFolder>();

		/// <summary>
		/// A login command which will be invoked automatically after the container logs on.
		/// </summary>
		[XmlElement( "LoginCommand" )]
		public LoginCommand LoginCommand { get; protected set; } = new LoginCommand();

		#endregion

		[XmlIgnore]
		public static readonly XmlWriterSettings xmlWriterSettings = new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true };

		[XmlIgnore]
		public static readonly XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces( new XmlQualifiedName[] { new XmlQualifiedName( string.Empty, string.Empty ) } );

		public WSBConfigModel() { }

		public WSBConfigModel( WSBConfigModel wSBConfigModel ) {
			VGpu = wSBConfigModel.VGpu;
			Network = wSBConfigModel.Network;
			MappedFolders = new List<MappedFolder>( wSBConfigModel.MappedFolders );
			LoginCommand = new LoginCommand( wSBConfigModel.LoginCommand );
		}

		/// <summary>
		/// Imports from a configuration xml text reader.
		/// </summary>
		/// <param name="webConfig">A configuration xml text reader</param>
		/// <returns></returns>
		public static WSBConfigModel Import( TextReader webConfig ) {
			using( var xr = XmlReader.Create( webConfig ) ) {
				var serializer = new XmlSerializer( typeof( WSBConfigModel ) );
				return ( WSBConfigModel )serializer.Deserialize( xr );
			}
		}

		/// <summary>
		/// Exports to a xml text stream.
		/// </summary>
		/// <returns></returns>
		public TextWriter Export( TextWriter textWriter ) {
			using( var xw = XmlWriter.Create( textWriter, xmlWriterSettings ) ) {
				var serializer = new XmlSerializer( GetType() );
				serializer.Serialize( xw, GetType(), xmlSerializerNamespaces );
				return textWriter;
			}
		}

		/// <summary>
		/// Converts to a xml string.
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			using( var sw = new StringWriter() ) {
				return Export( sw ).ToString();
			}
		}

		/// <summary>
		///	The event handler to be generated after the property changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		///	Notifies the property change that corresponds to the specified property name.
		/// </summary>
		/// <param name="propertyName">Property name</param>
		protected void NotifyPropertyChanged( [CallerMemberName]string propertyName = null ) =>
			PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
	}
}
