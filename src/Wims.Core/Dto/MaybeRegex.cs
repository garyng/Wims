using System;
using System.Text.RegularExpressions;

namespace Wims.Core.Dto
{
	/// <summary>
	/// If string starts and ends with /, then use Regex,
	/// else plain string comparison is used (case insensitive).
	/// </summary>
	public class MaybeRegex
	{
		private Regex _regex;
		private string _pattern;

		public bool IsRegex => _regex != null;

		public MaybeRegex(string pattern)
		{
			var match = Regex.Match(pattern, @"^/(.*)/$");
			if (match.Success)
			{
				_regex = new Regex(match.Groups[1].Value, RegexOptions.Compiled);
			}
			else
			{
				_pattern = pattern;
			}
		}

		public bool IsMatch(string input)
		{
			return _regex?.IsMatch(input)
			       ?? _pattern.Equals(input, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}