using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;
using Wims.Core.Dto;

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