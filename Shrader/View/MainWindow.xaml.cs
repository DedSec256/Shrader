using Shrader.IDE.ViewModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
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

namespace Shrader.IDE.View
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
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

		        //if int a = 0.0 return 2;

		    var higlights = SyntaxHighlighter.Parse(codeEditSpace.Text);
	        int selectionStart = codeEditSpace.SelectionStart;
	        int selectionLength = codeEditSpace.SelectionLength;
	        foreach (var higlight in higlights)
	        {
		        codeEditSpace.SelectionStart = higlight.StartPosition;
		        codeEditSpace.SelectionLength = higlight.EndPosition - higlight.StartPosition;
		        codeEditSpace.SelectionColor = higlight.Color;
	        }
	        codeEditSpace.SelectionStart = selectionStart;
	        codeEditSpace.SelectionLength = selectionLength;

        }


        #endregion

        #region Render part


        private void WindowsFormsHost_Initialized(object sender, EventArgs e)
        {
            RenderCanvas.MakeCurrent();
        }

        private void RenderCanvas_Load(object sender, EventArgs e)
        {
            var timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;            
            timer.Interval = TimeSpan.FromMilliseconds(25);
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
