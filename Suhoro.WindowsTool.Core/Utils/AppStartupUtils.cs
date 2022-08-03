using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Suhoro.WindowsTool.Core.Utils
{
    public static class AppStartupUtils
    {
        public static string WindowsTaskNameForBootStart => $"{CommonVariables.ProjectName}_BootStart";

        public static bool RestartAppWithAdministrator()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = CommonVariables.WorkingDirectory;
            startInfo.FileName = CommonVariables.ExecuteFilePath;
            startInfo.Verb = "runas";
            try
            {
                Process.Start(startInfo);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 开机自启-启用-windows计划任务
        /// </summary>
        public static bool EnableBootStartByWindowsTask()
        {
            try
            {
                using TaskService ts = new TaskService();
                using Microsoft.Win32.TaskScheduler.Task task = ts.GetTask(WindowsTaskNameForBootStart);
                if (task == null)
                {
                    using TaskDefinition td = ts.NewTask();
                    td.RegistrationInfo.Description = WindowsTaskNameForBootStart;
                    td.Principal.RunLevel = TaskRunLevel.Highest;
                    td.Triggers.Add(new LogonTrigger { Delay=TimeSpan.FromSeconds(10)});
                    td.Actions.Add(new ExecAction(CommonVariables.ExecuteFilePath, null, CommonVariables.WorkingDirectory));
                    var t = ts.RootFolder.RegisterTaskDefinition(WindowsTaskNameForBootStart, td);
                    t.Enabled = true;

                }
                else
                {
                    task.Enabled = true;
                } 
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 开机自启-禁用-windows计划任务
        /// </summary>
        public static bool DisableBootStartByWindowsTask()
        {
            try
            {
                using TaskService ts = new TaskService();
                using Microsoft.Win32.TaskScheduler.Task task = ts.GetTask(WindowsTaskNameForBootStart);
                if (task != null)
                {
                    task.Enabled = false;
                }
            }
            catch (Exception)
            {
                return false;
            }
           
            return true;
        }

        /// <summary>
        /// 开机自启-启用-开机启动程序目录中的快捷方式
        /// </summary>
        /// <returns></returns>
        public static bool EnableBootStartByShortcut()
        {
            try
            {
                if (!Directory.Exists(CommonVariables.WindowsBootStartDirectory))
                {
                    Directory.CreateDirectory(CommonVariables.WindowsBootStartDirectory);
                }
                var shellType = Type.GetTypeFromProgID("WScript.Shell");
                dynamic shell = Activator.CreateInstance(shellType);
                var shortcut = shell.CreateShortcut(CommonVariables.ShortcutPathInWindowsBootStartDirectory);
                shortcut.TargetPath = CommonVariables.ExecuteFilePath;
                shortcut.WorkingDirectory = CommonVariables.WorkingDirectory;
                shortcut.Save();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 开机自启-禁用-开机启动程序目录中的快捷方式
        /// </summary>
        /// <returns></returns>
        public static bool DisableBootStartByShortcut()
        {
            try
            {
                if (Directory.Exists(CommonVariables.WindowsBootStartDirectory))
                {
                    File.Delete(CommonVariables.ShortcutPathInWindowsBootStartDirectory);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
