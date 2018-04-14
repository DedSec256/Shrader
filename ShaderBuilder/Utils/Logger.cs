using System;
using OpenTK.Graphics.OpenGL;

namespace ShaderBuilder.Utils
{
    public class Logger
    {
	    private static Logger _holder;

	    public static Logger Instance
	    {
		    get
		    {
			    if(_holder is null) _holder = new Logger();
			    return _holder;
		    }
	    }
        private string _messages = "";

        public void ClearMessages()
        {
            _messages = "";
        }

        public void CompileShaderError(ShaderType shaderType, string error)
        {
            _messages += $"Failed to compile {shaderType} shader: {error}" + Environment.NewLine;
        }

        public void UnableCreatingError()
        {
            _messages += "Unable to create shader..." + Environment.NewLine;
        }

        public void LinkProgramError(string error)
        {
            _messages += $"Failed to link program: {error}" + Environment.NewLine;
        }

        public void CreateProgramError()
        {
            _messages += "Failed to create program..." + Environment.NewLine;
        }

        public void WritePosError()
        {
            _messages += "Failed to write the positions of vertices to a vertex shader" + Environment.NewLine;
        }

        public void AttachOfPosError()
        {
            _messages += "Failed to get the storage location of a_Position" + Environment.NewLine;
        }

        public void InitShaderError()
        {
            _messages += "Failed to initialize the shader..." + Environment.NewLine;
        }

        public void LoadShaderError()
        {
            _messages += "Failed to load shader file..." + Environment.NewLine;
        }
	    public void LinkerError(string message = "")
	    {
		    _messages += "Failed to link source files: " + message + Environment.NewLine;
	    }

		public string GetAllMessage()
        {
            return _messages;
        }
    }
}
