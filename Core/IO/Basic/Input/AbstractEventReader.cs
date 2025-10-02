using System;
using System.Collections.Generic;
using System.Reflection;
using InputLog.Core.Events;

namespace InputLog.Core.IO.Basic.Input
{

    /// <summary>
    /// Abstract base class for EventReaders.
    /// </summary>
    public abstract class AbstractEventReader
    {
        #region Fields
        /// <summary>
        /// The LogFormat of the Reader.
        /// </summary>
        private readonly string _logFormat;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logFormat">The LogFormat of the Reader.</param>
        protected AbstractEventReader(string logFormat)
        {
            _logFormat = logFormat;
        }

        // Following methods should be overridden by the extending classes
        // These methods are just passed through from the EventReader-interface.
        public abstract List<Event> ReadEvents();
        public abstract SessionIdentification ReadHeader();

        protected IEventPart ReadEventPart(string partType, string eventType, object arg)
        {
            var handle = EventIOHandlers.GetReadHandler(partType, eventType, _logFormat);
            Type t = Assembly.GetCallingAssembly().GetType(handle.Item1);
            MethodInfo meth = t.GetMethod(handle.Item2);
            IEventPart res = null;
            try
            {
                if (meth != null) res = (IEventPart)meth.Invoke(null, [arg]);
            }
            catch (Exception e)
            {
                // In case the eventPart is empty or can not be correctly read we return a null event part.
                // The caller can handle this better. 
                return null;
            }
            return res;
        }

        public virtual void ReadFooter()
        {
        }

        public virtual void Close()
        {
        }
    }
}