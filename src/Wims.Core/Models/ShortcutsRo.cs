using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Wims.Core.Models
{
	/// <summary>
	/// Represent a single config file that stores both
	/// <see cref="ContextRo"/>s and <see cref="ShortcutRo"/>s
	/// </summary>
	public class ShortcutsRo
	{
		[YamlIgnore]
		public string Path { get; set; }
		public Dictionary<string, ContextRo> Contexts { get; set; }
		public Dictionary<string, ShortcutRo> Shortcuts { get; set; }
	}


	/// <summary>
	/// Specify when the <see cref="ShortcutRo"/> is active
	/// </summary>
	public class ContextRo
	{
		public string Icon { get; set; }
		public MatchRo Match { get; set; }
	}

	/// <summary>
	/// Specify when the <see cref="ContextRo"/> is active
	/// </summary>
	public class MatchRo
	{
		public string Class { get; set; }
		public string Exe { get; set; }
	}

	/// <summary>
	/// A single shortcut
	/// </summary>
	public class ShortcutRo
	{
		/// <summary>
		/// The name of the context, corresponds to <see cref="Context"/>
		/// </summary>
		public string Context { get; set; }
		public List<BindingRo> Bindings { get; set; }
	}

	/// <summary>
	/// A single binding
	/// </summary>
	public class BindingRo
	{
		public string[] Keys { get; set; }
	}

}
