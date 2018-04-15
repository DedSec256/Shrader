using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ShaderBuilder.Utils;

namespace Shrader.IDE.Compilation
{
	internal static class StaticShaderLinker
	{
		static Logger _logger = Logger.Instance;
		static readonly Regex Include = new Regex("#include \"([\\W|\\w]+?\\.glsl)\"");
		static readonly Regex Main = new Regex("void main\\(\\)");

		public static void LincRec(string nameOfFile, int level, Dictionary<string, int> links)
		{
			if (links.ContainsKey(nameOfFile))
				return;
			links.Add(nameOfFile, level);

			var file = new FileInfo(nameOfFile);
			using (var fileStream = new StreamReader(file.OpenRead()))
			{
				string code = fileStream.ReadToEnd();
				var result = Main.Matches(code);

				foreach (var link in result)
					LincRec((link as Match).Groups[1].Value, level + 1, links);
			}
		}

		public static void LinkSources(IEnumerable<string> namesOfShaderFile, string endFile)
		{
            try
			{
				Dictionary<string, int> links = new Dictionary<string, int>();
				LincRec(namesOfShaderFile.First(x => x == FindMain(namesOfShaderFile)), 0, links);

				using (var fileWriter = new StreamWriter(endFile))
				{
                    fileWriter.WriteLine("#version 330");
                    foreach (var pair in links.OrderByDescending(pair => pair.Value))
					{
						using (var streamReader = new StreamReader(pair.Key))
						{
							fileWriter.WriteLine(Main.Replace(streamReader.ReadToEnd(), ""));
						}
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LinkerError(ex.Message);
			}
		}

		public static string FindMain(IEnumerable<string> namesOfShaderFile)
		{
			foreach (var name in namesOfShaderFile)
			{
				using (var fileStream = new StreamReader(name))
				{
					string code = fileStream.ReadToEnd();
					if (Main.IsMatch(code))
					{
						return name;
					}
				}
			}
			throw new FileNotFoundException("void main() not found");
		}
	}
}
