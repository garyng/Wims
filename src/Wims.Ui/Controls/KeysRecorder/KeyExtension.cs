using System.Text;
using System.Windows.Input;
using Vanara.PInvoke;

namespace Wims.Ui.Controls.KeysRecorder
{
	public static class KeyExtension
	{
		/// <summary>
		/// Get the name of the key by using Win32 API. This will resolve weird keys like D1, OEM5, etc.
		/// </summary>
		/// <param name="this"></param>
		/// <returns></returns>
		public static string GetName(this Key @this)
		{
			var vk = (uint) KeyInterop.VirtualKeyFromKey(@this);
			var hKl = User32.GetKeyboardLayout(0);
			var sc = (int) User32.MapVirtualKeyEx(vk, User32.MAPVK.MAPVK_VK_TO_VSC_EX, hKl);

			// seems like the MapVirtualKeyEx is already fixed to include extended bit
			// (http://web.archive.org/web/20101022020556/http://blogs.msdn.com/b/michkap/archive/2006/08/29/729476.aspx)
			// but weirdly it doesn't
			// so we need to add in the extended bit manually
			// ref: http://www.setnode.com/blog/mapvirtualkey-getkeynametext-and-a-story-of-how-to/
			//		https://stackoverflow.com/questions/38100667/windows-virtual-key-codes

			switch ((VK) vk)
			{
				case VK.LEFT:
				case VK.UP:
				case VK.RIGHT:
				case VK.DOWN:
				case VK.RCONTROL:
				case VK.RMENU:
				case VK.LWIN:
				case VK.RWIN:
				case VK.APPS:
				case VK.PRIOR:
				case VK.NEXT:
				case VK.END:
				case VK.HOME:
				case VK.INSERT:
				case VK.DELETE:
				case VK.DIVIDE:
				case VK.NUMLOCK:
					sc |= (int) KF.EXTENDED;
					break;
			}

			var result = new StringBuilder(255);
			User32.GetKeyNameText(sc << 16, result, result.Capacity);

			var str = result.ToString();
			return string.IsNullOrEmpty(str) ? @this.ToString() : str;
		}

		public static KeyEventDto Up(this Key @this) => KeyEventDto.Up(@this);
		public static KeyEventDto Down(this Key @this) => KeyEventDto.Down(@this);
	}
}