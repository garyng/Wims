using JetBrains.Annotations;

namespace Wims.Core.Models
{
	/// <summary>
	/// Specify when the <see cref="ShortcutRo"/> is active
	/// </summary>
	public class ContextRo
	{
		[NotNull]
		public string Icon { get; set; }

		[NotNull]
		public MatchRo Match { get; set; }
	}
}