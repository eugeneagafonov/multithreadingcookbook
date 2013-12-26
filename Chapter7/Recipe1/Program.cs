using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Chapter7.Recipe1
{
	class Program
	{
		static void Main(string[] args)
		{
			Parallel.Invoke(
				() => EmulateProcessing("Task1"),
				() => EmulateProcessing("Task2"),
				() => EmulateProcessing("Task3")
			);

			var cts = new CancellationTokenSource();

			var result = Parallel.ForEach(
				Enumerable.Range(1, 30),
				new ParallelOptions
				{
					CancellationToken = cts.Token,
					MaxDegreeOfParallelism = Environment.ProcessorCount,
					TaskScheduler = TaskScheduler.Default
				},
				(i, state) =>
				{
					Console.WriteLine(i);
					if (i == 20)
					{
						state.Break();
						Console.WriteLine("Loop is stopped: {0}", state.IsStopped);
					}
				});

			Console.WriteLine("---");
			Console.WriteLine("IsCompleted: {0}", result.IsCompleted);
			Console.WriteLine("Lowest break iteration: {0}", result.LowestBreakIteration);
		}

		static string EmulateProcessing(string taskName)
		{
			Thread.Sleep(TimeSpan.FromMilliseconds(
				new Random(DateTime.Now.Millisecond).Next(250, 350)));
			Console.WriteLine("{0} task was processed on a thread id {1}",
					taskName, Thread.CurrentThread.ManagedThreadId);
			return taskName;
		}
	}
}
