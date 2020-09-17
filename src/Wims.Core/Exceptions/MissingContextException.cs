using System;
using System.Collections.Generic;
using System.Linq;

namespace Wims.Core.Exceptions
{
	public class MissingContextException : Exception
	{
		public MissingContextException(IEnumerable<string> contexts)
			: base($"Missing context(s): {string.Join(", ", contexts.Select(key => $"'{key}'"))}")

		{
		}
	}
}