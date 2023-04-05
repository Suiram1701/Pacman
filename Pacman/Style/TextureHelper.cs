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
        private int Rows { get; }
        private int Coloumns { get; }

        /// <summary>
        /// Init the texture helper
        /// </summary>
        /// <param name="TextureCollection">Basic texture</param>
        /// <param name="TextureHeight">Height of each texture</param>
        /// <param name="TextureWidht">Widht of each texture</param>
        /// <param name="Rows">All rows in the basic texture</param>
        /// <param name="Colomns">All colomns in the basic texture</param>
        public TextureHelper(Bitmap TextureCollection, int TextureHeight, int TextureWidht, int Rows, int Colomns)
        {
            // Setup
            Bitmaps = new List<Bitmap>();
            this.Rows = Rows;
            this.Coloumns = Coloumns;
            Rectangle CopyRect = new Rectangle(0, 0, TextureWidht, TextureHeight);

            // Get all bitmaps
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Colomns; c++)
                {
                    // Get bitmap
                    Bitmaps.Add(TextureCollection.Clone(CopyRect, TextureCollection.PixelFormat));
                    CopyRect.X += c * TextureWidht;
                }
                CopyRect.Y += r * TextureHeight;
            }
        }

        /// <summary>
        /// Get a texture with their index of the texture
        /// </summary>
        /// <param name="index">Textures index</param>
        /// <returns>The texture. When the index is the return value <see langword="null"/></returns>
        public Bitmap this[int index]
        {
            get
            {
                if (index >= Bitmaps.Count())
                    return null;

                return Bitmaps[index];
            }
        }

        /// <summary>
        /// Get a texture with their row and colomn of the texture
        /// </summary>
        /// <param name="Row">Textures row</param>
        /// <param name="Colomn">Textures colomn</param>
        /// <returns>The texture. When the row or colomn isnt valid is the return value <see langword="null"/></returns>
        public Bitmap this[int Row, int Colomn]
        {
            get
            {
                int index = Row * Coloumns + Colomn;
                if (index >= Bitmaps.Count())
                    return null;
                return Bitmaps[index];
            }
        }

        public IEnumerator GetEnumerator() =>
            Bitmaps.GetEnumerator();
    }
}
