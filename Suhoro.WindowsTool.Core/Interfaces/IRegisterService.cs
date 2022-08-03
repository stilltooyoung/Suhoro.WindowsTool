using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suhoro.WindowsTool.Core.Interfaces
{
    public interface IRegisterService
    {
        IServiceCollection Register(IServiceCollection services);
    }
}
