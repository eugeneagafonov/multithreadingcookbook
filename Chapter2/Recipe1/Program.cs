using System;
using System.Threading;

namespace Chapter2.Recipe1
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			Console.WriteLine("Incorrect counter");

			var c = new Counter();

			var t1 = new Thread(() => TestCounter(c));
			var t2 = new Thread(() => TestCounter(c));
			var t3 = new Thread(() => TestCounter(c));
			t1.Start();
			t2.Start();
			t3.Start();
			t1.Join();
			t2.Join();
			t3.Join();

			Console.WriteLine("Total count: {0}", c.Count);
			Console.WriteLine("--------------------------");

			Console.WriteLine("Correct counter");

			var c1 = new CounterNoLock();

			t1 = new Thread(() => TestCounter(c1));
			t2 = new Thread(() => TestCounter(c1));
			t3 = new Thread(() => TestCounter(c1));
			t1.Start();
			t2.Start();
			t3.Start();
			t1.Join();
			t2.Join();
			t3.Join();

			Console.WriteLine("Total count: {0}", c1.Count);
		}

		static void TestCounter(CounterBase c)
		{
			for (int i = 0; i < 100000; i++)
			{
				c.Increment();
				c.Decrement();
			}
		}

		class Counter : CounterBase
		{
			private int _count;

			public int Count { get { return _count; } }

			public override void Increment()
			{
				_count++;
			}

			public override void Decrement()
			{
				_count--;
			}
		}

		class CounterNoLock : CounterBase
		{
			private int _count;

			public int Count { get { return _count; } }

			public override void Increment()
			{
				Interlocked.Increment(ref _count);
			}

			public override void Decrement()
			{
				Interlocked.Decrement(ref _count);
			}
		}

		abstract class CounterBase
		{
			public abstract void Increment();

			public abstract void Decrement();
		}
	}
}
