using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Splat.Microsoft.Extensions.DependencyInjection;

namespace Wims.Ui
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private IHost _host;

		public App()
		{
			var bootstrapper = new Bootstrapper();
			_host = Host.CreateDefaultBuilder()
				.ConfigureServices((context, services) =>
				{
					bootstrapper.ConfigureServices(context.HostingEnvironment, context.Configuration, services);
				})
				.Build();

			// re-register MS DI with Splat
			_host.Services.UseMicrosoftDependencyResolver();
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			_host.StartAsync();
			var mainView = new MainView
			{
				ViewModel = _host.Services.GetRequiredService<MainViewModel>()
			};
			mainView.Closed += (o, args) => Shutdown();
			mainView.Show();
		}

		protected override async void OnExit(ExitEventArgs e)
		{
			await _host.StopAsync();
			_host.Dispose();
		}
	}
}