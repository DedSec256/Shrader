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
using Shrader.IDE.Model;
using Shrader.IDE.Tools;
using Shrader.IDE.View;
using Shrader.IDE.Tools.VideoSaver;

namespace Shrader.IDE.ViewModel
{
    public class MainPageViewModel : BaseViewModel
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
        /// <summary>
        /// Model for binding settings
        /// </summary>
        public SettingModel SettingModel { get;
            set; } = new SettingModel();
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
        /// <summary>
        /// Open settings window command
        /// </summary>
        public ICommand OpenSettingsCommand { get; set; }
        /// <summary>
        /// Stop render command
        /// </summary>
        public ICommand StopCommand { get; set; }
        /// <summary>
        /// Command which ellaps fullscreen mode
        /// </summary>
        public ICommand FullscreenCommand { get; set; }
        /// <summary>
        /// Start recording video command
        /// </summary>
        public ICommand StartRecordCommand { get; set; }
        #endregion

        #region Constructor

        private const string FILTER = "(*.GLSL)|*.GLSL";
        private const string PROJECTFILTER = "(*.SHADEPROJ, *.GLSL)|*.SHADEPROJ;*GLSL";

        public MainPageViewModel(GLControl RenderCanvas, CustomDynamicTab DynamicTab)
        {
            GLControl = RenderCanvas;

            CreateFileCommand = new RelayCommand((obj) =>
            {
                var dialog = new SaveFileDialog
                {
                    DefaultExt = ".glsl",
                    Filter = FILTER
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
                    Filter = PROJECTFILTER,
                    CheckFileExists = true,
                    Multiselect = true
                };
                if (dialog.ShowDialog() == true)
                {
                    // Open shadeproj project
                    if (dialog.FileNames.Count() == 1 && Path.GetExtension(dialog.FileNames[0]) == ".shadeproj")
                    {
                        try
                        {
                            StreamReader sr = new StreamReader(dialog.FileNames[0]);
                            SettingModel model = new SettingModel();
                            bool flag, isOk = true;

                            string nameOfSourseFile = sr.ReadLine();

                            isOk &= bool.TryParse(sr.ReadLine(), out flag);
                            model.IsTime = flag;

                            isOk &= bool.TryParse(sr.ReadLine(), out flag);
                            model.IsMouse = flag;

                            isOk &= bool.TryParse(sr.ReadLine(), out flag);
                            model.IsViewPort = flag;

                            sr.Close();

                            if (!isOk)
                            {
                                ErrorText += "Failed open .shadeproj file" + Environment.NewLine;
                                return;
                            }

                            SettingModel = model;
                            Dictionary<string, int> solution = new Dictionary<string, int>();
                            StaticShaderLinker.LincRec(nameOfSourseFile, 0, solution);
                            foreach (var source in solution)
                            {
                                var tab = AddToTabItems(source.Key);
                                if (tab != null)
                                    FilledTab(tab, source.Key);
                            }
                        }
                        catch
                        {
                            ErrorText += "Failed open .shadeproj file" + Environment.NewLine;
                            return;
                        }
                    }
                    else
                    {
                        Dictionary<string, int> solution = new Dictionary<string, int>();
                        foreach (var name in dialog.FileNames)
                            StaticShaderLinker.LincRec(name, 0, solution);
                        foreach (var source in solution)
                        {
                            var tab = AddToTabItems(source.Key);
                            if (tab != null)
                                FilledTab(tab, source.Key);
                        }
                    }
                }
            });

            SaveCommand = new RelayCommand((obj) =>
            {
                var dialog = new SaveFileDialog
                {
                    DefaultExt = ".shaderproj",
                    Filter = "(*.SHADERPROJ)|*.SHADERPROJ"
                };
                if (dialog.ShowDialog() == true)
                {
                    var name = dialog.FileName;
                    using (StreamWriter sr = new StreamWriter(name))
                    {
                        sr.WriteLine(StaticShaderLinker.FindMain(GetTabFilesPath()));
                        sr.WriteLine(SettingModel.IsTime);
                        sr.WriteLine(SettingModel.IsMouse);
                        sr.WriteLine(SettingModel.IsViewPort);
                    }
                }
            });

            RunCommand = new RelayCommand((obj) =>
            {
                Logger _logger = Logger.Instance;
                _logger.ClearMessages();
                StaticShaderBuilder.StopRender();

                SaveInFiles(TabItems);

                StaticShaderBuilder.RenderShader(GetTabFilesPath(), SettingModel);
                StaticShaderBuilder.Paint(RenderCanvas);

                ErrorText = _logger.GetAllMessage();
            });

            SaveCommand = new RelayCommand((obj) =>
            {
                if (DynamicTab.SelectedItem == null)
                    return;
                var tab = DynamicTab.SelectedItem as TabItem;
                SaveInFiles(new TabItem[] { tab });                  
            });

            OpenSettingsCommand = new RelayCommand((obj) =>
            {
                var window = new SettingsWindow(SettingModel);
                window.Show();                
            });

            StopCommand = new RelayCommand((obj) =>
            {
                StaticShaderBuilder.StopRender();
            });

            FullscreenCommand = new RelayCommand((obj) =>
            {
                StopCommand.Execute(null);
                RenderCanvas.Enabled = false;
                var full = new FullScreenRenderWindow(()=>RunCommand.Execute(null));
                full.ShowDialog();
                RenderCanvas.Enabled = true;
                RenderCanvas.Invalidate();
                RenderCanvas.MakeCurrent();   
            });
			/*
	        StartRecordCommand = new RelayCommand((obj) => new Action<GLControl>((gl) =>
	        {
		        
	        }));*/
        }



        #endregion

        #region Help methods

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

        private TabItem AddToTabItems(string name)
        {
            if (TabItems.FirstOrDefault(t => t.Name == Path.GetFileNameWithoutExtension(name)) != null) return null;
            var tab = CreateTabItem(name);
            TabItems.Add(tab);
            return tab;
        }

		private TabItem CreateTabItem(string name)
        {
            return new TabItem
            {
                Header = name,
                Name = "tab" + TabItems.Count().ToString()
            };
        } 

        private IEnumerable<string> GetTabFilesPath()
        {
            return from t in TabItems
                   select t.Header as string;
        }

        private void SaveInFiles(IEnumerable<TabItem> tabs)
        {
            foreach (var tab in tabs)
            {
                var path = tab.Header.ToString();
                using (var file = File.Open(path, FileMode.Create))
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
