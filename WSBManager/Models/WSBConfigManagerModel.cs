using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
using WSBManager.Common;

namespace WSBManager.Models {

	public class WSBConfigManagerModel : WSBConfigModel {
		/// <summary>
		/// UUID
		/// </summary>
		public string UUID { get; protected set; } = Guid.NewGuid().ToString( "B" );

		/// <summary>
		/// Name
		/// </summary>
		public string Name { get; set; } = null;

		/// <summary>
		/// Created datetime
		/// </summary>
		public DateTime CreatedAt { get; protected set; } = DateTime.Now;

		public bool IsVGpuEnabled => VGpu == VGpu.Default;

		public bool IsNetworkEnabled => Networking == Networking.Default;

		public bool IsMappedFolderEnabled => MappedFolders.Count > 0;

		public bool IsLoginCommandEnabled => !string.IsNullOrEmpty( LoginCommand?.Command );

		public WSBConfigManagerModel() : base() { }

		public WSBConfigManagerModel( WSBConfigManagerModel wSBConfigManagerModel ) : base( wSBConfigManagerModel ) {
			UUID = wSBConfigManagerModel.UUID;
			Name = wSBConfigManagerModel.Name;
			CreatedAt = wSBConfigManagerModel.CreatedAt;
		}

		/// <summary>
		/// Imports from a configuration xml text reader.
		/// </summary>
		/// <param name="webConfig">A configuration xml text reader</param>
		/// <returns></returns>
		public new static WSBConfigManagerModel Import( TextReader webConfig ) {
			using( var xr = XmlReader.Create( webConfig ) ) {
				return FromXElement( XElement.Load( xr ) );
			}
		}

		public new static WSBConfigManagerModel FromXElement( XElement xElement ) {
			var wsbConfigManagerModel = new WSBConfigManagerModel();

			// Metadata
			if( xElement.Attribute( nameof( Name ) ) is XAttribute xName ) {
				wsbConfigManagerModel.Name = xName.Value;
			}
			if( xElement.Attribute( nameof( CreatedAt ) ) is XAttribute xCreatedAt
				&& Utility.TryConvert( xCreatedAt.Value, TypeCode.DateTime ) is DateTime createdAt ) {
				wsbConfigManagerModel.CreatedAt = createdAt;
			}

			// VGPU
			if( xElement.Element( nameof( VGpu ) ) is XElement xVGpu
				&& Utility.TryConvert( typeof( VGpu ), xVGpu.Value ) is VGpu vGpu ) {
				wsbConfigManagerModel.VGpu = vGpu;
			}
			// Networking
			if( xElement.Element( nameof( Networking ) ) is XElement xNetworking
				&& Utility.TryConvert( typeof( Networking ), xNetworking.Value ) is Networking networking ) {
				wsbConfigManagerModel.Networking = networking;
			}
			// Mapped Folders
			if( xElement.Element( nameof( MappedFolders ) ) is XElement xMappedFolders ) {
				foreach( var xMappedFolder in xMappedFolders.Elements( nameof( MappedFolder ) ) ) {
					var mf = new MappedFolder();
					if( xMappedFolder.Element( nameof( mf.HostFolder ) ) is XElement xHostFolder
						&& xMappedFolder.Element( nameof( mf.ReadOnly ) ) is XElement xReadOnly
						&& Utility.TryConvert( xReadOnly.Value, TypeCode.Boolean ) is bool readOnly ) {
						mf.HostFolder = xHostFolder.Value;
						mf.ReadOnly = readOnly;
					}
					wsbConfigManagerModel.MappedFolders.Add( mf );
				}
			}
			// Login Command
			if( xElement.Element( nameof( LoginCommand ) )?.Element( nameof( wsbConfigManagerModel.LoginCommand.Command ) ) is XElement xCommand ) {
				wsbConfigManagerModel.LoginCommand.Command = xCommand.Value;
			}

			return wsbConfigManagerModel;
		}

		/// <summary>
		/// Exports to a xml text stream.
		/// </summary>
		/// <returns></returns>
		public override TextWriter Export( TextWriter textWriter ) {
			using( var xw = XmlWriter.Create( textWriter, xmlWriterSettings ) ) {
				ToXElement().Save( xw );
				return textWriter;
			}
		}

		public override XElement ToXElement() =>
			new XElement( RootNodeName,
				// Metadata
				new XAttribute( nameof( Name ), Name ),
				new XAttribute( nameof( UUID ), UUID ),
				new XAttribute( nameof( CreatedAt ), CreatedAt.ToString( "yyyy-MM-dd HH:mm:ss zzz" ) ),
				//VGPU
				new XElement( nameof( VGpu ), VGpu.ToString() ),
				// Networking
				new XElement( nameof( Networking ), Networking.ToString() ),
				// Mapped Folders
				new XElement( nameof( MappedFolders ),
					MappedFolders.Select( mf =>
						new XElement( nameof( MappedFolder ),
							new XElement( nameof( mf.HostFolder ), mf.HostFolder ),
							new XElement( nameof( mf.ReadOnly ), mf.ReadOnly.ToString() )
						)
					)
				),
				// Login Command
				new XElement( nameof( LoginCommand ),
					new XElement( nameof( LoginCommand.Command ), LoginCommand.Command )
				)
			);
	}
}
