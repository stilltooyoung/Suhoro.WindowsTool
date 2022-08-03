using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Suhoro.WindowsTool.ShortcutKey.Utils
{
    public class KeyboardImitator
    {
        private static Lazy<KeyboardImitator> instance=new Lazy<KeyboardImitator>(() => new KeyboardImitator());

        public static KeyboardImitator Instance => instance.Value;

        const uint KEYEVENTF_KEYDOWN = 0;
        const uint KEYEVENTF_KEYUP = 0x0002;

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        public void KeyDown(Key key)
        {
            var vk= KeyInterop.VirtualKeyFromKey(key);
            keybd_event((byte)vk, 0, KEYEVENTF_KEYDOWN, 0);
        }

        public void KeyUp(Key key)
        {
            var vk = KeyInterop.VirtualKeyFromKey(key);
            keybd_event((byte)vk, 0, KEYEVENTF_KEYUP, 0);
        }

        public void KeysDownUp(IEnumerable<Key> keys)
        {
            foreach (var key in keys)
            {
                KeyDown(key);
                KeyUp(key);
            }
        }
    }
}
