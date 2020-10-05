using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FuzzySharp;
using MediatR;
using Wims.Core.Dto;
using Wims.Ui.Controls;

namespace Wims.Ui.Requests
{
	public class SearchResultDto
	{
		public ShortcutDto Shortcut { get; set; }
		public List<OrderedRange> Matches { get; set; }

		public SearchResultDto(ShortcutDto shortcut, List<OrderedRange> matches)
		{
			Shortcut = shortcut;
			Matches = matches;
		}
	}

	// todo: make abstractions

	public class SearchByText : IRequest<IList<SearchResultDto>>
	{
		public IList<ShortcutDto> Shortcuts { get; set; }
		public string Query { get; set; }
	}

	public class SearchByTextRequestHandler : IRequestHandler<SearchByText, IList<SearchResultDto>>
	{
		public async Task<IList<SearchResultDto>> Handle(SearchByText request, CancellationToken cancellationToken)
		{
			if (string.IsNullOrEmpty(request.Query))
				return request.Shortcuts
					.Select(s => new SearchResultDto(s, new List<OrderedRange>()))
					.ToList();

			return Process.ExtractTop(new ShortcutDto
				{
					Description = request.Query
				}, request.Shortcuts, s => s.Description, limit: 10, cutoff: 60)
				.Select(r => r.Value)
				.Select(r => new SearchResultDto(r,
					Levenshtein.GetMatchingBlocks(r.Description, request.Query)
						.Where(b => b.Length > 0)
						.Select(b => new OrderedRange(b.SourcePos, b.SourcePos + b.Length))
						.ToList()))
				.ToList();
		}
	}
}