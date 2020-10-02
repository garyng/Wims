using System;
using System.Collections.Generic;
using System.Linq;

namespace Wims.Core.Dto
{
	public class ShortcutsDto
	{
		public Dictionary<string, ContextDto> Contexts { get; set; }
		public List<ShortcutDto> Shortcuts { get; set; }
	}

	public class ContextDto
	{
		public string Name { get; set; }
		public string Icon { get; set; }
		public MatchDto Match { get; set; }
	}

	public class MatchDto
	{
		public string Class { get; set; }
		public string Exe { get; set; }
	}

	public class ShortcutDto
	{
		public ContextDto Context { get; set; }
		public string Description { get; set; }
		// todo: change this to Sequence
		public List<BindingDto> Bindings { get; set; }
	}

	public class BindingDto
	{
		public string[] Keys { get; set; }

		public override string ToString()
		{
			return string.Join(" + ", Keys);
		}

		public static BindingDto FromString(string keys)
		{
			return new BindingDto
			{
				Keys = keys.Split(" + ", StringSplitOptions.RemoveEmptyEntries)
			};
		}
	}

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