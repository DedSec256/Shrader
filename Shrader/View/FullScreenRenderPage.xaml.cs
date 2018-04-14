using OpenTK.Graphics;
using Shrader.IDE.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Shrader.IDE.View
{
    /// <summary>
    /// Логика взаимодействия для FullScreenRenderPage.xaml
    /// </summary>
    public partial class FullScreenRenderPage : Page
    {
        

        public FullScreenRenderPage()
        {
            
            InitializeComponent();



            DataContext = new FullScreenRenderPage();

            OpenTK.Toolkit.Init();
        }


        #region Render part

        private const int TICK_PERIOD = 1000;

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
