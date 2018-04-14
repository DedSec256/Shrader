using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicTab.SyntaxHighlighter
{
	enum States
	{
		Comment,
		Preprocessor,
		Digit,
		Keyword,
		Default,
		Symbol,
		InText,
		Text,
	}

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
		public string Key { get; set; }
	}

	public static class SyntaxHighlighter
	{
		private static Dictionary<string, SyntaxKeyword> Keywords 
							= new Dictionary<string, SyntaxKeyword>();

		public static void LoadOrCreate(string filename)
		{
			try
			{
				if (!File.Exists(filename))
				{
					using (var fileStream = new StreamWriter(filename))
					{
						fileStream.WriteLine(JsonConvert.SerializeObject(GenerateDefault()));
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

		private static SyntaxKeyword[] GenerateDefault()
		{
			Color typeColor = Color.FromArgb(255, 100, 20, 250);
			Color keywordColor = Color.FromArgb(255, 0, 70, 100);
			Color funcColor = Color.FromArgb(255, 0, 20, 250);

			string[] types = { "bool", "int", "float", "vec3", "vec2", "vec4", "mat3", };
			string[] keywords = { "uniform", "void", "return", "if", "else", "for", "out", "break", "in", "const", "inout", "sign", "normalize", "clamp", "step", "smoothstep", "mix", "pow", "reflect", "texture" };
			string[] funcs = { "cos", "sin", "fract", "dot", "max", "abs", "length", "floor", "min", "mod", "log"};

			SyntaxKeyword[] syntaxStandarts =
			{
				new SyntaxKeyword(){ Keyword = States.Comment.ToString(), Color = Color.FromArgb(255, 0, 120, 0)},
				new SyntaxKeyword(){ Keyword = States.Symbol.ToString(), Color = Color.FromArgb(255, 100, 100, 120)},
				new SyntaxKeyword(){ Keyword = States.Digit.ToString(), Color = Color.FromArgb(255, 0, 100, 200)},
				new SyntaxKeyword(){ Keyword = States.Preprocessor.ToString(), Color = Color.FromArgb(255, 10, 50, 40)},
				new SyntaxKeyword(){ Keyword = States.Text.ToString(), Color = Color.FromArgb(255, 200, 40, 50)}
			};

			var syntaxKeywords = syntaxStandarts
				.Union(keywords.Select(k => new SyntaxKeyword() { Color = keywordColor, Keyword = k }))
				.Union(types.Select(k => new SyntaxKeyword() { Color = typeColor, Keyword = k }))
				.Union(funcs.Select(k => new SyntaxKeyword() { Color = funcColor, Keyword = k })).ToArray();

			return syntaxKeywords;
		}

		public static IEnumerable<HiglightArea> Parse(string text)
		{
			return new StatesMachine().Parse(text, Keywords);
		}
		public static Task<IEnumerable<HiglightArea>> ParseAsync(string text)
		{
			return Task.Run(() => new StatesMachine().Parse(text, Keywords));
		}
		private class StatesMachine
		{
			private States _state = States.Default;
			private readonly List<HiglightArea> _areas = new List<HiglightArea>(1000);
			private HashSet<char> Symbols = new HashSet<char>() {',', '{', '}', '(', '[', ']', ')', '%', ';'};

			private const string DEFAULT = "[d]";
			public IEnumerable<HiglightArea> Parse(string text, Dictionary<string, SyntaxKeyword> keywords)
			{
				int index = 0;
				int startPosition = 0;
				string lastState = DEFAULT;
				StringBuilder buffer = new StringBuilder();
				char lastS = '\t';

				foreach (var s in text)
				{
					switch (_state)
					{
						case States.Default:
						{
							if (Char.IsWhiteSpace(s) || Char.IsControl(s))
								break;
							else if (Char.IsLetter(s))
							{
								_state = States.Keyword;
							}
							else if (s is '#')
							{
								_state = States.Preprocessor;
							}
							else if (s is '\"')
							{
								_state = States.InText;
							}
							else if (s == '/' && lastS == '/')
							{
								_state = States.Comment;
								startPosition = index - 1;
								break;
							}
							else if (Symbols.Contains(s))
							{
								if (index == text.Length - 1)
								{
									++index;
								}
								lastState = States.Symbol.ToString();
							}
							else if (Char.IsDigit(s))
							{
								_state = States.Digit;
							}
							startPosition = index;
							buffer = new StringBuilder();
								break;
						}
						case States.InText:
						{
							if (s is '\"')
							{
								_state = States.Text;
								if (index == text.Length - 1)
								{
									lastState = States.Text.ToString();
									++index;
								}
							}

							break;
						}
						case States.Text:
						{
							lastState = States.Text.ToString();
							break;
						}
						case States.Comment:
						{
							if (s is '\n' || s is '\r') lastState = States.Comment.ToString();
							else if (index == text.Length - 1)
							{
								lastState = States.Comment.ToString();
								++index;
							}
							break;
						}
						case States.Keyword:
						{
							if (!Char.IsLetterOrDigit(s))
							{
								lastState = buffer.ToString();
							}
							break;
						}
						case States.Digit:
						{
							if (!(Char.IsDigit(s) || s == '.')) 
								lastState = States.Digit.ToString();
							break;
						}
						case States.Preprocessor:
						{
							if (s == '\n' || s == '\r') lastState = States.Preprocessor.ToString();
							else if (index == text.Length - 1)
							{
								lastState = States.Preprocessor.ToString();
								++index;
							}
							break;
						}
					}
					buffer.Append(s);
					if (lastState != DEFAULT)
					{
						if (keywords.TryGetValue(lastState, out var value))
						{
							_areas.Add(new HiglightArea()
							{
								StartPosition = startPosition,
								EndPosition = index,
								Color = value.Color
							});
						}
						_state = States.Default;
						lastState = DEFAULT;
					}
					++index;
					lastS = s;
				}
				return _areas;
			}
		}
	}


}
