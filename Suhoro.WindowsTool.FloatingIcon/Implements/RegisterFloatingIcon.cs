using Microsoft.Extensions.DependencyInjection;
using Suhoro.WindowsTool.Core.Interfaces;
using Suhoro.WindowsTool.FloatingIcon.ViewModels;
using Suhoro.WindowsTool.FloatingIcon.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suhoro.WindowsTool.FloatingIcon.Implements
{
    public class RegisterFloatingIcon : IRegisterService
    {
        public IServiceCollection Register(IServiceCollection services)
        {
            services.AddTransient<VmFloatingSettings>();
            services.AddTransient<WindowFloatingSettings>();
            services.AddSingleton<WindowFloating>();
            services.AddSingleton<IFloatingIcon>(provider => provider.GetService<WindowFloating>());
            
            return services;
        }
    }
}
