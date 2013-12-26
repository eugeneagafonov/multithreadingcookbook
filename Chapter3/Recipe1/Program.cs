using System;
using System.Threading;

namespace Chapter3.Recipe1
{
	class Program
	{
		static void Main(string[] args)
		{
			int threadId = 0;

			RunOnThreadPool poolDelegate = Test;

			var t = new Thread(() => Test(out threadId));
			t.Start();
			t.Join();

			Console.WriteLine("Thread id: {0}", threadId);

			IAsyncResult r = poolDelegate.BeginInvoke(out threadId, Callback, "a delegate asynchronous call");
			r.AsyncWaitHandle.WaitOne();

			string result = poolDelegate.EndInvoke(out threadId, r);
			
			Console.WriteLine("Thread pool worker thread id: {0}", threadId);
			Console.WriteLine(result);

			Thread.Sleep(TimeSpan.FromSeconds(2));
		}

		private delegate string RunOnThreadPool(out int threadId);

		private static void Callback(IAsyncResult ar)
		{
			Console.WriteLine("Starting a callback...");
			Console.WriteLine("State passed to a callbak: {0}", ar.AsyncState);
			Console.WriteLine("Is thread pool thread: {0}", Thread.CurrentThread.IsThreadPoolThread);
			Console.WriteLine("Thread pool worker thread id: {0}", Thread.CurrentThread.ManagedThreadId);
		}


		private static string Test(out int threadId)
		{
			Console.WriteLine("Starting...");
			Console.WriteLine("Is thread pool thread: {0}", Thread.CurrentThread.IsThreadPoolThread);
			Thread.Sleep(TimeSpan.FromSeconds(2));
			threadId = Thread.CurrentThread.ManagedThreadId;
			return string.Format("Thread pool worker thread id was: {0}", threadId);
		}
	}
}
