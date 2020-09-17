﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
				this.OneWayBind(ViewModel, vm => vm.Context, v => v.Context.Text)
					.DisposeWith(d);

				this.OneWayBind(ViewModel, vm => vm.Results, v => v.Results.ItemsSource)
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