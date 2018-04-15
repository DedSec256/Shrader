using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Accord.Video.FFMPEG;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Timer = System.Timers.Timer;

namespace Shrader.IDE.Tools.VideoSaver
{
	static class VideoShaderRecorder
	{
		private static List<Bitmap> Images;
		private static Timer Timer;
		public static bool IsRecording { get; private set; }
		private static string Filename;

		static VideoShaderRecorder()
		{
			Images = new List<Bitmap>();
		}

		public static void StartRecord(string filename, double interval)
		{
			Images = new List<Bitmap>(500);
			IsRecording = true;
			Filename = filename;

			Timer = new Timer(interval);
			Timer.Elapsed += Timer_Elapsed;
			Timer.Start();
		}

		private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			StopRecord();
		}

		public static void StopRecord()
		{
			if (IsRecording)
			{
				Timer.Stop();
				Timer.Dispose();
				IsRecording = false;
				Thread.Sleep(100);
				Task.Run(() =>
				{
					CreateMovie(Filename);
				});
			}
			IsRecording = false;
		}

		public static void GrabScreenshot(GLControl control)
		{
			if (!IsRecording) return;

			Bitmap bmp = new Bitmap(control.ClientSize.Width, control.ClientSize.Height);
			System.Drawing.Imaging.BitmapData data =
				bmp.LockBits(control.ClientRectangle, System.Drawing.Imaging.ImageLockMode.WriteOnly,
					System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			GL.ReadPixels(0, 0, control.ClientSize.Width, control.ClientSize.Height, PixelFormat.Bgr, PixelType.UnsignedByte,
				data.Scan0);
			bmp.UnlockBits(data);
			bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
			if (IsRecording) Images.Add(bmp);
		}

		/// <summary>
		/// Получаем изображеине из битов
		/// </summary>
		/// <param name="byteArrayIn"></param>
		/// <returns></returns>
		public static Bitmap ToBitmap(byte[] byteArrayIn)
		{
			var ms = new System.IO.MemoryStream(byteArrayIn);
			var returnImage = Image.FromStream(ms);
			var bitmap = new Bitmap(returnImage);

			return bitmap;
		}

		/// <summary>
		/// Сжимаем изображение 
		/// </summary>
		/// <param name="original"></param>
		/// <param name="reducedWidth"></param>
		/// <param name="reducedHeight"></param>
		/// <returns></returns>
		public static Bitmap ReduceBitmap(Bitmap original, int reducedWidth, int reducedHeight)
		{
			var reduced = new Bitmap(reducedWidth, reducedHeight);
			using (var dc = Graphics.FromImage(reduced))
			{
				dc.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
				dc.DrawImage(original, new Rectangle(0, 0, reducedWidth, reducedHeight), new Rectangle(0, 0, original.Width, original.Height), GraphicsUnit.Pixel);
			}

			return reduced;
		}

		public static void CreateMovie(string path)
		{

			int width = 720;
			int height = 480;
			var framRate = 25;

			using (var vFWriter = new VideoFileWriter())
			{
				vFWriter.Open(path, width, height, framRate, VideoCodec.MPEG4);
				foreach (var image in Images)
				{
					var bmpReduced = ReduceBitmap(image, width, height);

					vFWriter.WriteVideoFrame(bmpReduced);
				}
				vFWriter.Close();
				Images.Clear();
				MessageBox.Show("Запись успешно сохранена.", "Запись завершена", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}
	}
}
