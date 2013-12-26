using System;
using System.Threading;

namespace Chapter1.Recipe10
{
	class Program
	{
		static void Main(string[] args)
		{
			object lock1 = new object();
			object lock2 = new object();

			new Thread(() => LockTooMuch(lock1, lock2)).Start();

			lock (lock2)
			{
				Thread.Sleep(1000);
				Console.WriteLine("Monitor.TryEnter allows not to get stuck, returning false after a specified timeout is elapsed");
				if (Monitor.TryEnter(lock1, TimeSpan.FromSeconds(5)))
				{
					Console.WriteLine("Acquired a protected resource succesfully");
				}
				else
				{
					Console.WriteLine("Timeout acquiring a resource!");
				}
			}

			new Thread(() => LockTooMuch(lock1, lock2)).Start();

			Console.WriteLine("----------------------------------");
			lock (lock2)
			{
				Console.WriteLine("This will be a deadlock!");
				Thread.Sleep(1000);
				lock (lock1)
				{
					Console.WriteLine("Acquired a protected resource succesfully");
				}
			}
		}

		static void LockTooMuch(object lock1, object lock2)
		{
			lock (lock1)
			{
				Thread.Sleep(1000);
				lock (lock2);
			}
		}
	}
}
