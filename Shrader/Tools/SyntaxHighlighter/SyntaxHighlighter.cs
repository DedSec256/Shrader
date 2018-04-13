using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Newtonsoft.Json;

namespace Shrader.IDE.Tools.SyntaxHighlighter
{
				enum States
			{
				Comment, 
			    Preprocessor,
				Digit, 
				Keyword, 
				Default,
				Symbol,
				Text, 
			}
	public class SyntaxKeyword
	{
		public string Keyword { get; set; }
		public Color Color { get; set; }
	}
	public class HiglightArea
	{
		public long StartPosition { get; set; }
		public long EndPosition { get; set; }
		public Color Color { get; set; } 
	}
	static class SyntaxHighlighter
	{
		private static Dictionary<string, SyntaxKeyword> Keywords 
							= new Dictionary<string, SyntaxKeyword>();

		public static void LoadOrCreate(string filename)
		{
			try
			{
				if (!File.Exists(filename))
				{
					Color typeColor = Color.FromArgb(255, 100, 20, 250);
					Color keywordColor = Color.FromArgb(255, 0, 0, 100); 
					Color funcColor = Color.FromArgb(255, 0, 20, 250);

					string[] types = {"int", "float", "vec3", "vec2", "vec4", "mat3", };
					string[] keywords = {"return", "if", "else", "for", "out", "break", "in", "const", "inout", "sign", "normalize", "clamp", "step", "smoothstep", "mix", "pow", "reflect" };
					string[] funcs = { "cos", "sin", "fract", "dot", "max", "abs", "length", "floor", "min", "mod", };

					SyntaxKeyword[] syntaxStandarts = 
					{
						new SyntaxKeyword(){ Keyword = States.Comment.ToString(), Color = Color.FromArgb(255, 0, 120, 0)},
						new SyntaxKeyword(){ Keyword = States.Symbol.ToString(), Color = Color.FromArgb(255, 100, 100, 120)},
						new SyntaxKeyword(){ Keyword = States.Digit.ToString(), Color = Color.FromArgb(255, 0, 100, 200)},
						new SyntaxKeyword(){ Keyword = States.Preprocessor.ToString(), Color = Color.FromArgb(255, 40, 40, 40)},
						new SyntaxKeyword(){ Keyword = States.Text.ToString(), Color = Color.FromArgb(255, 200, 40, 50)}
					};

					var syntaxKeywords = syntaxStandarts
						.Union(keywords.Select(k => new SyntaxKeyword() {Color = keywordColor, Keyword = k}))
						.Union(types.Select(k => new SyntaxKeyword() {Color = typeColor, Keyword = k}))
						.Union(funcs.Select(k => new SyntaxKeyword() {Color = funcColor, Keyword = k})).ToArray();

					using (var fileStream = new StreamWriter(filename))
					{
						fileStream.WriteLine(JsonConvert.SerializeObject(syntaxKeywords));
					}
				}
				else
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
				}
			}
			catch (IOException)
			{}
		}
		public static IEnumerable<HiglightArea> Parse(string text)
		{
			//return Task.Run(
				//() => 
				return new StatesMachine().Parse(text, Keywords);
		}

		private class StatesMachine
		{
			private States State = States.Default;
			private readonly List<HiglightArea> _areas = new List<HiglightArea>();
			public IEnumerable<HiglightArea> Parse(string text, Dictionary<string, SyntaxKeyword> keywords)
			{
				int index = 0;
				int startPosition = 0;
				States lastState = States.Default;
				StringBuilder buffer = new StringBuilder();
				char lastS = '\t';

				foreach (var s in text)
				{
					switch (State)
					{
						case States.Default:
						{
							if (Char.IsWhiteSpace(s) || Char.IsControl(s))
								break;
							else if (Char.IsLetter(s))
							{
								buffer = new StringBuilder();
								buffer.Append(s);
								State = States.Keyword;
							}
							else if (s is '#')
							{
								State = States.Preprocessor;
							}
							else if (s is '\"')
							{
								State = States.Text;
							}
							else if (s == '/' && lastS == '/')
							{
								State = States.Comment;
							}
							else if ((Char.IsPunctuation(s) || Char.IsSeparator(s)) && s != '/')
							{
								lastState = States.Symbol;
							}
							else if (Char.IsDigit(s))
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
							if (s is '\n' || s is '\r') lastState = States.Comment;
							break;
						}
						case States.Keyword:
						{
							if (!Char.IsLetterOrDigit(s))
							{
								if (keywords.TryGetValue(buffer.ToString(), out var value))
								{
									_areas.Add(new HiglightArea()
									{
										StartPosition = startPosition,
										EndPosition = index,
										Color = value.Color
									});
								}
								State = lastState = States.Default;
								break;
							}
							buffer.Append(s);
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
							if (s == '\n' || s == '\r') lastState = States.Preprocessor;
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
