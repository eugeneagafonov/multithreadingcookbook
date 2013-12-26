using System;
using System.Threading;

namespace Chapter3.Recipe6
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Press 'Enter' to stop the timer...");
			DateTime start = DateTime.Now;
			_timer = new Timer(_ => TimerOperation(start), null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));

			Thread.Sleep(TimeSpan.FromSeconds(6));

			_timer.Change(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(4));

			Console.ReadLine();

			_timer.Dispose();
		}

		static Timer _timer;

		static void TimerOperation(DateTime start)
		{
			TimeSpan elapsed = DateTime.Now - start;
			Console.WriteLine("{0} seconds from {1}. Timer thread pool thread id: {2}", elapsed.Seconds, start,
				Thread.CurrentThread.ManagedThreadId);
		}
	}
}
