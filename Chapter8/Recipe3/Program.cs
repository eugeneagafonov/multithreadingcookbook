using System;
using System.Reactive.Subjects;
using System.Threading;

namespace Chapter8.Recipe3
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Subject");
			var subject = new Subject<string>();

			subject.OnNext("A");
			using (var subscription = OutputToConsole(subject))
			{
				subject.OnNext("B");
				subject.OnNext("C");
				subject.OnNext("D");
				subject.OnCompleted();
				subject.OnNext("Will not be printed out");
			}

			Console.WriteLine("ReplaySubject");
			var replaySubject = new ReplaySubject<string>();

			replaySubject.OnNext("A");
			using (var subscription = OutputToConsole(replaySubject))
			{
				replaySubject.OnNext("B");
				replaySubject.OnNext("C");
				replaySubject.OnNext("D");
				replaySubject.OnCompleted();
			}

			Console.WriteLine("Buffered ReplaySubject");
			var bufferedSubject = new ReplaySubject<string>(2);

			bufferedSubject.OnNext("A");
			bufferedSubject.OnNext("B");
			bufferedSubject.OnNext("C");
			using (var subscription = OutputToConsole(bufferedSubject))
			{
				bufferedSubject.OnNext("D");
				bufferedSubject.OnCompleted();
			}

			Console.WriteLine("Time window ReplaySubject");
			var timeSubject = new ReplaySubject<string>(TimeSpan.FromMilliseconds(200));

			timeSubject.OnNext("A");
			Thread.Sleep(TimeSpan.FromMilliseconds(100));
			timeSubject.OnNext("B");
			Thread.Sleep(TimeSpan.FromMilliseconds(100));
			timeSubject.OnNext("C");
			Thread.Sleep(TimeSpan.FromMilliseconds(100));
			using (var subscription = OutputToConsole(timeSubject))
			{
				Thread.Sleep(TimeSpan.FromMilliseconds(300));
				timeSubject.OnNext("D");
				timeSubject.OnCompleted();
			}

			Console.WriteLine("AsyncSubject");
			var asyncSubject = new AsyncSubject<string>();

			asyncSubject.OnNext("A");
			using (var subscription = OutputToConsole(asyncSubject))
			{
				asyncSubject.OnNext("B");
				asyncSubject.OnNext("C");
				asyncSubject.OnNext("D");
				asyncSubject.OnCompleted();
			}

			Console.WriteLine("BehaviorSubject");
			var behaviorSubject = new BehaviorSubject<string>("Default");
			using (var subscription = OutputToConsole(behaviorSubject))
			{
				behaviorSubject.OnNext("B");
				behaviorSubject.OnNext("C");
				behaviorSubject.OnNext("D");
				behaviorSubject.OnCompleted();
			}
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
