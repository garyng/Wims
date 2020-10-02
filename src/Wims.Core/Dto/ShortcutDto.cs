using System.Collections.Generic;

namespace Wims.Core.Dto
{
	public class ShortcutDto
	{
		public ContextDto Context { get; set; }
		public string Description { get; set; }
		// todo: change this to Sequence
		public List<BindingDto> Bindings { get; set; }
	}
}