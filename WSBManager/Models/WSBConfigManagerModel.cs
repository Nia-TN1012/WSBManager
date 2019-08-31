using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace WSBManager.Models {

	[XmlRoot( "Configuration" )]
	public class WSBConfigManagerModel : WSBConfigModel {

		/// <summary>
		/// UUID
		/// </summary>
		[XmlAttribute( "uuid" )]
		public string UUID { get; private set; } = Guid.NewGuid().ToString( "B" );

		/// <summary>
		/// Created datetime
		/// </summary>
		[XmlAttribute( "create-at" )]
		public DateTime CreateAt { get; private set; } = DateTime.Now;

		/// <summary>
		/// Imports from a configuration xml text reader.
		/// </summary>
		/// <param name="webConfig">A configuration xml text reader</param>
		/// <returns></returns>
		public new static WSBConfigModel Import( TextReader webConfig ) {
			using( var xr = XmlReader.Create( webConfig ) ) {
				var serializer = new XmlSerializer( typeof( WSBConfigManagerModel ) );
				return ( WSBConfigManagerModel )serializer.Deserialize( xr );
			}
		}

	}
}
