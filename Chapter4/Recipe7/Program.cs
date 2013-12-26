using System;
using System.Threading;
using System.Threading.Tasks;

namespace Chapter4.Recipe7
{
	class Program
	{
		static void Main(string[] args)
		{
			Task<int> task;
			try
			{
				task = Task.Run(() => TaskMethod("Task 1", 2));
				int result = task.Result;
				Console.WriteLine("Result: {0}", result);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception caught: {0}", ex);
			}
			Console.WriteLine("----------------------------------------------");
			Console.WriteLine();

			try
			{
				task = Task.Run(() => TaskMethod("Task 2", 2));
				int result = task.GetAwaiter().GetResult();
				Console.WriteLine("Result: {0}", result);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception caught: {0}", ex);
			}
			Console.WriteLine("----------------------------------------------");
			Console.WriteLine();

			var t1 = new Task<int>(() => TaskMethod("Task 3", 3));
			var t2 = new Task<int>(() => TaskMethod("Task 4", 2));
			var complexTask = Task.WhenAll(t1, t2);
			var exceptionHandler = complexTask.ContinueWith(t => 
					Console.WriteLine("Exception caught: {0}", t.Exception), 
					TaskContinuationOptions.OnlyOnFaulted
				);
			t1.Start();
			t2.Start();

			Thread.Sleep(TimeSpan.FromSeconds(5));
		}

		static int TaskMethod(string name, int seconds)
		{
			Console.WriteLine("Task {0} is running on a thread id {1}. Is thread pool thread: {2}",
				name, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
			Thread.Sleep(TimeSpan.FromSeconds(seconds));
			throw new Exception("Boom!");
			return 42 * seconds;
		}
	}
}
