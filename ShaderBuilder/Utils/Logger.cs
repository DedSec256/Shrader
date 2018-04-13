using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace ShaderBuilder
{
    public class Logger
    {
        private static string messages = "";

        public static void ClearMessages()
        {
            messages = "";
        }

        public static void CompileShaderError(ShaderType shaderType, string error)
        {
            messages += string.Format("Failed to compile {0} shader: {1}", shaderType.ToString(), error) + Environment.NewLine;
        }

        public static void UnableCreatingError()
        {
            messages += "Unable to create shader..." + Environment.NewLine;
        }

        public static void LinkProgramError(string error)
        {
            messages += string.Format("Failed to link program: {0}", error) + Environment.NewLine;
        }

        public static void CreateProgramError()
        {
            messages += "Failed to create program..." + Environment.NewLine;
        }

        public static void WritePosError()
        {
            messages += "Failed to write the positions of vertices to a vertex shader" + Environment.NewLine;
        }

        public static void AttachOfPosError()
        {
            messages += "Failed to get the storage location of a_Position" + Environment.NewLine;
        }

        public static void InitShaderError()
        {
            messages += "Failed to initialize the shader..." + Environment.NewLine;
        }

        public static void LoadShaderError()
        {
            messages += "Failed to load shader file..." + Environment.NewLine;
        }

        public static string GetAllMessage()
        {
            return messages;
        }
    }
}
