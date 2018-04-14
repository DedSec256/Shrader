using Shrader.IDE.ViewModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Shrader.IDE.Tools.SyntaxHighlighter;

using Shrader.IDE.Compilation;
using System.Threading;
using System.Windows.Threading;
using MahApps.Metro.Controls;

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
			int selectionLength = codeEditSpace.SelectionLength;

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
				for (int i = 0; i < codeEditSpace.Lines.Length; i++)
				{
					var higlights = SyntaxHighlighter.Parse(codeEditSpace.Lines[i]);
					foreach (var higlight in higlights)
					{
						var startPos = codeEditSpace.GetFirstCharIndexFromLine(i);
						Select(codeEditSpace, startPos + higlight.StartPosition,
							higlight.EndPosition - higlight.StartPosition, higlight.Color);
					}
					
				}
				codeEditSpace.Select(selectionStart, selectionLength);
				codeEditSpace.SelectionColor = codeEditSpace.ForeColor;
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

        private const int TICK_PERIOD = 25;

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

		#endregion
	}
}
