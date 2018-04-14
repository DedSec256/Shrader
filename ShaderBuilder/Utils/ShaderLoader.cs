using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.IO;

namespace ShaderBuilder
{
    public class ShaderLoader
    {
        private static int vertexShader = 0, fragmentShader = 0;

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
                Logger.CreateProgramError();
                return false;
            }

            GL.UseProgram(program);
            return true;
        }

        public static void LoadShader(string shaderFileName, out string shaderSource)
        {
            Logger.ClearMessages();
            shaderSource = null;

            using (StreamReader sr = new StreamReader(shaderFileName))
            {
                shaderSource = sr.ReadToEnd();
            }
        }

        public static void DeleteShaders()
        {
            if (vertexShader != 0)
                GL.DeleteShader(vertexShader);

            if (fragmentShader != 0)
                GL.DeleteShader(fragmentShader);
        }

        private static int CreateProgram(string vShader, string fShader)
        {
            vertexShader = LoadShader(ShaderType.VertexShader, vShader);
            fragmentShader = LoadShader(ShaderType.FragmentShader, fShader);
            if (vertexShader == 0 || fragmentShader == 0)
            {
                return 0;
            }

            int program = GL.CreateProgram();
            if (program == 0)
            {
                return 0;
            }

            GL.AttachShader(program, vertexShader);
            GL.AttachShader(program, fragmentShader);

            GL.LinkProgram(program);

            int status;
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out status);
            if (status == 0)
            {
                Logger.LinkProgramError(GL.GetProgramInfoLog(program));
                GL.DeleteProgram(program);
                GL.DeleteShader(vertexShader);
                GL.DeleteShader(fragmentShader);
                return 0;
            }

            return program;
        }

        private static int LoadShader(ShaderType shaderType, string shaderSource)
        {
            int shader = GL.CreateShader(shaderType);
            if (shader == 0)
            {
                Logger.UnableCreatingError();
                return 0;
            }

            GL.ShaderSource(shader, shaderSource);

            GL.CompileShader(shader);

            int status;
            GL.GetShader(shader, ShaderParameter.CompileStatus, out status);
            if (status == 0)
            {
                Logger.CompileShaderError(shaderType, GL.GetShaderInfoLog(shader));
                GL.DeleteShader(shader);
                return 0;
            }

            return shader;
        }
    }
}
