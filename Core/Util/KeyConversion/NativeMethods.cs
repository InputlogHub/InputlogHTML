using System;
using System.Runtime.InteropServices;
using System.Text;

namespace InputLog.Core.Util.KeyConversion {

	class NativeMethods {
		/// <summary>
		/// Unicode version of MapVirtualKeyEx.
		/// (See http://msdn.microsoft.com/en-us/library/ms646307.aspx.)
		/// </summary>
		/// <param name="uCode">The virtual-key code or scan code for a key.
		/// How this value is interpreted depends on the value of the uMapType parameter.
		/// 
		/// Starting with Windows Vista, the high byte of the uCode value can contain
		/// either 0xe0 or 0xe1 to specify the extended scan code.</param>
		/// <param name="uMapType">The translation to perform. The value of this
		/// parameter depends on the value of the uCode parameter.</param>
		/// <param name="dwhkl">Input locale identifier to use for translating the specified code.
		/// This parameter can be any input locale identifier previously returned by the
		/// LoadKeyboardLayout function.</param>
		/// <returns></returns>
		[DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "MapVirtualKeyExW", ExactSpelling = true)]
		public static extern uint MapVirtualKeyEx(uint uCode, uint uMapType, IntPtr dwhkl);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "LoadKeyboardLayoutW", ExactSpelling = true)]
		public static extern IntPtr LoadKeyboardLayout(string pwszKlid, uint flags);

		[DllImport("user32.dll", ExactSpelling = true)]
		public static extern bool UnloadKeyboardLayout(IntPtr hkl);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, KeysEx[] lpKeyState,
			StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "VkKeyScanExW", ExactSpelling = true)]
		public static extern ushort VkKeyScanEx(char ch, IntPtr dwhkl);

		[DllImport("user32.dll", ExactSpelling = true)]
		public static extern int GetKeyboardLayoutList(int nBuff, [Out, MarshalAs(UnmanagedType.LPArray)] IntPtr[] lpList);

		/// <summary>
		/// Returns the key state for the given virtual key.
		/// (See http://msdn.microsoft.com/en-us/library/ms646301.aspx.)
		/// </summary>
		/// <param name="virtualKeyCode">The virtual key where to get the state for.</param>
		/// <returns>The state of the given virtual key.</returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern short GetKeyState(int virtualKeyCode);
	}
}