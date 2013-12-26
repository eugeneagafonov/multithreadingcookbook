using System;
using System.Threading;

namespace Chapter2.Recipe2
{
	class Program
	{
		static void Main(string[] args)
		{
			const string MutexName = "CSharpThreadingCookbook";

			using (var m = new Mutex(false, MutexName))
			{
				if (!m.WaitOne(TimeSpan.FromSeconds(5), false))
				{
					Console.WriteLine("Second instance is running!");
				}
				else
				{
					Console.WriteLine("Running!");
					Console.ReadLine();
					m.ReleaseMutex();
				}
			}
		}
	}
}
