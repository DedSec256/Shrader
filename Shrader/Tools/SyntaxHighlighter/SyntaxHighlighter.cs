using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Newtonsoft.Json;

namespace Shrader.IDE.Tools.SyntaxHighlighter
{
	public class SyntaxKeyword
	{
		public string Keyword { get; set; }
		public Color Color { get; set; }
	}
	public class HiglightArea
	{
		public int StartPosition { get; set; }
		public int EndPosition { get; set; }
		public Color Color { get; set; } 
	}
	static class SyntaxHighlighter
	{
		private static Dictionary<string, SyntaxKeyword> Keywords 
							= new Dictionary<string, SyntaxKeyword>();
		public static bool Load(string filename)
		{
			try
			{
				using (var fileStream = new StreamReader(filename))
				{
					string data = fileStream.ReadToEnd();
					var syntaxRules = JsonConvert.DeserializeObject<SyntaxKeyword[]>(data);

					foreach (var keyword in syntaxRules)
					{
						Keywords.Add(keyword.Keyword, keyword);
					}
				}
				return true;
			}
			catch (IOException)
			{
				return false;
			}
		}
		public static IEnumerable<HiglightArea> Parse(string text)
		{
			return new StatesMachine().Parse(text, Keywords);
		}

		private class StatesMachine
		{
			private States State = States.Default;
			private readonly List<HiglightArea> _areas = new List<HiglightArea>();
			enum States
			{
				Comment, 
			    Preprocessor,
				Digit, 
				Keyword, 
				Default,
				Text, 
			}
			public IEnumerable<HiglightArea> Parse(string text, Dictionary<string, SyntaxKeyword> keywords)
			{
				int index = 0;
				int startPosition = 0;
				States lastState = States.Default;
				char lastS = '\t';

				foreach (var s in text)
				{
					switch (State)
					{
						case States.Default:
						{
							if (s is '#')
							{
								State = States.Preprocessor;
							}
							if (s is '\"')
							{
								State = States.Text;
							}
							if (s == '/' && lastS == '/')
							{
								State = States.Comment;
							}
							if (Char.IsDigit(s))
							{
								State = States.Digit;
							}
							startPosition = index;
							break;
						}
						case States.Text:
						{
							if (s is '\"') lastState = States.Text;
							break;
						}
						case States.Comment:
						{
							if (s is '\n') lastState = States.Comment;
							break;
						}
						case States.Keyword:
						{
							if (!Char.IsLetterOrDigit(s))
								lastState = States.Keyword;
							break;
						}
						case States.Digit:
						{
							if (!(Char.IsDigit(s) || s == '.' || s == ',')) //тут точно баг
								lastState = States.Digit;
							break;
						}
						case States.Preprocessor:
						{
							if (s == '\n') lastState = States.Preprocessor;
							break;
						}
					}
					if (lastState != States.Default)
					{
						_areas.Add(new HiglightArea()
						{
							StartPosition = startPosition,
							EndPosition = index,
							Color = keywords[lastState.ToString()].Color
						});
						State = lastState = States.Default;
					}
					++index;
					lastS = s;
				}
				return _areas;
			}
		}
	}


}
