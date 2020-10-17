namespace Wims.Core.Models
{
	public class WimsConfig
	{
		/// <summary>
		/// The home directory where all the <see cref="ShortcutsRaw"/> files are saved.
		/// </summary>
		public string Directory { get; set; }


		/// <summary>
		/// Shortcut for activating the app.
		/// </summary>
		public string Activation { get; set; }

		/// <summary>
		/// Auto hide app when lost focus.
		/// </summary>
		public bool AutoHide { get; set; }

		/// <summary>
		/// Keep window always on top
		/// </summary>
		public bool Topmost { get; set; }
	}
}