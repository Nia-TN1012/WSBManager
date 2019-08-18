using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using WSBManager.Configurations;

namespace WSBManager {

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
	/// Represents Windows Sandbox configuration model.
	/// </summary>
	public class WSBModel {

		/// <summary>
		/// Enables or disables GPU sharing.
		/// </summary>
		public VGpu VGpu { get; set; } = VGpu.Default;

		/// <summary>
		/// Enables or disables networking in the sandbox.
		/// </summary>
		public Network Network { get; set; } = Network.Default;

		/// <summary>
		/// Folders on the host shared with the sandbox.
		/// </summary>
		public List<MappedFolder> MappedFolders { get; private set; } = new List<MappedFolder>();

		/// <summary>
		/// A login command which will be invoked automatically after the container logs on.
		/// </summary>
		public string LoginCommand { get; set; } = null;

		/// <summary>
		/// Converts from a configuration xml string.
		/// </summary>
		/// <param name="wsbString">A configuration xml string</param>
		/// <returns></returns>
		public static WSBModel FromXML( string wsbString ) => FromXML( XDocument.Parse( wsbString ) );

		/// <summary>
		/// Converts from a configuration xml object ( <see cref="XDocument"/> ).
		/// </summary>
		/// <param name="wsbXml">A configuration xml object</param>
		/// <returns></returns>
		public static WSBModel FromXML( XDocument wsbXml ) {

			var wsbModel = new WSBModel();

			var configrations = wsbXml.Root;

			if( Enum.TryParse( configrations.Element( "VGpu" )?.Value, out VGpu vGpu ) ) {
				wsbModel.VGpu = vGpu;
			}
			if( Enum.TryParse( configrations.Element( "Network" )?.Value, out Network network ) ) {
				wsbModel.Network = network;
			}

			if( configrations.Element( "MappedFolders" ) != null ) {
				wsbModel.MappedFolders = configrations.Element( "MappedFolders" )
													.Elements( "MappedFolder" )
													.Select( mf => new MappedFolder {
														HostFolder = mf.Element( "HostFolder" ).Value,
														ReadOnly = bool.Parse( mf.Element( "ReadOnly" ).Value )
													} )
													.ToList();
			}

			wsbModel.LoginCommand = configrations.Element( "LoginCommand" )?.Element( "Command" )?.Value;

			return wsbModel;
		}

		/// <summary>
		/// Converts to a xml object.
		/// </summary>
		/// <returns></returns>
		public XDocument ToXML() {
			var xDocment = new XDocument();
			xDocment.Add(
				new XElement( "Configrations",
					new XElement( "VGpu", VGpu.ToString() ),
					new XElement( "Network", Network.ToString() ),
					new XElement( "MappedFolders",
						MappedFolders.Select( mf => new XElement( "MappedFolder",
							new XElement( "HostFolder", mf.HostFolder ),
							new XElement( "ReadOnly", mf.ReadOnly.ToString() )
						))
						.ToArray()
					),
					new XElement( "LoginCommand",
						new XElement( "Command", LoginCommand )
					)
				)
			);

			return xDocment;
		}

		/// <summary>
		/// Converts to a xml string.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => ToXML().ToString();
	}
}
