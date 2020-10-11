using System;
using System.IO.Abstractions;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using Wims.Ui.Profiles;
using Wims.Ui.Requests;
using Wims.Ui.Validators;

namespace Wims.Ui
{
	public class Bootstrapper
	{
		public void ConfigureServices(IHostEnvironment environment, IConfiguration configuration,
			IServiceCollection services)
		{
			var splat = ConfigureSplat(services);

			services.AddMediatR(typeof(IMediatorMarker));
			services.AddAutoMapper(typeof(IProfileMarker));
			services.AddValidatorsFromAssembly(typeof(IValidatorsMarker).Assembly);

			services.AddSingleton<ISchedulers, Schedulers>();
			services.AddSingleton<MainViewModel>();
			services.AddSingleton<IFileSystem>(new FileSystem());

			services.AddSingleton<ContextService>();


			//services.Scan(scan => scan
			//	.FromAssemblyOf<ViewModelBase>()
			//	.AddClasses(classes => classes.AssignableTo<ViewModelBase>())
			//	.AsSelf()
			//	.WithSingletonLifetime()
			//);
		}

		private IMutableDependencyResolver ConfigureSplat(IServiceCollection services)
		{
			services.UseMicrosoftDependencyResolver();

			var splat = Locator.CurrentMutable;
			splat.InitializeSplat();
			splat.InitializeReactiveUI();
			splat.RegisterViewsForViewModels(typeof(App).Assembly);
			splat.RegisterConstant(new RxStringToImageSourceConverter(), typeof(IBindingTypeConverter));

			return splat;
		}

		public async Task Startup(IServiceProvider provider)
		{
			provider.GetService<ContextService>().Refresh();
		}
	}
}