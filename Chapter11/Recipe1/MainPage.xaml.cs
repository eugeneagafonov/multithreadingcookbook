using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Chapter11.Recipe1
{
	/// <summary>
	///   An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private readonly DispatcherTimer _timer;
		private int _ticks;

		public MainPage()
		{
			InitializeComponent();
			_timer = new DispatcherTimer();
			_ticks = 0;
		}

		/// <summary>
		///   Invoked when this page is about to be displayed in a Frame.
		/// </summary>
		/// <param name="e">
		///   Event data that describes how this page was reached.  The Parameter
		///   property is typically used to configure the page.
		/// </param>
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			Grid.Children.Clear();
			var commonPanel = new StackPanel
			{
				Orientation = Orientation.Vertical,
				HorizontalAlignment = HorizontalAlignment.Center
			};

			var buttonPanel = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				HorizontalAlignment = HorizontalAlignment.Center
			};

			var textBlock = new TextBlock
			{
				Text = "Sample timer application",
				FontSize = 32,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(40)
			};

			var timerTextBlock = new TextBlock
			{
				Text = "0",
				FontSize = 32,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(40)
			};

			var timerStateTextBlock = new TextBlock
			{
				Text = "Timer is enabled",
				FontSize = 32,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(40)
			};

			var startButton = new Button { Content = "Start", FontSize = 32};
			var stopButton = new Button { Content = "Stop", FontSize = 32};

			buttonPanel.Children.Add(startButton);
			buttonPanel.Children.Add(stopButton);

			commonPanel.Children.Add(textBlock);
			commonPanel.Children.Add(timerTextBlock);
			commonPanel.Children.Add(timerStateTextBlock);
			commonPanel.Children.Add(buttonPanel);

			_timer.Interval = TimeSpan.FromSeconds(1);
			_timer.Tick += (sender, eventArgs) =>
			{
					timerTextBlock.Text = _ticks.ToString(); _ticks++;
			};
			_timer.Start();

			startButton.Click += (sender, eventArgs) =>
			{
				timerTextBlock.Text = "0";
				_timer.Start();
				_ticks = 1;
				timerStateTextBlock.Text = "Timer is enabled";
			};

			stopButton.Click += (sender, eventArgs) =>
			{
				_timer.Stop();
				timerStateTextBlock.Text = "Timer is disabled";
			};

			Grid.Children.Add(commonPanel);
		}
	}
}