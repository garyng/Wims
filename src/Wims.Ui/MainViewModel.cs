using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Wims.Ui
{
	public interface ISchedulers
	{
		IScheduler MainThread { get; }
		IScheduler TaskPool { get; }
	}

	public class Schedulers : ISchedulers
	{
		public IScheduler MainThread { get; } = RxApp.MainThreadScheduler;
		public IScheduler TaskPool { get; } = RxApp.TaskpoolScheduler;
	}

	public class ViewModelBase : ReactiveObject, IActivatableViewModel
	{
		public ViewModelActivator Activator { get; }
		public ISchedulers Schedulers { get; }

		public ViewModelBase()
		{
			Schedulers = new Schedulers();
			Activator = new ViewModelActivator();
		}
	}

	public class MainViewModel : ViewModelBase
	{
		[Reactive] public string SearchText { get; set; }
		[ObservableAsProperty] public string Context { get; set; }

		public MainViewModel()
		{
			this.WhenAnyValue(vm => vm.SearchText)
				.Throttle(TimeSpan.FromMilliseconds(300))
				.ObserveOn(RxApp.MainThreadScheduler)
				.ToPropertyEx(this, vm => vm.Context);
		}
	}
}