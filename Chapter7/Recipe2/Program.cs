using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Chapter7.Recipe2
{
	class Program
	{
		static void Main(string[] args)
		{
			var sw = new Stopwatch();
			sw.Start();
			var query = from t in GetTypes()
									select EmulateProcessing(t);

			foreach (string typeName in query)
			{
				PrintInfo(typeName);
			}
			sw.Stop();
			Console.WriteLine("---");
			Console.WriteLine("Sequential LINQ query.");
			Console.WriteLine("Time elapsed: {0}", sw.Elapsed);
			Console.WriteLine("Press ENTER to continue....");
			Console.ReadLine();
			Console.Clear();
			sw.Reset();

			sw.Start();
			var parallelQuery = from t in ParallelEnumerable.AsParallel(GetTypes())
													select EmulateProcessing(t);

			foreach (var typeName in parallelQuery)
			{
				PrintInfo(typeName);
			}
			sw.Stop();
			Console.WriteLine("---");
			Console.WriteLine("Parallel LINQ query. The results are being merged on a single thread");
			Console.WriteLine("Time elapsed: {0}", sw.Elapsed);
			Console.WriteLine("Press ENTER to continue....");
			Console.ReadLine();
			Console.Clear();
			sw.Reset();

			sw.Start();
			parallelQuery = from t in GetTypes().AsParallel()
											select EmulateProcessing(t);

			parallelQuery.ForAll(PrintInfo);

			sw.Stop();
			Console.WriteLine("---");
			Console.WriteLine("Parallel LINQ query. The results are being processed in parallel");
			Console.WriteLine("Time elapsed: {0}", sw.Elapsed);
			Console.WriteLine("Press ENTER to continue....");
			Console.ReadLine();
			Console.Clear();
			sw.Reset();

			sw.Start();
			query = from t in GetTypes().AsParallel().AsSequential()
											select EmulateProcessing(t);

			foreach (string typeName in query)
			{
				PrintInfo(typeName);
			}

			sw.Stop();
			Console.WriteLine("---");
			Console.WriteLine("Parallel LINQ query, transformed into sequential.");
			Console.WriteLine("Time elapsed: {0}", sw.Elapsed);
			Console.WriteLine("Press ENTER to continue....");
			Console.ReadLine();
			Console.Clear();
		}

		static void PrintInfo(string typeName)
		{
			Thread.Sleep(TimeSpan.FromMilliseconds(150));
			Console.WriteLine("{0} type was printed on a thread id {1}",
					typeName, Thread.CurrentThread.ManagedThreadId);
		}

		static string EmulateProcessing(string typeName)
		{
			Thread.Sleep(TimeSpan.FromMilliseconds(150));
			Console.WriteLine("{0} type was processed on a thread id {1}",
					typeName, Thread.CurrentThread.ManagedThreadId);
			return typeName;
		}

		static IEnumerable<string> GetTypes()
		{
			return from assembly in AppDomain.CurrentDomain.GetAssemblies()
						 from type in assembly.GetExportedTypes()
						 where type.Name.StartsWith("Web")
						 select type.Name;

		}
	}
}
