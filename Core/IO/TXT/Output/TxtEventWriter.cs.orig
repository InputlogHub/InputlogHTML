using System;
using System.IO;
using InputLog.Core.Events;
using InputLog.Core.IO.Basic;
using InputLog.Core.IO.Basic.Output;

namespace InputLog.Core.IO.Txt.Output
{
    /// <summary>
    ///     Eventwriter using plain text.
    /// </summary>
    public class TxtEventWriter : AbstractEventWriter
    {
        #region Fields

        /// <summary>
        ///     The writer used for writing to the output stream.
        /// </summary>
        private readonly StreamWriter Writer;

        #endregion

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="stream">The stream to which the output will be sent.</param>
        public TxtEventWriter(Stream stream) : base(LogFormat.TXT)
        {
            Writer = new StreamWriter(stream);
        }

        /// <summary>
        ///     Prepares the Writer for writing eventz by writing a header containing the information
        ///     stored in SessionIdentification.
        /// </summary>
        /// <param name="sessionIdentification">Some data about the session.</param>
        public override void Start(SessionIdentification sessionIdentification)
        {
            Writer.WriteLine("LOG START");

            Writer.WriteLine("META (START)");
            foreach (var entry in sessionIdentification.MetaInfo)
            {
                Writer.WriteLine(entry.Key + "=" + entry.Value);
            }
            Writer.WriteLine("META (END)");

            Writer.WriteLine("SESSION-IDENTIFICATION (START)");
            foreach (var entry in sessionIdentification.SessionInfo)
            {
                Writer.WriteLine(entry.Key + "=" + entry.Value);
            }
            Writer.WriteLine("SESSION-IDENTIFICATION (END)");
        }

        protected override Object GetWriter()
        {
            return Writer;
        }

        /// <summary>
        ///     Flushes and closes the Writer, after this call, the object is disposed.
        /// </summary>
        public override void Stop()
        {
            Writer.WriteLine("LOG END");
            base.Stop();
        }

        /// <summary>
        ///     Opens an event by writing a header.
        ///     This method is called before each part of it is written (using the EventPartWriters).
        /// </summary>
        /// <param name="even">The event where to write the header for.</param>
        protected override void OpenEvent(Event even)
        {
            Writer.Write("BEGIN EVENT [");
            foreach (var attribute in even.Properties)
            {
                Writer.Write(attribute.Key + "=" + attribute.Value + " ");
            }
            Writer.WriteLine("]");
        }

        /// <summary>
        ///     Closes the last opened event by writing a footer.
        ///     This method is called after all parts of the current event is written (using the EventPartWriters).
        /// </summary>
        protected override void CloseEvent()
        {
            Writer.WriteLine("END EVENT");
        }

        /// <summary>
        ///     Disposes the object.
        /// </summary>
        /// <param name="disposing">True if the managed resources should also be disposed, false if not.</param>
        protected override void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    Writer.Flush();
                    Writer.Close();
                }
                Disposed = true;
            }
        }
    }
}