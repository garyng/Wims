using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Windows.Threading;
using DynamicData;
using MediatR;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Wims.Core.Dto;
using Wims.Core.Models;
using Wims.Ui.Requests;
using Wims.Ui.Vo;
using Void = GaryNg.Utils.Void.Void;

namespace Wims.Ui
{
	public class MainViewModel : ViewModelBase
	{
		public WimsConfig Config { get; }
		public IErrorHandler Error { get; }
		private readonly ContextService _context;
		public ReactiveCommand<Void, ShortcutsDto> LoadShortcuts { get; set; }
		public ReactiveCommand<Void, Void> RefreshContext { get; set; }
		public ReactiveCommand<Void, Void> RemoveContext { get; set; }

		[Reactive]
		public QueryModes QueryMode { get; set; }

		[ObservableAsProperty]
		public bool IsTextQuery { get; set; }

		[ObservableAsProperty]
		public bool IsKeysQuery { get; set; }

		[Reactive]
		public string TextQuery { get; set; }

		[Reactive]
		public SequenceDto KeysQuery { get; set; }

		private ReadOnlyObservableCollection<ResultVo> _results;

		public ReadOnlyObservableCollection<ResultVo> Results
		{
			get => _results;
			set => _results = value;
		}

		[ObservableAsProperty]
		public ContextDto Context { get; set; }

		public MainViewModel(ISchedulers schedulers, IMediator mediator, WimsConfig config, ContextService context,
			IErrorHandler error) :
			base(schedulers, mediator)
		{
			Config = config;
			Error = error;
			_context = context;

			LoadShortcuts = ReactiveCommand.CreateFromObservable<Void, ShortcutsDto>(_ => _mediator
					.Send(new LoadRawShortcutsFromFiles
					{
						SourceDirectory = config.Directory
					})
					.ToObservable()
					.Do(_ => { }, error.OnError)
					.Catch<IList<ShortcutsRo>, Exception>(e => Observable.Empty<IList<ShortcutsRo>>())
					.SelectMany(shortcuts => _mediator.Send(new TransformRawShortcutsToDto
					{
						Shortcuts = shortcuts
					}))
					.Do(_ => { }, error.OnError)
					.Catch<ShortcutsDto, Exception>(e => Observable.Return(new ShortcutsDto()))
				);

			RefreshContext = ReactiveCommand.Create<Void, Void>(_ =>
			{
				_context.Refresh();
				return Void.Default;
			});

			RemoveContext = ReactiveCommand.Create<Void, Void>(_ =>
			{
				_context.Clear();
				return Void.Default;
			});


			// find the first context that matches the active context
			_context.ActiveContext
				.Log(this)
				.CombineLatest(LoadShortcuts, (_, s) => s)
				// todo: check for null
				.Select(shortcuts => shortcuts.Contexts.Values.Where(c => _context.Match(c.Match)))
				.Select(contexts => contexts.FirstOrDefault())
				.ToPropertyEx(this, vm => vm.Context);


			this.WhenAnyValue(vm => vm.QueryMode)
				.Select(q => q == QueryModes.Text)
				.ToPropertyEx(this, vm => vm.IsTextQuery);

			this.WhenAnyValue(vm => vm.QueryMode)
				.Select(q => q == QueryModes.Keys)
				.ToPropertyEx(this, vm => vm.IsKeysQuery);


			var results = new SourceList<ResultVo>();

			results.Connect()
				.ObserveOn(_schedulers.MainThread)
				.DisposeMany()
				.Bind(out _results)
				.Subscribe();

			Observable.Merge(
					Search(vm => vm.TextQuery,
						(query, shortcuts) => new SearchByText
						{
							Query = query,
							Shortcuts = shortcuts
						}, QueryModes.Text),
					Search(vm => vm.KeysQuery,
						((query, shortcuts) => new SearchByKeys
						{
							Query = query,
							Shortcuts = shortcuts
						}), QueryModes.Keys))
				.SelectMany(r => _mediator.Send(r).ToObservable())
				.Do(r => results.Edit(update =>
				{
					update.Clear();
					update.AddRange(r);
				}))
				.Subscribe();
		}

		private IObservable<IRequest<IList<ResultVo>>> Search<TQuery>(
			Expression<Func<MainViewModel, TQuery>> queryProperty,
			Func<TQuery, IList<ShortcutDto>, IRequest<IList<ResultVo>>> requestFunc, QueryModes mode)
		{
			var modes = this.WhenAnyValue(vm => vm.QueryMode);

			// filter input by query
			var queries = this.WhenAnyValue(queryProperty)
				.Throttle(TimeSpan.FromMilliseconds(300))
				.CombineLatest(modes, (query, m) => new {query, mode = m})
				.Where(q => q.mode == mode)
				.Select(q => q.query);

			// filter shortcuts by context
			var shortcuts = this.WhenAnyValue(vm => vm.Context)
				.CombineLatest(LoadShortcuts, (c, ss) =>
					c == null
						? ss.Shortcuts
						: ss.Shortcuts
							.Where(s => c.Equals(s.Context)).ToList());

			return queries
				.CombineLatest(shortcuts, (query, all) =>
					new {query, shortcuts = all})
				.Select(q => requestFunc(q.query, q.shortcuts));
		}
	}
}