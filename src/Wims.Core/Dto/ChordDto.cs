using System;

namespace Wims.Core.Dto
{
	public class ChordDto
	{
		public string[] Keys { get; set; }

		public override string ToString()
		{
			return string.Join(" + ", Keys);
		}
		
		// todo: make this implicit conversion
		public static ChordDto FromString(string keys)
		{
			return new ChordDto
			{
				Keys = keys.Split(" + ", StringSplitOptions.RemoveEmptyEntries)
			};
		}
	}
}