using System.Collections.Generic;

namespace Wims.Core.Dto
{
	public class ShortcutsDto
	{
		public Dictionary<string, ContextDto> Contexts { get; set; }
		public List<ShortcutDto> Shortcuts { get; set; }
	}
}