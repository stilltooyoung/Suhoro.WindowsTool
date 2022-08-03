using Microsoft.Extensions.DependencyInjection;
using Suhoro.WindowsTool.Core.Interfaces;
using Suhoro.WindowsTool.ShortcutKey.Interfaces;
using Suhoro.WindowsTool.ShortcutKey.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suhoro.WindowsTool.ShortcutKey.Implements
{
    public class RegisterShortcutKey : IRegisterService
    {
        public IServiceCollection Register(IServiceCollection services)
        {
            services.AddSingleton<IServiceShortcutKey,ServiceShortcutKey>();            
            services.AddTransient<VmWindow>();

            return services;
        }
    }
}
