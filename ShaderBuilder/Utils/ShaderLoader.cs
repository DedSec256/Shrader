using System;
using System.IO;
using OpenTK.Graphics.OpenGL;

namespace ShaderBuilder.Utils
{
    public class ShaderLoader
    {
		static Logger _logger = Logger.Instance;
        private static int _vertexShader = 0, _fragmentShader = 0;

        public static bool InitShaders(string fShaderSource, out int program)
        {
            string vShaderSource =
                "#version 330"                      + Environment.NewLine +
                "in vec4 a_Position;"               + Environment.NewLine +
                "void main()"                       + Environment.NewLine +
                "{"                                 + Environment.NewLine +
                "    gl_Position = a_Position;"     + Environment.NewLine +
                "}"                                 + Environment.NewLine;

            program = CreateProgram(vShaderSource, fShaderSource);
            if (program == 0)
            {
                _logger.CreateProgramError();
                return false;
            }

            GL.UseProgram(program);
            return true;
        }

        public static void LoadShader(string shaderFileName, out string shaderSource)
        {
            shaderSource = null;

            using (StreamReader sr = new StreamReader(shaderFileName))
            {
                shaderSource = sr.ReadToEnd();
            }
        }

        public static void DeleteShaders()
        {
            if (_vertexShader != 0)
                GL.DeleteShader(_vertexShader);

            if (_fragmentShader != 0)
                GL.DeleteShader(_fragmentShader);
        }

        private static int CreateProgram(string vShader, string fShader)
        {
            _vertexShader = LoadShader(ShaderType.VertexShader, vShader);
            _fragmentShader = LoadShader(ShaderType.FragmentShader, fShader);
            if (_vertexShader == 0 || _fragmentShader == 0)
            {
                return 0;
            }

            int program = GL.CreateProgram();
            if (program == 0)
            {
                return 0;
            }

            GL.AttachShader(program, _vertexShader);
            GL.AttachShader(program, _fragmentShader);

            GL.LinkProgram(program);

            int status;
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out status);
            if (status == 0)
            {
                _logger.LinkProgramError(GL.GetProgramInfoLog(program));
                GL.DeleteProgram(program);
                GL.DeleteShader(_vertexShader);
                GL.DeleteShader(_fragmentShader);
                return 0;
            }

            return program;
        }

        private static int LoadShader(ShaderType shaderType, string shaderSource)
        {
            int shader = GL.CreateShader(shaderType);
            if (shader == 0)
            {
                _logger.UnableCreatingError();
                return 0;
            }

            GL.ShaderSource(shader, shaderSource);

            GL.CompileShader(shader);

            int status;
            GL.GetShader(shader, ShaderParameter.CompileStatus, out status);
            if (status == 0)
            {
                _logger.CompileShaderError(shaderType, GL.GetShaderInfoLog(shader));
                GL.DeleteShader(shader);
                return 0;
            }

            return shader;
        }
    }
}
