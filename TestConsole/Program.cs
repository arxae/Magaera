using System;
using Magaera;

namespace TestConsole
{
	internal class Program
	{
		private static void Main()
		{
			var magParser = new MagaeraParser();
			magParser.ParseFile("test.mg");

			for (int i = 0; i < 10; i++)
			{
				Console.WriteLine(magParser.GenerateSentenc());
			}

			Console.ReadKey();
		}
	}
}