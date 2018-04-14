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
		static readonly Regex Regex = new Regex("#include \"([\\W|\\w]+?\\.glsl)\"");

		public static void LincRec(string nameOfFile, int level, Dictionary<string, int> links)
		{
			if (links.ContainsKey(nameOfFile))
				return;
			links.Add(nameOfFile, level);

			var file = new FileInfo(nameOfFile);
			using (var fileStream = new StreamReader(file.OpenRead()))
			{
				string code = fileStream.ReadToEnd();
				var result = Regex.Matches(code);

				foreach (var link in result)
					LincRec((link as Match).Groups[1].Value, level + 1, links);
			}
		}

		public static void LinkSources(IEnumerable<string> namesOfShaderFile, string endFile)
		{
            try
			{
				Dictionary<string, int> links = new Dictionary<string, int>();
				LincRec(namesOfShaderFile.First(), 0, links);

				using (var fileWriter = new StreamWriter(endFile))
				{
                    fileWriter.WriteLine("#version 330");
                    foreach (var pair in links.OrderByDescending(pair => pair.Value))
					{
						using (var streamReader = new StreamReader(pair.Key))
						{
							fileWriter.WriteLine(Regex.Replace(streamReader.ReadToEnd(), ""));
						}
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LinkerError(ex.Message);
			}
		}
	}
}
