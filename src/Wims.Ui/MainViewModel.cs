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

	public class BindingConverter : IYamlTypeConverter
	{
		public bool Accepts(Type type)
		{
			return type == typeof(Binding);
		}

		public object ReadYaml(IParser parser, Type type)
		{
			var value = parser.Consume<Scalar>().Value;
			var keys = value.Split('+', StringSplitOptions.RemoveEmptyEntries)
				.Select(key => key.Trim());
			return new Binding
			{
				Keys = keys.ToArray()
			};
		}

		public void WriteYaml(IEmitter emitter, object value, Type type)
		{
			var binding = (Binding)value;
			var keys = string.Join(" + ", binding.Keys);
			emitter.Emit(new Scalar(null, null, keys, ScalarStyle.Any, true, false));
		}
	}

	/// <summary>
	/// Represent a single binding
	/// <example>
	/// Ctrl + Shift + C
	/// </example>
	/// </summary>
	[Equals]
	public class Binding
	{
		// todo: custom converter
		public string[] Keys { get; set; }

		public static bool operator ==(Binding left, Binding right) => Operator.Weave(left, right);
		public static bool operator !=(Binding left, Binding right) => Operator.Weave(left, right);
	}

	/// <summary>
	/// Represent the matcher for a <see cref="Context"/>
	/// </summary>
	[Equals]
	public class Match
	{
		public string Class { get; set; }
		public string Exe { get; set; }

		public static bool operator ==(Match left, Match right) => Operator.Weave(left, right);
		public static bool operator !=(Match left, Match right) => Operator.Weave(left, right);
	}

	/// <summary>
	/// Represent when the <see cref="Shortcut"/> is active
	/// </summary>
	[Equals]
	public class Context
	{
		public string Name { get; set; }
		public string Icon { get; set; }
		public Match Match { get; set; }

		public static bool operator ==(Context left, Context right) => Operator.Weave(left, right);
		public static bool operator !=(Context left, Context right) => Operator.Weave(left, right);
	}

	[Equals]
	public class Shortcut
	{
		public string Context { get; set; }
		public string Description { get; set; }

		/// <summary>
		/// Can have more one or more bindings.
		/// If there are more than one, they are treated as binding sequence
		/// (eg: Ctrl + Shift + C, K)
		/// </summary>
		public List<Binding> Bindings { get; set; }

		public static bool operator ==(Shortcut left, Shortcut right) => Operator.Weave(left, right);
		public static bool operator !=(Shortcut left, Shortcut right) => Operator.Weave(left, right);
	}

	/// <summary>
	/// Collection of all shortcuts and contexts
	/// </summary>
	[Equals]
	public class ShortcutsRaw
	{
		public Dictionary<string, Context> Contexts { get; set; }
		public Dictionary<string, Shortcut> Shortcuts { get; set; }

		public static bool operator ==(ShortcutsRaw left, ShortcutsRaw right) => Operator.Weave(left, right);
		public static bool operator !=(ShortcutsRaw left, ShortcutsRaw right) => Operator.Weave(left, right);
	}

	[Equals]
	public class ShortcutsDto1
	{

		public List<Context> Contexts { get; set; }
		public List<Shortcut> Shortcuts { get; set; }

		public ShortcutsDto1()
		{
			Contexts = new List<Context>();
			Shortcuts = new List<Shortcut>();
		}
		
		public ShortcutsDto1(ShortcutsRaw raw)
		{
			Contexts = raw.Contexts.Select(kvp =>
			{
				kvp.Value.Name = kvp.Key;
				return kvp.Value;
			}).ToList();
			Shortcuts = raw.Shortcuts.Select(kvp =>
			{
				kvp.Value.Description = kvp.Key;
				return kvp.Value;
			}).ToList();

		}

		public static bool operator ==(ShortcutsDto1 left, ShortcutsDto1 right) => Operator.Weave(left, right);
		public static bool operator !=(ShortcutsDto1 left, ShortcutsDto1 right) => Operator.Weave(left, right);
	}

	public class WimsConfig
	{
		/// <summary>
		/// The home directory where all the <see cref="ShortcutsRaw"/> files are saved.
		/// </summary>
		public string Directory { get; set; }
	}

	public class LoadShortcuts : IRequest<ShortcutsDto1>
	{
		public string Directory { get; set; }
	}

	public class LoadShortcutsRequestHandler : IRequestHandler<LoadShortcuts, ShortcutsDto1>
	{
		private readonly IFileSystem _fs;

		public LoadShortcutsRequestHandler(IFileSystem fs)
		{
			_fs = fs;
		}

		public async Task<ShortcutsDto1> Handle(LoadShortcuts request, CancellationToken cancellationToken)
		{
			var rootDir = request.Directory;
			var d = new DeserializerBuilder()
				.WithNamingConvention(CamelCaseNamingConvention.Instance)
				.WithTypeConverter(new BindingConverter())
				.Build();

			var files = _fs.Directory.EnumerateFiles(rootDir, "*.yml", SearchOption.AllDirectories);
			var contents = await Task.WhenAll(files.Select(f => _fs.File.ReadAllTextAsync(f)));

			var shortcuts = contents
				.Select(c => d.Deserialize<ShortcutsRaw>(c))
				.Select(r => new ShortcutsDto1(r))
				.ToList();

			var dupContexts = shortcuts
				.SelectMany(s => s.Contexts)
				.GroupBy(c => c.Name)
				.Where(g => g.Count() > 1)
				.ToList();

			
			return new ShortcutsDto1();
		}
	}

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