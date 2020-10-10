using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Wims.Core.Models;
using Wims.Ui.Utils;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Wims.Ui.Requests
{
	public class LoadRawShortcutsFromFiles : IRequest<IList<ShortcutsRo>>
	{
		public string SourceDirectory { get; set; }
	}

	public class LoadRawShortcutsFromFilesRequestHandler
		: IRequestHandler<LoadRawShortcutsFromFiles, IList<ShortcutsRo>>
	{
		private readonly IFileSystem _fs;
		private readonly IValidator<ShortcutsRo> _validator;

		public LoadRawShortcutsFromFilesRequestHandler(IFileSystem fs, IValidator<ShortcutsRo> validator)
		{
			_fs = fs;
			_validator = validator;
		}

		public async Task<IList<ShortcutsRo>> Handle(LoadRawShortcutsFromFiles request,
			CancellationToken cancellationToken)
		{
			var sourceDir = request.SourceDirectory;
			var d = new DeserializerBuilder()
				.WithNamingConvention(CamelCaseNamingConvention.Instance)
				.WithTypeConverter(new ChordRoConverter())
				.Build();

			return await _fs.Directory.EnumerateFiles(sourceDir, "*.yml", SearchOption.AllDirectories)
				.ToAsyncEnumerable()
				.SelectAwait(async path => new {path, content = await _fs.File.ReadAllTextAsync(path)})
				.Where(item => !string.IsNullOrWhiteSpace(item.content))
				.Select(item =>
				{
					var obj = d.Deserialize<ShortcutsRo>(item.content);
					obj.Path = item.path;
					_validator.ValidateAndThrow(obj);
					return obj;
				})
				.Where(s => s.Contexts != null || s.Shortcuts != null)
				.ToListAsync();
		}
	}
}