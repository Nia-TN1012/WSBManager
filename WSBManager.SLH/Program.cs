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
			try
			{
				var wsbPath = ApplicationData.Current.LocalSettings.Values["wsbpath"];
				Console.WriteLine($"Launching sandbox ...: {Path.GetFileName(wsbPath?.ToString())}");
				Process.Start(@"WindowsSandbox.exe", wsbPath?.ToString());
			} catch (Exception e)
			{
				Console.Error.WriteLine("Failed to launch sandbox.");
				Console.Error.WriteLine(e);
				Console.WriteLine("Press any key to exit ...");
				Console.ReadLine();
			}
		}
	}
}
