using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace InputLog.Core.Util.Server
{
    /// <summary>
    /// Utility class for handling a user's account settings.
    /// For safety, these are stored in the registry.
    /// </summary>
    [Serializable()]
    public class AccountSettings
    {
        private const String APP_NAME = "Inputlog";
        public String ID { get; set; }
        public String Pass { get; set; }
        public bool AutoLogin { get; set; }

        public AccountSettings(String id, String pass, bool autoLogin)
        {
            ID = id;
            Pass = pass;
            AutoLogin = autoLogin;
        }

        // Constructor to be used in deserialization
        public AccountSettings()
        {
        }

        /// <summary>
        /// Saves the current settings to registry
        /// </summary>
        public void SaveSettings(string appName = APP_NAME)
        {
            var aes = new SimpleAES();
            String encPass = aes.ByteArrToString(aes.Encrypt(Pass + ID));
            RegistryTools.SaveSetting(appName, "ID", ID);
            RegistryTools.SaveSetting(appName, "AutoLogin", AutoLogin);
            RegistryTools.SaveSetting(appName, "GUID", encPass);
        }

        /// <summary>
        /// Loads an AccountSettings object from registry
        /// </summary>
        /// <returns></returns>
        public static AccountSettings LoadSettings(string appName=APP_NAME)
        {
            String id = RegistryTools.GetSetting(appName, "ID", "").ToString();
            bool autoLogin = Boolean.Parse(RegistryTools.GetSetting(appName, "AutoLogin", "false").ToString());
            String pass = RegistryTools.GetSetting(appName, "GUID", "").ToString();
            if (!String.IsNullOrEmpty(pass))
            {
                var aes = new SimpleAES();
                var decr = aes.Decrypt(aes.StrToByteArray(pass));
                pass = decr.Substring(0, decr.Length - id.Length);
            }
            return new AccountSettings(id, pass, autoLogin);
        }

        public void Serialize(string filepath)
        {
            try
            {
                FileStream test = File.Open(filepath, FileMode.OpenOrCreate);
                var aes = new SimpleAES();
                string oldPass = Pass;
                Pass = aes.ByteArrToString(aes.Encrypt(Pass + ID));
                BinaryFormatter bif = new BinaryFormatter();
                bif.Serialize(test, this);
                test.Close();
                Pass = oldPass;
            }
            catch (Exception)
            {
            }
        }

        public static AccountSettings Deserialize(string filepath)
        {
            BinaryFormatter bif = new BinaryFormatter();
            Console.WriteLine("Test");
            FileStream test = File.OpenRead(filepath);
            Console.WriteLine("Test2");
            AccountSettings res = new AccountSettings();
            res = (AccountSettings) bif.Deserialize(test);
            if (!String.IsNullOrEmpty(res.Pass))
            {
                var aes = new SimpleAES();
                var decr = aes.Decrypt(aes.StrToByteArray(res.Pass));
                res.Pass = decr.Substring(0, decr.Length - res.ID.Length);
            }
            return res;
        }
    }
}
