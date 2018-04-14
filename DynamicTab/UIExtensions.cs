using System.Drawing;

namespace DynamicTab
{
	static class UIExtensions
	{
		public static void LoadHighlightinngs(this System.Windows.Forms.RichTextBox codeEditSpace)
		{
			void Select(System.Windows.Forms.RichTextBox codeEdit, int start, int length, Color color)
			{
				codeEdit.Select(start, length);
				codeEdit.SelectionColor = color;
			}
			
			codeEditSpace.Enabled = false;

			int selectionStart = codeEditSpace.SelectionStart;

			var higlights = SyntaxHighlighter.SyntaxHighlighter.Parse(codeEditSpace.Text);
			foreach (var higlight in higlights)
			{
				Select(codeEditSpace, higlight.StartPosition,
					higlight.EndPosition - higlight.StartPosition,
					higlight.Color);
			}

			/*
			if (codeEditSpace.Lines.Length == 0)
			{
				var higlights = SyntaxHighlighter.SyntaxHighlighter.Parse(codeEditSpace.Text);
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
					var higlights = SyntaxHighlighter.SyntaxHighlighter.Parse(codeEditSpace.Lines[i]);
					foreach (var higlight in higlights)
					{

							//writer.WriteLine(
						//$"{higlight.Key} - {higlight.Color.A};  {higlight.Color.R}; {higlight.Color.B}; {higlight.Color.G}");

						var startPos = codeEditSpace.GetFirstCharIndexFromLine(i);
						Select(codeEditSpace, startPos + higlight.StartPosition,
							higlight.EndPosition - higlight.StartPosition, higlight.Color);
					}
				}
				//}
				*/
			codeEditSpace.Select(selectionStart, 0);
			//}
			codeEditSpace.Enabled = true;
			codeEditSpace.Focus();
		}

	}
}
