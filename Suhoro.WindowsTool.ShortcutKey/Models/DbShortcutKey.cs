using Microsoft.EntityFrameworkCore;
using Suhoro.WindowsTool.Core.Interfaces;
using Suhoro.WindowsTool.Core.Models;
using Suhoro.WindowsTool.ShortcutKey.Consts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Suhoro.WindowsTool.ShortcutKey.Models
{
    public class DbShortcutKey:DbEntity
    {
        public bool IsEnable { get; set; } = true;
        public string Name { get; set; }
        public string Description { get; set; } = String.Empty;
        /// <summary>
        /// 监听键位键值（每3位为一个键值，如“001”）
        /// </summary>
        public string ListenKeyCodes { get; set; }
        /// <summary>
        /// 快捷键类型
        /// </summary>
        public ShortcutKeyType Type { get; set; }
        /// <summary>
        /// 映射
        /// Type=Key时，Mapping为关联的实体键位（每3位为一个键值，如“001”）
        /// Type=Cmd时，Mapping为关联的cmd命令
        /// Type=Command时，Mapping为关联的程序命令类的类名
        /// </summary>
        public string? Mapping { get; set; }
        /// <summary>
        /// 额外参数
        /// </summary>
        public string? Args { get; set; }
        /// <summary>
        /// 是否一直生效
        /// </summary>
        public bool IsAlwaysEffect { get; set; }

        /// <summary>
        /// 关联的程序命令
        /// </summary>
        [NotMapped]
        public IUserVisibleCommand? Command { get; set; }
    }
}
