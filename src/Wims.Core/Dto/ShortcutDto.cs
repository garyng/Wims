using System.Collections.Generic;

namespace Wims.Core.Dto
{
	public class ShortcutDto
	{
		public ContextDto Context { get; set; }
		public string Description { get; set; }
		public List<BindingDto> Sequence { get; set; }
	}
}