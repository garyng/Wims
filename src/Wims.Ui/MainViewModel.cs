﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using DynamicData;
using GaryNg.Utils.Enumerable;
using MediatR;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Wims.Core.Dto;
using Wims.Core.Models;
using Wims.Ui.Requests;
using Void = GaryNg.Utils.Void.Void;

namespace Wims.Ui
{
	public class BindingDtoToStringConverter : MarkupExtension, IValueConverter
	{
		private static Lazy<BindingDtoToStringConverter> _instance =
			new Lazy<BindingDtoToStringConverter>(() => new BindingDtoToStringConverter());

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return _instance.Value;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is IEnumerable<BindingDto> binding)
			{
				return binding.AsString();
			}

			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is string binding)
			{
				return binding.ToBindingDto();
			}

			return value;
		}
	}

	public class SearchByText : IRequest<IList<ShortcutDto>>
	{
		public IList<ShortcutDto> Shortcuts { get; set; }
		public string Query { get; set; }
	}

	public class SearchByTextRequestHandler : IRequestHandler<SearchByText, IList<ShortcutDto>>
	{
		public async Task<IList<ShortcutDto>> Handle(SearchByText request, CancellationToken cancellationToken)
		{
			if (string.IsNullOrEmpty(request.Query)) return request.Shortcuts;

			return FuzzySharp.Process.ExtractTop(new ShortcutDto
				{
					Description = request.Query
				}, request.Shortcuts, s => s.Description, limit: 10, cutoff: 60)
				.Select(r => r.Value)
				.ToList();
		}
	}

	public class SearchByKeys : IRequest<IList<ShortcutDto>>
	{
		public IList<ShortcutDto> Shortcuts { get; set; }
		public List<BindingDto> Query { get; set; }
	}

	public class SearchByKeysRequestHandler : IRequestHandler<SearchByKeys, IList<ShortcutDto>>
	{
		public async Task<IList<ShortcutDto>> Handle(SearchByKeys request, CancellationToken cancellationToken)
		{
			if (request.Query.Empty()) return request.Shortcuts;

			return FuzzySharp.Process.ExtractTop(new ShortcutDto
				{
					Bindings = request.Query
				}, request.Shortcuts, s => s.Bindings.AsString(), limit: 10, cutoff: 60)
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
		[Reactive] public List<BindingDto> KeysQuery { get; set; }


		private ReadOnlyObservableCollection<ShortcutDto> _results;

		public ReadOnlyObservableCollection<ShortcutDto> Results
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


			var results = new SourceList<ShortcutDto>();

			results.Connect()
				.ObserveOn(_schedulers.MainThread)
				.DisposeMany()
				.Bind(out _results)
				.Subscribe();


			Observable.Merge(
					Search(this,
						vm => vm.TextQuery,
						(query, shortcuts) => new SearchByText
						{
							Query = query,
							Shortcuts = shortcuts
						}, this.WhenAnyValue(vm => vm.QueryMode), QueryModes.Text),
					Search(this,
						vm => vm.KeysQuery,
						((query, shortcuts) => new SearchByKeys
						{
							Query = query,
							Shortcuts = shortcuts
						}), this.WhenAnyValue(vm => vm.QueryMode), QueryModes.Keys))
				.SelectMany(r => _mediator.Send(r).ToObservable())
				.Do(r => results.Edit(update =>
				{
					update.Clear();
					update.AddRange(r);
				}))
				.Subscribe();
		}

		private IObservable<IRequest<IList<ShortcutDto>>> Search<TSender, TRet>(TSender sender,
			Expression<Func<TSender, TRet>> queryProperty,
			Func<TRet, IList<ShortcutDto>, IRequest<IList<ShortcutDto>>> requestFunc,
			IObservable<QueryModes> modes,
			QueryModes mode)
		{
			return sender.WhenAnyValue(queryProperty)
				.Throttle(TimeSpan.FromMilliseconds(300))
				.CombineLatest(modes, (query, mode) => new {query, mode})
				.Where(q => q.mode == mode)
				.Select(q => q.query)
				.CombineLatest(LoadShortcuts, (query, all) => new {query, shortcuts = all.Shortcuts})
				.Select(q => requestFunc(q.query, q.shortcuts));
		}
	}
}