using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using WSBManager.Common;

namespace WSBManager.Models
{

	/// <summary>
	/// Enables or disables GPU sharing.
	/// </summary>
	public enum VGpu
	{
		Default,
		Disable
	}

	/// <summary>
	/// Enables or disables networking in the sandbox.
	/// </summary>
	public enum Networking
	{
		Default,
		Disable
	}

	/// <summary>
	/// A folder on the host shared with the sandbox.
	/// </summary>
	public class MappedFolder
	{

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
	/// A logon command which will be invoked automatically after the container logs on.
	/// </summary>
	public class LogonCommand
	{
		/// <summary>
		/// A command.
		/// </summary>
		public string Command { get; set; } = null;

		/// <summary>
		/// Constructor
		/// </summary>
		public LogonCommand() { }

		/// <summary>
		/// Creates a new instance of the <see cref="LogonCommand"/> class from an existing instance.
		/// </summary>
		/// <param name="logonCommand">A existing instance of the <see cref="LogonCommand"/> class</param>
		public LogonCommand(LogonCommand logonCommand)
		{
			Command = logonCommand.Command;
		}
	}

	/// <summary>
	/// Windows Sandbox configuration model.
	/// </summary>
	public class WSBConfigModel
	{

		/// <summary>
		/// Root node name
		/// </summary>
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

		/// <summary>
		/// Folders on the host shared with the sandbox.
		/// </summary>
		public List<MappedFolder> MappedFolders { get; set; } = new List<MappedFolder>();

		/// <summary>
		/// A logon command which will be invoked automatically after the container logs on.
		/// </summary>
		public LogonCommand LogonCommand { get; set; } = new LogonCommand();

		#endregion

		/// <summary>
		/// A setting for <see cref="XmlWriter"/>.
		/// </summary>
		public static readonly XmlWriterSettings xmlWriterSettings = new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true };

		/// <summary>
		/// Constructor
		/// </summary>
		public WSBConfigModel() { }

		/// <summary>
		/// Creates a new instance of the <see cref="WSBConfigModel"/> class from an existing instance.
		/// </summary>
		/// <param name="wSBConfigModel">A existing instance of the <see cref="WSBConfigModel"/> class</param>
		public WSBConfigModel(WSBConfigModel wSBConfigModel)
		{
			VGpu = wSBConfigModel.VGpu;
			Networking = wSBConfigModel.Networking;
			MappedFolders = new List<MappedFolder>(wSBConfigModel.MappedFolders);
			LogonCommand = new LogonCommand(wSBConfigModel.LogonCommand);
		}

		/// <summary>
		/// Imports from a configuration xml text reader.
		/// </summary>
		/// <param name="webConfig">A configuration xml text reader</param>
		/// <returns>A <see cref="WSBConfigModel"/> instance</returns>
		public static WSBConfigModel Import(TextReader webConfig)
		{
			using (var xr = XmlReader.Create(webConfig))
			{
				var xElement = XElement.Load(xr);
				return FromXElement(XElement.Load(xr));
			}
		}

		/// <summary>
		/// Converts a <see cref="XElement"/> instance to a <see cref="WSBConfigModel"/> instance.
		/// </summary>
		/// <param name="xElement">A <see cref="XElement"/> instance contains configuration</param>
		/// <returns>A <see cref="WSBConfigModel"/> instance</returns>
		public static WSBConfigModel FromXElement(XElement xElement)
		{
			var wsbConfigModel = new WSBConfigModel();

			// VGPU
			if (xElement.Element(nameof(VGpu)) is XElement xVGpu
				&& Utility.TryConvert(typeof(VGpu), xVGpu.Value) is VGpu vGpu)
			{
				wsbConfigModel.VGpu = vGpu;
			}
			// Networking
			if (xElement.Element(nameof(Networking)) is XElement xNetworking
				&& Utility.TryConvert(typeof(Networking), xNetworking.Value) is Networking networking)
			{
				wsbConfigModel.Networking = networking;
			}
			// Mapped Folders
			if (xElement.Elements(nameof(MappedFolders)) is XElement xMappedFolders)
			{
				foreach (var xMappedFolder in xElement.Elements(nameof(MappedFolder)))
				{
					var mf = new MappedFolder();
					if (xMappedFolder.Element(nameof(mf.HostFolder)) is XElement xHostFolder
						&& xMappedFolder.Element(nameof(mf.ReadOnly)) is XElement xReadOnly
						&& Utility.TryConvert(xReadOnly.Value, TypeCode.Boolean) is bool readOnly)
					{
						mf.HostFolder = xHostFolder.Value;
						mf.ReadOnly = readOnly;
					}
					wsbConfigModel.MappedFolders.Add(mf);
				}
			}
			// Logon Command
			if (xElement.Element(nameof(LogonCommand))?.Element(nameof(wsbConfigModel.LogonCommand.Command)) is XElement xCommand)
			{
				wsbConfigModel.LogonCommand.Command = xCommand.Value;
			}

			return wsbConfigModel;
		}

		/// <summary>
		/// Exports to a xml text writer.
		/// </summary>
		/// <param name="textWriter">A <see cref="TextWriter"/> instance</param>
		/// <param name="includeExtraMetada">Include or not extra metadata.</param>
		/// <returns>The <see cref="TextWriter"/> instance as same as <paramref name="textWriter"/>.</returns>
		public virtual TextWriter Export(TextWriter textWriter, bool includeExtraMetada = false)
		{
			using (var xw = XmlWriter.Create(textWriter, xmlWriterSettings))
			{
				ToXElement(includeExtraMetada).Save(xw);
				return textWriter;
			}
		}

		/// <summary>
		/// Converts to a <see cref="XElement"/> instance.
		/// </summary>
		/// <param name="includeExtraMetada">Include or not extra metadata.</param>
		/// <returns>A <see cref="XElement"/> instance.</returns>
		public virtual XElement ToXElement(bool includeExtraMetada = false) =>
			new XElement(RootNodeName,
				//VGPU
				new XElement(nameof(VGpu), VGpu.ToString()),
				// Networking
				new XElement(nameof(Networking), Networking.ToString()),
				// Mapped Folders
				new XElement(nameof(MappedFolders),
					MappedFolders.Select(mf =>
					   new XElement(nameof(MappedFolder),
						   new XElement(nameof(mf.HostFolder), mf.HostFolder),
						   new XElement(nameof(mf.ReadOnly), mf.ReadOnly.ToString().ToLower())
					   )
					)
				),
				// Logon Command
				new XElement(nameof(LogonCommand),
					new XElement(nameof(LogonCommand.Command), LogonCommand.Command)
				)
			);

		/// <summary>
		/// Converts to a xml string.
		/// </summary>
		public override string ToString()
		{
			using (var sw = new StringWriter())
			{
				return Export(sw).ToString();
			}
		}
	}
}
