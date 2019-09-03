using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

using WSBManager.Configurations;

namespace WSBManager.Models {

	/// <summary>
	/// A folder on the host shared with the sandbox.
	/// </summary>
	public class MappedFolder {

		/// <summary>
		/// The host folder path.
		/// </summary>
		public string HostFolder { get; set; } = null;

		/// <summary>
		/// The flag of whether to make it read-only on the sandbox.
		/// </summary>
		public bool ReadOnly { get; set; } = false;

	}

	/// <summary>
	/// A login command which will be invoked automatically after the container logs on.
	/// </summary>
	public class LoginCommand {
		/// <summary>
		/// A command.
		/// </summary>
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
	public class WSBConfigModel {

		#region Properties

		/// <summary>
		/// Enables or disables GPU sharing.
		/// </summary>
		public VGpu VGpu { get; set; } = VGpu.Default;

		/// <summary>
		/// Enables or disables networking in the sandbox.
		/// </summary>
		public Networking Networking { get; set; } = Networking.Default;

		protected List<MappedFolder> mappedFolders = new List<MappedFolder>();
		/// <summary>
		/// Folders on the host shared with the sandbox.
		/// </summary>
		[XmlArray( "MappedFolders" ), XmlArrayItem( "MappedFolder" )]
		public List<MappedFolder> MappedFolders { get => mappedFolders; set { } }


		protected LoginCommand loginCommand = new LoginCommand();
		/// <summary>
		/// A login command which will be invoked automatically after the container logs on.
		/// </summary>
		public LoginCommand LoginCommand { get => loginCommand; set { } }

		#endregion

		[XmlIgnore]
		public static readonly XmlWriterSettings xmlWriterSettings = new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true };

		[XmlIgnore]
		public static readonly XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces( new XmlQualifiedName[] { new XmlQualifiedName( string.Empty, string.Empty ) } );

		public WSBConfigModel() { }

		public WSBConfigModel( WSBConfigModel wSBConfigModel ) {
			VGpu = wSBConfigModel.VGpu;
			Networking = wSBConfigModel.Networking;
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
				serializer.Serialize( xw, this, xmlSerializerNamespaces );
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
	}
}
