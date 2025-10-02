using Microsoft.Win32;

namespace InputLog.Core.Util
{
    /// <summary>
    /// Access to the Windows Registry.
    /// </summary>
    public static class RegistryTools
    {
        public const string DEFAULT_APP_NAME = "Inputlog";
        public const string LITE_APP_NAME = "InputlogLite";
        // Processor used
        private enum ARCHITECTURE_TYPE { x64, x32 }

        /// <summary>
        /// Save a file value in the registry
        /// </summary>
        /// <param name="appName">The 'official' name of the application</param>
        /// <param name="name">Key</param>
        /// <param name="value">The file path</param>
        public static void SaveSetting(string appName, string name, object value)
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Software", true);
            RegistryKey subKey = regKey?.CreateSubKey(appName);
            subKey?.SetValue(name, value);
        }

        /// <summary>
        /// Get a value.
        /// </summary>
        /// <param name="appName">The 'official' name of the application</param>
        /// <param name="name">Key</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static object GetSetting(string appName, string name, object defaultValue)
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Software", true);
            var subKey = regKey?.CreateSubKey(appName);
            return subKey?.GetValue(name, defaultValue);
        }

        /// <summary>
        /// Delete a file pointer.
        /// </summary>
        /// <param name="appName">The 'official' name of the application</param>
        /// <param name="name">Key to the file path</param>
        public static void DeleteSetting(string appName, string name)
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Software", true);
            if (regKey == null) return;
            RegistryKey subKey = regKey.CreateSubKey(appName);
            try
            {
                subKey?.DeleteValue(name);
            }
            catch
            {
                // Ignore
            }
        }

        /// <summary>
        /// Checking if the 'Sogou_Pinyin' keyboard layout is registered.
        /// This keyboard is necessary to correctly record Chinese input in Word.
        /// </summary>
        /// <returns>'true' if registered</returns>
        public static bool FoundSogouInputRegistryKey()
        {
            var arch = GetSystemArchitecture();
            var key = arch == ARCHITECTURE_TYPE.x64 ? @"SOFTWARE\WOW6432Node\SogouInput" : @"SOFTWARE\SogouInput";
            var sogouKey = Registry.LocalMachine.OpenSubKey(key);
            if (sogouKey == null) return false;
            var installPath = sogouKey.GetValue(null).ToString();
            return !installPath.IsNullOrEmpty();
        }

        /// <summary>
        /// Location of the software depends on the processor of the machine.
        /// </summary>
        /// <returns>processor type</returns>
        private static ARCHITECTURE_TYPE? GetSystemArchitecture()
        {
            RegistryKey arch = Registry.LocalMachine.OpenSubKey(
                    @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment");
            if (arch == null) return null;
            string exactArch = arch.GetValue("PROCESSOR_ARCHITECTURE", "").ToString();
            return exactArch.ToLower().StartsWith("x86") ? ARCHITECTURE_TYPE.x32 : ARCHITECTURE_TYPE.x64;
        }
    }
}