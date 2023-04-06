using Pacman.Extension;
using Pacman.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using static Packman.Properties.Settings;
using Pacman.Figures;
using Packman.Style.Textures;
using System.IO;

namespace Pacman
{
    /// <summary>
    /// Interaktionslogik für Game.xaml
    /// </summary>
    public partial class Game : Window
    {
        public Game()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Calculate if the element is in Field
        /// </summary>
        /// <param name="Figure">Element to check</param>
        /// <returns>True if is in field and false when not</returns>
        public static bool IsInField(int X, int Y, int Height, int Widht)
        {
            // Map all values to map size
            decimal MapFactor = (decimal)Textures.Map.Height / 746;
            decimal MapedX = X * MapFactor;
            decimal MapedY = Y * MapFactor;
            decimal MapedWidht = Widht * MapFactor;
            decimal MapedHeight = Height * MapFactor;

            Rectangle Rect = new Rectangle((int)MapedX, (int)MapedY, (int)MapedWidht, (int)MapedHeight);
            Color Border = Color.FromArgb(33, 33, 222);     // Map border color

            using (Bitmap Bmp = Textures.Map.Clone(Rect, Textures.Map.PixelFormat))
            {
                for (int y = 0; y < Bmp.Height; y++)
                {
                    for (int x = 0; x < Bmp.Width; x++)
                    {
                        if (Bmp.GetPixel(x, y) == Border)
                        {
                            Bmp.Dispose();
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Ckeck if move direction of pacman change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == (Key)Default.Control_Left)
                Pacman.Direction = Direction.Left;

            else if (e.Key == (Key)Default.Control_Down)
                Pacman.Direction = Direction.Down;

            else if (e.Key == (Key)Default.Control_Right)
                Pacman.Direction = Direction.Right;

            else if (e.Key == (Key)Default.Control_Up)
                Pacman.Direction = Direction.Up;
        }
    }
}
