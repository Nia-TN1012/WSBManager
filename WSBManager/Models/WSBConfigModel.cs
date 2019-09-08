using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
using WSBManager.Common;

namespace WSBManager.Models {

	/// <summary>
	/// Enables or disables GPU sharing.
	/// </summary>
	public enum VGpu {
		Default,
		Disable,
		Enable
	}

	/// <summary>
	/// Enables or disables networking in the sandbox.
	/// </summary>
	public enum Networking {
		Default,
		Disable,
		Enable
	}

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
	public class WSBConfigModel {

		public const string RootNodeName = "Configuration";

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
		public List<MappedFolder> MappedFolders { get => mappedFolders; set { } }


		protected LoginCommand loginCommand = new LoginCommand();
		/// <summary>
		/// A login command which will be invoked automatically after the container logs on.
		/// </summary>
		public LoginCommand LoginCommand { get => loginCommand; set { } }

		#endregion

		public static readonly XmlWriterSettings xmlWriterSettings = new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true };

		public WSBConfigModel() { }

		public WSBConfigModel( WSBConfigModel wSBConfigModel ) {
			VGpu = wSBConfigModel.VGpu;
			Networking = wSBConfigModel.Networking;
			mappedFolders = new List<MappedFolder>( wSBConfigModel.MappedFolders );
			loginCommand = new LoginCommand( wSBConfigModel.LoginCommand );
		}

		/// <summary>
		/// Imports from a configuration xml text reader.
		/// </summary>
		/// <param name="webConfig">A configuration xml text reader</param>
		/// <returns></returns>
		public static WSBConfigModel Import( TextReader webConfig ) {
			using( var xr = XmlReader.Create( webConfig ) ) {
				var xElement = XElement.Load( xr );
				return FromXElement( XElement.Load( xr ) );
			}
		}

		public static WSBConfigModel FromXElement( XElement xElement ) {
			var wsbConfigModel = new WSBConfigModel();

			// VGPU
			if( xElement.Element( nameof( VGpu ) ) is XElement xVGpu
				&& Utility.TryConvert( typeof( VGpu ), xVGpu.Value ) is VGpu vGpu ) {
				wsbConfigModel.VGpu = vGpu;
			}
			// Networking
			if( xElement.Element( nameof( Networking ) ) is XElement xNetworking
				&& Utility.TryConvert( typeof( Networking ), xNetworking.Value ) is Networking networking ) {
				wsbConfigModel.Networking = networking;
			}
			// Mapped Folders
			if( xElement.Elements( nameof( MappedFolders ) ) is XElement xMappedFolders ) {
				foreach( var xMappedFolder in xElement.Elements( nameof( MappedFolder ) ) ) {
					var mf = new MappedFolder();
					if( xMappedFolder.Element( nameof( mf.HostFolder ) ) is XElement xHostFolder
						&& xMappedFolder.Element( nameof( mf.ReadOnly ) ) is XElement xReadOnly
						&& Utility.TryConvert( xReadOnly.Value, TypeCode.Boolean ) is bool readOnly ) {
						mf.HostFolder = xHostFolder.Value;
						mf.ReadOnly = readOnly;
					}
					wsbConfigModel.MappedFolders.Add( mf );
				}
			}
			// Login Command
			if( xElement.Element( nameof( LoginCommand ) )?.Element( nameof( wsbConfigModel.LoginCommand.Command ) ) is XElement xCommand ) {
				wsbConfigModel.LoginCommand.Command = xCommand.Value;
			}

			return wsbConfigModel;
		}

		/// <summary>
		/// Exports to a xml text stream.
		/// </summary>
		/// <returns></returns>
		public virtual TextWriter Export( TextWriter textWriter, bool includeExtraMetada = false ) {
			using( var xw = XmlWriter.Create( textWriter, xmlWriterSettings ) ) {
				ToXElement( includeExtraMetada ).Save( xw );
				return textWriter;
			}
		}

		public virtual XElement ToXElement( bool includeExtraMetada = false ) =>
			new XElement( RootNodeName,
				//VGPU
				new XElement( nameof( VGpu ), VGpu.ToString() ),
				// Networking
				new XElement( nameof( Networking ), Networking.ToString() ),
				// Mapped Folders
				new XElement( nameof( MappedFolders ),
					MappedFolders.Select( mf =>
						new XElement( nameof( MappedFolder ),
							new XElement( nameof( mf.HostFolder ), mf.HostFolder ),
							new XElement( nameof( mf.ReadOnly ), mf.ReadOnly.ToString().ToLower() )
						)
					)
				),
				// Login Command
				new XElement( nameof( LoginCommand ),
					new XElement( nameof( LoginCommand.Command ), LoginCommand.Command )
				)
			);

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
