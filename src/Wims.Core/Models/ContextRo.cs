namespace Wims.Core.Models
{
	/// <summary>
	/// Specify when the <see cref="ShortcutRo"/> is active
	/// </summary>
	public class ContextRo
	{
		public string Icon { get; set; }
		public MatchRo Match { get; set; }
	}
}