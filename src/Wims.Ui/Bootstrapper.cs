using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;

namespace Wims.Ui
{
	public class Bootstrapper
	{
		public void ConfigureServices(IHostEnvironment environment, IConfiguration configuration,
			IServiceCollection services)
		{
			var splat = ConfigureSplat(services);


			services.AddSingleton<MainViewModel>();

		}

		private IMutableDependencyResolver ConfigureSplat(IServiceCollection services)
		{
			services.UseMicrosoftDependencyResolver();

			var splat = Locator.CurrentMutable;
			splat.InitializeSplat();
			splat.InitializeReactiveUI();
			splat.RegisterViewsForViewModels(typeof(App).Assembly);

			return splat;
		}
	}
}