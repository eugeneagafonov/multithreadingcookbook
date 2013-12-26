using System;
using System.Threading;

namespace Chapter1.Recipe3
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Starting program...");
			Thread t = new Thread(PrintNumbersWithDelay);
			t.Start();
			t.Join();
			Console.WriteLine("Thread completed");
		}

		static void PrintNumbersWithDelay()
		{
			Console.WriteLine("Starting...");
			for (int i = 1; i < 10; i++)
			{
				Thread.Sleep(TimeSpan.FromSeconds(2));
				Console.WriteLine(i);
			}
		}
	}
}
