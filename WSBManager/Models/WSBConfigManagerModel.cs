using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
using WSBManager.Common;

namespace WSBManager.Models {

	/// <summary>
	/// Windows Sandbox configuration model for WSB Manager.
	/// </summary>
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

		/// <summary>
		/// Datetime when the sandbox was last launched.
		/// </summary>
		public DateTime LastLaunchedAt { get; protected set; } = DateTime.MinValue;

		/// <summary>
		/// Enable or disable the vGPU.
		/// </summary>
		public bool IsVGpuEnabled => VGpu != VGpu.Disable;

		/// <summary>
		/// Enable or disable the Networking.
		/// </summary>
		public bool IsNetworkEnabled => Networking != Networking.Disable;

		/// <summary>
		/// Enable or disable the Mapped Folders.
		/// </summary>
		public bool IsMappedFolderEnabled => MappedFolders.Count > 0;

		/// <summary>
		/// Enable or disable the Login Command.
		/// </summary>
		public bool IsLoginCommandEnabled => !string.IsNullOrEmpty( LoginCommand?.Command );

		/// <summary>
		/// Constructor
		/// </summary>
		public WSBConfigManagerModel() : base() { }

		/// <summary>
		/// Creates a new instance of the <see cref="WSBConfigManagerModel"/> class from an existing instance.
		/// </summary>
		/// <param name="wSBConfigManagerModel">A existing instance of the <see cref="WSBConfigManagerModel"/> class</param>
		public WSBConfigManagerModel( WSBConfigManagerModel wSBConfigManagerModel ) : base( wSBConfigManagerModel ) {
			UUID = wSBConfigManagerModel.UUID;
			Name = wSBConfigManagerModel.Name;
			CreatedAt = wSBConfigManagerModel.CreatedAt;
		}

		/// <summary>
		/// Updates the datetime when the sandbox was last launched.
		/// </summary>
		public void UpdateLastLaunchedAt() => LastLaunchedAt = DateTime.Now;

		/// <summary>
		/// Imports from a configuration xml text reader.
		/// </summary>
		/// <param name="webConfig">A configuration xml text reader</param>
		/// <returns>A <see cref="WSBConfigManagerModel"/> instance</returns>
		public new static WSBConfigManagerModel Import( TextReader webConfig ) {
			using( var xr = XmlReader.Create( webConfig ) ) {
				return FromXElement( XElement.Load( xr ) );
			}
		}

		/// <summary>
		/// Converts a <see cref="XElement"/> instance to a <see cref="WSBConfigManagerModel"/> instance.
		/// </summary>
		/// <param name="xElement">A <see cref="XElement"/> instance contains configuration</param>
		/// <returns>A <see cref="WSBConfigManagerModel"/> instance</returns>
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
			if( xElement.Attribute( nameof( UUID ) ) is XAttribute xUUID ) {
				wsbConfigManagerModel.UUID = xUUID.Value;
			}
			if( xElement.Attribute( nameof( LastLaunchedAt ) ) is XAttribute xLastLaunchedAt
				&& Utility.TryConvert( xLastLaunchedAt.Value, TypeCode.DateTime ) is DateTime lastLaunchedAt ) {
				wsbConfigManagerModel.LastLaunchedAt = lastLaunchedAt;
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
		/// Exports to a xml text writer.
		/// </summary>
		/// <param name="textWriter">A <see cref="TextWriter"/> instance</param>
		/// <param name="includeExtraMetada">Include or not extra metadata.</param>
		/// <returns>The <see cref="TextWriter"/> instance as same as <paramref name="textWriter"/>.</returns>
		public override TextWriter Export( TextWriter textWriter, bool includeExtraMetada = false ) {
			using( var xw = XmlWriter.Create( textWriter, xmlWriterSettings ) ) {
				ToXElement( includeExtraMetada ).Save( xw );
				return textWriter;
			}
		}

		/// <summary>
		/// Converts to a <see cref="XElement"/> instance.
		/// </summary>
		/// <param name="includeExtraMetada">Include or not extra metadata.</param>
		/// <returns>A <see cref="XElement"/> instance.</returns>
		public override XElement ToXElement( bool includeExtraMetada = false ) {
			// Metadata
			List<XAttribute> metadatas = new List<XAttribute> {
				new XAttribute( nameof( Name ), Name ),
				new XAttribute( nameof( CreatedAt ), CreatedAt.ToString( "yyyy-MM-dd HH:mm:ss zzz" ) )
			};
			if( includeExtraMetada ) {
				metadatas.Add( new XAttribute( nameof( UUID ), UUID ) );
				metadatas.Add( new XAttribute( nameof( LastLaunchedAt ), LastLaunchedAt.ToString( "yyyy-MM-dd HH:mm:ss zzz" ) ) );
			}

			return new XElement( RootNodeName,
				// Metadata
				metadatas.ToArray(),
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
		}
	}
}
