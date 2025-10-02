using System.Collections.Generic;
using InputLog.Core.Util.KeyConversion;
using System.Diagnostics;

namespace InputLog.Core.Plugin.WordLog.Keys
{
    /// <summary>
    /// Class that identifies Word-navigation keys or other keys that should not be replayed (in the replay/revision analysis).
    /// </summary>
    internal static class NavigationKey
    {
        #region Fields

        /// <summary>
        /// A variable that can be reused to increase performance.
        /// </summary>
        private static Key Current = new Key();

		
		/// <summary>
		/// Normal navigation keys or keys that do nothing towards the output, do not
		/// have any replay value and thus can be ignored in the replay of the document's 
		/// events.
		/// </summary>
		private static List<KeysEx> NavigationKeys = new List<KeysEx>
		{
			KeysEx.VK_LEFT,
            KeysEx.VK_RIGHT,
            KeysEx.VK_UP,
            KeysEx.VK_DOWN,
            KeysEx.VK_HOME,
            KeysEx.VK_END,
            KeysEx.VK_PRIOR, // Page Up
            KeysEx.VK_NEXT, // Page Down
            KeysEx.VK_ESCAPE,
		};

		/// <summary>
		/// A list of control keys.
		/// Control keys, by themselves, or in combination with the 'NavigationKeys' do 
		/// not contribute any output to a document and thus can be ignored in the replay of the
		/// document's events.
		/// </summary>
		private static List<KeysEx> ControlKeys = new List<KeysEx>
		{
            KeysEx.VK_SHIFT,
            KeysEx.VK_RSHIFT,
            KeysEx.VK_LSHIFT,
            KeysEx.VK_CONTROL,
            KeysEx.VK_RCONTROL,
            KeysEx.VK_LCONTROL,
            KeysEx.VK_MENU,
            KeysEx.VK_LMENU,
            KeysEx.VK_RMENU,
		};

 
        #endregion

        /// <summary>
        /// Identifies whether a key is a navigation (word-based) key or not.
        /// </summary>
		/// <param name="keypressed">The key that has been pressed</param>
		/// <param name="keyboardState">The keyboard state containing other keys that are also in the 
		/// downstate while the other keys were being pressed.</param>
        /// <returns></returns>
		public static bool isNavigation(KeysEx keyPressed, string output, ICollection<KeysEx> kbState)
        {

			Debug.Print("key: {0} >> output=[{1}], outputlenght=[{2}] >> keyboardstate:", keyPressed, output,output.Length);
			foreach (KeysEx key in kbState)
			{
				Debug.Print("\tStatekey: {0}", key);
			}

			// Every key that does not have an output is a navigation key or a shortcut that 
			// should be ignored. The only exception is the backspace key that does not have a keystate
			if (output.Length != 0)
			{
				if (keyPressed == KeysEx.VK_BACK && (kbState.Contains(KeysEx.VK_CONTROL)
					|| kbState.Contains(KeysEx.VK_LCONTROL) || kbState.Contains(KeysEx.VK_RCONTROL)))
				{
					return true;
				}
				return false;
			}

			return true;

			/*
			// The current key pressed is a control key
			// The current key is a navigation key
			if (ControlKeys.Contains(keyPressed) || NavigationKeys.Contains(keyPressed))
			{
				return true;
			}

			return false;
			*/
        }
    }
}
