using System;
using System.Windows;

namespace Wims.Ui
{
	public interface IErrorHandler
	{
		void OnError(Exception e);
	}

	public class ErrorHandler : IErrorHandler
	{
		public void OnError(Exception e)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				var window = new ErrorWindow(e);
				window.ShowDialog();
			});

		}
	}
}