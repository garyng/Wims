using System;

namespace Wims.Ui.Controls.KeysRecorder
{
	[Flags]
	public enum KF : int
	{
		/// <summary>
		/// Manipulates the extended key flag.
		/// </summary>
		EXTENDED = 0x0100,

		/// <summary>
		/// Manipulates the dialog mode flag, which indicates whether a dialog box is active.
		/// </summary>
		DLGMODE = 0x0800,

		/// <summary>
		/// Manipulates the menu mode flag, which indicates whether a menu is active.
		/// </summary>
		MENUMODE = 0x1000,

		/// <summary>
		/// Manipulates the ALT key flag, which indicated if the ALT key is pressed.
		/// </summary>
		ALTDOWN = 0x2000,

		/// <summary>
		/// Manipulates the repeat count.
		/// </summary>
		REPEAT = 0x4000,

		/// <summary>
		/// Manipulates the transition state flag.
		/// </summary>
		UP = 0x8000
	}
}