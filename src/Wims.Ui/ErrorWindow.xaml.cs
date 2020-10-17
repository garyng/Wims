using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Bogus;

namespace Wims.Ui
{
	/// <summary>
	/// Interaction logic for ErrorWindow.xaml
	/// </summary>
	public partial class ErrorWindow : Window
	{
		public static readonly DependencyProperty ExceptionProperty = DependencyProperty.Register("Exception",
			typeof(Exception), typeof(ErrorWindow), new PropertyMetadata(default(Exception)));

		public Exception Exception
		{
			get { return (Exception) GetValue(ExceptionProperty); }
			set { SetValue(ExceptionProperty, value); }
		}

		public ErrorWindow(Exception e)
		{
			InitializeComponent();
			Exception = e;
		}


		private void OnClosing(object sender, CancelEventArgs e)
		{
			Application.Current.Shutdown();
		}
	}
}