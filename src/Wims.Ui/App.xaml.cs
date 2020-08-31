using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using ReactiveUI;
using Splat;

namespace Wims.Ui
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			Locator.CurrentMutable.Register(() => new MainViewModel());
			Locator.CurrentMutable.RegisterViewsForViewModels(typeof(App).Assembly);


			var mainView = new MainView
			{
				ViewModel = Locator.Current.GetService<MainViewModel>()
			};
			mainView.Closed += (o, args) => Shutdown();
			mainView.Show();
		}
	}
}