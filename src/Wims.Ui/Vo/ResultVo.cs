using System;
using System.Collections.Generic;
using System.Linq;
using FuzzySharp;
using Wims.Core.Dto;
using Wims.Ui.Controls.Highlighter;

namespace Wims.Ui.Vo
{
	public class ResultVo
	{
		public ShortcutDto Shortcut { get; set; }
		public List<OrderedRange> Matches { get; set; }

		public ResultVo(ShortcutDto shortcut, Func<ShortcutDto, string> selector = null, string query = null)
		{
			Shortcut = shortcut;
			Matches = query == null
				? new List<OrderedRange>()
				: Levenshtein.GetMatchingBlocks(selector?.Invoke(shortcut).ToLowerInvariant(), query.ToLowerInvariant())
					.Where(b => b.Length > 0)
					.Select(b => new OrderedRange(b.SourcePos, b.SourcePos + b.Length))
					.ToList();
		}
	}
}