using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Suhoro.WindowsTool.Core.Interfaces
{
    public interface IUserVisibleCommand:ICommand
    {
        string Name { get; set; }
        string Description { get; set; }
    }
}
