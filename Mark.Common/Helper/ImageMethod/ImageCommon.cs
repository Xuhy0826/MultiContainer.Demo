using System;
using System.Drawing;
using System.IO;

namespace Mark.Common
{
    public class ImageCommon
    {
        public static string ImagesFilter = "所有图片 " + "(*.jpg;*.bmp;*.png;*.jpeg)|*.jpg;*.bmp;*.png;*.jpeg|jpg (*.jpg)|*.jpg|bmp (*.bmp)|*.bmp|png (*.png)|*.png|jpeg (*.jpeg)|*.jpeg";

        #region WPF，暂时没实现

        ///// <summary>
        ///// 将“bitmap类型”转换成“ImageSource类型”,记得释放
        ///// </summary>
        ///// <param name="imgBitmap"></param>
        ///// <returns></returns>
        //public static BitmapSource ChangeBitmapToImageSource(Bitmap imgBitmap)
        //{
        //    using (MemoryStream stream = new MemoryStream())
        //    {
        //        imgBitmap.Save(stream, ImageFormat.Jpeg);
        //        BitmapImage bitmap = new BitmapImage();
        //        bitmap.BeginInit();
        //        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        //        stream.Seek(0, SeekOrigin.Begin);
        //        bitmap.StreamSource = stream;
        //        bitmap.EndInit();
        //        bitmap.Freeze();
        //        //imgBitmap.Dispose();
        //        return bitmap;
        //    }
        //}
        ///// <summary>
        ///// 将“bitmap类型”转换成“ImageSource类型”,记得释放
        ///// </summary>
        ///// <param name="bitmap"></param>
        ///// <returns></returns>
        //public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        //{
        //    BitmapImage bitmapImage = new BitmapImage();
        //    using (System.IO.MemoryStream ms = new MemoryStream())
        //    {
        //        bitmap.Save(ms, bitmap.RawFormat);
        //        bitmapImage.BeginInit();
        //        bitmapImage.StreamSource = ms;
        //        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        //        bitmapImage.EndInit();
        //        bitmapImage.Freeze();
        //    }
        //    return bitmapImage;
        //}

        ///// <summary>
        ///// 将WPF窗口（Window）保存为图片，dpi建议96
        ///// </summary>
        ///// <param name="window"></param>
        ///// <param name="dpi"></param>
        ///// <param name="filename"></param>
        //public static void SaveWindow(Window window, int dpi, string filename)
        //{
        //    var rtb = new RenderTargetBitmap(
        //        (int)window.Width, //width
        //        (int)window.Width, //height
        //        dpi, //dpi x
        //        dpi, //dpi y
        //        PixelFormats.Pbgra32 // pixelformat
        //        );
        //    rtb.Render(window);
        //    SaveRTBAsPNG(rtb, filename);

        //}
        ///// <summary>
        ///// 将WPF画布（Canvas）保存为图片，dpi建议96
        ///// </summary>
        ///// <param name="window"></param>
        ///// <param name="canvas"></param>
        ///// <param name="dpi"></param>
        ///// <param name="filename"></param>
        //public static void SaveCanvas(Canvas canvas, System.Windows.Size size, int dpi, string filename)
        //{
        //    //Size size = new Size(window.Width, window.Height);
        //    //canvas.Measure(size);
        //    ////canvas.Arrange(new Rect(size));

        //    //var rtb = new RenderTargetBitmap(
        //    //    (int)window.Width, //width
        //    //    (int)window.Height, //height
        //    //    dpi, //dpi x
        //    //    dpi, //dpi y
        //    //    PixelFormats.Pbgra32 // pixelformat
        //    //    );
        //    //rtb.Render(canvas);

        //    //SaveRTBAsPNG(rtb, filename);

        //    canvas.Measure(size);
        //    //canvas.Arrange(new Rect(size));
        //    var rtb = new RenderTargetBitmap(
        //        (int)canvas.Width, //width
        //        (int)canvas.Height, //height
        //        dpi, //dpi x
        //        dpi, //dpi y
        //        PixelFormats.Pbgra32 // pixelformat
        //        );
        //    rtb.Render(canvas);
        //    SaveRTBAsPNG(rtb, filename);
        //}

        //private static void SaveRTBAsPNG(RenderTargetBitmap bmp, string filename)
        //{
        //    var enc = new System.Windows.Media.Imaging.PngBitmapEncoder();
        //    enc.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bmp));
        //    using (var stm = System.IO.File.Create(filename))
        //    {
        //        enc.Save(stm);
        //    }
        //}
        #endregion

        /// <summary>
        /// 将图片从base64转成bitmap
        /// </summary>
        /// <param name="base64string"></param>
        /// <returns></returns>
        public static Bitmap GetImageFromBase64(string base64string)
        {
            byte[] b = Convert.FromBase64String(base64string);
            MemoryStream ms = new MemoryStream(b);
            Bitmap bitmap = new Bitmap(ms);
            return bitmap;
        }
        /// <summary>
        /// 从字节数组中读取图片
        /// </summary>
        /// <param name="b"></param>
        /// <param name="savePath"></param>
        /// <returns></returns>
        public static Bitmap GetImageFrombytes(byte[] b, string savePath)
        {
            MemoryStream ms = new MemoryStream(b);
            FileStream fs = File.Open(savePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fs.Write(b, 0, b.Length);
            fs.Close();
            Bitmap bitmap = new Bitmap(ms);
            return bitmap;
        }

        
    }
}
