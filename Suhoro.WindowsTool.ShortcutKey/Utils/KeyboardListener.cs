using Suhoro.WindowsTool.ShortcutKey.Configs;
using Suhoro.WindowsTool.ShortcutKey.Consts;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Suhoro.WindowsTool.ShortcutKey.Utils
{
    public class KeyboardListener : IDisposable
    {
        bool isDisposed = false;
        IntPtr hookId = IntPtr.Zero;
        delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);
        /// <summary>
        /// 必须要显示定义回调函数入口，否则会被垃圾回收？
        /// </summary>
        HookProc hookProc;
        /// <summary>
        ///  键盘状态，记录KeyDown事件是否被传递，若KeyDown传递，则KeyUp也应该被传递
        /// </summary>
        bool[] keyStates=new bool[Enum.GetValues<Key>().Count()];

        private static readonly Lazy<KeyboardListener> instance = new Lazy<KeyboardListener>(() => new KeyboardListener());
        public static KeyboardListener Instance => instance.Value;
        /// <summary>
        /// 是否传递键盘事件
        /// </summary>
        public bool ShouldPassKeyboard { get; set; } = true;

        public EventHandler<HotKeyEventArgs> KeyboardEventHandler { get; set; }

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetWindowsHookEx(HookType hookType, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private KeyboardListener()
        {
            hookProc = new HookProc(CallBack);
            hookId = SetWindowsHookEx(
                    HookType.WH_KEYBOARD_LL,
                    hookProc,
                    GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName),
                    0);
        }


        IntPtr CallBack(int nCode, IntPtr wParam, IntPtr lParam)
        {
            var vkCode = Marshal.ReadInt32(lParam);
            var key = KeyInterop.KeyFromVirtualKey(vkCode);
            var keyEvent = (KeyboardEvent)wParam.ToInt32();
            var args = new HotKeyEventArgs
            {
                Key = key,
                KeyEvent = keyEvent,
                ShouldCallNext = true
            };
            try
            {
                KeyboardEventHandler?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(() => { HandyControl.Controls.MessageBox.Show(ex.Message); });
            }

            if (keyEvent == KeyboardEvent.KEYDOWN || keyEvent == KeyboardEvent.SYSKEYDOWN)
            {
                if (args.ShouldCallNext&&ShouldPassKeyboard)
                {
                    keyStates[(int)key] = true;
                    return CallNextHookEx(hookId, nCode, wParam, lParam);
                }
                else
                {
                    keyStates[(int)key] = false;
                }
            }
            else
            {
                if (keyStates[(int)key])
                {
                    return CallNextHookEx(hookId, nCode, wParam, lParam);
                }
                keyStates[(int)key] = false;
            }
            return (IntPtr)1;
        }

        private void Dispose(bool isDisposing)
        {
            if (isDisposed)
                return;
            if (isDisposing)
            {
                KeyboardEventHandler = null;
            }
            if (hookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hookId);
            }
            isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~KeyboardListener()
        {
            Dispose(false);
        }
    }

    public class HotKeyEventArgs : EventArgs
    {
        public Key Key { get; internal set; }
        public KeyboardEvent KeyEvent { get; internal set; }

        public bool ShouldCallNext { get; internal set; }
    }
}
