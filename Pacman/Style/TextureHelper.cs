using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Packman.Style
{
    internal class TextureHelper : IEnumerable
    {
        private List<Bitmap> Bitmaps { get; }

        public TextureHelper(Bitmap TextureCollection, int TextureHeight, int TextureWidht, int Rows, int Coloumns)
        {
            // Setup
            Bitmaps = new List<Bitmap>();
            Rectangle CopyRect = new Rectangle(0, 0, TextureWidht, TextureHeight);

            // Get all bitmaps
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Coloumns; c++)
                {
                    // Get bitmap
                    Bitmaps.Add(TextureCollection.Clone(CopyRect, TextureCollection.PixelFormat));
                    CopyRect.X += c * TextureWidht;
                }
                CopyRect.Y += r * TextureHeight;
            }
        }

        public Bitmap this[int index] =>
            Bitmaps[index];

        public IEnumerator GetEnumerator() =>
            Bitmaps.GetEnumerator();
    }
}
