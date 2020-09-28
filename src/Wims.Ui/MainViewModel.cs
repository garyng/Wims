using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using MediatR;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Wims.Core.Dto;
using Wims.Core.Models;
using Wims.Ui.Requests;
using Void = GaryNg.Utils.Void.Void;

namespace Wims.Ui
{
	public class SearchByText : IRequest<IList<ShortcutDto>>
	{
		public IList<ShortcutDto> Shortcuts { get; set; }
		public string Query { get; set; }
	}

	public class SearchByTextRequestHandler : IRequestHandler<SearchByText, IList<ShortcutDto>>
	{
		public async Task<IList<ShortcutDto>> Handle(SearchByText request, CancellationToken cancellationToken)
		{
			return FuzzySharp.Process.ExtractTop(new ShortcutDto
				{
					Description = request.Query
				}, request.Shortcuts, s => s.Description, limit: 10, cutoff: 60)
				.Select(r => r.Value)
				.ToList();
		}
	}

	public enum QueryModes
	{
		Text, 
		Keys
	}

	public class MainViewModel : ViewModelBase
	{
		public ReactiveCommand<Void, ShortcutsDto> LoadShortcuts { get; set; }
		
		[Reactive] public QueryModes QueryMode { get; set; }

		[ObservableAsProperty] public bool IsTextQuery { get; set; }
		[ObservableAsProperty] public bool IsKeysQuery { get; set; }

		[Reactive] public string TextQuery { get; set; }
		[Reactive] public string KeysQuery { get; set; }


		private ReadOnlyObservableCollection<string> _results;

		public ReadOnlyObservableCollection<string> Results
		{
			get => _results;
			set => _results = value;
		}


		public MainViewModel(ISchedulers schedulers, IMediator mediator, WimsConfig config) : base(schedulers, mediator)
		{
			LoadShortcuts = ReactiveCommand.CreateFromObservable<Void, ShortcutsDto>(_ => _mediator
				.Send(new LoadRawShortcutsFromFiles
				{
					SourceDirectory = config.Directory
				})
				.ToObservable()
				.SelectMany(shortcuts => _mediator.Send(new TransformRawShortcutsToDto
				{
					Shortcuts = shortcuts
				})));

			this.WhenAnyValue(vm => vm.QueryMode)
				.Select(q => q == QueryModes.Text)
				.ToPropertyEx(this, vm => vm.IsTextQuery);

			this.WhenAnyValue(vm => vm.QueryMode)
				.Select(q => q == QueryModes.Keys)
				.ToPropertyEx(this, vm => vm.IsKeysQuery);


			// todo: map to IsTextQuery/IsKeysQuery
			// todo: show/hide KeysRecorder


			var results = new SourceList<string>();

			results.Connect()
				.ObserveOn(_schedulers.MainThread)
				.DisposeMany()
				.Bind(out _results)
				.Subscribe();

			// query			+xxxxx---xxxx-------xxxx---
			// throttle			+------x------x----------x-
			// config			+c1----------c2------------
			// combine latest	+---(x,c1)--(x,c2)---(x,c2)


			this.WhenAnyValue(vm => vm.TextQuery)
				.Throttle(TimeSpan.FromMilliseconds(300))
				.CombineLatest(LoadShortcuts, (query, all) => new {text = query, shortcuts = all.Shortcuts})
				.Publish(query =>
					Observable.Merge(
						// show everything if query text is empty
						query.Where(q => string.IsNullOrEmpty(q.text))
							.Select(q => q.shortcuts),
						// else perform the search
						query.Where(q => !string.IsNullOrEmpty(q.text))
							.SelectMany(q => _mediator.Send(new SearchByText
							{
								Query = q.text, Shortcuts = q.shortcuts
							}).ToObservable())
					))
				.Select(ss => ss.Select(s => s.Description))
				.Do(r => results.Edit(update =>
				{
					update.Clear();
					update.AddRange(r);
				}))
				.Subscribe();
		}
	}
}