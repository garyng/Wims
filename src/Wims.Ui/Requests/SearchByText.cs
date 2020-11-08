using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Wims.Core.Dto;
using Wims.Ui.Vo;

namespace Wims.Ui.Requests
{
	public class SearchByText : IRequest<IList<ResultVo>>
	{
		public IList<ShortcutDto> Shortcuts { get; set; }
		public string Query { get; set; }
	}

	public class SearchByTextRequestHandler : IRequestHandler<SearchByText, IList<ResultVo>>
	{
		public async Task<IList<ResultVo>> Handle(SearchByText request, CancellationToken cancellationToken)
		{
			if (string.IsNullOrEmpty(request.Query)) 
				return request.Shortcuts
					.Select(s => new ResultVo(s))
					.ToList();

			return FuzzySharp.Process.ExtractSorted(new ShortcutDto
				{
					Description = request.Query
				}, request.Shortcuts, s => s.Description.ToLowerInvariant(), cutoff: 60)
				.Select(r => new ResultVo(r.Value, s => s.Description, request.Query))
				.ToList();
		}
	}

}