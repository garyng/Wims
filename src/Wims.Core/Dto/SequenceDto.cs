using System;
using System.Collections.Generic;
using System.Linq;

namespace Wims.Core.Dto
{
	public class SequenceDto : List<BindingDto>
	{
		public SequenceDto()
		{
		}

		public SequenceDto(IEnumerable<BindingDto> collection) : base(collection)
		{
		}

		public override string ToString()
		{
			return string.Join(", ", this);
		}
	}

	public static class SequenceDtoExtensions
	{
		public static SequenceDto ToSequenceDto(this IEnumerable<BindingDto> @this)
		{
			return new SequenceDto(@this);
		}

		public static SequenceDto ToSequenceDto(this string @this)
		{
			return @this.Split(", ", StringSplitOptions.RemoveEmptyEntries)
				.Select(BindingDto.FromString)
				.ToSequenceDto();
		}

		public static SequenceDto ToSequenceDto(this List<List<string>> @this)
		{
			return @this.Select(keys => new BindingDto
			{
				Keys = keys.ToArray()
			}).ToSequenceDto();
		}
	}
}