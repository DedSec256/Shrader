using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shrader.IDE.Compile
{
	internal static class StaticShaderLinker
	{
		static Regex regex = new Regex("#include \"(\\w +.glsl)\"");

		private static void LincRec(string nameOfFile, int level, Dictionary<string, int> links)
		{
			if (links.ContainsKey(nameOfFile))
				return;
			links.Add(nameOfFile, level);
			var file = new FileInfo(nameOfFile);
			using (var fileStream = new StreamReader(file.OpenRead()))
			{
				string code = fileStream.ReadToEnd();
				var result = regex.Match(code).Groups;

				foreach (var link in result)
					LincRec((string) link, level + 1, links);
			}
		}

		public static void LinkSources(string[] namesOfShaderFile, string endFile)
		{
			try
			{
				var files = namesOfShaderFile.Select(p => File.Exists(p)
					? new FileInfo(p)
					: throw new FileNotFoundException($"Fatal linker error: source file {p} not found!"));

				Dictionary<string, int> links = new Dictionary<string, int>();
				LincRec(namesOfShaderFile.First(), 0, links);

				using (var fileWriter = new StreamWriter(endFile))
				{
					foreach (var pair in links.OrderByDescending(pair => pair.Value))
					{
						using (var streamReader = new StreamReader(pair.Key))
						{
							fileWriter.WriteLine(regex.Replace(streamReader.ReadToEnd(), ""));
						}
					}
				}
			}
			catch (Exception ex)
			{
				
			}
		}
	}
}
