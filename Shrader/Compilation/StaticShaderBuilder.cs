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

        public static void RenderShader(string[] namesOfShaderFile)
        {
            StaticShaderLinker.LinkSources(namesOfShaderFile, EXECUTE_PATH);
            shaderBuilder.RenderShader(EXECUTE_PATH);
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
