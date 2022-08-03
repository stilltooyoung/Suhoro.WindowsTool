using Suhoro.WindowsTool.ShortcutKey.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suhoro.WindowsTool.ShortcutKey.Interfaces
{
    public interface IServiceShortcutKey
    {
        IQueryable<DbShortcutKey> GetValids();
        IQueryable<DbShortcutKey> GetEnables();
        void SaveData(List<DbShortcutKey> toSyncList);
        void AddRange(IEnumerable<DbShortcutKey> keys);
        void Enable();
        void Disable();
        void ReloadHotKey();
    }
}
