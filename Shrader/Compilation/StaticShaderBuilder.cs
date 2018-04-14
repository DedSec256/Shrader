using Shrader.IDE.Model;
using System.Collections.Generic;
using System.IO;

namespace Shrader.IDE.Compilation
{
    /// <summary>
    /// Static shader builder class
    /// </summary>
    internal static class StaticShaderBuilder
    {
        private static ShaderBuilder.ShaderBuilder shaderBuilder = new ShaderBuilder.ShaderBuilder();
	    private const string EXECUTE_PATH = "executive.glsl";

		public static void Paint(OpenTK.GLControl control)
        {
            shaderBuilder.Paint(control);
        }

        public static void RenderShader(IEnumerable<string> namesOfShaderFile, SettingModel model)
        {
            using (StreamWriter sr = new StreamWriter(EXECUTE_PATH, false))
            {
                sr.WriteLine("");
                sr.Close();
            }

            StaticShaderLinker.LinkSources(namesOfShaderFile, EXECUTE_PATH);
            shaderBuilder.RenderShader(EXECUTE_PATH, model);
        }

        public static void Resize(OpenTK.GLControl control)
        {
            shaderBuilder.Resize(control);
        }

        public static void StopRender()
        {
            shaderBuilder.StopRender();
        }
    }


}
