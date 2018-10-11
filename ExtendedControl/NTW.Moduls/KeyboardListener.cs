using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;
using System.Windows.Threading;

namespace NTW.Moduls {
    public class KeyboardListener : IDisposable {

        public KeyboardListener() {
            this.downCode = new List<int>();

            this.dispatcher = Dispatcher.CurrentDispatcher;

            hookedLowLevelKeyboardProc = (InterceptKeys.LowLevelKeyboardProc)LowLevelKeyboardProc;

            hookId = InterceptKeys.SetHook(hookedLowLevelKeyboardProc);

            hookedKeyboardCallbackAsync = new KeyboardCallbackAsync(KeyboardListener_KeyboardCallbackAsync);
        }

        private Dispatcher dispatcher;

        private List<int> downCode;
        private int[] secondDownCodes;

        ~KeyboardListener() {
            downCode.Clear();
            downCode = null;
            Dispose();
        }

        public event RawKeyEventHandler KeyDown;

        public event RawKeyEventHandler KeyUp;

        #region Inner workings

        public bool Look = false;
        public bool def = false;

        private IntPtr hookId = IntPtr.Zero;


        private delegate void KeyboardCallbackAsync(InterceptKeys.KeyEvent keyEvent, int vkCode, string character);

        [MethodImpl(MethodImplOptions.NoInlining)]
        private IntPtr LowLevelKeyboardProc(int nCode, UIntPtr wParam, IntPtr lParam) {
            string chars = "";

            if (nCode >= 0) {

                if (wParam.ToUInt32() == (int)InterceptKeys.KeyEvent.WM_KEYDOWN)
                    if (!downCode.Contains(Marshal.ReadInt32(lParam))) {
                        downCode.Add(Marshal.ReadInt32(lParam));
                        Console.WriteLine("-> on down (set)");
                    }

                if (wParam.ToUInt32() == (int)InterceptKeys.KeyEvent.WM_KEYDOWN ||
                    wParam.ToUInt32() == (int)InterceptKeys.KeyEvent.WM_KEYUP) {
                    chars = InterceptKeys.VKCodeToString((uint)Marshal.ReadInt32(lParam), (wParam.ToUInt32() == (int)InterceptKeys.KeyEvent.WM_KEYDOWN || wParam.ToUInt32() == (int)InterceptKeys.KeyEvent.WM_SYSKEYDOWN));
                    secondDownCodes = downCode.ToArray();
                    if (!def)
                        if (wParam.ToUInt32() == (int)InterceptKeys.KeyEvent.WM_KEYUP) {
                            downCode.Remove(Marshal.ReadInt32(lParam));
                            Console.WriteLine("-> on up (clear)");
                        }
                    hookedKeyboardCallbackAsync.Invoke((InterceptKeys.KeyEvent)wParam.ToUInt32(), Marshal.ReadInt32(lParam), chars);
                }

                if(def)
                    if (wParam.ToUInt32() == (int)InterceptKeys.KeyEvent.WM_KEYUP) {
                        downCode.Clear();
                        Console.WriteLine("-> on up (clear)");
                    }
            }
            if (Look) {
                Look = false;
                return (IntPtr)1;
            }
            else
                return InterceptKeys.CallNextHookEx(hookId, nCode, wParam, lParam);
        }

        private KeyboardCallbackAsync hookedKeyboardCallbackAsync;

        private InterceptKeys.LowLevelKeyboardProc hookedLowLevelKeyboardProc;

        void KeyboardListener_KeyboardCallbackAsync(InterceptKeys.KeyEvent keyEvent, int vkCode, string character) {
            switch (keyEvent) {
                // KeyDown events
                case InterceptKeys.KeyEvent.WM_KEYDOWN:
                    if (KeyDown != null)
                        dispatcher.Invoke(new RawKeyEventHandler(KeyDown), this, new RawKeyEventArgs(vkCode, false, character, secondDownCodes));
                    break;
                case InterceptKeys.KeyEvent.WM_SYSKEYDOWN:
                    if (KeyDown != null)
                        dispatcher.Invoke(new RawKeyEventHandler(KeyDown), this, new RawKeyEventArgs(vkCode, true, character, secondDownCodes));
                    break;

                // KeyUp events
                case InterceptKeys.KeyEvent.WM_KEYUP:
                    if (KeyUp != null)
                        dispatcher.Invoke(new RawKeyEventHandler(KeyUp), this, new RawKeyEventArgs(vkCode, false, character, secondDownCodes));
                    break;
                case InterceptKeys.KeyEvent.WM_SYSKEYUP:
                    if (KeyUp != null)
                        dispatcher.Invoke(new RawKeyEventHandler(KeyUp), this, new RawKeyEventArgs(vkCode, true, character, secondDownCodes));
                    break;

                default:
                    break;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose() {
            InterceptKeys.UnhookWindowsHookEx(hookId);
        }

        #endregion
    }

    public class RawKeyEventArgs : EventArgs {
        public int VKCode;

        public Key Key;

        public int[] VKCodes;

        public Key[] Keys;

        public bool IsSysKey;

        public override string ToString() {
            return Character;
        }

        public string Character;

        public RawKeyEventArgs(int VKCode, bool isSysKey, string Character) {
            this.VKCode = VKCode;
            this.IsSysKey = isSysKey;
            this.Character = Character;
            this.Key = System.Windows.Input.KeyInterop.KeyFromVirtualKey(VKCode);
        }

        public RawKeyEventArgs(int VKCode, bool isSysKey, string Character, int[] downCode)
            : this(VKCode, isSysKey, Character) {
            VKCodes = downCode;
            Keys = new Key[downCode.Length];
            for (int i = 0; i < Keys.Length; i++) {
                Keys[i] = System.Windows.Input.KeyInterop.KeyFromVirtualKey(downCode[i]);
            }
        }
    }

    public delegate void RawKeyEventHandler(object sender, RawKeyEventArgs args);

    #region WINAPI Helper class
    internal static class InterceptKeys {
        public delegate IntPtr LowLevelKeyboardProc(int nCode, UIntPtr wParam, IntPtr lParam);
        public static int WH_KEYBOARD_LL = 13;

        public enum KeyEvent : int {
            WM_KEYDOWN = 256,

            WM_KEYUP = 257,

            WM_SYSKEYUP = 261,

            WM_SYSKEYDOWN = 260
        }

        public static IntPtr SetHook(LowLevelKeyboardProc proc) {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule) {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, UIntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        #region Convert VKCode to string

        [DllImport("user32.dll")]
        private static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);

        [DllImport("user32.dll")]
        private static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        private static extern uint MapVirtualKeyEx(uint uCode, uint uMapType, IntPtr dwhkl);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetKeyboardLayout(uint dwLayout);

        [DllImport("User32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("User32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        private static uint lastVKCode = 0;
        private static uint lastScanCode = 0;
        private static byte[] lastKeyState = new byte[255];
        private static bool lastIsDead = false;

        public static string VKCodeToString(uint VKCode, bool isKeyDown) {
            System.Text.StringBuilder sbString = new System.Text.StringBuilder(5);

            byte[] bKeyState = new byte[255];
            bool bKeyStateStatus;
            bool isDead = false;

            IntPtr currentHWnd = GetForegroundWindow();
            uint currentProcessID;
            uint currentWindowThreadID = GetWindowThreadProcessId(currentHWnd, out currentProcessID);

            uint thisProgramThreadId = GetCurrentThreadId();

            if (AttachThreadInput(thisProgramThreadId, currentWindowThreadID, true)) {
                bKeyStateStatus = GetKeyboardState(bKeyState);

                AttachThreadInput(thisProgramThreadId, currentWindowThreadID, false);
            }
            else {
                bKeyStateStatus = GetKeyboardState(bKeyState);
            }

            if (!bKeyStateStatus)
                return "";

            IntPtr HKL = GetKeyboardLayout(currentWindowThreadID);

            uint lScanCode = MapVirtualKeyEx(VKCode, 0, HKL);

            if (!isKeyDown)
                return "";

            int relevantKeyCountInBuffer = ToUnicodeEx(VKCode, lScanCode, bKeyState, sbString, sbString.Capacity, (uint)0, HKL);

            string ret = "";

            switch (relevantKeyCountInBuffer) {
                case -1:
                    isDead = true;

                    ClearKeyboardBuffer(VKCode, lScanCode, HKL);
                    break;

                case 0:
                    break;

                case 1:
                    ret = sbString[0].ToString();
                    break;

                case 2:
                default:
                    ret = sbString.ToString().Substring(0, 2);
                    break;
            }

            if (lastVKCode != 0 && lastIsDead) {
                System.Text.StringBuilder sbTemp = new System.Text.StringBuilder(5);
                ToUnicodeEx(lastVKCode, lastScanCode, lastKeyState, sbTemp, sbTemp.Capacity, (uint)0, HKL);
                lastVKCode = 0;

                return ret;
            }

            lastScanCode = lScanCode;
            lastVKCode = VKCode;
            lastIsDead = isDead;
            lastKeyState = (byte[])bKeyState.Clone();

            return ret;
        }

        private static void ClearKeyboardBuffer(uint vk, uint sc, IntPtr hkl) {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(10);

            int rc;
            do {
                byte[] lpKeyStateNull = new Byte[255];
                rc = ToUnicodeEx(vk, sc, lpKeyStateNull, sb, sb.Capacity, 0, hkl);
            } while (rc < 0);
        }
        #endregion
    }
    #endregion
}
