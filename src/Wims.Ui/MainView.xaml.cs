using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReactiveUI;
using Wims.Core.Dto;
using Application = System.Windows.Application;
using Void = GaryNg.Utils.Void.Void;

namespace Wims.Ui
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainView
	{
		public MainView()
		{
			InitializeComponent();
			this.WhenActivated(d =>
			{
				this.Bind(ViewModel, vm => vm.TextQuery, v => v.TextQuery.Text)
					.DisposeWith(d);

				this.OneWayBind(ViewModel, vm => vm.IsTextQuery, v => v.TextQuery.Visibility)
					.DisposeWith(d);

				this.WhenAnyValue(v => v.KeysQuery.Keys)
					.Select(keys => keys.ToBindingDto())
					.BindTo(this, v => v.ViewModel.KeysQuery)
					.DisposeWith(d);

				this.OneWayBind(ViewModel, vm => vm.IsKeysQuery, v => v.KeysQuery.Visibility)
					.DisposeWith(d);

				this.OneWayBind(ViewModel, vm => vm.Results, v => v.Results.ItemsSource)
					.DisposeWith(d);

				this.OneWayBind(ViewModel, vm => vm.Results, v => v.ResultsContainer.Collection)
					.DisposeWith(d);

				this.Bind(ViewModel, vm => vm.QueryMode, v => v.IsKeyModeToggle.IsChecked,
						m => m switch
						{
							QueryModes.Text => false,
							QueryModes.Keys => true,
							_ => throw new ArgumentOutOfRangeException(nameof(m), m, null)
						}, t => t switch
						{
							false => QueryModes.Text,
							true => QueryModes.Keys
						})
					.DisposeWith(d);

				this.ViewModel.LoadShortcuts.Execute().Subscribe();

				// move window to center on results first load
				// https://stackoverflow.com/a/32599760
				this.ResultsContainer
					.Events().SizeChanged
					.Where(s => s.NewSize.Height > 0)
					.Subscribe(e =>
					{
						var mainWindow = Application.Current.MainWindow;
						var hwnd = new WindowInteropHelper(mainWindow).Handle;
						var currentMonitor = Screen.FromHandle(hwnd);

						var source = PresentationSource.FromVisual(mainWindow);
						double dpiScaling = source?.CompositionTarget?.TransformFromDevice.M11 ?? 1;

						var workArea = currentMonitor.WorkingArea;
						var width = (int)Math.Floor(workArea.Width * dpiScaling);
						var height = (int)Math.Floor(workArea.Height * dpiScaling);

						mainWindow.Left = (((width - (mainWindow.ActualWidth * dpiScaling)) / 2) +
										   (workArea.Left * dpiScaling));
						mainWindow.Top = (((height - (mainWindow.ActualHeight * dpiScaling)) / 2) +
										  (workArea.Top * dpiScaling));
					})
					.DisposeWith(d);
			});
		}

		private void OnDrag(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				DragMove();
			}
		}
	}
}