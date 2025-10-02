using System;

namespace InputLog.Core.Util.KeyConversion {

	/// <summary>
	/// Helper enumeration representing possible modifying states of the keyboard
	/// like Shift, Ctrl, ...
	/// </summary>
	[Flags]
	public enum ShiftState
	{
		BASE = 0,							    // 0
		SHFT = 1,							    // 1
		CTRL = 2,							    // 2
		SHFT_CTRL = SHFT | CTRL,			    // 3
		MENU = 4,							    // 4 -- NOT USED (Alt button)
		SHFT_MENU = SHFT | MENU,			    // 5 -- NOT USED
		MENU_CTRL = MENU | CTRL,			    // 6
		SHFT_MENU_CTRL = SHFT | MENU | CTRL,	// 7
		XXXX = 8,							    // 8
		SHFT_XXXX = SHFT | XXXX,				// 9
	}
}