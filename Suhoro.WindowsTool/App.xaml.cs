using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using Suhoro.WindowsTool.Core.Interfaces;
using Suhoro.WindowsTool.Core.Models;
using Suhoro.WindowsTool.Core.Utils;
using Suhoro.WindowsTool.Implements;
using Suhoro.WindowsTool.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using AutoMapper;
using Suhoro.WindowsTool.Configs;
using System.Windows.Media.Imaging;

namespace Suhoro.WindowsTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        private IConfiguration config;
        private Mutex mutex;
        public ServiceProvider Services;
        public IEnumerable<IPlugin> Plugins { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            ConfigureExceptionHandles();

            //限制程序单实例运行
            mutex = new Mutex(true, CommonVariables.ProjectName, out var createdNew);
            if (!createdNew)
            {
                Shutdown();
                return;
            }

            //注册服务
            BuildService(e.Args);

            //初始化表
            using (var localDb = Services.GetRequiredService<LocalDb>())
            {
                var databaseCreator = localDb.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
                databaseCreator.EnsureCreated();
                //databaseCreator.CreateTables();
            }

            //启动主窗口
            Services.GetRequiredService<MainWindow>();
            //生成系统托盘图标
            Services.GetService<ITrayIcon>();
            //启动插件
            Plugins = Services.GetServices<IPlugin>();
            foreach (var plugin in Plugins)
            {
                plugin.Init();
                plugin.Enable();
            }
            //floatingIcon
            var floatingIcon=Services.GetService<IFloatingIcon>();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            foreach (var plugin in Plugins)
            {
                plugin?.Disable();
                plugin?.Exit();
            }
            DispatcherUnhandledException -= App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            Services?.Dispose();
            SettingsApp.Default.Save();
            mutex?.Close();
            LogManager.Shutdown();
            base.OnExit(e);
        }

        #region 异常
        private void ConfigureExceptionHandles()
        {
            //UI线程异常
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            //非UI线程异常
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }
        private void ExceptionHandle(Exception exception)
        {
            if (exception is UiException ue)
            {
                HandyControl.Controls.MessageBox.Show(ue.UiMessage ?? ue.Message);
            }
            else
            {
                HandyControl.Controls.MessageBox.Show(exception.Message);
            }
            logger.Error(exception,exception.Message);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex=e.ExceptionObject as Exception;
            if (e.IsTerminating)
            {
                HandyControl.Controls.MessageBox.Show($"致命错误，即将退出。详情：{ex.Message}");
            }
            else
            {
                ExceptionHandle(ex);
            }
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ExceptionHandle(e.Exception);
        }
        #endregion

        #region 服务管理
        private void BuildService(string[] args)
        {
            config = new ConfigurationBuilder()
                            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                            .AddJsonFile("Configs/appsettings.json", optional: true, reloadOnChange: true)
                            .AddCommandLine(args)
                            .Build();
            var serviceCollection = new ServiceCollection();
            RegisterNormalServices(serviceCollection);
            serviceCollection.AddSingleton<IConfiguration>(config);
            serviceCollection.AddSingleton<ITrayIcon, TrayIcon>();
            
            serviceCollection.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
                loggingBuilder.AddNLog(config);
            });
            serviceCollection.AddDbContext<LocalDb>(builder => {
                builder.UseSqlite(config.GetConnectionString("LocalDb"));
            }, ServiceLifetime.Transient);

            serviceCollection.AddAutoMapper(CommonVariables.ProjectAssemblies);
            Services = serviceCollection.BuildServiceProvider();
        }
        private void RegisterNormalServices(ServiceCollection serviceCollection)
        {
            //var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var assemblies = CommonVariables.ProjectAssemblies;
            
            var types = assemblies.SelectMany(a => a.GetTypes());
            foreach (var type in types)
            {
                if (type.IsClass && !type.IsAbstract && type.IsAssignableTo(typeof(IRegisterService)))
                {
                    var configureService = Activator.CreateInstance(type) as IRegisterService;
                    configureService!.Register(serviceCollection);
                }
                else if (type.IsSubclassOf(typeof(Window)))
                {
                    if (!serviceCollection.Any(d => d.ServiceType == type))
                    {
                        serviceCollection.AddTransient(type);
                    }
                }
                else if (type.IsClass && !type.IsAbstract&&type.IsAssignableTo(typeof(IPlugin)))
                {
                    if (!serviceCollection.Any(d => d.ServiceType == typeof(IPlugin)&&d.ImplementationType==type))
                    {
                        serviceCollection.AddSingleton(typeof(IPlugin),type);
                    }
                }
                else if (type.IsClass&&!type.IsAbstract&&type.IsAssignableTo(typeof(IUserVisibleCommand)))
                {
                    serviceCollection.AddSingleton(typeof(IUserVisibleCommand), type);
                }
            }
        }

        #endregion
    }
}
