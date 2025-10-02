using System;
using InputLog.Core.Hooks.Keyboard;
using InputLog.Core.Hooks.Mouse;
using InputLog.Core.Util;
using InputLog.Core.Util.CommandProcessing;

namespace InputLog.Core.Hooks
{
    /// <summary>
    /// Class with event that are asynchronously triggered when a keyboard event
    /// or a mouse event occurs.
    /// </summary>
    public class SystemMonitor
    {
        #region Subclasses

        #region Nested type: KeyboardTrigger

        /// <summary>
        /// Command that, when executed, triggers the KeyboardEvent
        /// with the data provided during construction.
        /// </summary>
        private class KeyboardTrigger : ICommand
        {
            /// <summary>
            /// The event data.
            /// </summary>
            private readonly KeyboardEvent _event;

            /// <summary>
            /// The sender of the event.
            /// </summary>
            private readonly object _sender;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="sender">The sender of the event.</param>
            /// <param name="e">The event data.</param>
            public KeyboardTrigger(object sender, KeyboardEvent e)
            {
                _sender = sender;
                _event = e;
            }

            #region ICommand Members

            /// <summary>
            /// Executes the command, triggering the apropriate event.
            /// </summary>
            public void Execute()
            {
                KeyboardEvent?.Invoke(_sender, _event);
            }

            #endregion
        }

        #endregion

        #region Nested type: MouseTrigger

        /// <summary>
        /// Command that, when executed, triggers the MouseEvent
        /// with the data provided during construction.
        /// </summary>
        private class MouseTrigger : ICommand
        {
            /// <summary>
            /// The event data.
            /// </summary>
            private readonly MouseEvent _event;

            /// <summary>
            /// The sender of the event.
            /// </summary>
            private readonly object _sender;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="s">The sender of the event.</param>
            /// <param name="e">The event data.</param>
            public MouseTrigger(object s, MouseEvent e)
            {
                _sender = s;
                _event = e;
            }

            #region ICommand Members

            /// <summary>
            /// Executes the command, triggering the apropriate event.
            /// </summary>
            public void Execute()
            {
                MouseEvent?.Invoke(_sender, _event);
            }

            #endregion
        }

        #endregion

        #endregion

        #region Fields

        /// <summary>
        /// Event asynchronously triggered on keyboard events.
        /// </summary>
        public static KeyboardListener KeyboardEvent;

        /// <summary>
        /// Event asynchronously triggered on mouse events.
        /// </summary>
        public static MouseListener MouseEvent;

        /// <summary>
        /// The command processor used for asynchronously triggering the events.
        /// </summary>
        private static Processor _commandProcessor;

        /// <summary>
        /// Reference to the wrapper object of the mouseHook
        /// </summary>
        private static MouseEventListener MouseListener { get; set; }

        /// <summary>
        /// Reference to the wrapper object of the keyboardhook
        /// </summary>
        private static KeyboardEventListener KeyboardListener { get; set; }

        #endregion

        /// <summary>
        /// Installs the system hooks.
        /// </summary>
        public static void Install()
        {
            if (MouseListener == null)
            {
                // First start processor
                _commandProcessor = new Processor();
                _commandProcessor.Start();

                // Then install hooks
                MouseListener = new MouseEventListener();
                KeyboardListener = new KeyboardEventListener();
                MouseListener.MouseEvent += MouseCallback;
                KeyboardListener.KeyboardEvent += KeyboardCallback;
            }
        }

        /// <summary>
        /// Uninstalls the installed hooks. Should be called whenever the program is about to be finished.
        /// </summary>
        public static void Uninstall()
        {
            // First uninstall hooks
            MouseListener.MouseEvent -= MouseCallback;
            KeyboardListener.KeyboardEvent -= KeyboardCallback;
            MouseListener.Dispose();
            KeyboardListener.Dispose();
            MouseListener = null; // Let GC collect it
            KeyboardListener = null; // Let GC collect it

            // Stop CommandProcessor (do not force stop it, we want to process every resting command in it!)
            _commandProcessor.Stop();
            _commandProcessor.Dispose();
            _commandProcessor = null; // Let GC collect it
        }

        /// <summary>
        /// Callback for mouse eventz, adds the appropriate command to the
        /// command processor.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event data.</param>
        private static void MouseCallback(object sender, MouseEvent e)
        {
            try
            {
                _commandProcessor.Execute(new MouseTrigger(sender, e));
            }
            catch (Exception ex)
            {
                MessageLogger.CatchException(null, ex, Severity.ERROR);
            }
        }

        /// <summary>
        /// Callback for keyboard eventz, adds the appropriate command to the
        /// command processor.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event data.</param>
        private static void KeyboardCallback(object sender, KeyboardEvent e)
        {
            try
            {
                _commandProcessor.Execute(new KeyboardTrigger(sender, e));
            }
            catch (Exception ex)
            {
                MessageLogger.CatchException(null, ex, Severity.ERROR);
            }
        }
    }
}