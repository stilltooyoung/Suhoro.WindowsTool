using Suhoro.WindowsTool.Core.Interfaces;
using Suhoro.WindowsTool.Core.Utils;
using Suhoro.WindowsTool.FloatingIcon.Configs;
using Suhoro.WindowsTool.FloatingIcon.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Suhoro.WindowsTool.FloatingIcon.ViewModels
{
    public class VmFloatingSettings : INotifyPropertyChanged
    {
        private readonly IFloatingIcon floatingIcon;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand CommandChangeIcon { get; set; }
        public VmFloatingSettings(IFloatingIcon floatingIcon)
        {
            InitCommandChangeIcon();
            this.floatingIcon = floatingIcon;
        }

        void InitCommandChangeIcon()
        {
            CommandChangeIcon = new GeneralCommand<VmFloatingSettings>(this, (obj, vm) => {
                var uri=obj as Uri;
                var newName = uri.LocalPath.Substring(uri.LocalPath.LastIndexOfAny(new[] { '/', '\\' }) + 1);
                var newUri = new Uri(Path.Combine(SettingsPlugin.Default.UriResources, newName));
                File.Copy(uri.LocalPath, AppDomain.CurrentDomain.BaseDirectory+newUri.LocalPath, true);
                //var bitmap = new BitmapImage();
                //bitmap.BeginInit();
                //bitmap.StreamSource = new MemoryStream(File.ReadAllBytes(newPath));
                //bitmap.EndInit();
                //floatingIcon.FloatingIcon.Source = bitmap;
                (floatingIcon.FloatingIcon as HandyControl.Controls.GifImage).Uri = newUri;
                SettingsPlugin.Default.CustomFloatingIconName = newName;
            });
        }
    }
}
