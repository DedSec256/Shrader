using Microsoft.Win32;
using OpenTK;
using Shrader.IDE.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public MainWindowViewModel(GLControl RenderCanvas)
        {
            GLControl = RenderCanvas;

            CreateFileCommand = new RelayCommand((obj) =>
            {
                var dialog = new SaveFileDialog();
                dialog.DefaultExt = ".glsl";
                dialog.Filter = "(*.GLSL)|*.GLSL";
                if (dialog.ShowDialog() == true)
                {
                    //TODO: Add tabs create loop
                }
            });

            AddExistFileCommand = new RelayCommand((obj) =>
            {
                var dialog = new OpenFileDialog();
                dialog.Filter = "(*.GLSL)|*.GLSL";
                dialog.CheckFileExists = true;
                dialog.Multiselect = true;
                if (dialog.ShowDialog() == true)
                {
                    //TODO: Add tabs create loop
                }
            });
        }

        #endregion
    }
}
