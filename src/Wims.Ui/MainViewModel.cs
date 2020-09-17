using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using DynamicData;
using MediatR;
using MoreLinq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Wims.Ui
{
	public class SearchByText : IRequest<List<string>>
	{
		public string Query { get; set; }
	}

	public class SearchByTextRequestHandler : IRequestHandler<SearchByText, List<string>>
	{
		public async Task<List<string>> Handle(SearchByText request, CancellationToken cancellationToken)
		{
			var faker = new Faker();
			return Enumerable.Range(0, faker.Random.Int(0, 10))
				.Select(_ => faker.Commerce.ProductName())
				.ToList();
		}
	}

	public class MainViewModel : ViewModelBase
	{
		[Reactive] public string TextQuery { get; set; }
		[Reactive] public string KeysQuery { get; set; }

		[ObservableAsProperty] public string Context { get; set; }


		private ReadOnlyObservableCollection<string> _results;

		public ReadOnlyObservableCollection<string> Results
		{
			get => _results;
			set => _results = value;
		}

		public MainViewModel(ISchedulers schedulers, IMediator mediator) : base(schedulers, mediator)
		{
			var results = new SourceList<string>();


			this.WhenAnyValue(vm => vm.TextQuery)
				.Throttle(TimeSpan.FromMilliseconds(300))
				.SelectMany(query => _mediator.Send(new SearchByText {Query = query}).ToObservable())
				.Do(r => results.Edit(items =>
				{
					items.Clear();
					items.AddRange(r);
				}))
				.ObserveOn(RxApp.MainThreadScheduler)
				.Subscribe();

			results.Connect()
				.ObserveOn(_schedulers.MainThread)
				.DisposeMany()
				.Bind(out _results)
				.Subscribe();
		}
	}
}