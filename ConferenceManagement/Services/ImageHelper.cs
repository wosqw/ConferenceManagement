using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace ConferenceManagement.Services
{
    public static class ImageHelper
    {
        public static BitmapImage FromBytes(byte[] data)
        {
            if (data == null || data.Length == 0) return null;
            var bmp = new BitmapImage();
            using (var ms = new MemoryStream(data))
            {
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.StreamSource = ms;
                bmp.EndInit();
                bmp.Freeze();
            }
            return bmp;
        }
    }
}
