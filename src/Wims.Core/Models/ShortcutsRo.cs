﻿using System.Collections.Generic;
using JetBrains.Annotations;
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
		[NotNull]
		public string Path { get; set; }
		[CanBeNull]
		public Dictionary<string, ContextRo> Contexts { get; set; }
		[CanBeNull]
		public Dictionary<string, ShortcutRo> Shortcuts { get; set; }
	}
}
