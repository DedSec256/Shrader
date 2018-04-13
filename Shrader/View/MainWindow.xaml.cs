﻿using Shrader.IDE.ViewModel;
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

            DataContext = new MainWindowViewModel(RenderCanvas);
			SyntaxHighlighter.LoadOrCreate("settings.ini");
		}

        #region Highlight part

        private void CodeEditSpace_TextChanged(object sender, TextChangedEventArgs e)
        {
			// 0123456789
			//"if int a = 0.0 return 2"

			TextRange documentRange = new TextRange(CodeEditSpace.Document.ContentStart, CodeEditSpace.Document.ContentEnd);
            var text = documentRange.Text;
	        var cursorPosition = CodeEditSpace.CaretPosition;

	        var higlights = SyntaxHighlighter.Parse(text);
	        foreach (var higlight in higlights)
	        {
		        Select(higlight.StartPosition, higlight.EndPosition - higlight.StartPosition, higlight.Color);
	        }
	        CodeEditSpace.CaretPosition = cursorPosition;
        }

		private static TextPointer GetTextPointAt(TextPointer from, long pos)
		{
			TextPointer ret = from;
			long i = 0;

			while ((i < pos) && (ret != null))
			{
				if ((ret.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.Text) ||
					(ret.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.None))
					i++;

				if (ret.GetPositionAtOffset(1, LogicalDirection.Forward) == null)
					return ret;

				ret = ret.GetPositionAtOffset(1, LogicalDirection.Forward);
			}

			return ret;
		}

		private void Select(long offset, long length, Color color)
		{
			// Get text selection:
			TextSelection textRange = CodeEditSpace.Selection;

			// Get text starting point:
			TextPointer start = CodeEditSpace.Document.ContentStart;

			// Get begin and end requested:
			TextPointer startPos = GetTextPointAt(start, offset);
			TextPointer endPos = GetTextPointAt(start, offset + length);

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
