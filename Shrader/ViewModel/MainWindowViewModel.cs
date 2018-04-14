using Microsoft.Win32;
using OpenTK;
using Shrader.IDE.ViewModel.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace Shrader.IDE.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {

        #region Properties
        /// <summary>
        /// ErrorText on Render
        /// </summary>
        public string ErrorText { get; set; }
        /// <summary>
        /// Path to file to execute
        /// </summary>
        public string FilePath { get; set; } = @"D:\default.glsl";
        /// <summary>
        /// Control for rendering
        /// </summary>
        public GLControl GLControl { get; set; }
        #endregion

        #region Commands
        /// <summary>
        /// Command for run shaders
        /// </summary>
        public ICommand RunCommand { get; set; }
        /// <summary>
        /// Create new gcls file command and add it
        /// </summary>
        public ICommand CreateFileCommand { get; set; }
        /// <summary>
        /// Add exist gcls file command
        /// </summary>
        public ICommand AddExistFileCommand { get; set; }
        #endregion

        #region Constructor
        public MainWindowViewModel(GLControl RenderCanvas, ObservableCollection<TabItem> tabItems)
        {
            GLControl = RenderCanvas;

            CreateFileCommand = new RelayCommand((obj) =>
            {
                var dialog = new SaveFileDialog
                {
                    DefaultExt = ".glsl",
                    Filter = "(*.GLSL)|*.GLSL"
                };
                if (dialog.ShowDialog() == true)
                {
                    var name = dialog.SafeFileName;
                    tabItems.Add(CreateTabItem(name));
                }
            });

            AddExistFileCommand = new RelayCommand((obj) =>
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "(*.GLSL)|*.GLSL",
                    CheckFileExists = true,
                    Multiselect = true
                };
                if (dialog.ShowDialog() == true)
                {
                    foreach (var name in dialog.SafeFileNames)
                    {
                        tabItems.Add(CreateTabItem(name));
                    }
                }
            });

            RunCommand = new RelayCommand((obj) =>
            {
                // TODO: Add sending of shader files
            });
        }

        private static TabItem CreateTabItem(string name)
        {
            return new TabItem
            {
                Header = name,
                Name = name.Remove(name.IndexOf("."))
            };
        }

        #endregion
    }
}
