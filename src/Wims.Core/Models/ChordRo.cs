using JetBrains.Annotations;

namespace Wims.Core.Models
{
	/// <summary>
	/// A single chord, ie. keys that are pressed together
	/// </summary>
	public class ChordRo
	{
		[NotNull]
		[ItemNotNull]
		public string[] Keys { get; set; }
	}
}