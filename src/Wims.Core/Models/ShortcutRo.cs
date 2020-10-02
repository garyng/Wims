namespace Wims.Core.Models
{
	/// <summary>
	/// A single shortcut
	/// </summary>
	public class ShortcutRo
	{
		/// <summary>
		/// The name of the context, corresponds to <see cref="Context"/>
		/// </summary>
		public string Context { get; set; }
		public SequenceRo Sequence { get; set; }
	}
}