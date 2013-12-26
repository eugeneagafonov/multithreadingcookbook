using System;
using System.Threading;

namespace Chapter2.Recipe6
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Starting two operations");
			var t1 = new Thread(() => PerformOperation("Operation 1 is completed", 4));
			var t2 = new Thread(() => PerformOperation("Operation 2 is completed", 8));
			t1.Start();
			t2.Start();
			_countdown.Wait();
			Console.WriteLine("Both operations have been completed.");
			_countdown.Dispose();
		}

		static CountdownEvent _countdown = new CountdownEvent(2);

		static void PerformOperation(string message, int seconds)
		{
			Thread.Sleep(TimeSpan.FromSeconds(seconds));
			Console.WriteLine(message);
			_countdown.Signal();
		}
	}
}
