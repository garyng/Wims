namespace Wims.Core.Dto
{
	public class ShortcutDto
	{
		public ContextDto Context { get; set; }
		public string Description { get; set; }
		public SequenceDto Sequence { get; set; }
	}
}