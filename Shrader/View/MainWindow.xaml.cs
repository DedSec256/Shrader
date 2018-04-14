using Shrader.IDE.ViewModel;
using System;
using System.Drawing;
using System.Threading;
using System.Windows;
using Shrader.IDE.Tools.SyntaxHighlighter;

using Shrader.IDE.Compilation;
using System.Windows.Threading;
using MahApps.Metro.Controls;

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
            DataContext = IoC.IoC.MainWindowViewModel;
        }

    }
}
