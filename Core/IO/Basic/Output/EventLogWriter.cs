using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using InputLog.Core.Events;
using InputLog.Core.Events.WinLog;
using InputLog.Core.Util;

namespace InputLog.Core.IO.Basic.Output
{
    /// <summary>
    /// An EventLog that writes Logged eventz in an ordered way with the help of an EventWriter.
    /// </summary>
    public class EventLogWriter : IDisposable
    {
        //private const string SESSIONIDENTIFICATION_PLUGINS = "__Plugins";

        /// <summary>
        /// Check whether the relative creation time is smaller than the start time of
        /// the first event, as it should be. If not, the start time becomes the relative
        /// creatiion time.
        /// </summary>
        private bool TimeAdjusted;
        private static SessionIdentification _sessionId;

        /// <summary>
        /// Constructs an Eventlog (internal ctor, use the EventLogFactory to create actual
        /// EventLogs).
        /// </summary>
        /// <param name="eventWriter"> Writer that is used for writing the finished/closed
        /// eventz to.</param>
        internal EventLogWriter(AbstractEventWriter eventWriter)
        {
            EventWriter = eventWriter;
            EventProcessor = new Thread(ProcessEvents) {Name = "EventLogProcessor"};
        }

        #region IDisposable Members
        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        /// <summary>
        /// Starts the eventlog. An eventlog must be started before it can be used.
        /// An eventlog can only be started once and cannot be restarted after
        /// it is stopped.
        /// </summary>
        /// <param name="sessionIdentification">SessionIdentification containing key-value pairs that
        /// identify this logging session (e.g. user that started the log, time, etc).</param>
        /// <param name="keepOldSessionInformation">This parameter specifies whether the sessionIdentification 
        /// from the parameter should be used as is, or should be updated to reflect that the logfile has been
        /// recreated at the time of calling the Start() method. For merging or rewriting existing files
        /// we suggest to keep the old session information.</param>
        public void Start(SessionIdentification sessionIdentification, bool keepOldSessionInformation = false) 
        {
            Debug.Assert(State == States.IDLE, "Logging can only be started once.");
            if (!keepOldSessionInformation)
            {
                AddMetaInfo(sessionIdentification);
            }
            State = States.RUNNING;
            EventWriter.Start(sessionIdentification);
            EventProcessor.Start();
        }

		/// <summary>
		/// Writes an existing list of events and session identification to a stream.
		/// </summary>
		/// <param name="sessionIdentification">SessionIdentification containing key-value pairs that
		/// identify this logging session (e.g. user that started the log, time, etc).</param>
		/// <param name="events">List of events to write to stream.</param>
		public void WriteExistingFile(SessionIdentification sessionIdentification, List<Event> events)
		{
			State = States.RUNNING;
			EventWriter.Start(sessionIdentification);
			foreach (Event e in events)
			{
				EventWriter.Write(e);
			}
			EventWriter.Stop();
			State = States.STOPPED;
		}

        /// <summary>
        /// Adds meta-data about the current logsession to the sessionidentification document.
        /// When a given piece of meta-data is already present in the given SessionIdentification-data, 
        /// this method will copy the old meta-data to a new property with the same name but prefixed 
        /// with SessionIdentication.OLD_META_PREFIX.
        /// The existing property itself will be overwritten with the new value. 
        /// </summary>
        /// <param name="sessionIdentification">SessionIdentification to add the meta information to.</param>
        private static void AddMetaInfo(SessionIdentification sessionIdentification)
        {
            // Add some custom session-information
            sessionIdentification.CopyOldInfo();
            sessionIdentification.SetCreationDate(DateTime.Now);
            sessionIdentification.GenerateGuid();
           
            // Add relative time (relative to this machine's boottime).
            // The value of this property is derived from the system timer and is stored as a 32-bit signed integer. 
            // Consequently, if the system runs continuously, TickCount will increment from zero to Int32.MaxValue 
            // for approximately 24.9 days, then jump to Int32.MinValue. This seems to happen in class rooms when the machines 
            // are not switched off. The next code line removes the sign bit to yield a nonnegative number. This will 
            // probably solve a number of problems such as negative or maximum value pause times (Tickets #7 and #19).
			
			// 03.06.2013 - Changed by Tom.
			// META_LOGRELATIVECREATIONTIME is now calculated only if it is not yet set in the MetaInfo.
			// REASON: This relative log creation date takes the time from the current machine, when writing away the 
			// the log file. It is ASSUMED that this is the same machine as the logging has taken place on.
			// However when converting from different formats, such as the TRANSLOG format, which uses 0 as a start time,
			// the LOGRELATIVECREATIONTIMEE should also reflect 0.
			// 
            if (!sessionIdentification.HasRelativeCreationTime())
            {
                sessionIdentification.SetRelativeCreationTime((ulong)Environment.TickCount & Int32.MaxValue);
            }
            _sessionId = sessionIdentification;
        }

        /// <summary>
        /// Stops the eventlog. An eventlog must be started before it can be stopped.
        /// This method flushes all waiting eventz to the Writer and then disposes the object.
        /// </summary>
        public void Stop()
        {
            Debug.Assert(State == States.RUNNING, "Eventlog must be started before it can be stopped.");

            State = States.STOPPED;
            EventAdded.Set();

            // Wait for events being processed
            // Timeout in settings is in time, we need millis!
            var timeout = Properties.Settings.Default.Stop_Timeout * 1000;
            if (!EventProcessor.Join(timeout))
            {
                // Thread not stopped => force stop it and write events manually
                EventProcessor.Abort();

                // Write eventz manually
                while (ThreadEvents.Count > 0)
                {
                    EventWriter.Write(Events[ThreadEvents.First().Key]);
                    ThreadEvents.Remove(ThreadEvents.First().Key);
                }
            }
            Dispose();
        }

        /// <summary>
        /// Adds the given EventPart to the event with the given alias.
        /// </summary>
        /// <param name="e">The EventPart to add to the event.</param>
        /// <param name="alias">The alias of the event where to add 'e' to.</param>
        public void Write(IEventPart e, string alias)
        {
            Debug.Assert(State == States.RUNNING, "Events can only be written to an eventlog that has been started.");

            lock (Events)
            {
                ulong eventId;
                if (Aliases.TryGetValue(alias, out eventId))
                {
                    Events[eventId].Parts.Add(e);
                }
                //else TODO throw exception
            }
        }

        /// <summary>
        /// Writes a whole event.
        /// </summary>
        /// <param name="even">The complete event to write.</param>
        public void Write(Event even)
        {
            var alias = "__" + EventID;
            AddEvent(even, alias);
            EndEvent(alias);
        }

        /// <summary>
        /// Creates a new event with the given alias used for refering to it.
        /// The event will not be flushed to the Writer until 
        /// </summary>
        /// <param name="alias">The alias used for refering to the newly
        /// created event.</param>
        public void StartEvent(string alias)
        {
            Debug.Assert(State == States.RUNNING, "Events can only be created in an eventlog that has been started.");

            AddEvent(new Event(), alias);
        }

        /// <summary>
        /// By calling this method, the caller indicates that it has finished processing
        /// the event and that he considers it finished/closed.
        /// Only after nrOfProcessors (as given in NewEvent) calls are received,
        /// the event will actually be closed and flushed to the writer.
        /// </summary>
        /// <param name="eventAlias">The alias of the event for which the caller indicates
        /// that it is finished processing it.</param>
        public void EndEvent(string eventAlias)
        {
            Debug.Assert(State == States.RUNNING, "Events can only be closed in an eventlog that has been started.");

            lock (Events)
            {
                ulong eventId;
                if (Aliases.TryGetValue(eventAlias, out eventId))
                {
                    Aliases.Remove(eventAlias);
                    ThreadEvents[eventId].Set();
                }

                if (!TimeAdjusted)
                {
                    AdjustRelativeCreationTime(Events[eventId]);
                }
            }
            //else TODO unknown alias
        }

        /// <summary>
        /// When there is already an event logged before setting the relative creation time,
        /// a negative result is obtained when the relative start is subtracted from the event start time.
        /// In that case the starttime of the first event with a TimedEventPart is used as
        /// RelativeCreationTime. We need to check this only once at the start of the logging.
        /// </summary>
        /// <param name="even">The most recent event</param>
        private void AdjustRelativeCreationTime(Event even)
        {
            var t = Event.GetFirstEventPart<TimedEventPart>(even);
            if (t == null) return;
            var time = t.StartTime;
            if (_sessionId.GetRelativeCreationTime() > time)
            {
                _sessionId.SetRelativeCreationTime(time);
            }
            TimeAdjusted = true;
   
        }

        /// <summary>
        /// Maps the property with the given key to the given value.
        /// </summary>
        /// <param name="alias">The alias of the event of which to set the property.</param>
        /// <param name="key">The id of the property.</param>
        /// <param name="value">The value of the property.</param>
        public void SetProperty(string alias, string key, string value)
        {
            lock (Events)
            {
                ulong eventId;
                if (Aliases.TryGetValue(alias, out eventId))
                {
                    Events[eventId].Properties.Add(key, value);
                }
            }
        }

        /// <summary>
        /// Adds the given event under the given alias to the internal data structure.
        /// </summary>
        /// <param name="even">The event to add.</param>
        /// <param name="eventAlias">The alias used for referencing to the event.</param>
        private void AddEvent(Event even, string eventAlias)
        {
            lock (Events)
            {
                ThreadEvents.Add(EventID, new AutoResetEvent(false));
                Events.Add(EventID, even);
                Aliases.Add(eventAlias, EventID); // TODO will throw an exception if the alias already exists
                
                EventID++;
            }

            // Signal Processor
            EventAdded.Set();
        }

        /// <summary>
        /// Processes the events in order. Whenever called, it will run until the state
        /// is not Running and there are no more eventz waiting.
        /// </summary>
        private void ProcessEvents()
        {
            try
            {
                // Keep running while we are logging are while there are events to process
                while (State == States.RUNNING || ThreadEvents.Count > 0)
                {
                    //System.Threading.Thread.Sleep(250); // Uncomment to simulate slow computer
                    KeyValuePair<ulong, AutoResetEvent> first;
                    Event even;
                    while (ThreadEvents.Count == 0)
                    {
                        EventAdded.WaitOne(); // Wait for EventAdded to be signaled

                        // If we have stopped logging (stoplogging will call this.EventAdded.Set())
                        // and there are no more events to process, then we may quit.
                        if (State == States.STOPPED)
                        {
                            return;
                        }
                    }

                    lock (Events)
                    {
                        first = ThreadEvents.First();
                        even = Events[first.Key];
                    }

                    // Wait until every processing entity has indicated it has finished processing it
                    // TODO add while loop? had an exception here when I pressed stop recording.
                    // A first chance exception of type 'System.OperationCanceledException' occurred in mscorlib.dll
                    //A first chance exception of type 'System.Threading.ThreadAbortException' occurred in mscorlib.dll
                    //A first chance exception of type 'System.Threading.ThreadAbortException'
                    // occurred in InputLogCore.dll
                    //An exception of type 'System.Threading.ThreadAbortException' 
                    // occurred in InputLogCore.dll but was not handled in user code
                    //Exception: System.Threading.ThreadAbortException: Thread was being aborted.
                    //   at System.Threading.WaitHandle.WaitOneNative(SafeHandle waitableSafeHandle, 
                    // UInt32 millisecondsTimeout, Boolean hasThreadAffinity, Boolean exitContext)
                    //   at System.Threading.WaitHandle.InternalWaitOne(SafeHandle waitableSafeHandle, 
                    // Int64 millisecondsTimeout, Boolean hasThreadAffinity, Boolean exitContext)
                    //   at System.Threading.WaitHandle.WaitOne(Int32 millisecondsTimeout, Boolean exitContext)
                    //   at System.Threading.WaitHandle.WaitOne()
                    //   at InputLog.Core.IO.Basic.Output.EventLog.ProcessEvents() in
                    //C:\Users\Joris\Documents\Visual Studio 2010\Projects\InputLog\Core\IO\Basic
                    //\Output\EventLog.cs:line 282

                    // 20141126 E. The ThreadAbortException is happening when there is a 'stop record' event
                    // on an empty document having a focus change, some mouse events but without any keylogging.
                    // TODO: using ResetAbort in a try/catch is a temporary fix and is probably not be the definite answer.
                    // See: http://msdn.microsoft.com/en-us/library/system.threading.thread.resetabort.aspx
                    // and: http://stackoverflow.com/questions/1856286/threadabortexception

                    try
                    {
                        first.Value.WaitOne();
                    }
                    catch (ThreadAbortException)
                    {
                        Thread.ResetAbort();
                    }
                    finally
                    {
                        first.Value.Close();
                    }

                    // If the event is closed, its corresponding event can (and should) be disposed
                    // Event is finished => flush to writer & remove from buffers
                    lock (Events)
                    {
                        ThreadEvents.Remove(first.Key);
                        Events.Remove(first.Key);
                    }

                    // Flush event to writer
                    EventWriter.Write(even);
                }
            }
            catch (Exception e)
            {
                Debug.Print("Exception safely caught in EventLogWriter.ProcessEvents()");
                MessageLogger.CatchException(this, e, Severity.ERROR);
            }
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">True if the managed resources should also be disposed, false if not.</param>
        private void Dispose(bool disposing)
        {
            if (Disposed) return;
            if (disposing)
            {
                EventAdded.Close();
                EventWriter.Stop();
            }

            Disposed = true;
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~EventLogWriter()
        {
            Dispose(false);
        }

        #region Nested type: States
        /// <summary>
        /// Enumeration of the possible states this Eventlog can be in.
        /// </summary>
        private enum States
        {
            IDLE,
            RUNNING,
            STOPPED
        }
        #endregion

        #region UnusedFields
        //TODO currently not used.
        // Should we make eventWriter an ABC to allow us to copy the outputformat from the eventWriter here ?

        /// <summary>
        /// Maps each event alias to its ID.
        /// </summary>
        private readonly Dictionary<string, ulong> Aliases = new Dictionary<string, ulong>();

        /// <summary>
        /// Set whenever an event is added.
        /// </summary>
        private readonly AutoResetEvent EventAdded = new AutoResetEvent(false);

        /// <summary>
        /// Processes the closed events by handing them to the EventWriter.
        /// </summary>
        private readonly Thread EventProcessor;

        /// <summary>
        /// Writer used for writing the finished/closed events.
        /// </summary>
        private readonly AbstractEventWriter EventWriter;

        /// <summary>
        /// Dictionary containing the open events.
        /// </summary>
        private readonly Dictionary<ulong, Event> Events = new Dictionary<ulong, Event>();

        /// <summary>
        /// Contains for each event a corresponding AutoResetEvent that will be set if
        /// the event is closed by calling EndEvent.
        /// </summary>
        private readonly SortedDictionary<ulong, AutoResetEvent> ThreadEvents =
            new SortedDictionary<ulong, AutoResetEvent>();

        /// <summary>
        /// True if the instance is disposed, false if not.
        /// </summary>
        private bool Disposed;

        /// <summary>
        /// Counter for creating unique event IDs when needed.
        /// </summary>
        private ulong EventID;

        /// <summary>
        /// The current state of the eventlog. The current state determines the legal actions
        /// on the eventlog. For example, writing to the eventlog is only allowed when the
        /// current state == CurrentState.RUNNING.
        /// </summary>
        private States State = States.IDLE;

        /// <summary>
        /// Outputformat for this EventLog (e.g. TXT, XML).
        /// </summary>
        public string OutputFormat { get; set; }
        #endregion
    }
}