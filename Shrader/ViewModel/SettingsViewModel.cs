using Microsoft.Win32;
using Shrader.IDE.Model;
using Shrader.IDE.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Shrader.IDE.ViewModel
{
    public class SettingsViewModel : BaseViewModel
    {
        public bool IsTime { get; set; }
        public bool IsMouse { get; set; }
        public bool IsViewPort { get; set; }
        public ObservableCollection<string> ImagesPath { get; set; } = new ObservableCollection<string>();

        public ICommand AddImageCommand { get; set; }
        public ICommand SubmitCommand { get; set; }

        public SettingsViewModel(SettingModel settingModel, Action close)
        {
            AddImageCommand = new RelayCommand((obj) =>
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "Image Files(*.BMP; *.JPG; *.GIF) | *.BMP; *.JPG; *.GIF | All files(*.*) | *.*",
                    CheckFileExists = true,
                    Multiselect = true
                };
                if (dialog.ShowDialog() == true)
                {
                    foreach (var name in dialog.FileNames)
                    {
                        ImagesPath.Add(name);
                    }
                }
            });

            SubmitCommand = new RelayCommand((obj) =>
            {
                settingModel.IsTime = IsTime;
                settingModel.IsMouse = IsMouse;
                settingModel.IsViewPort = IsViewPort;
                settingModel.ImagesPath = ImagesPath;
                close();
            });
        }
    }
}
