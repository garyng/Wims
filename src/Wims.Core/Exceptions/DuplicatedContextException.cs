using System;
using System.Collections.Generic;
using System.Linq;

namespace Wims.Core.Exceptions
{
	public class DuplicatedContextException : Exception
	{
		public DuplicatedContextException(IEnumerable<string> contexts)
			: base($"Duplicated context(s): {string.Join(", ", contexts.Select(key => $"'{key}'"))}")
		{
		}
	}
}