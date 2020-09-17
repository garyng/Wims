using System.Reactive.Concurrency;
using ReactiveUI;

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

}