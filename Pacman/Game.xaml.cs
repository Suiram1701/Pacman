using Pacman.Extension;
using Pacman.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Pacman.Collectable;
using System.Windows.Data;
using System.Windows.Documents;
using Control = System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using static Pacman.Properties.Settings;
using Pacman.Figures;
using Pacman.Style.Textures;
using System.IO;
using Point = System.Drawing.Point;
using System.Windows;
using System.Timers;
using System.Windows.Threading;

namespace Pacman
{
    /// <summary>
    /// Interaktionslogik für Game.xaml
    /// </summary>
    public partial class Game : Window
    {
        #region Datacontext
        public int Points { get; set; }
        public int Hightscore { get; set; }
        #endregion

        private readonly Timer GameLoop = new Timer(50);

        public Game()
        {
            InitializeComponent();

            // Init map
            ResetFigures();
            ResetPoints();

            // Setup game loop
            GameLoop.Elapsed += PointCheck;
            GameLoop.Start();
        }

        /// <summary>
        /// Set all figures on their start positions
        /// </summary>
        private void ResetFigures()
        {
            // Pacman
            Pacman.Direction = Direction.None;
            Canvas.SetLeft(Pacman, 318);
            Canvas.SetTop(Pacman, 545);
            Panel.SetZIndex(Pacman, 2);
        }

        /// <summary>
        /// Summon all points new on map
        /// </summary>
        private void ResetPoints()
        {
            // Energizer
            foreach (Point point in Energizer)
            {
                int Index = Canvas.Children.Add(new Energizer());
                Canvas.SetLeft(Canvas.Children[Index], point.X);
                Canvas.SetTop(Canvas.Children[Index], point.Y);
                Panel.SetZIndex(Canvas.Children[Index], 1);
            }

            // Normal points
            Point Point = new Point(30, 26);
            int Distance = 24;
            foreach (int[] Row in PointSpawning)
            {
                foreach (int Colomn in Row)
                {
                    if (Colomn > 0)
                    {
                        for (int i = 0; i < Colomn; i++)
                        {
                            // Create and positioning point
                            int Index = Canvas.Children.Add(new Collectable.Point());
                            Canvas.SetLeft(Canvas.Children[Index], Point.X);
                            Canvas.SetTop(Canvas.Children[Index], Point.Y);
                            Panel.SetZIndex(Canvas.Children[Index], 1);

                            Point.X += Distance;
                        }
                    }
                    else
                        Point.X += Math.Abs(Colomn) * Distance;
                }
                Point.X = 30;
                Point.Y += Distance;
            }
        }

        /// <summary>
        /// Check if pacman can eat a point
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PointCheck(object sender, ElapsedEventArgs e) =>
            Dispatcher.Invoke(() =>
            {
                // Check distance and get near item
                IEnumerable<ICollectable> Items = Canvas.Children.OfType<ICollectable>()
                    .Where(Object => Math.Abs(Canvas.GetLeft((UIElement)Object) - (Canvas.GetLeft(Pacman)) - 10) <= 20 &&
                    Math.Abs(Canvas.GetTop((UIElement)Object) - (Canvas.GetTop(Pacman)) - 10) <= 20)
                    .ToList();

                // Add points and remove item
                foreach (ICollectable Item in Items)
                {
                    Points += (int)Item.Point;
                    Canvas.Children.Remove((UIElement)Item);
                }
            }, DispatcherPriority.Loaded);

        /// <summary>
        /// Position of all energizers
        /// </summary>
        private static readonly IEnumerable<Point> Energizer = new List<Point>()
        {
            new Point(30, 74),
            new Point(630, 74),
            new Point(30, 553),
            new Point(630, 553)
        };

        /// <summary>
        /// Position of all single points
        /// </summary>
        private static readonly int[][] PointSpawning = new int[29][]
        {
            new int[] {12, -2, 12},
            new int[] {1, -4, 1, -5, 1, -2, 1, -5, 1, -4, 1},
            new int[] {-5, 1, -5, 1, -2, 1, -5, 1, -5},
            new int[] {1, -4, 1, -5, 1, -2, 1, -5, 1, -4, 1},
            new int[] {26},
            new int[] {1, -4, 1, -2, 1, -8, 1, -2, 1, -4, 1},
            new int[] {1, -4, 1, -2, 1, -8, 1, -2, 1, -4, 1},
            new int[] {6, -2, 4, -2, 4, -2, 6},
            new int[] {-5, 1, -14, 1, -5},
            new int[] {-5, 1, -14, 1, -5},
            new int[] {-5, 1, -14, 1, -5},
            new int[] {-5, 1, -14, 1, -5},
            new int[] {-5, 1, -14, 1, -5},
            new int[] {-5, 1, -14, 1, -5},
            new int[] {-5, 1, -14, 1, -5},
            new int[] {-5, 1, -14, 1, -5},
            new int[] {-5, 1, -14, 1, -5},
            new int[] {-5, 1, -14, 1, -5},
            new int[] {-5, 1, -14, 1, -5},
            new int[] {12, -2, 12},
            new int[] {1, -4, 1, -5, 1, -2, 1, -5, 1, -4, 1},
            new int[] {1, -4, 1, -5, 1, -2, 1, -5, 1, -4, 1},
            new int[] {-1, 2, -2, 7, -2, 7, -2, 2, -1},
            new int[] {-2, 1, -2, 1, -2, 1, -8, 1, -2, 1, -2, 1, -2},
            new int[] {-2, 1, -2, 1, -2, 1, -8, 1, -2, 1, -2, 1, -2},
            new int[] {6, -2, 4, -2, 4, -2, 6},
            new int[] {1, -10, 1, -2, 1, -10, 1},
            new int[] {1, -10, 1, -2, 1, -10, 1},
            new int[] {26}
        };

        /// <summary>
        /// Calculate if the element is in Field
        /// </summary>
        /// <param name="Figure">Element to check</param>
        /// <returns>True if is in field and false when not</returns>
        public static bool IsInField(int X, int Y, int Height, int Widht, bool HouseBorder = true)
        {
            // Map all values to map size
            decimal MapFactor = (decimal)Textures.Map.Height / 746;
            decimal MapedX = X * MapFactor;
            decimal MapedY = Y * MapFactor;
            decimal MapedWidht = Widht * MapFactor;
            decimal MapedHeight = Height * MapFactor;

            Rectangle Rect = new Rectangle((int)MapedX, (int)MapedY, (int)MapedWidht, (int)MapedHeight);
            Color Border = Color.FromArgb(33, 33, 222);     // Map border color
            Color ExtraBorderC = HouseBorder ? Color.FromArgb(255, 184, 222) : Color.Transparent;     // Color of ghosts house door 

            using (Bitmap Bmp = Textures.Map.Clone(Rect, Textures.Map.PixelFormat))
            {
                for (int y = 0; y < Bmp.Height; y++)
                {
                    for (int x = 0; x < Bmp.Width; x++)
                    {
                        if (Bmp.GetPixel(x, y) == Border || Bmp.GetPixel(x, y) == ExtraBorderC)
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
