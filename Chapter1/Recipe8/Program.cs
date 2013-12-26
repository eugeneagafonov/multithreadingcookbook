using System;
using System.Threading;

namespace Chapter1.Recipe8
{
	class Program
	{
		static void Main(string[] args)
		{
			var sample = new ThreadSample(10);

			var threadOne = new Thread(sample.CountNumbers);
			threadOne.Name = "ThreadOne";
			threadOne.Start();
			threadOne.Join();

			Console.WriteLine("--------------------------");

			var threadTwo = new Thread(Count);
			threadTwo.Name = "ThreadTwo";
			threadTwo.Start(8);
			threadTwo.Join();

			Console.WriteLine("--------------------------");

			var threadThree = new Thread(() => CountNumbers(12));
			threadThree.Name = "ThreadThree";
			threadThree.Start();
			threadThree.Join();
			Console.WriteLine("--------------------------");

			int i = 10;
			var threadFour = new Thread(() => PrintNumber(i));
			i = 20;
			var threadFive = new Thread(() => PrintNumber(i));
			threadFour.Start(); 
			threadFive.Start();
		}

		static void Count(object iterations)
		{
			CountNumbers((int)iterations);
		}

		static void CountNumbers(int iterations)
		{
			for (int i = 1; i <= iterations; i++)
			{
				Thread.Sleep(TimeSpan.FromSeconds(0.5));
				Console.WriteLine("{0} prints {1}", Thread.CurrentThread.Name, i);
			}
		}

		static void PrintNumber(int number)
		{
			Console.WriteLine(number);
		}

		class ThreadSample
		{
			private readonly int _iterations;

			public ThreadSample(int iterations)
			{
				_iterations = iterations;
			}
			public void CountNumbers()
			{
				for (int i = 1; i <= _iterations; i++)
				{
					Thread.Sleep(TimeSpan.FromSeconds(0.5));
					Console.WriteLine("{0} prints {1}", Thread.CurrentThread.Name, i);
				}
			}
		}
	}

	
}
