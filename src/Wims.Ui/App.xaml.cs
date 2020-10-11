using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Splat.Microsoft.Extensions.DependencyInjection;
using Wims.Core.Models;

namespace Wims.Ui
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private IHost _host;
		private Bootstrapper _bootstrapper;

		public App()
		{
			_bootstrapper = new Bootstrapper();
			_host = Host.CreateDefaultBuilder()
				.ConfigureAppConfiguration((context, config) =>
				{
					var homeDir = Environment.OSVersion.Platform == PlatformID.Win32NT
						? Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%")
						: Environment.GetEnvironmentVariable("HOME");

					config.AddYamlFile(Path.Join(homeDir, "wims.yml"), true, false);
					config.AddYamlFile("wims.yml", true, false);
				})
				.ConfigureServices((context, services) =>
				{
					var config = context.Configuration.Get<WimsConfig>();
					PreprocessConfig(config);

					services.AddSingleton(config);

					_bootstrapper.ConfigureServices(context.HostingEnvironment, context.Configuration, services);
				})
				.Build();

			// re-register MS DI with Splat
			_host.Services.UseMicrosoftDependencyResolver();
		}

		private void PreprocessConfig(WimsConfig config)
		{
			config.Directory = string.IsNullOrEmpty(config.Directory)
				? Directory.GetCurrentDirectory()
				: Path.GetFullPath(config.Directory);
		}

		protected override async void OnStartup(StartupEventArgs e)
		{
			await _host.StartAsync();
			await _bootstrapper.Startup(_host.Services);

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