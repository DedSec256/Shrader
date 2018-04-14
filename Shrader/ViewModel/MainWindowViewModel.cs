using Microsoft.Win32;
using OpenTK;
using Shrader.IDE.ViewModel.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using DynamicTab;
using System.ComponentModel;
using System.IO;
using System.Windows.Documents;

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
        /// <summary>
        /// Collection of tabitems
        /// </summary>
        public ObservableCollection<TabItem> TabItems { get; set; }
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
        /// <summary>
        /// Save file changes command
        /// </summary>
        public ICommand SaveCommand { get; set; }
        #endregion

        #region Constructor
        public MainWindowViewModel(GLControl RenderCanvas, CustomDynamicTab DynamicTab)
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
                    var name = dialog.FileName;
                    AddToTabItems(name);
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
                    foreach (var name in dialog.FileNames)
                    {
                        AddToTabItems(name);
                    }
                }
            });

            RunCommand = new RelayCommand((obj) =>
            {
                // TODO: Add sending of shader files
            });

            SaveCommand = new RelayCommand((obj) =>
            {
                if (DynamicTab.SelectedItem == null)
                    return;
                var tab = DynamicTab.SelectedItem as TabItem;
                var path = tab.Header.ToString();
                using (var file = File.Open(path, FileMode.CreateNew))
                {
                    using (var writer = new StreamWriter(file))
                    {
                        var rtb = tab.Content as RichTextBox;
                        var text = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd).Text;
                        writer.Write(text);
                    }
                }                   
            });
        }

        #endregion

        #region Help methods
        private void AddToTabItems(string name)
        {
            if (TabItems.FirstOrDefault(t => t.Name == Path.GetFileNameWithoutExtension(name)) != null) return;
            TabItems.Add(CreateTabItem(name));
        }

        private static TabItem CreateTabItem(string name)
        {
            return new TabItem
            {
                Header = name,
                Name = Path.GetFileNameWithoutExtension(name)
            };
        } 

        private IEnumerable<string> GetTabFilesPath()
        {
            return from t in TabItems
                   select t.Header.ToString();
        }
        #endregion


    }
}
