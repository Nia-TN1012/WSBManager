using System;
using System.Diagnostics;
using System.IO;
using Windows.Storage;

namespace WSBManager
{
	internal class Program
	{
		const string windowsSandboxExe = @"WindowsSandbox.exe";
		static void Main(string[] args)
		{
			Console.Title = "WSB Manager Sandbox Launch Helper";

			var wsbPath = ApplicationData.Current.LocalSettings.Values["wsbpath"];
			if (!File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.System)}\\{windowsSandboxExe}"))
			{
				Console.Error.WriteLine("Failed to launch sandbox: Windows Sandbox is not enabled or not supported.");
				return;
			}
			if (wsbPath == null || !File.Exists(wsbPath.ToString()))
			{
				Console.Error.WriteLine("Failed to launch sandbox: The temporary wsb file when booting from WSB Manager is missing or invalid.");
				return;
			}

			Console.WriteLine($"Launching sandbox ...: {Path.GetFileName(wsbPath.ToString())}");
			Process.Start(windowsSandboxExe, wsbPath.ToString());
		}
	}
}
