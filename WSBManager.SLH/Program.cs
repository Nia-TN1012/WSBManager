using System;
using System.Diagnostics;
using System.IO;
using Windows.Storage;

/*
 * [NOTE]
 * Turn off the "Prefer 32 bit" checkbox in the project's "Properties" -> "Build".
 * (This is to prevent a forced redirect to %windir%\SysWOW64 directory on 64-bit Windows.)
 * 
 * 
 * https://stackoverflow.com/questions/62342330/starting-the-windows-sandbox-from-managed-code
 */

namespace WSBManager
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Console.Title = "WSB Manager Sandbox Launch Helper";
			string winSbPath = @"WindowsSandbox.exe";
			string wsbPath = "";
			try
			{
				// Get the wsb file path.
				wsbPath = $@"{ApplicationData.Current.TemporaryFolder.Path}\wsb-tmp.wsb";
				Console.WriteLine($"Launching sandbox ...");

				// Check the wsb file existence and whether the OS is 64-bit.
				if (!File.Exists(wsbPath))
				{
					throw new Exception($"[ERROR] wsb file \"{wsbPath}\" not found.");
				}
				if (!Environment.Is64BitOperatingSystem)
				{
					throw new Exception("[ERROR] Your Windows is 32-bit. 64-bit Windows is required to launch sandbox.");
				}

				bool launched = false;
				try
				{
					// Try to launch sandbox via WindowsSandbox.exe
					Process.Start(winSbPath, wsbPath);
					launched = true;
				}
				catch (Exception)
				{
					Console.Error.WriteLine("[WARN] Failed to launch sandbox with WindowsSandbox. Trying launch with cmd.");
				}

				if (!launched)
				{
					// Try to launch sandbox via cmd.exe
					var sbProc = new Process();
					sbProc.StartInfo.FileName = "cmd";
					sbProc.StartInfo.Arguments = $"/c start \"\" \"{wsbPath}\"";
					sbProc.StartInfo.CreateNoWindow = true;
					sbProc.Start();
				}
			}
			catch (Exception e)
			{
				Console.Error.WriteLine("[ERROR] Failed to launch sandbox.");
				Console.Error.WriteLine($"\tWindows Sandbox path: {winSbPath} (Exists: {File.Exists(winSbPath)})");
				Console.Error.WriteLine($"\twsb file path: {wsbPath} (Exists: {File.Exists(wsbPath)})");
				Console.Error.WriteLine(e);
				Console.WriteLine("Press any key to exit ...");
				Console.ReadLine();
			}
		}
	}
}
