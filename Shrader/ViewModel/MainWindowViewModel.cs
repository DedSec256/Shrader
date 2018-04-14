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
using Shrader.IDE.Compilation;
using ShaderBuilder.Utils;
using System;
using System.Windows.Forms.Integration;

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
                        var tab = AddToTabItems(name);
                        FilledTab(tab, name);
                    }
                }
            });

            RunCommand = new RelayCommand((obj) =>
            {
                Logger _logger = Logger.Instance;
                _logger.ClearMessages();

                //SaveInFiles(TabItems)
                //StaticShaderBuilder.RenderShader(GetTabFilesPath());

                StaticShaderBuilder.RenderShader(GetTabFilesPath());
                ErrorText = _logger.GetAllMessage();
            });

            SaveCommand = new RelayCommand((obj) =>
            {
                if (DynamicTab.SelectedItem == null)
                    return;
                var tab = DynamicTab.SelectedItem as TabItem;
                SaveInFiles(new TabItem[] { tab });                   
            });
        }

        private void FilledTab(TabItem tab, string name)
        {
            var rtb = (tab.Content as WindowsFormsHost).Child as System.Windows.Forms.RichTextBox;
            using (var file = File.Open(name, FileMode.Open))
            {
                using (var reader = new StreamReader(file))
                {
                    rtb.Text = reader.ReadToEnd();
                }
            }
        }

        #endregion

        #region Help methods
        private TabItem AddToTabItems(string name)
        {
            if (TabItems.FirstOrDefault(t => t.Name == Path.GetFileNameWithoutExtension(name)) != null) return null;
            var tab = CreateTabItem(name);
            TabItems.Add(tab);
            return tab;
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

        private void SaveInFiles(IEnumerable<TabItem> tabs)
        {
            foreach (var tab in tabs)
            {
                var path = tab.Header.ToString();
                using (var file = File.Open(path, FileMode.CreateNew))
                {
                    using (var writer = new StreamWriter(file))
                    {
                        var rtb = (tab.Content as WindowsFormsHost).Child as System.Windows.Forms.RichTextBox;
                        var text = rtb.Text;
                        writer.Write(text);
                    }
                }
            }
        }
        #endregion


    }
}
