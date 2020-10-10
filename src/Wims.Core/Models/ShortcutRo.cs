using JetBrains.Annotations;

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
		[CanBeNull]
		public string Context { get; set; }

		[NotNull]
		[ItemNotNull]
		public SequenceRo Sequence { get; set; }
	}
}