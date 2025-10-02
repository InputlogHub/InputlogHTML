using System;
using System.Collections.Generic;
using InputLog.Core.Events;
using InputLog.Core.IO.Basic;
using System.IO;
using InputLog.Core.IO.Basic.Input;

namespace InputLog.Core.IO.Txt {

    /// <summary>
    /// EventReader that can read the Txt format.
    /// </summary>
    public class TxtEventReader : AbstractEventReader {

        /// <summary>
        /// Constructs a TxtEventReader.
        /// </summary>
        /// <param name="stream">Stream to read eventz from.</param>
        public TxtEventReader(Stream stream)
            : base(LogFormat.TXT.ToString()) {
        }

        public override List<Event> ReadEvents() {
            throw new NotImplementedException();
        }

        public override void ReadFooter() {
            throw new NotImplementedException();
        }

        public override SessionIdentification ReadHeader() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">True if the managed resources should also be disposed, false if not.</param>
        protected void Dispose(bool disposing) {

        }
    }
}