using System;
using System.Collections.Generic;

namespace InputLog.Core.Util.KeyConversion
{
    /// <summary>
    ///     A virtual key from the keyboard.
    /// </summary>
    public class VirtualKey
    {
        /// <summary>
        ///     Constructor using a known virtual key code.
        /// </summary>
        /// <param name="hkl">Handle to the keyboard layout.</param>
        /// <param name="virtualKey">The virtual key code where to construct an instance for.</param>
        public VirtualKey(IntPtr hkl, KeysEx virtualKey) :
            this((uint) virtualKey, NativeMethods.MapVirtualKeyEx((uint) virtualKey, 0, hkl))
        {
        }

        /// <summary>
        ///     Constructor using a known scan code.
        /// </summary>
        /// <param name="hkl">Handle to the keyboard layout.</param>
        /// <param name="scanCode">The scan code where to construct an instance for.</param>
        public VirtualKey(IntPtr hkl, uint scanCode)
            : this(NativeMethods.MapVirtualKeyEx(scanCode, 1, hkl), scanCode)
        {
        }

        /// <summary>
        ///     Internal constructor, contains the actual initializing of the instance.
        /// </summary>
        /// <param name="virtualKey">The virtual key code of the instance.</param>
        /// <param name="scanCode">The corresponding scan code.</param>
        private VirtualKey(uint virtualKey, uint scanCode)
        {
            VkCode = virtualKey;
            ScanCode = scanCode;
        }

        /// <summary>
        ///     Returns the value corresponding to the given ShiftState and capslock.
        /// </summary>
        /// <param name="shiftState">
        ///     The ShiftState of the keyboard for which the corresponding
        ///     value should be returned.
        /// </param>
        /// <param name="capsLock">True if the capslock is on, false if not.</param>
        /// <returns>
        ///     The value resulting when the key for which this instance is a model for,
        ///     is pressed and the keyboard is in the given ShiftState and capslock state.
        /// </returns>
        public string GetValue(ShiftState shiftState, bool capsLock)
        {
            var result = Values[(capsLock ? 1 : 0)].ContainsKey(shiftState)
                ? Values[(capsLock ? 1 : 0)][shiftState] : "";
            return result;
        }

        /// <summary>
        ///     Returns true if the Virtual Key is a dead key under the given ShiftState
        ///     and capslock state.
        /// </summary>
        /// <param name="shiftState">The ShiftState.</param>
        /// <param name="capsLock">The state of capslock (true = on, false = of).</param>
        /// <returns></returns>
        public bool IsDead(ShiftState shiftState, bool capsLock)
        {
            return DeadKeys[(capsLock ? 1 : 0)].ContainsKey(shiftState);
        }

        /// <summary>
        ///     Sets the value resulting when the key for which this instance is a model for,
        ///     is pressed and the keyboard is in the given ShiftState and capslock state.
        /// </summary>
        /// <param name="shiftState">The ShiftState of the keyboard.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="capsLock">True if the capslock is on, false if not.</param>
        public void SetValue(ShiftState shiftState, string value, bool capsLock)
        {
            //this.DeadKeys[(capsLock ? 1 : 0)][shiftState] = isDeadKey;
            Values[(capsLock ? 1 : 0)][shiftState] = value;
        }

        /// <summary>
        ///     Sets the given DeadKey as the DeadKey resulting when the key for which
        ///     this instance is a model for, is pressed and the keyboard is in the
        ///     given ShiftState and capslock state.
        /// </summary>
        /// <param name="shiftState">The ShiftState of the keyboard.</param>
        /// <param name="capsLock">True if the capslock is on, false if not.</param>
        /// <param name="key">The DeadKey.</param>
        public void SetValue(ShiftState shiftState, bool capsLock, DeadKey key)
        {
            DeadKeys[(capsLock ? 1 : 0)][shiftState] = key;
        }

        /// <summary>
        ///     Returns the DeadKey resulting from pressing the putting while the keyboard
        ///     is in the given state or null if the resulting value would not be a dead one.
        /// </summary>
        /// <param name="shiftState">The ShiftState of the keyboard.</param>
        /// <param name="capsLock">True if the capslock is on, false if not.</param>
        public DeadKey GetDeadKey(ShiftState shiftState, bool capsLock)
        {
            DeadKey result = null;

            if (DeadKeys[(capsLock ? 1 : 0)].ContainsKey(shiftState))
            {
                result = DeadKeys[(capsLock ? 1 : 0)][shiftState];
            }

            return result;
        }

        #region Fields

        /// <summary>
        ///     The actual virtual key code.
        /// </summary>
        public uint VkCode { get; private set; }

        /// <summary>
        ///     Returns the corresponding KeysEx value.
        /// </summary>
        public KeysEx Key
        {
            get { return (KeysEx) VkCode; }
        }

        /// <summary>
        ///     The corresponding scan code.
        /// </summary>
        public uint ScanCode { get; private set; }

        /// <summary>
        ///     Contains for capslock off (index 0) and capslock on (index 1) a dictionary
        ///     that maps the DeadKeys that can obtained by pressing this key under a certain
        ///     keyboard state to that keyboard state.
        /// </summary>
        private readonly Dictionary<ShiftState, DeadKey>[] DeadKeys =
        {
            new Dictionary<ShiftState, DeadKey>((int) ShiftState.SHFT_XXXX + 1),
            new Dictionary<ShiftState, DeadKey>((int) ShiftState.SHFT_XXXX + 1)
        };

        /// <summary>
        ///     Contains for capslock off (index 0) and capslock on (index 1) a dictionary
        ///     containing for each ShiftState that matters a string representing the resulting
        ///     value when the key is pressed under that ShiftState.
        /// </summary>
        private readonly Dictionary<ShiftState, string>[] Values =
        {
            new Dictionary<ShiftState, string>((int) ShiftState.SHFT_XXXX + 1),
            new Dictionary<ShiftState, string>((int) ShiftState.SHFT_XXXX + 1)
        };

        /// <summary>
        ///     Returns true if SG is equal to caps lock for this key.
        /// </summary>
        public bool SgCaps
        {
            get
            {
                var stBase = GetValue(ShiftState.BASE, false);
                var stShift = GetValue(ShiftState.SHFT, false);
                var stCaps = GetValue(ShiftState.BASE, true);
                var stShiftCaps = GetValue(ShiftState.SHFT, true);
                return ((stCaps.Length > 0) && (!stBase.Equals(stCaps)) && (!stShift.Equals(stCaps)))
                       || ((stShiftCaps.Length > 0) && (!stBase.Equals(stShiftCaps)) && (!stShift.Equals(stShiftCaps)));
            }
        }

        /// <summary>
        ///     Returns true if the caps lock is equal to pressing the shift key for this key.
        /// </summary>
        public bool CapsEqualToShift
        {
            get
            {
                var stBase = GetValue(ShiftState.BASE, false);
                var stShift = GetValue(ShiftState.SHFT, false);
                var stCaps = GetValue(ShiftState.BASE, true);
                return (stBase.Length > 0) && (stShift.Length > 0) &&
                       (!stBase.Equals(stShift)) && (stShift.Equals(stCaps));
            }
        }

        /// <summary>
        ///     Returns true if the caps lock + Alt Gr is equal to pressing shift + Alt Gr
        /// </summary>
        public bool AltGrCapsEqualToAltGrShift
        {
            get
            {
                var stBase = GetValue(ShiftState.MENU_CTRL, false);
                var stShift = GetValue(ShiftState.SHFT_MENU_CTRL, false);
                var stCaps = GetValue(ShiftState.MENU_CTRL, true);
                return (stBase.Length > 0) && (stShift.Length > 0) &&
                       (!stBase.Equals(stShift)) && (stShift.Equals(stCaps));
            }
        }

        /// <summary>
        ///     Returns true if caps lock + Alt Gr + Xxxx is equal to pressing shift + Alt Gr + Xxxx
        /// </summary>
        public bool XxxxGrCapsEqualToXxxxShift
        {
            get
            {
                var stBase = GetValue(ShiftState.XXXX, false);
                var stShift = GetValue(ShiftState.SHFT_XXXX, false);
                var stCaps = GetValue(ShiftState.XXXX, true);
                return (
                    (stBase.Length > 0) &&
                    (stShift.Length > 0) &&
                    (!stBase.Equals(stShift)) &&
                    (stShift.Equals(stCaps)));
            }
        }

        /// <summary>
        ///     Returns true if no result is stored for any keyboard state
        ///     (combination of ShiftState and CapsLock).
        /// </summary>
        public bool Empty
        {
            get { return (Values[0].Count == 0) && (Values[1].Count == 0); }
        }

        #endregion
    }
}