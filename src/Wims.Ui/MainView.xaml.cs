﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReactiveUI;

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