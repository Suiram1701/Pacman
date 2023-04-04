using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Packman.Extension
{
    internal static class Image
    {
        public static BitmapImage ToImage(this System.Drawing.Bitmap img)
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
