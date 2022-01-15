using System;
using System.Diagnostics;
using System.IO;
using Windows.Storage;

namespace WSBManager
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Console.Title = "WSB Manager Sandbox Launch Helper";
			string wsbPath = "";
			try
			{
				wsbPath = ApplicationData.Current.LocalSettings.Values["wsbpath"]?.ToString();
				Console.WriteLine($"Launching sandbox ...: {Path.GetFileName(wsbPath)}");
				Process.Start(@$"{Environment.GetFolderPath(Environment.SpecialFolder.System)}\WindowsSandbox.exe", wsbPath);
			} catch (Exception e)
			{
				Console.Error.WriteLine($"Failed to launch sandbox.: ${wsbPath}");
				Console.Error.WriteLine(e);
				Console.WriteLine("Press any key to exit ...");
				Console.ReadLine();
			}
		}
	}
}
