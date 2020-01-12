using System;

namespace SETrimToClipboard
{
	class Program
	{
		[STAThread()]
		static void Main(string[] args)
		{
			string path = "";
			if (args.Length > 0) path = args[0];

			while (!System.IO.Directory.Exists(path))
			{
				Console.WriteLine(">" + path);
				Console.WriteLine("Invalid path. Enter the path the directory with your files. It can be sent as an argument or typed here.");
				path = Console.ReadLine().Replace("\"", "");
			}

			var watch = System.Diagnostics.Stopwatch.StartNew();

			Trim trim = new Trim(path);

			watch.Stop();
			var elapsedMs = watch.ElapsedMilliseconds;

			Console.WriteLine("\nCompleted in " + watch.ElapsedMilliseconds + "ms");
		}
	}
}
