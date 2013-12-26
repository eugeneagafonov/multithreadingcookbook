using System;
using System.Threading;

namespace Chapter1.Recipe1
{
	class Program
	{
		static void Main(string[] args)
		{
			Thread t = new Thread(PrintNumbers);
			t.Start();
			PrintNumbers();
		}

		static void PrintNumbers()
		{
			Console.WriteLine("Starting...");
			for (int i = 1; i < 10; i++)
			{
				Console.WriteLine(i);
			}
		}
	}
}
