using System;
using System.Collections.Generic;
using System.Threading;

namespace Chapter2.Recipe8
{
	class Program
	{
		static void Main(string[] args)
		{
			new Thread(Read){ IsBackground = true }.Start();
			new Thread(Read){ IsBackground = true }.Start();
			new Thread(Read){ IsBackground = true }.Start();

			new Thread(() => Write("Thread 1")){ IsBackground = true }.Start();
			new Thread(() => Write("Thread 2")){ IsBackground = true }.Start();

			Thread.Sleep(TimeSpan.FromSeconds(30));
		}

		static ReaderWriterLockSlim _rw = new ReaderWriterLockSlim();
		static Dictionary<int, int> _items = new Dictionary<int, int>();

		static void Read()
		{
			Console.WriteLine("Reading contents of a dictionary");
			while (true)
			{
				try
				{
					_rw.EnterReadLock();
					foreach (var key in _items.Keys)
					{
						Thread.Sleep(TimeSpan.FromSeconds(0.1));
					}
				}
				finally
				{
					_rw.ExitReadLock();
				}
			}
		}

		static void Write(string threadName)
		{
			while (true)
			{
				try
				{
					int newKey = new Random().Next(250);
					_rw.EnterUpgradeableReadLock();
					if (!_items.ContainsKey(newKey))
					{
						try
						{
							_rw.EnterWriteLock();
							_items[newKey] = 1;
							Console.WriteLine("New key {0} is added to a dictionary by a {1}", newKey, threadName);
						}
						finally
						{
							_rw.ExitWriteLock();
						}
					}
					Thread.Sleep(TimeSpan.FromSeconds(0.1));
				}
				finally
				{
					_rw.ExitUpgradeableReadLock();
				}
			}
		}
	}
}
