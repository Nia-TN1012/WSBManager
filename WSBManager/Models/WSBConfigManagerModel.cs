using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Serialization;

using WSBManager.Configurations;

namespace WSBManager.Models {

	[XmlRoot( "Configuration" )]
	public class WSBConfigManagerModel : WSBConfigModel {

		protected string uuid = Guid.NewGuid().ToString( "B" );
		/// <summary>
		/// UUID
		/// </summary>
		public string UUID { get => uuid; set { } }

		/// <summary>
		/// Name
		/// </summary>
		public string Name { get; set; } = "New Sandbox";

		protected DateTime createdAt = DateTime.Now;
		/// <summary>
		/// Created datetime
		/// </summary>
		public DateTime CreateAt { get => createdAt; set { } }

		[XmlIgnore]
		public bool IsVGpuEnabled => VGpu == VGpu.Default;

		[XmlIgnore]
		public bool IsNetworkEnabled => Networking == Networking.Default;

		[XmlIgnore]
		public bool IsMappedFolderEnabled => MappedFolders.Count > 0;

		[XmlIgnore]
		public bool IsLoginCommandEnabled => !string.IsNullOrEmpty( LoginCommand?.Command );

		public WSBConfigManagerModel() : base() { }

		public WSBConfigManagerModel( WSBConfigManagerModel wSBConfigManagerModel ) : base( wSBConfigManagerModel ) {
			UUID = wSBConfigManagerModel.UUID;
			Name = wSBConfigManagerModel.Name;
			CreateAt = wSBConfigManagerModel.CreateAt;
		}

		/// <summary>
		/// Imports from a configuration xml text reader.
		/// </summary>
		/// <param name="webConfig">A configuration xml text reader</param>
		/// <returns></returns>
		public new static WSBConfigManagerModel Import( TextReader webConfig ) {
			using( var xr = XmlReader.Create( webConfig ) ) {
				var serializer = new XmlSerializer( typeof( WSBConfigManagerModel ) );
				return ( WSBConfigManagerModel )serializer.Deserialize( xr );
			}
		}

	}
}
