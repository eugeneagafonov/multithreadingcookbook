using System;
using System.Threading;
using System.Threading.Tasks;

namespace Chapter5.Recipe5
{
	class Program
	{
		static void Main(string[] args)
		{
			Task t = AsynchronousProcessing();
			t.Wait();
		}

		async static Task AsynchronousProcessing()
		{
			Console.WriteLine("1. Single exception");

			try
			{
				string result = await GetInfoAsync("Task 1", 2);
				Console.WriteLine(result);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception details: {0}", ex);
			}

			Console.WriteLine();
			Console.WriteLine("2. Multiple exceptions");

			Task<string> t1 = GetInfoAsync("Task 1", 3);
			Task<string> t2 = GetInfoAsync("Task 2", 2);
			try
			{
				string[] results = await Task.WhenAll(t1, t2);
				Console.WriteLine(results.Length);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception details: {0}", ex);
			}

			Console.WriteLine();
			Console.WriteLine("2. Multiple exceptions with AggregateException");

			t1 = GetInfoAsync("Task 1", 3);
			t2 = GetInfoAsync("Task 2", 2);
			Task<string[]> t3 = Task.WhenAll(t1, t2);
			try
			{
				string[] results = await t3;
				Console.WriteLine(results.Length);
			}
			catch
			{
				var ae = t3.Exception.Flatten();
				var exceptions = ae.InnerExceptions;
				Console.WriteLine("Exceptions caught: {0}", exceptions.Count);
				foreach (var e in exceptions)
				{
					Console.WriteLine("Exception details: {0}", e);
					Console.WriteLine();
				}
			}
		}

		async static Task<string> GetInfoAsync(string name, int seconds)
		{
			await Task.Delay(TimeSpan.FromSeconds(seconds));
			throw new Exception(string.Format("Boom from {0}!", name));
		}
	}
}
