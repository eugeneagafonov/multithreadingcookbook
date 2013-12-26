using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Chapter8.Recipe6
{
	class Program
	{
		private delegate string AsyncDelegate(string name);

		static void Main(string[] args)
		{
			IObservable<string> o = LongRunningOperationAsync("Task1");
			using (var sub = OutputToConsole(o))
			{
				Thread.Sleep(TimeSpan.FromSeconds(2));
			};
			Console.WriteLine(" ---------------- ");

			Task<string> t = LongRunningOperationTaskAsync("Task2");
			using (var sub = OutputToConsole(t.ToObservable()))
			{
				Thread.Sleep(TimeSpan.FromSeconds(2));
			};
			Console.WriteLine(" ---------------- ");

			AsyncDelegate asyncMethod = LongRunningOperation;

			// marked as obsolete, use tasks instead
			Func<string, IObservable<string>> observableFactory = 
				Observable.FromAsyncPattern<string, string>(asyncMethod.BeginInvoke, asyncMethod.EndInvoke);
			o = observableFactory("Task3");
			using (var sub = OutputToConsole(o))
			{
				Thread.Sleep(TimeSpan.FromSeconds(2));
			};
			Console.WriteLine(" ---------------- ");

			o = observableFactory("Task4");
			AwaitOnObservable(o).Wait();
			Console.WriteLine(" ---------------- ");

			using (var timer = new Timer(1000))
			{
				var ot = Observable.FromEventPattern<ElapsedEventHandler, ElapsedEventArgs>(
					h => timer.Elapsed += h
					,h => timer.Elapsed -= h);
				timer.Start();

				using (var sub = OutputToConsole(ot))
				{
					Thread.Sleep(TimeSpan.FromSeconds(5));
				}
				Console.WriteLine(" ---------------- ");
				timer.Stop();
			}
		}

		static async Task<T> AwaitOnObservable<T>(IObservable<T> observable)
		{
			T obj = await observable;
			Console.WriteLine("{0}", obj );
			return obj;
		}

		static Task<string> LongRunningOperationTaskAsync(string name)
		{
			return Task.Run(() => LongRunningOperation(name));
		}

		static IObservable<string> LongRunningOperationAsync(string name)
		{
			return Observable.Start(() => LongRunningOperation(name));
		}

		static string LongRunningOperation(string name)
		{
			Thread.Sleep(TimeSpan.FromSeconds(1));
			return string.Format("Task {0} is completed. Thread Id {1}", name, Thread.CurrentThread.ManagedThreadId);
		}

		static IDisposable OutputToConsole(IObservable<EventPattern<ElapsedEventArgs>> sequence)
		{
			return sequence.Subscribe(
				obj => Console.WriteLine("{0}", obj.EventArgs.SignalTime)
				, ex => Console.WriteLine("Error: {0}", ex.Message)
				, () => Console.WriteLine("Completed")
			);
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
