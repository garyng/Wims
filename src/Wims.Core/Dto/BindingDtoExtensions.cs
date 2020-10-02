using System;
using System.Collections.Generic;
using System.Linq;

namespace Wims.Core.Dto
{
	public static class BindingDtoExtensions
	{
		public static string AsString(this IEnumerable<BindingDto> @this)
		{
			return string.Join(", ", @this);
		}

		public static List<BindingDto> ToBindingDto(this string @this)
		{
			return @this.Split(", ", StringSplitOptions.RemoveEmptyEntries)
				.Select(BindingDto.FromString)
				.ToList();
		}

		public static List<BindingDto> ToBindingDto(this List<List<string>> @this)
		{
			return @this.Select(keys => new BindingDto
			{
				Keys = keys.ToArray()
			}).ToList();
		}
	}
}