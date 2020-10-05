using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FuzzySharp;
using GaryNg.Utils.Enumerable;
using MediatR;
using Wims.Core.Dto;
using Wims.Ui.Controls;

namespace Wims.Ui.Requests
{
	public class SearchByKeys : IRequest<IList<SearchResultDto>>
	{
		public IList<ShortcutDto> Shortcuts { get; set; }
		public SequenceDto Query { get; set; }
	}

	public class SearchByKeysRequestHandler : IRequestHandler<SearchByKeys, IList<SearchResultDto>>
	{
		public async Task<IList<SearchResultDto>> Handle(SearchByKeys request, CancellationToken cancellationToken)
		{
			if (request.Query.Empty())
				return request.Shortcuts
					.Select(s => new SearchResultDto(s, new List<OrderedRange>()))
					.ToList();

			return FuzzySharp.Process.ExtractTop(new ShortcutDto
				{
					Sequence = request.Query
				}, request.Shortcuts, s => s.Sequence.ToString(), limit: 10, cutoff: 60)
				.Select(r => r.Value)
				.Select(r => new SearchResultDto(r,
					Levenshtein.GetMatchingBlocks(r.Sequence.ToString(), request.Query.ToString())
						.Select(b => new OrderedRange(b.DestPos, b.DestPos + b.Length))
						.ToList()))
				.ToList();
		}
	}
}