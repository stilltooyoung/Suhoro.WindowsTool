using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Suhoro.WindowsTool.ShortcutKey.Utils
{
    public static class KeyExtensions
    {
        public static string ToCode(this Key key)
        {
            return $"{(int)key:000}";
        }
        public static string ToCodes(this IEnumerable<Key> keys)
        {
            return string.Concat(keys.Select(k => k.ToCode()));
        }

        public static IEnumerable<Key> ToKeys(this string s)
        {
            if (string.IsNullOrEmpty(s))
                yield break;
            for (int i = 0; i < s.Length; i += 3)
            {
                var range = new Range(i, i + 3);
                yield return (Key)int.Parse(s[range]);
            }
        }

        public static string ToKeysDisplay(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            return string.Join("+", s.ToKeys().Select(x => x.ToString()));
        }
    }
}
