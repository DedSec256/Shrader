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

using Shrader.IDE.Compilation;

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

        private async void DynamicTab_TextChangedRichTextBoxEvent(object sender, EventArgs e)
        {
            var codeEditSpace = sender as RichTextBox;
            if (codeEditSpace == null)
                return;

	        if (e.Changes.Count != 0)
	        {
		        var change = e.Changes.First();
		        TextRange documentRange = new TextRange(GetTextPointAt(codeEditSpace.Document.ContentStart, change.Offset),
			        GetTextPointAt(codeEditSpace.Document.ContentStart, change.Offset + change.AddedLength));

		        var text = documentRange.Text;
		        //if int a = 0.0 return 2;

		        var higlights = await SyntaxHighlighter.Parse(text);
		        foreach (var higlight in higlights)
		        {
			        Select(codeEditSpace, change.Offset + higlight.StartPosition - 1,
				        change.Offset + higlight.EndPosition - higlight.StartPosition - 1,
				        higlight.Color);
		        }
	        }
        }


		private void Select(RichTextBox codeEditSpace, int offset, int length, Color color)
		{
			// Get text selection:
			TextRange textRange = new TextRange(codeEditSpace.Document.ContentStart, codeEditSpace.Document.ContentEnd);

			// Get text starting point:
			TextPointer start = codeEditSpace.Document.ContentStart;

			// Get begin and end requested:
			TextPointer startPos = GetTextPointAt(start, offset);
			TextPointer endPos = GetTextPointAt(start, offset + length);

			// New selection of text:
			textRange.Select(startPos, endPos);

			// Apply property to the selection:
			textRange.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));

		}

		private static TextPointer GetTextPointAt(TextPointer from, int pos)
		{
			TextPointer ret = from;
			int i = 0;

			while ((i < pos) && (ret != null))
			{
				if ((ret.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.Text) || (ret.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.None))
					i++;

				if (ret.GetPositionAtOffset(1, LogicalDirection.Forward) == null)
					return ret;

				ret = ret.GetPositionAtOffset(1, LogicalDirection.Forward);
			}

			return ret;
		}

		#endregion

		#region Render part

        private void WindowsFormsHost_Initialized(object sender, EventArgs e)
        {
            RenderCanvas.MakeCurrent();
        }

        private void RenderCanvas_Load(object sender, EventArgs e)
        {
        }

        private void RenderCanvas_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            StaticShaderBuilder.Paint(RenderCanvas);
        }

		#endregion
	}
}
