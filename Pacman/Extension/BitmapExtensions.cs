using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Pacman.Extension
{
    internal static class BitmapExtensions
    {
        public static BitmapImage ToImage(this Bitmap img)
        {
            MemoryStream MS = new MemoryStream();
            img.Save(MS, System.Drawing.Imaging.ImageFormat.Bmp);

            BitmapImage ix = new BitmapImage();
            ix.BeginInit();
            ix.CacheOption = BitmapCacheOption.OnLoad;
            ix.StreamSource = MS;
            ix.EndInit();
            return ix;
        }
    }
}
