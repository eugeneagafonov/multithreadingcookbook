using System;
using System.Reactive.Linq;

namespace Chapter8.Recipe5
{
	class Program
	{
		static void Main(string[] args)
		{
			IObservable<long> sequence = Observable.Interval(TimeSpan.FromMilliseconds(50)).Take(21);

			var evenNumbers = from n in sequence
												where n % 2 == 0
												select n;

			var oddNumbers = from n in sequence
											 where n % 2 != 0
											 select n;

			var combine = from n in evenNumbers.Concat(oddNumbers)
										select n;

			var nums = (from n in combine
									where n % 5 == 0
									select n)
					.Do(n => Console.WriteLine("------Number {0} is processed in Do method", n));

			using (var sub = OutputToConsole(sequence, 0))
			using (var sub2 = OutputToConsole(combine, 1))
			using (var sub3 = OutputToConsole(nums, 2))
			{
				Console.WriteLine("Press enter to finish the demo");
				Console.ReadLine();
			}
		}

		static IDisposable OutputToConsole<T>(IObservable<T> sequence, int innerLevel)
		{
			string delimiter = innerLevel == 0 ? string.Empty : new string('-', innerLevel*3);
			return sequence.Subscribe(
				obj => Console.WriteLine("{0}{1}", delimiter, obj)
				, ex => Console.WriteLine("Error: {0}", ex.Message)
				, () => Console.WriteLine("{0}Completed", delimiter)
			);
		}
	}
}
