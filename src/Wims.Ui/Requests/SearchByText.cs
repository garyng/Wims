using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Wims.Core.Dto;

namespace Wims.Ui.Requests
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
			if (string.IsNullOrEmpty(request.Query)) return request.Shortcuts;

			return FuzzySharp.Process.ExtractTop(new ShortcutDto
				{
					Description = request.Query
				}, request.Shortcuts, s => s.Description, limit: 10, cutoff: 60)
				.Select(r => r.Value)
				.ToList();
		}
	}

}