using System;
using System.IO;

namespace ConsoleApplication1
{
	internal class Program
	{
		private const string RootFolderFullPath = @"C:\prg\ConsoleApplication1";

		private static void Main()
		{
			string[] csprojFullPathes = Directory.GetFiles(RootFolderFullPath, "*.csproj", SearchOption.AllDirectories);

			foreach (var csprojFullPath in csprojFullPathes)
			{
				using (CsprojCorrector c = new CsprojCorrector(csprojFullPath))
				{
					c.SetLangVersion("default");
					Console.WriteLine($"{Path.GetFileName(csprojFullPath)} - {c.GetLangVersion()}");
				}
			}
		}
	}
}