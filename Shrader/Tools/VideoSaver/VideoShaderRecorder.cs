using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Accord.Video.FFMPEG;

namespace Shrader.IDE.Tools.VideoSaver
{
	static class VideoShaderRecorder
	{
		private static List<Bitmap> Images;
		private const int MaxShots = 100;
		private static bool isRecord = false;
		public static bool IsRecording => isRecord;

		static VideoShaderRecorder()
		{
			Images = new List<Bitmap>();
		}

		public static void StartRecord()
		{
			if (isRecord) return;
			Images = new List<Bitmap>();
			isRecord = true;
		}

		public static void StopRecord()
		{
			isRecord = false;
		}
		public static bool AddImage(Bitmap bitmap)
		{
			if (isRecord && Images.Count <= MaxShots)
			{
				Images.Add(bitmap);
				return true;
			}
			return false;
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

			int width = 480;
			int height = 320;
			var framRate = 200;

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
