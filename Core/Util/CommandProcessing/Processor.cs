using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace InputLog.Core.Util.CommandProcessing
{
    /// <summary>
    /// Class that asynchronously (but in order) processes commands added
    /// to its queue.
    /// </summary>
    public sealed class Processor : IDisposable
    {
        #region Fields

        /// <summary>
        /// The queue of the commands to process.
        /// </summary>
        private readonly BlockingCollection<ICommand> _buffer =
            new BlockingCollection<ICommand>(new ConcurrentQueue<ICommand>());

        /// <summary>
        /// The thread that processes the commands in the buffer.
        /// </summary>
        private readonly Thread _executor;

        private bool _thisRunning; // Locked by Buffer

        /// <summary>
        /// The source for the Cancellation token used when taking the next
        /// command from the buffer.
        /// </summary>
        private CancellationTokenSource _tokenSource;

        /// <summary>
        /// True if the Processor is actively executing commands,
        /// false if it is on hold.
        /// </summary>
        private bool Running
        {
            get
            {
                lock (_buffer)
                {
                    return _thisRunning;
                }
            }
            set
            {
                lock (_buffer)
                {
                    _thisRunning = value;
                }
            }
        }

        /// <summary>
        /// True if the object is disposed, false if not.
        /// </summary>
        private bool Disposed { get; set; }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public Processor()
        {
            _executor = new Thread(ProcessCommands){ Priority = ThreadPriority.AboveNormal, Name = "Executor" };
            Running = false;
            Disposed = false;
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
        /// Make the Processor asynchronously process the commands in the buffer.
        /// </summary>
        public void Start()
        {
            if (Running) return;
            Running = true;

            // Create new uncanceled token source
            _tokenSource = new CancellationTokenSource();

            // Start processing
            _executor.Start();
        }

        /// <summary>
        /// Signals the processor to stop executing commands,
        /// but will not force stop it. (That is, any commands
        /// currently in the buffer will still be executed.)
        /// </summary>
        public void Stop()
        {
            foreach (ICommand cmd in ForceStop())
            {
                try
                {
                    cmd.Execute();
                }
                catch (Exception e)
                {
                    MessageLogger.CatchException(this, e, Severity.ERROR);
                }
            }
        }

        /// <summary>
        /// Stops the Processor executing commands.
        /// Commands currently in the buffer will not be executed.
        /// </summary>
        /// <returns>The commands currently in the buffer that aren't executed.</returns>
        private IEnumerable<ICommand> ForceStop()
        {
            if (!Running) return _buffer.ToList(); // Return yet unprocessed commands
            Running = false; // First mark as not running any more
            _tokenSource.Cancel(); // Then cancel
            _executor.Join(); // Then wait for thread to finish
            _tokenSource.Dispose(); // Then dispose TokenSource
            _tokenSource = null; // Let GC collect it

            return _buffer.ToList(); // Return yet unprocessed commands
        }

        /// <summary>
        /// Adds the given command to the buffer in order to execute it when
        /// all previously added commands are executed.
        /// </summary>
        /// <param name="cmd"></param>
        public void Execute(ICommand cmd)
        {
            _buffer.Add(cmd);
        }

        /// <summary>
        /// Processes the commands in the buffer while in the running state.
        /// </summary>
        private void ProcessCommands()
        {
            while (Running)
            {
                try
                {
                    _buffer.Take(_tokenSource.Token).Execute();
                }
                catch (OperationCanceledException)
                {
                    // Operation canceled => jump out of loop
                    break;
                }
                catch (Exception e)
                {
                    MessageLogger.CatchException(this, e, Severity.ERROR);
                }
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
                Stop();
                _buffer.Dispose();
            }

            Disposed = true;
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~Processor()
        {
            Dispose(false);
        }
    }
}