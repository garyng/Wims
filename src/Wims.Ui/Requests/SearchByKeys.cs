﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GaryNg.Utils.Enumerable;
using MediatR;
using Wims.Core.Dto;

namespace Wims.Ui.Requests
{
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

}