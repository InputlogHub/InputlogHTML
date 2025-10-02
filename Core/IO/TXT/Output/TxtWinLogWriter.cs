using System.IO;
using System.Windows.Forms;
using InputLog.Core.Events.WinLog;

namespace InputLog.Core.IO.Txt.Output
{
    public class TxtWinLogWriter
    {
        /// <summary>
        ///     Write the given EventPart.
        /// </summary>
        /// <param name="e">The EventPart to write.</param>
        /// <param name="writer"></param>
        public static void WriteClick(Click e, StreamWriter writer)
        {
            writer.WriteLine("  CORE  MOUSE CLICK x: {0}, y: {1}, "
                             + "startTime: {2}, endTime: {3}, button: {4}",
                e.X, e.Y, e.StartTime, e.EndTime, e.Button);
        }

        /// <summary>
        ///     Write the given EventPart.
        /// </summary>
        /// <param name="e">The EventPart to write.</param>
        /// <param name="writer"></param>
        public static void WriteFocusChange(FocusChange e, StreamWriter writer)
        {
            // Error: "Index (zero based) must be greater than or equal to zero 
            // and less than the size of the argument list", when including StartTime & EndTime here.
            //Writer.WriteLine("  CORE  FOCUSCHANGE title: {0}" + "startTime: {2}, endTime: {3}",
            //                 e.WindowTitle, e.StartTime, e.EndTime);
            writer.WriteLine("  CORE  FOCUSCHANGE title: {0}",
                e.WindowTitle);
        }

        /// <summary>
        ///     Write the given EventPart.
        /// </summary>
        /// <param name="e">The EventPart to write.</param>
        /// <param name="writer"></param>
        public static void WriteKeyPress(KeyPress e, StreamWriter writer)
        {
            writer.WriteLine("  CORE  KEYBOARD key: {0}, char {1}, "
                             + "startTime: {2}, endTime: {3}",
                e.Key, e.Value, e.StartTime, e.EndTime);

            writer.Write("    Keyboardstate:");
            foreach (var keysEx in e.KeyboardState)
            {
                var key = (Keys) keysEx;
                writer.Write(" " + key);
            }
            writer.WriteLine();
        }

        /// <summary>
        ///     Write the given EventPart.
        /// </summary>
        /// <param name="e">The EventPart to write.</param>
        /// <param name="writer"></param>
        public static void WriteMouseMovement(MouseMovement e, StreamWriter writer)
        {
            writer.WriteLine("  CORE  MOUSE MOVE x: {0}, y: {1}, "
                             + "startTime: {2}, endTime: {3}",
                e.X, e.Y, e.StartTime, e.EndTime);
        }

        /// <summary>
        ///     Write the given EventPart.
        /// </summary>
        /// <param name="e">The EventPart to write.</param>
        /// <param name="writer"></param>
        public static void WriteScroll(Scroll e, StreamWriter writer)
        {
            writer.WriteLine("  CORE  MOUSE SCROLL x: {0}, y: {1}, "
                             + "startTime: {2}, endTime: {3}, orientation: {4}, delta: {5}",
                e.X, e.Y, e.StartTime, e.EndTime, e.Orientation, e.Delta);
        }
    }
}