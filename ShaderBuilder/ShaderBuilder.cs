using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using ShaderBuilder.Utils;

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
		private Logger _logger = Logger.Instance;
        /// <summary>
        /// Standart constructor
        /// </summary>
        public ShaderBuilder()
        {
            GL.ClearColor(Color.DarkSlateBlue);
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
        public void RenderShader(string nameOfShaderFile)
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

            GL.ClearColor(Color.DarkSlateBlue);
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
            GL.ClearColor(Color.DarkSlateBlue);
            canDraw = false;
        }
    }
}
