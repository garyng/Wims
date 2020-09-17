using MediatR;
using ReactiveUI;

namespace Wims.Ui
{
	public class ViewModelBase : ReactiveObject, IActivatableViewModel
	{
		protected readonly ISchedulers _schedulers;
		protected readonly IMediator _mediator;
		public ViewModelActivator Activator { get; }

		public ViewModelBase(ISchedulers schedulers, IMediator mediator)
		{
			_schedulers = schedulers;
			_mediator = mediator;
			Activator = new ViewModelActivator();
		}
	}
}