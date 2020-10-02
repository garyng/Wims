using System;
using System.Collections.Generic;
using System.Linq;

namespace Wims.Core.Dto
{
	public class SequenceDto : List<ChordDto>
	{
		public SequenceDto()
		{
		}

		public SequenceDto(IEnumerable<ChordDto> collection) : base(collection)
		{
		}

		public override string ToString()
		{
			return string.Join(", ", this);
		}
	}

	public static class SequenceDtoExtensions
	{
		public static SequenceDto ToSequenceDto(this IEnumerable<ChordDto> @this)
		{
			return new SequenceDto(@this);
		}

		public static SequenceDto ToSequenceDto(this string @this)
		{
			return @this.Split(", ", StringSplitOptions.RemoveEmptyEntries)
				.Select(ChordDto.FromString)
				.ToSequenceDto();
		}

		public static SequenceDto ToSequenceDto(this List<List<string>> @this)
		{
			return @this.Select(keys => new ChordDto
			{
				Keys = keys.ToArray()
			}).ToSequenceDto();
		}
	}
}