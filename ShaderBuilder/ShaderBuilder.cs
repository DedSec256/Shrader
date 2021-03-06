﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using ShaderBuilder.Utils;
using Shrader.IDE.Model;
using Tao.DevIl;

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
            private const string UNIFORM_TEXTURE = "iTexture";

            private int UniformMouse = -1;
            private int UniformTime = -1;
            private int UniformDisplay = -1;
            private int UniformTexture = -1;
            private int TextureID = -1;

            static int LoadTexture(string filename)
            {
                if (String.IsNullOrEmpty(filename))
                    throw new ArgumentException(filename);

                GL.Enable(EnableCap.Texture2D);

                int id;
                GL.GenTextures(1, out id);
                GL.ActiveTexture(TextureUnit.Texture0 + id);
                GL.BindTexture(TextureTarget.Texture2D, id);
                
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);

	            GL.ActiveTexture(TextureUnit.Texture0 + id);
	            GL.BindTexture(TextureTarget.Texture2D, id);

				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                // GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 0);

                Bitmap bmp = new Bitmap(filename);
                BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

	            GL.ActiveTexture(TextureUnit.Texture0 + id);
	            GL.BindTexture(TextureTarget.Texture2D, id);

				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, bmp_data.Scan0);
                GL.BindTexture(TextureTarget.Texture2D, id);

	            GL.ActiveTexture(TextureUnit.Texture0 + id);
	            GL.BindTexture(TextureTarget.Texture2D, id);

				bmp.UnlockBits(bmp_data);

                return id;
				/*uniform sampler2D iTexture; 

out vec4 fragColor; 

vec2 fragCoord = gl_FragCoord.xy; 

void main() 
{ 
// vec4 color = vec4(texture2D(iTexture, fragCoord).xyz, 0); 

vec4 color = vec4(fragCoord.xy / 1000.0, 0.5, 0); 
fragColor = color; 
}*/
			}

			public Uniforms(SettingModel model, int program)
            {
                if (model.IsMouse)
                    UniformMouse = GL.GetUniformLocation(program, UNIFORM_MOUSE);

                if (model.IsTime)
                    UniformTime = GL.GetUniformLocation(program, UNIFORM_TIME);

                if (model.IsViewPort)
                    UniformDisplay = GL.GetUniformLocation(program, UNIFORM_DISPLAY);

                if (model.ImagesPath == null || !model.ImagesPath.Any())
                    return;

                string name = model.ImagesPath.First();
                TextureID = LoadTexture(name);
                UniformTexture = GL.GetUniformLocation(program, UNIFORM_TEXTURE);
            }

            public void UpdateUniforms(OpenTK.GLControl control)
            {
                if (UniformTime != -1)
                    // Time sending in seconds
                    GL.Uniform1(UniformTime, ((float)Environment.TickCount / 1000)); 

                if (UniformDisplay != -1)
                    GL.Uniform3(UniformDisplay, (float)control.Width, control.Height, 0);

                if (UniformMouse != -1)
                    // TODO: Mouse uniforms
                    GL.Uniform4(UniformMouse, 100, 100, 0, 0);

                GL.ActiveTexture(TextureUnit.Texture0 + TextureID);
                GL.BindTexture(TextureTarget.Texture2D, TextureID);
                GL.Uniform1(UniformTexture, TextureID);
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

            GL.EnableVertexAttribArray(a_Position);
			GL.VertexAttribPointer(a_Position, 2, VertexAttribPointerType.Float, false, 0, 0);
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
                uniforms.UpdateUniforms(control);
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

	        uniforms?.DeleteUniforms();
	        canDraw = false;
        }
    }
}
