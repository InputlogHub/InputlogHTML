using System;
using System.Reflection;
using InputLog.Core.Events;

namespace InputLog.Core.IO.Basic.Output
{
    /// <summary>
    /// Abstract base class for EventWriters.
    /// </summary>
    public abstract class AbstractEventWriter
    {
        #region Fields
        /// <summary>
        /// Counter used for numbering the eventz when writing them.
        /// </summary>
        private ulong EventID;

        /// <summary>
        /// The LogFormat of the Writer.
        /// </summary>
        private readonly string LogFormat;

        protected bool Disposed;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logFormat">The LogFormat of the Writer.</param>
        protected AbstractEventWriter(string logFormat)
        {
            LogFormat = logFormat;
        }

        /// <summary>
        /// Write the given event.
        /// </summary>
        /// <param name="even">The event to write.</param>
        public virtual void Write(Event even)
        {
            if (even.Parts.Count <= 0) return;
            // CHANGE: 6 Jan 2013 - Existing events that get rewritten should not have their ID reset.
            if (!even.Properties.ContainsKey("id"))
            {
                even.Properties["id"] = EventID.ToString();
            }
            else
            {
                // We do not want follow up events to accidently end up with a lower event ID than a 
                // preceding event due to not updating the EventID to the last events EventID.
                EventID = ulong.Parse(even.Properties["id"]);
            }
            OpenEvent(even);
            foreach (IEventPart eventPart in even.Parts)
            {
                WriteEventPart(eventPart, even.Type, GetWriter());
            }
            CloseEvent();
            EventID++;
        }

        protected virtual void WriteEventPart(IEventPart part, string eType, object arg)
        {
            var handle = EventIOHandlers.GetWriteHandler(part.GetType(), eType, LogFormat);
            Type t = Assembly.GetCallingAssembly().GetType(handle.Item1);
            MethodInfo meth = t.GetMethod(handle.Item2);
            meth.Invoke(null, new[] { part, arg });
        }

        /// <summary>
        /// Flushes and closes the Writer, after this call, the object is disposed.
        /// </summary>
        public virtual void Stop()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            Disposed = true;
        }

        #region Following methods should be overridden by the extending classes

        /// <summary>
        /// Prepares the Writer for writing eventz. During this call, the writer may write a header.
        /// </summary>
        /// <param name="sessionIdentification">Some data about the session.</param>
        public abstract void Start(SessionIdentification sessionIdentification);


        protected abstract object GetWriter();

        /// <summary>
        /// Opens an event by writing a header.
        /// This method is called before each part of it is written (using the EventPartWriters).
        /// </summary>
        /// <param name="even">The event where to write the header for.</param>
        protected abstract void OpenEvent(Event even);

        /// <summary>
        /// Closes the last opened event by writing a footer.
        /// This method is called after all parts of the current event is written (using the EventPartWriters).
        /// </summary>
        protected abstract void CloseEvent();

        #endregion
    }
}