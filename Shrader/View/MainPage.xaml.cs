using Shrader.IDE.Compilation;
using Shrader.IDE.ViewModel;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Drawing;
using System.Windows.Threading;
using Shrader.IDE.Tools;
using DynamicTab.SyntaxHighlighter;

namespace Shrader.IDE.View
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
	        SyntaxHighlighter.LoadOrCreate("settings.ini");
			OpenTK.Toolkit.Init();
            InitializeComponent();

            DataContext = new MainPageViewModel(RenderCanvas, DynamicTab)
            {
                TabItems = DynamicTab.TabItems
            };
        }

        #region Highlight part

        private void DynamicTab_TextChangedRichTextBoxEvent(object sender, EventArgs e)
        {
            var codeEditSpace = sender as System.Windows.Forms.RichTextBox;
            if (codeEditSpace == null)
                return;

            codeEditSpace.LoadHighlightinngs();
        }

        #endregion

        #region Render part

        private const int TICK_PERIOD = 25;

        private void WindowsFormsHost_Initialized(object sender, EventArgs e)
        {
            RenderCanvas.MakeCurrent();
        }

        DispatcherTimer timer;
        private void RenderCanvas_Load(object sender, EventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(TICK_PERIOD);
            timer.Start();
        }

        private static bool isExecute = false;
        private static object lockToken = new object();
        private void Timer_Tick(object sender, EventArgs e)
        {
            lock (lockToken)
            {
                if (isExecute == true)
                    return;
            }
            isExecute = true;
            RenderCanvas.Invalidate();
            isExecute = false;
        }

        private void RenderCanvas_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            StaticShaderBuilder.Paint(RenderCanvas);
        }


        private void MenuItem_PauseClick(object sender, RoutedEventArgs e)
        {
            timer?.Stop();
        }

        #endregion
    }
}
