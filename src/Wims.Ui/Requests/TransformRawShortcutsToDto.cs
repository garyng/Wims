using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GaryNg.Utils.Enumerable;
using MediatR;
using Wims.Core.Dto;
using Wims.Core.Exceptions;
using Wims.Core.Models;

namespace Wims.Ui.Requests
{
	public class TransformRawShortcutsToDto : IRequest<ShortcutsDto>
	{
		public IEnumerable<ShortcutsRo> Shortcuts { get; set; }
	}

	public class TransformRawShortcutsToDtoRequestHandler : IRequestHandler<TransformRawShortcutsToDto, ShortcutsDto>
	{
		private readonly IMapper _mapper;
		private readonly IFileSystem _fs;

		public TransformRawShortcutsToDtoRequestHandler(IMapper mapper, IFileSystem fs)
		{
			_mapper = mapper;
			_fs = fs;
		}

		public async Task<ShortcutsDto> Handle(TransformRawShortcutsToDto request, CancellationToken cancellationToken)
		{
			var shortcutsRo = request.Shortcuts.ToList();

			var dupContexts = shortcutsRo
				.SelectManyNotNull(s => s.Contexts)
				.GroupBy(c => c.Key)
				.Where(g => g.Count() > 1)
				.Select(g => g.Key)
				.ToList();

			if (dupContexts?.Any() == true) throw new DuplicatedContextException(dupContexts);

			var contexts = shortcutsRo
				.NotNullBy(s => s.Contexts)
				.SelectMany(shortcut => shortcut.Contexts
					.Select(c => (path: shortcut.Path, context: c)))
				.Select(item =>
				{
					var context = _mapper.Map<ContextDto>(item.context.Value);
					context.Name = item.context.Key;

					var icon = context.Icon;
					if (!_fs.Path.IsPathFullyQualified(icon))
					{
						var dir = _fs.Path.GetDirectoryName(item.path);
						var iconPath = Path.Join(dir, icon);
						context.Icon = _fs.Path.GetFullPath(iconPath);
					}

					return context;
				})
				.ToDictionary(c => c.Name, c => c);

			var missingContexts = shortcutsRo
				.SelectManyNotNull(s => s.Shortcuts, s => s.Values)
				.SelectNotNull(s => s.Context)
				.Where(c => !contexts.ContainsKey(c))
				.ToList();

			if (missingContexts.Any()) throw new MissingContextException(missingContexts);

			var shortcutsDto = shortcutsRo
				.SelectManyNotNull(shortcut => shortcut.Shortcuts)
				.Select(kv =>
				{
					var shortcut = kv.Value;
					var desc = kv.Key;

					return new ShortcutDto
					{
						Description = desc,
						Context = contexts[shortcut.Context],
						Sequence = _mapper.Map<SequenceDto>(shortcut.Sequence)
					};
				}).ToList();

			
			return new ShortcutsDto
			{
				Contexts = contexts,
				Shortcuts = shortcutsDto
			};
		}
	}

}