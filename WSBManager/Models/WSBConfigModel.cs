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
	/// 3 state: Default, Enable and Disable
	/// </summary>
	public enum ThreeState
	{
		Default,
		Enable,
		Disable
	}

	/// <summary>
	/// 2 state: Default and Disable
	/// </summary>
	public enum TwoState
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
	/// Amount of memory that the sandbox can use in megabytes (MB).
	/// </summary>
	public class MemoryInMB
	{
		/// <summary>
		/// Gets or sets the amount of memory in megabytes (MB).
		/// </summary>
		public int AmountInMB { get; set; } = 0;

		/// <summary>
		/// Enables or disables specification the amount of memory
		/// </summary>
		public bool Enabled { get; set; } = false;
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
		public ThreeState VGpu { get; set; } = ThreeState.Default;

		/// <summary>
		/// Enables or disables networking in the sandbox.
		/// </summary>
		public TwoState Networking { get; set; } = TwoState.Default;

		/// <summary>
		/// Folders on the host shared with the sandbox.
		/// </summary>
		public List<MappedFolder> MappedFolders { get; set; } = new List<MappedFolder>();

		/// <summary>
		/// A logon command which will be invoked automatically after the container logs on.
		/// </summary>
		public LogonCommand LogonCommand { get; set; } = new LogonCommand();

		/// <summary>
		/// Enables or disables audio input in the sandbox.
		/// </summary>
		public ThreeState AudioInput { get; set; } = ThreeState.Default;

		/// <summary>
		/// Enables or disables video input in the sandbox.
		/// </summary>
		public ThreeState VideoInput { get; set; } = ThreeState.Default;

		/// <summary>
		/// Enables or disables additional security in the sandbox.
		/// </summary>
		public ThreeState ProtectedClient { get; set; } = ThreeState.Default;

		/// <summary>
		/// Enables or disables printer sharing in the sandbox.
		/// </summary>
		public ThreeState PrinterRedirection { get; set; } = ThreeState.Default;

		/// <summary>
		/// Enables or disables clipboard sharing in the sandbox.
		/// </summary>
		public TwoState ClipboardRedirection { get; set; } = TwoState.Default;

		/// <summary>
		/// Specifies the amount of memory that the sandbox can use in megabytes (MB).
		/// </summary>
		public MemoryInMB MemoryInMB { get; set; } = new MemoryInMB();

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
			AudioInput = wSBConfigModel.AudioInput;
			VideoInput = wSBConfigModel.VideoInput;
			ProtectedClient = wSBConfigModel.ProtectedClient;
			PrinterRedirection = wSBConfigModel.PrinterRedirection;
			ClipboardRedirection = wSBConfigModel.ClipboardRedirection;
			MemoryInMB = wSBConfigModel.MemoryInMB;
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
				&& Utility.TryConvert(typeof(ThreeState), xVGpu.Value) is ThreeState vGpu)
			{
				wsbConfigModel.VGpu = vGpu;
			}
			// Networking
			if (xElement.Element(nameof(Networking)) is XElement xNetworking
				&& Utility.TryConvert(typeof(TwoState), xNetworking.Value) is TwoState networking)
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

			// Audio Input
			if (xElement.Element(nameof(AudioInput)) is XElement xAudioInput
				&& Utility.TryConvert(typeof(ThreeState), xAudioInput.Value) is ThreeState audioInput)
			{
				wsbConfigModel.AudioInput = audioInput;
			}

			// Video Input
			if (xElement.Element(nameof(VideoInput)) is XElement xVideoInput
				&& Utility.TryConvert(typeof(ThreeState), xVideoInput.Value) is ThreeState videoInput)
			{
				wsbConfigModel.VideoInput = videoInput;
			}

			// Protected Client
			if (xElement.Element(nameof(ProtectedClient)) is XElement xProtectedClient
				&& Utility.TryConvert(typeof(ThreeState), xProtectedClient.Value) is ThreeState protectedClient)
			{
				wsbConfigModel.ProtectedClient = protectedClient;
			}

			// Printer Redirection
			if (xElement.Element(nameof(PrinterRedirection)) is XElement xPrinterRedirection
				&& Utility.TryConvert(typeof(ThreeState), xPrinterRedirection.Value) is ThreeState printerRedirection)
			{
				wsbConfigModel.PrinterRedirection = printerRedirection;
			}

			// Clipboard Redirection
			if (xElement.Element(nameof(ClipboardRedirection)) is XElement xClipboardRedirection
				&& Utility.TryConvert(typeof(TwoState), xClipboardRedirection.Value) is TwoState clipboardRedirection)
			{
				wsbConfigModel.ClipboardRedirection = clipboardRedirection;
			}

			// Memory in MB
			if (xElement.Element(nameof(MemoryInMB)) is XElement xMemoryInMB
				&& Utility.TryConvert(typeof(int), xMemoryInMB.Value) is int memoryInMB
				&& memoryInMB > 0)
			{
				wsbConfigModel.MemoryInMB.AmountInMB = memoryInMB;
				wsbConfigModel.MemoryInMB.Enabled = true;
			}

			return wsbConfigModel;
		}

		/// <summary>
		/// Exports to a xml text writer.
		/// </summary>
		/// <param name="textWriter">A <see cref="TextWriter"/> instance</param>
		/// <param name="includeExtraMetada">Include or not extra metadata.</param>
		/// <returns>The <see cref="TextWriter"/> instance as same as <paramref name="textWriter"/>.</returns>
		public TextWriter Export(TextWriter textWriter, bool includeExtraMetada = false)
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
		public XElement ToXElement(bool includeExtraMetada = false)
		{
			List<XElement> nodes = new List<XElement>
			{
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
				),
				// Audio Input
				new XElement(nameof(AudioInput), AudioInput.ToString()),
				// Video Input
				new XElement(nameof(VideoInput), VideoInput.ToString()),
				// Protected Client
				new XElement(nameof(ProtectedClient), ProtectedClient.ToString()),
				// Printer Redirection
				new XElement(nameof(PrinterRedirection), PrinterRedirection.ToString()),
				// Clipboard Redirection
				new XElement(nameof(ClipboardRedirection), ClipboardRedirection.ToString()),
			};
			
			if (MemoryInMB.Enabled && MemoryInMB.AmountInMB > 0)
			{
				// Memory in MB
				nodes.Add(new XElement(nameof(MemoryInMB), MemoryInMB.AmountInMB.ToString()));
			}

			return new XElement(RootNodeName, nodes);
		}

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
