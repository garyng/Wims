using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

		public LoadRawShortcutsFromFilesRequestHandler(IFileSystem fs)
		{
			_fs = fs;
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
				.SelectAwait(async path =>
				{
					var content = await _fs.File.ReadAllTextAsync(path);
					var obj = d.Deserialize<ShortcutsRo>(content);
					obj.Path = path;
					return obj;
				})
				.ToListAsync();
		}
	}

}