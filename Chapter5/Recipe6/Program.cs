using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Chapter5.Recipe6
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			var app = new Application();
			var win = new Window();
			var panel = new StackPanel();
			var button = new Button();
			_label = new Label();
			_label.FontSize = 32;
			_label.Height = 200;
			button.Height = 100;
			button.FontSize = 32;
			button.Content = new TextBlock {Text = "Start asynchronous operations"};
			button.Click += Click;
			panel.Children.Add(_label);
			panel.Children.Add(button);
			win.Content = panel;
			app.Run(win);

			Console.ReadLine();
		}

		async static void Click(object sender, EventArgs e)
		{
			_label.Content = new TextBlock {Text = "Calculating..."};
			TimeSpan resultWithContext = await Test();
			TimeSpan resultNoContext = await TestNoContext();
			//TimeSpan resultNoContext = await TestNoContext().ConfigureAwait(false);
			var sb = new StringBuilder();
			sb.AppendLine(string.Format("With the context: {0}", resultWithContext));
			sb.AppendLine(string.Format("Without the context: {0}", resultNoContext));
			sb.AppendLine(string.Format("Ratio: {0:0.00}",
				resultWithContext.TotalMilliseconds/resultNoContext.TotalMilliseconds));
			_label.Content = new TextBlock {Text = sb.ToString()};
		}

		async static Task<TimeSpan> Test()
		{
			const int iterationsNumber = 100000;
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < iterationsNumber; i++)
			{
				var t = Task.Run(() => { });
				await t;
			}
			sw.Stop();
			return sw.Elapsed;
		}

		async static Task<TimeSpan> TestNoContext()
		{
			const int iterationsNumber = 100000;
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < iterationsNumber; i++)
			{
				var t = Task.Run(() => { });
				await t.ConfigureAwait(
					continueOnCapturedContext: false);
			}
			sw.Stop();
			return sw.Elapsed;
		}

		private static Label _label;
	}
}
