using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Chapter7.Recipe3
{
	class Program
	{
		static void Main(string[] args)
		{
			var parallelQuery = from t in GetTypes().AsParallel()
													select EmulateProcessing(t);

			var cts = new CancellationTokenSource();
			cts.CancelAfter(TimeSpan.FromSeconds(3));

			try
			{
				parallelQuery
					.WithDegreeOfParallelism(Environment.ProcessorCount)
					.WithExecutionMode(ParallelExecutionMode.ForceParallelism)
					.WithMergeOptions(ParallelMergeOptions.Default)
					.WithCancellation(cts.Token)
					.ForAll(Console.WriteLine);
			}
			catch (OperationCanceledException)
			{
				Console.WriteLine("---");
				Console.WriteLine("Operation has been canceled!");
			}

			Console.WriteLine("---");
			Console.WriteLine("Unordered PLINQ query execution");
			var unorderedQuery = from i in ParallelEnumerable.Range(1, 30)
													 select i;

			foreach (var i in unorderedQuery)
			{
				Console.WriteLine(i);
			}

			Console.WriteLine("---");
			Console.WriteLine("Ordered PLINQ query execution");
			var orderedQuery = from i in ParallelEnumerable.Range(1, 30).AsOrdered()
													 select i;

			foreach (var i in orderedQuery)
			{
				Console.WriteLine(i);
			}
		}

		static string EmulateProcessing(string typeName)
		{
			Thread.Sleep(TimeSpan.FromMilliseconds(
				new Random(DateTime.Now.Millisecond).Next(250,350)));
			Console.WriteLine("{0} type was processed on a thread id {1}",
					typeName, Thread.CurrentThread.ManagedThreadId);
			return typeName;
		}

		static IEnumerable<string> GetTypes()
		{
			return from assembly in AppDomain.CurrentDomain.GetAssemblies()
						 from type in assembly.GetExportedTypes()
						 where type.Name.StartsWith("Web")
						 orderby type.Name.Length
						 select type.Name;
		}
	}
}
