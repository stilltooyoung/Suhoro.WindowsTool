using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Suhoro.WindowsTool.Core.Utils
{
    public static class CommonVariables
    {
        /// <summary>
        /// windows开机启动程序目录
        /// </summary>
        public static readonly string WindowsBootStartDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup);
        /// <summary>
        /// 应用快捷方式在windows开机启动程序目录中的完整路径
        /// </summary>
        public static readonly string ShortcutPathInWindowsBootStartDirectory = Path.Combine(WindowsBootStartDirectory, $"{ProjectName}.lnk");
        /// <summary>
        /// 应用程序的工作目录
        /// </summary>
        public static readonly string WorkingDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase??AppDomain.CurrentDomain.BaseDirectory;
        /// <summary>
        /// 应用程序的执行文件完整路径
        /// </summary>
        public static readonly string ExecuteFilePath = Process.GetCurrentProcess().MainModule?.FileName;
        /// <summary>
        /// 应用程序名
        /// </summary>
        public static readonly string ProjectName = AppDomain.CurrentDomain.FriendlyName;
        /// <summary>
        /// 应用程序的执行文件名
        /// </summary>
        public static readonly string ExecuteFileName = Path.GetFileName(ExecuteFilePath);
        /// <summary>
        /// 屏幕宽度
        /// </summary>
        public static readonly double ScreenWidth = SystemParameters.PrimaryScreenWidth;
        /// <summary>
        /// 屏幕高度
        /// </summary>
        public static readonly double ScreenHeight = SystemParameters.PrimaryScreenHeight;
        public static IEnumerable<Assembly> ProjectAssemblies = DependencyContext.Default.CompileLibraries
                .Where(lib => !lib.Serviceable && lib.Name.StartsWith(nameof(Suhoro)))
                .Select(lib => AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name)));
    }
}
