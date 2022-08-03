using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suhoro.WindowsTool.Core.Interfaces
{
    public interface IDbEntity
    {
        Guid Id { get; set; }
        DateTime CreateTime { get; set; }
        DateTime UpdateTime { get; set; }
        bool IsDeleted { get; set; }
    }
}
