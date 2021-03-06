﻿using Shrader.IDE.ViewModel;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows;

using Shrader.IDE.Compilation;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Shrader.IDE.Tools;
using Shrader.IDE.Tools.VideoSaver;

namespace Shrader.IDE.View
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
			InitializeComponent();
            IoC.IoC.SetupWindow();
            IoC.IoC.SetupMainPage();
            DataContext = IoC.IoC.MainWindowViewModel;
        }

    }
}
