using System;

namespace InputLog.Core.Plugin
{

    /// <summary>
    /// Exception thrown when someone tries to alter settings of a plugin
    /// while the plugin is running.
    /// </summary>
    [Serializable]
    public class ChangeNotAllowedException : PluginException
    {
        public ChangeNotAllowedException() { }
        public ChangeNotAllowedException(string message) : base(message) { }
        public ChangeNotAllowedException(string message, Exception inner) : base(message, inner) { }
        protected ChangeNotAllowedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        public override string Message
        {
            get
            {
                return "Settings cannot be changed if the plugin is running: "
                    + base.Message;
            }
        }
    }
}
