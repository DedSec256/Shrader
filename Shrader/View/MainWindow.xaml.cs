using Shrader.IDE.ViewModel;
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
using Shrader.IDE.Tools.SyntaxHighlighter;

using ShaderBuilder;

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

        private void DynamicTab_TextChangedRichTextBoxEvent(object sender, TextChangedEventArgs e)
        {
            var CodeEditSpace = sender as RichTextBox;
            if (CodeEditSpace == null)
                return;

			TextRange documentRange = new TextRange(CodeEditSpace.Document.ContentStart, CodeEditSpace.Document.ContentEnd);

			var text = documentRange.Text;
	        var cursorPosition = CodeEditSpace.CaretPosition;
			//if int a = 0.0 return 2;

	        var higlights = SyntaxHighlighter.Parse(text);
	        foreach (var higlight in higlights)
	        {
		        Select(CodeEditSpace, higlight.StartPosition, higlight.EndPosition - higlight.StartPosition, higlight.Color);
	        }
	        CodeEditSpace.CaretPosition = cursorPosition;
        }


		private void Select(RichTextBox CodeEditSpace, int offset, int length, Color color)
		{
			// Get text selection:
			TextRange textRange = new TextRange(CodeEditSpace.Document.ContentStart, CodeEditSpace.Document.ContentEnd);

			// Get text starting point:
			TextPointer start = CodeEditSpace.Document.ContentStart;

			// Get begin and end requested:
			TextPointer startPos = start.GetPositionAtOffset(offset, LogicalDirection.Forward);
			TextPointer endPos = start.GetPositionAtOffset(offset + length, LogicalDirection.Forward);
			// New selection of text:
			textRange.Select(startPos, endPos);

			// Apply property to the selection:
			textRange.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));
			// Return selection text:
			//return CodeEditSpace.Selection.Text;

		}
		
		#endregion

		#region Render part

        ShaderBuilder.ShaderBuilder shaderBuilder;

        private void WindowsFormsHost_Initialized(object sender, EventArgs e)
        {
            RenderCanvas.MakeCurrent();
        }

        private void RenderCanvas_Load(object sender, EventArgs e)
        {
            shaderBuilder = new ShaderBuilder.ShaderBuilder();

        }

        private void RenderCanvas_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            shaderBuilder.Paint(RenderCanvas);
        }

		#endregion
	}
}
