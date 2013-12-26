using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;

namespace Chapter8.Recipe4
{
	class Program
	{
		static void Main(string[] args)
		{
			IObservable<int> o = Observable.Return(0);
			using (var sub = OutputToConsole(o));
			Console.WriteLine(" ---------------- ");
	
			o = Observable.Empty<int>();
			using (var sub = OutputToConsole(o));
			Console.WriteLine(" ---------------- ");

			o = Observable.Throw<int>(new Exception());
			using (var sub = OutputToConsole(o));
			Console.WriteLine(" ---------------- ");

			o = Observable.Repeat(42);
			using (var sub = OutputToConsole(o.Take(5)));
			Console.WriteLine(" ---------------- ");

			o = Observable.Range(0, 10);
			using (var sub = OutputToConsole(o));
			Console.WriteLine(" ---------------- ");

			o = Observable.Create<int>(ob => {
				for (int i = 0; i < 10; i++)
				{
					ob.OnNext(i);
				}
				return Disposable.Empty;
			});
			using (var sub = OutputToConsole(o)) ;
			Console.WriteLine(" ---------------- ");

			o = Observable.Generate(
				0 // initial state
				, i => i < 5 // while this is true we continue the sequence
				, i => ++i // iteration
				, i => i*2 // selecting result
			);
			using (var sub = OutputToConsole(o));
			Console.WriteLine(" ---------------- ");

			IObservable<long> ol = Observable.Interval(TimeSpan.FromSeconds(1));
			using (var sub = OutputToConsole(ol))
			{
				Thread.Sleep(TimeSpan.FromSeconds(3));
			};
			Console.WriteLine(" ---------------- ");

			ol = Observable.Timer(DateTimeOffset.Now.AddSeconds(2));
			using (var sub = OutputToConsole(ol))
			{
				Thread.Sleep(TimeSpan.FromSeconds(3));
			};
			Console.WriteLine(" ---------------- ");
		}

		static IDisposable OutputToConsole<T>(IObservable<T> sequence)
		{
			return sequence.Subscribe(
				obj => Console.WriteLine("{0}", obj)
				, ex => Console.WriteLine("Error: {0}", ex.Message)
				, () => Console.WriteLine("Completed")
			);
		}
	}
}
