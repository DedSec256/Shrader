using Shrader.IDE.ViewModel;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows;
using Shrader.IDE.Tools.SyntaxHighlighter;

using Shrader.IDE.Compilation;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Shrader.IDE.Tools.VideoSaver;

namespace Shrader.IDE.View
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
		SynchronizationContext context = SynchronizationContext.Current;
        public MainWindow()
		{
            OpenTK.Toolkit.Init();
            InitializeComponent();

            DataContext = new MainWindowViewModel(RenderCanvas, DynamicTab)
            {
                TabItems = DynamicTab.TabItems                
            };
			SyntaxHighlighter.LoadOrCreate("settings.ini");            

        }

        #region Highlight part

		private void DynamicTab_TextChangedRichTextBoxEvent(object sender, EventArgs e)
		{
			var codeEditSpace = sender as System.Windows.Forms.RichTextBox;
			if (codeEditSpace == null)
				return;

			codeEditSpace.Enabled = false;

			int selectionStart = codeEditSpace.SelectionStart;

			if (codeEditSpace.Lines.Length == 0)
			{
				var higlights = SyntaxHighlighter.Parse(codeEditSpace.Text);
				foreach (var higlight in higlights)
				{
					Select(codeEditSpace, higlight.StartPosition,
						higlight.EndPosition - higlight.StartPosition,
						higlight.Color);
				}
			}
			else
			{
				//using (var writer = new StreamWriter("log.txt"))
			//	{
					for (int i = 0; i < codeEditSpace.Lines.Length; i++)
					{
						var higlights = SyntaxHighlighter.Parse(codeEditSpace.Lines[i]);
						foreach (var higlight in higlights)
						{

						//	writer.WriteLine(
							//	$"{higlight.Key} - {higlight.Color.A};  {higlight.Color.R}; {higlight.Color.B}; {higlight.Color.G}");

							var startPos = codeEditSpace.GetFirstCharIndexFromLine(i);
							Select(codeEditSpace, startPos + higlight.StartPosition,
								higlight.EndPosition - higlight.StartPosition, higlight.Color);
						}
					}
				//}
				codeEditSpace.Select(selectionStart, 0);
			}
            codeEditSpace.Enabled = true;
            codeEditSpace.Focus();
		}

		private void Select(System.Windows.Forms.RichTextBox codeEditSpace, int start, int length, Color color)
		{
			codeEditSpace.Select(start, length);
			codeEditSpace.SelectionColor = color;
		}

        #endregion

        #region Render part

	    private const int TICK_PERIOD = 10;

        private void WindowsFormsHost_Initialized(object sender, EventArgs e)
        {
            RenderCanvas.MakeCurrent();
        }

        private void RenderCanvas_Load(object sender, EventArgs e)
        {
            var timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;            
            timer.Interval = TimeSpan.FromMilliseconds(TICK_PERIOD);
            timer.Start();
        }

        private static bool isExecute = false;
        private static object lockToken = new object();
        private void Timer_Tick(object sender, EventArgs e)
        {
            //lock (lockToken)
            //{
            //    if (isExecute == true)
            //        return; 
            //}
            //isExecute = true;
            RenderCanvas.Invalidate();
            //isExecute = false;
        }

        private void RenderCanvas_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            StaticShaderBuilder.Paint(RenderCanvas);
	        if (VideoShaderRecorder.IsRecording)
	        {
		        if (!VideoShaderRecorder.AddImage(GrabScreenshot(RenderCanvas)))
		        {
			        VideoShaderRecorder.CreateMovie("bestMovieEver.mp4");
			        VideoShaderRecorder.StopRecord();
		        }
	        }
        }

	    static Bitmap GrabScreenshot(GLControl control)
	    {
		    Bitmap bmp = new Bitmap(control.ClientSize.Width, control.ClientSize.Height);
		    System.Drawing.Imaging.BitmapData data =
			    bmp.LockBits(control.ClientRectangle, System.Drawing.Imaging.ImageLockMode.WriteOnly,
				    System.Drawing.Imaging.PixelFormat.Format24bppRgb);
		    GL.ReadPixels(0, 0, control.ClientSize.Width, control.ClientSize.Height, PixelFormat.Bgr, PixelType.UnsignedByte,
			    data.Scan0);
		    bmp.UnlockBits(data);
		    bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
		    return bmp;
	    }

		#endregion
	}
}
