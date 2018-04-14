using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shrader.IDE
{
    /// <summary>
    /// Static shader builder class
    /// </summary>
    public static class StaticShaderBuilder
    {
        private static ShaderBuilder.ShaderBuilder shaderBuilder = new ShaderBuilder.ShaderBuilder();

        public static void Paint(OpenTK.GLControl control)
        {
            shaderBuilder.Paint(control);
        }

        public static void RenderShader(string[] namesOfShaderFile)
        {
            // TODO - make 1 files of namesOfShaderFile array...
            shaderBuilder.RenderShader(namesOfShaderFile[0]);
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
