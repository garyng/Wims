using JetBrains.Annotations;

namespace Wims.Core.Models
{
	/// <summary>
	/// Specify when the <see cref="ContextRo"/> is active
	/// </summary>
	public class MatchRo
	{
		[CanBeNull]
		public string Class { get; set; }

		[CanBeNull]
		public string Exe { get; set; }
	}
}