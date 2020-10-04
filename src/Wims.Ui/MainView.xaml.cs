using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using Gma.System.MouseKeyHook;
using NHotkey;
using ReactiveUI;
using Wims.Core.Dto;
using NHotkey.Wpf;
using Application = System.Windows.Application;

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
				this.OneWayBind(ViewModel, vm => vm.Context, v => v.ContextContainer.Visibility,
						c => c != null ? Visibility.Visible : Visibility.Collapsed)
					.DisposeWith(d);

				this.OneWayBind(ViewModel, vm => vm.Context.Name, v => v.ContextName.Text)
					.DisposeWith(d);

				this.OneWayBind(ViewModel, vm => vm.Context.Icon, v => v.ContextIcon.Source)
					.DisposeWith(d);

				this.BindCommand(ViewModel, vm => vm.RemoveContext, v => v.RemoveContext)
					.DisposeWith(d);

				this.Bind(ViewModel, vm => vm.TextQuery, v => v.TextQuery.Text)
					.DisposeWith(d);

				this.OneWayBind(ViewModel, vm => vm.IsTextQuery, v => v.TextQuery.Visibility)
					.DisposeWith(d);

				this.WhenAnyValue(v => v.KeysQuery.Keys)
					.Select(keys => keys.ToSequenceDto())
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

				// hotkey for switching query mode
				Observable.FromEventPattern<HotkeyEventArgs>(
						add =>
							HotkeyManager.Current.AddOrReplace("Switch Query Mode", Key.Back, ModifierKeys.Shift, add),
						remove => HotkeyManager.Current.Remove("Switch Query Mode"))
					.Select(_ => ViewModel.QueryMode switch
					{
						QueryModes.Text => QueryModes.Keys,
						QueryModes.Keys => QueryModes.Text,
						_ => throw new ArgumentOutOfRangeException()
					})
					.Subscribe(mode => { this.ViewModel.QueryMode = mode; });


				// auto focus on the search box when switching query mode
				this.WhenAnyValue(v => v.ViewModel.QueryMode)
					.Select(m => m switch
					{
						QueryModes.Text => TextQuery,
						QueryModes.Keys => KeysQuery,
						_ => throw new ArgumentOutOfRangeException()
					})
					.Subscribe(textbox => { textbox.Focus(); });

				this.Exit
					.Events()
					.Click
					.Subscribe(_ => Application.Current.Shutdown())
					.DisposeWith(d);

				this.BindCommand(ViewModel, vm => vm.LoadShortcuts, v => v.Reload)
					.DisposeWith(d);


				// load all shortcuts
				this.ViewModel.LoadShortcuts.Execute().Subscribe();

				this.TrayIcon
					.DisposeWith(d);

				Hook.GlobalEvents()
					.DisposeWith(d)
					.OnCombination(new Dictionary<Combination, Action>
					{
						{Combination.TriggeredBy(Keys.F2).With(Keys.LWin), ActivateWindow}
					});
			});
		}

		private void OnDrag(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				DragMove();
			}
		}

		private void OnTrayIconDoubleClick(object sender, RoutedEventArgs e)
		{
			ActivateWindow();
		}

		private async void ActivateWindow()
		{
			await ViewModel.RefreshContext.Execute();
			Show();
			Activate();
		}

		private void OnDeactivated(object? sender, EventArgs e)
		{
			// todo: config to show on launch
			Hide();
		}
	}
}