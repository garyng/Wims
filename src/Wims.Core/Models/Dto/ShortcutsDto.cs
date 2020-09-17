using System;
using System.Collections.Generic;
using System.Text;

namespace Wims.Core.Models.Dto
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
		public List<BindingDto> Bindings { get; set; }
	}

	public class BindingDto
	{
		public string[] Keys { get; set; }
	}
}