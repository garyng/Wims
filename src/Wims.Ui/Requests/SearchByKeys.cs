using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GaryNg.Utils.Enumerable;
using MediatR;
using Wims.Core.Dto;
using Wims.Ui.Vo;

namespace Wims.Ui.Requests
{
	public class SearchByKeys : IRequest<IList<ResultVo>>
	{
		public IList<ShortcutDto> Shortcuts { get; set; }
		public SequenceDto Query { get; set; }
	}

	public class SearchByKeysRequestHandler : IRequestHandler<SearchByKeys, IList<ResultVo>>
	{
		public async Task<IList<ResultVo>> Handle(SearchByKeys request, CancellationToken cancellationToken)
		{
			if (request.Query.Empty())
				return request.Shortcuts
					.Select(s => new ResultVo(s))
					.ToList();

			return FuzzySharp.Process.ExtractTop(new ShortcutDto
				{
					Sequence = request.Query
				}, request.Shortcuts, s => s.Sequence.ToString().ToLowerInvariant(), limit: 10, cutoff: 60)
				.Select(r => new ResultVo(r.Value, s => s.Sequence.ToString(), request.Query.ToString()))
				.ToList();
		}
	}
}