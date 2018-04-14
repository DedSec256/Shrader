using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using ShaderBuilder.Utils;
using Shrader.IDE.Model;

namespace ShaderBuilder
{
    /// <summary>
    /// Shader compiller class
    /// </summary>
    public class ShaderBuilder
    {
        #region Private variables
        /// <summary>
        /// Can draw flag
        /// </summary>
        private bool canDraw = false;

        /// <summary>
        /// Number of shader program
        /// </summary>
        private int program;

        /// <summary>
        /// Num of vertexes
        /// </summary>
        private int nVertices;

        /// <summary>
        /// Number of vertex buffer
        /// </summary>
        private int vertexBuffer = 0;
        #endregion

        private class Uniforms
        {
            private const string UNIFORM_MOUSE = "iMouse";
            private const string UNIFORM_TIME = "iTime";
            private const string UNIFORM_DISPLAY = "iResolution";

            private int UniformMouse = -1;
            private int UniformTime = -1;
            private int UniformDisplay = -1;

            public Uniforms(SettingModel model, int program)
            {
                if (model.IsMouse)
                    UniformMouse = GL.GetUniformLocation(program, UNIFORM_MOUSE);

                if (model.IsTime)
                    UniformTime = GL.GetUniformLocation(program, UNIFORM_TIME);

                if (model.IsViewPort)
                    UniformDisplay = GL.GetUniformLocation(program, UNIFORM_DISPLAY);
            }

            public void UpdateUniforms()
            {
                if (UniformTime != -1)
                    GL.Uniform1(UniformTime, (float)Environment.TickCount / 1000);
            }

            public void DeleteUniforms()
            {
                UniformTime = -1;
                UniformMouse = -1;
                UniformDisplay = -1;
            }
        }

        private Uniforms uniforms;

        private Logger _logger = Logger.Instance;
        /// <summary>
        /// Standart constructor
        /// </summary>
        public ShaderBuilder()
        {
            GL.ClearColor(Color.SteelBlue);
        }

        /// <summary>
        /// Initialisation vertex buffer function
        /// </summary>
        private int InitVertexBuffers()
        {
            float[] vertices = new float[] { 1f, 1f, -1f, 1f, -1f, -1f, 1f, 1f, -1f, -1f, 1f, -1f };

            GL.GenBuffers(1, out vertexBuffer);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);

            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            int a_Position = GL.GetAttribLocation(program, "a_Position");
            if (a_Position < 0)
            {
                _logger.AttachOfPosError();
                return -1;
            }

            GL.VertexAttribPointer(a_Position, 2, VertexAttribPointerType.Float, false, 0, 0);

            GL.EnableVertexAttribArray(a_Position);
            return vertices.Length / 2;
        }

        /// <summary>
        /// Main function in this program, render shader with his file name
        /// </summary>
        public void RenderShader(string nameOfShaderFile, SettingModel model)
        {
            string fShaderSource = null;
            ShaderLoader.LoadShader(nameOfShaderFile, out fShaderSource);
            if (fShaderSource == null)
            {
                _logger.LoadShaderError();
                return;
            }

            if (!ShaderLoader.InitShaders(fShaderSource, out program))
            {
                _logger.InitShaderError();
                return;
            }

            nVertices = InitVertexBuffers();
            if (nVertices <= 0)
            {
                _logger.WritePosError();
                return;
            }

            uniforms = new Uniforms(model, program);

            GL.ClearColor(Color.SteelBlue);
            canDraw = true;
        }

        /// <summary>
        /// Resize component function
        /// </summary>
        public void Resize(OpenTK.GLControl control)
        {
            GL.Viewport(0, 0, control.Width, control.Height);
        }

        /// <summary>
        /// Paint event
        /// </summary>
        public void Paint(OpenTK.GLControl control)
        {
            GL.Viewport(0, 0, control.Width, control.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            if (canDraw)
            {
                uniforms.UpdateUniforms();
                GL.DrawArrays(PrimitiveType.Triangles, 0, nVertices);
            }

            GL.Flush();
            control.SwapBuffers();
        }

        /// <summary>
        /// Stop render and clear data event
        /// </summary>
        public void StopRender()
        {
            GL.DeleteProgram(program);
            GL.DeleteBuffer(vertexBuffer);
            ShaderLoader.DeleteShaders();
            GL.ClearColor(Color.SteelBlue);

            if (uniforms != null)
                uniforms.DeleteUniforms();
            canDraw = false;
        }
    }
}
