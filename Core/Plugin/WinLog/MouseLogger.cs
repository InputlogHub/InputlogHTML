using System.Collections.Generic;
using InputLog.Core.Events.WinLog;
using InputLog.Core.Hooks.Keyboard;
using InputLog.Core.Hooks.Mouse;
using InputLog.Core.Util;
using InputLog.Core.Util.KeyConversion;

namespace InputLog.Core.Plugin.WinLog
{
    /// <summary>
    /// WinLogger for mouse events.
    /// </summary>
    public class MouseLogger : AbstractPlugin
    {
        #region Fields

        /// <summary>
        /// Contains the mouse buttons currently pressed and not yet released and
        /// maps them to their alias and their event part, waiting for their endtime to be set.
        /// </summary>
        private readonly IDictionary<MouseMessages, Pair<string, AbstractMouseEvent>> MouseState
            = new Dictionary<MouseMessages, Pair<string, AbstractMouseEvent>>();

        /// <summary>
        /// When the user checked the restricted write button in the GUI options, 
        /// keyboard logging outside the main word document will not be recorded.
        /// </summary>
        public bool IsRestricted { set; private get; }
        #endregion

        /// <summary>
        /// Callback for mouse events.
        /// </summary>
        /// <param name="sender">The originater of the event.</param>
        /// <param name="me">mouse event</param>
        /// <param name="alias">The alias corresponding to the event.</param>
        /// <param name="kbState">The keyboard state.</param>
        private void MouseCallback(object sender, MouseEvent me, string alias, ICollection<KeysEx> kbState)
        {
            lock (MouseState)
            {
                FlushAccumulationEvents(
                    me.Type != MouseMessages.WM_MOUSEMOVE,
                    me.Type != MouseMessages.WM_MOUSEWHEEL,
                    me.Type != MouseMessages.WM_MOUSEHWHEEL);

                var key = me.Type;
                switch (key)
                {
                        // First treat all button downs
                    case MouseMessages.WM_LBUTTONDOWN:
                    case MouseMessages.WM_MBUTTONDOWN:
                    case MouseMessages.WM_RBUTTONDOWN:
                    case MouseMessages.WM_XBUTTONDOWN:
                        FlushEvent(true, key); // If button was already down, flush old event
                        MouseState.Add(key, new Pair<string, AbstractMouseEvent>(alias, 
                            new Click(me.Point.x, me.Point.y, key, me.Time, me.Time)));

                        // We are not done processing this event, we're waiting on the corresponding
                        // WM_*BUTTONUP message => DO NOT call EndEvent yet
                        break;

                    case MouseMessages.WM_LBUTTONUP:
                    case MouseMessages.WM_MBUTTONUP:
                    case MouseMessages.WM_RBUTTONUP:
                    case MouseMessages.WM_XBUTTONUP:
                        HandleButtonUp(me);
                        SysLog.EndEvent(alias); // We're done processing this event
                        break;

                    case MouseMessages.WM_MOUSEMOVE:
                        if (MouseState.ContainsKey(key))
                        {
                            // If event already exists, accumulate
                            var prevEndTime = MouseState[key].Second.EndTime;
                            if (Settings.WinLogMouseMovementPauseThreshold > 0 && me.Time > prevEndTime &&
                                ((me.Time - prevEndTime) > Settings.WinLogMouseMovementPauseThreshold))
                            {
                                // the time between the 2 events exceeds the threshold
                                // flush the previous event, and add a new event
                                FlushEvent(true, key);
                                MouseState.Add(key, new Pair<string, AbstractMouseEvent>(alias, 
                                    new MouseMovement(me.Point.x, me.Point.y, me.Time, me.Time)));
                            }
                            else
                            {
                                MouseState[key].Second.EndTime = me.Time;
                                MouseState[key].Second.X = me.Point.x;
                                MouseState[key].Second.Y = me.Point.y;
                                SysLog.EndEvent(alias); // We are done processing this event
                            }
                        }
                        else
                        {
                            // No event yet, create one
                            MouseState.Add(key, new Pair<string, AbstractMouseEvent>(alias, 
                                new MouseMovement(me.Point.x, me.Point.y, me.Time, me.Time)));

                            // We are not done processing this event, we're waiting for possible extra
                            // data that needs to be accumulated => DO NOT call EndEvent yet
                        }
                        break;

                    case MouseMessages.WM_MOUSEWHEEL:
                    case MouseMessages.WM_MOUSEHWHEEL:
                        if (MouseState.ContainsKey(key))
                        {
                            // If event alaready exists, accumulate
                            MouseState[key].Second.EndTime = me.Time;
                            ((Scroll) MouseState[key].Second).Delta += me.WheelDelta;
                            SysLog.EndEvent(alias);
                        }
                        else
                        {
                            // No event yet, create one
                            MouseState.Add(key, new Pair<string, AbstractMouseEvent>(alias,
                                new Scroll(me.Point.x, me.Point.y, me.WheelDelta, key, me.Time, me.Time)));

                            // We are not done processing this event, we're waiting for possible extra
                            // data that needs to be accumulated => DO NOT call EndEvent yet
                        }
                        break;

                    default:
                        SysLog.EndEvent(alias); // We don't know the event, still need to end it though!
                        break;
                }
            }
        }

        /// <summary>
        /// Helper function to deal with WM_*BUTTONUP messages.
        /// </summary>
        /// <param name="e">The event data.</param>
        private void HandleButtonUp(MouseEvent e)
        {
            // Set key to corresponding WM_*BUTTONDWON
            MouseMessages key;
            switch (e.Type)
            {
                case MouseMessages.WM_LBUTTONUP:
                    key = MouseMessages.WM_LBUTTONDOWN;
                    break;
                case MouseMessages.WM_MBUTTONUP:
                    key = MouseMessages.WM_MBUTTONDOWN;
                    break;
                case MouseMessages.WM_RBUTTONUP:
                    key = MouseMessages.WM_RBUTTONDOWN;
                    break;
                case MouseMessages.WM_XBUTTONUP:
                    key = MouseMessages.WM_XBUTTONDOWN;
                    break;
                default:
                    key = 0;
                    break;
            }

            // Set end time and flush
            if (!MouseState.ContainsKey(key)) return;
            MouseState[key].Second.EndTime = e.Time;
            FlushEvent(true, key);
        }

        /// <summary>
        /// Callback for keyboard eventz, just flushes any accumulation eventz
        /// (mouse movements or wheel scrolling).
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        /// <param name="unicode">The unicode representation of the key (if there is one), taking into account
        /// previously pressed dead keys. (A maximum of one dead key is taken into account, ie chaining
        /// of dead keys is not supported.)</param>
        /// <param name="alias">The alias corresponding to the event.</param>
        /// <param name="kbState">The keyboard state.</param>
        private void KeyboardCallback(object sender, KeyboardEvent e, string unicode,
                                      string alias, ICollection<KeysEx> kbState)
        {
            lock (MouseState)
            {
                try
                {
                    FlushAccumulationEvents();
                }
                finally
                {
                    SysLog.EndEvent(alias);
                }
            }
        }

        /// <summary>
        /// Ends the different accumulated mouse eventz (mouse movement & mouse wheel scrolling)
        /// if their corresponding parameters are true.
        /// </summary>
        /// <param name="endMovement">If true, flush the current movement accumulation to Log.</param>
        /// <param name="endWheel">If true, flush the current wheel accumulation to Log.</param>
        /// <param name="endHwheel">If true, flush the current hwheel accumulation to Log.</param>
        private void FlushAccumulationEvents(bool endMovement, bool endWheel, bool endHwheel)
        {
            FlushEvent(endMovement, MouseMessages.WM_MOUSEMOVE);
            FlushEvent(endWheel, MouseMessages.WM_MOUSEWHEEL);
            FlushEvent(endHwheel, MouseMessages.WM_MOUSEHWHEEL);
        }

        /// <summary>
        /// Shorthand for this.FlushAccumulationEvents(true, true, true).
        /// </summary>
        private void FlushAccumulationEvents()
        {
            FlushAccumulationEvents(true, true, true);
        }

        /// <summary>
        /// Flushes the given mouse message to Log if end equals true and if
        /// MouseState contains a pair for the given MouseMessage.
        /// If the event was flushed, it will be removed from MouseState.
        /// </summary>
        /// <param name="end">True if the event should be flushed, false if not.</param>
        /// <param name="type"></param>
        private void FlushEvent(bool end, MouseMessages type)
        {
            if (end && MouseState.ContainsKey(type))
            {
                var pair = MouseState[type];
                if(IsRestricted) SysLog.RestrictedWrite(pair.Second, pair.First);
                else SysLog.Write(pair.Second, pair.First);
                SysLog.EndEvent(pair.First);
                MouseState.Remove(type);
            }
        }

        /// <summary>
        /// Launches the MouseLogger, logging mouse input from now on.
        /// </summary>
        protected override void Start()
        {
            SysLog.KeyboardEvent += KeyboardCallback;
            SysLog.MouseEvent += MouseCallback;
        }

        /// <summary>
        /// Terminates the MouseLogger.
        /// </summary>
        protected override void Stop()
        {
            SysLog.KeyboardEvent -= KeyboardCallback;
            SysLog.MouseEvent -= MouseCallback;
        }
    }
}