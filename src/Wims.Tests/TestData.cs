using System.IO;
using System.Reflection;
using Wims.Tests.Data;

namespace Wims.Tests
{
	public static class TestData
	{
		public static string OneContext1Shortcut() => "1context-1shortcut.yml".Load();
		public static string TwoContext2Shortcuts() => "2context-2shortcuts.yml".Load();

		private static string Load(this string filename)
		{
			var ns = typeof(IDataMarker).Namespace;
			var assembly = Assembly.GetExecutingAssembly();
			using var stream = assembly.GetManifestResourceStream($"{ns}.{filename}");
			using var reader = new StreamReader(stream);
			return reader.ReadToEnd();
		}
	}
}