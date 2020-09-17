using System;

namespace Wims.Core.Exceptions
{
	public class NullOrEmptyException : Exception
	{
		public NullOrEmptyException(string propertyName)
			: base($"`{propertyName}` is null or empty")
		{
		}
	}
}