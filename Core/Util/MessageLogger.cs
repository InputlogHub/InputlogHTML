using System;

namespace InputLog.Core.Util
{
    /// <summary>
    /// Represents a level of severeness of the exception.
    /// </summary>
    public enum Severity
    {
        DEBUG,
        INFO,
        WARNING,
        ERROR,
        FATAL
    }

    /// <summary>
    /// The event arguments that will be past on to the delegates
    /// that are registered to the ExceptionThrown-event.
    /// </summary>
    public class ExceptionEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="exc">The exception that has been received.</param>
        /// <param name="severity">The severity of the exception.</param>
        /// <param name="msg">An optional message.</param>
        public ExceptionEventArgs(Exception exc, Severity severity, string msg = "")
        {
            Exception = exc;
            Severity = severity;
            Message = msg;
        }

        /// <summary>
        /// The exception that has been received.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// The level of severeness of the Exception.
        /// </summary>
        public Severity Severity { get; private set; }

        /// <summary>
        /// An optional extra message.
        /// </summary>
        public string Message { get; private set; }
    }

    /// <summary>
    /// Delegate for the listeners registered to the ExceptionCaught-event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event arguments, contains the caught exception
    /// and the severity of it.</param>
    public delegate void ExceptionListener(object sender, ExceptionEventArgs e);

    /// <summary>
    /// The event arguments that will be past on to the delegates
    /// that are registered to the MessageReceived-event.
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="title">The title of the message.</param>
        /// <param name="msg">The message.</param>
        /// <param name="severity">The severity of the message.</param>
        public MessageEventArgs(string title, string msg, Severity severity)
        {
            Title = title;
            Message = msg;
            Severity = severity;
        }

        /// <summary>
        /// The title of the message.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// The message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// The level of severeness of the message.
        /// </summary>
        public Severity Severity { get; private set; }
    }

    /// <summary>
    /// Delegate for the listeners registered to the MessageReceived-event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event arguments, contains the message and the severity of it.</param>
    public delegate void MessageListener(object sender, MessageEventArgs e);

	public delegate void ErrorHandler<ErrorData>(object sender, ErrorData data);

    /// <summary>
    /// A static class used for logging.
    /// It has a method to pass on messages and a method to pass on exceptions
    /// and it has a corresponding event for each.
    /// </summary>
    public class MessageLogger
    {
        /// <summary>
        /// Event that will be raised whenever an exception was caught.
        /// </summary>
        public static event ExceptionListener ExceptionCaught;

        /// <summary>
        /// Event that will be raised whenever a message was received.
        /// </summary>
        public static event MessageListener MessageReceived;

        /// <summary>
        /// Raises the ExceptionCaught event with the given parameters.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="exc">The exception that has been caught.</param>
        /// <param name="severity">The severity.</param>
        /// <param name="msg">An optional message.</param>
        public static void CatchException(object sender, Exception exc, Severity severity, string msg = "")
        {
            if (ExceptionCaught == null) return;
            var args = new ExceptionEventArgs(exc, severity, msg);
            ExceptionCaught(sender, args);
        }

        /// <summary>
        /// Raises the MessageReceived event with the given parameters.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="title">The title of the message.</param>
        /// <param name="msg">The message.</param>
        /// <param name="severity">The severity.</param>
        public static void LogMessage(object sender, string title, string msg, Severity severity)
        {
            if (MessageReceived == null) return;
            var args = new MessageEventArgs(title, msg, severity);
            MessageReceived(sender, args);
        }
    }
}