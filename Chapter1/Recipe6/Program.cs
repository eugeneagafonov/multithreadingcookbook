using System;
using System.Diagnostics;
using System.Threading;

namespace Chapter1.Recipe6
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Current thread priority: {0}", Thread.CurrentThread.Priority);
			Console.WriteLine("Running on all cores available");
			RunThreads();
			Thread.Sleep(TimeSpan.FromSeconds(2));
			Console.WriteLine("Running on a single core");
			Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(1);
			RunThreads();
		}

		static void RunThreads()
		{
			var sample = new ThreadSample();

			var threadOne = new Thread(sample.CountNumbers);
			threadOne.Name = "ThreadOne";
			var threadTwo = new Thread(sample.CountNumbers);
			threadTwo.Name = "ThreadTwo";

			threadOne.Priority = ThreadPriority.Highest;
			threadTwo.Priority = ThreadPriority.Lowest;
			threadOne.Start();
			threadTwo.Start();

			Thread.Sleep(TimeSpan.FromSeconds(2));
			sample.Stop();
		}

		class ThreadSample
		{
			private bool _isStopped = false;

			public void Stop()
			{
				_isStopped = true;
			}

			public void CountNumbers()
			{
				long counter = 0;

				while (!_isStopped)
				{
					counter++;
				}

				Console.WriteLine("{0} with {1,11} priority " +
							"has a count = {2,13}", Thread.CurrentThread.Name,
							Thread.CurrentThread.Priority,
							counter.ToString("N0"));
			}
		}
	}
}
