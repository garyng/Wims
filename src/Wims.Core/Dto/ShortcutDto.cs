using JetBrains.Annotations;

namespace Wims.Core.Dto
{
	public class ShortcutDto
	{
		[CanBeNull]
		public ContextDto Context { get; set; }

		[NotNull]
		public string Description { get; set; }

		[NotNull]
		[ItemNotNull]
		public SequenceDto Sequence { get; set; }
	}
}