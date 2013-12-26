using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;

namespace Chapter8.Recipe1
{
	class Program
	{
		static void Main(string[] args)
		{
			foreach (int i in EnumerableEventSequence())
			{
				Console.Write(i);
			}
			Console.WriteLine();
			Console.WriteLine("IEnumerable");

			IObservable<int> o = EnumerableEventSequence().ToObservable();
			using (IDisposable subscription = o.Subscribe(Console.Write))
			{
				Console.WriteLine();
				Console.WriteLine("IObservable");
			}

			o = EnumerableEventSequence().ToObservable().SubscribeOn(TaskPoolScheduler.Default);
			using (IDisposable subscription = o.Subscribe(Console.Write))
			{
				Console.WriteLine();
				Console.WriteLine("IObservable async");
				Console.ReadLine();
			}
		}

		static IEnumerable<int> EnumerableEventSequence()
		{
			for (int i = 0; i < 10; i++)
			{
				Thread.Sleep(TimeSpan.FromSeconds(0.5));
				yield return i;
			}
		}
	}
}