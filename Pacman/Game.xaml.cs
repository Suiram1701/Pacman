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
using System.ComponentModel;
using System.Security.Cryptography;
using System.Threading;
using Timer = System.Timers.Timer;
using System.Windows.Media.Animation;
using System.Windows.Media.TextFormatting;

namespace Pacman
{
    /// <summary>
    /// Interaktionslogik für Game.xaml
    /// </summary>
    public partial class Game : Window
    {
        #region Datacontext
        /// <summary>
        /// Players current score
        /// </summary>
        public int Points
        {
            get => int.Parse(CPoints.Content.ToString());
            set
            {
                CPoints.Content = value;

                // If you have more points than the highscore override it
                if (Points > Hightscore)
                    Hightscore = Points;
            }
        }

        /// <summary>
        /// Highscore on this windows account
        /// </summary>
        public int Hightscore
        {
            get => int.Parse(CHightscore.Content.ToString());
            set
            {
                CHightscore.Content = value;

                // Save highscore
                Default.Highscore = value;
                using (SHA256 SHA = SHA256.Create())
                    Default.Highscore_sha = Convert.ToBase64String(SHA.ComputeHash(Encoding.UTF8.GetBytes(value.ToString())));

                Default.Save();
            }
        }

        /// <summary>
        /// Current playing level
        /// </summary>
        public int Level
        {
            get => int.Parse(CLevel.Content.ToString());
            set => CLevel.Content = value;
        }
        #endregion

        /// <summary>
        /// Timer for some mechanics
        /// </summary>
        private readonly Timer GameLoop = new Timer(50);

        private readonly Storyboard Story = new Storyboard();

        public Game()
        {
            InitializeComponent();

            // Load highscore and check if it is valid
            string sha;
            using (SHA256 SHA = SHA256.Create())
                sha = Convert.ToBase64String(SHA.ComputeHash(Encoding.UTF8.GetBytes(Default.Highscore.ToString())));

            if (sha == Default.Highscore_sha)
                CHightscore.Content = Default.Highscore.ToString();
            else
                Hightscore = 0;

            // Setup keyframes
            ObjectAnimationUsingKeyFrames Animation = new ObjectAnimationUsingKeyFrames();
            Animation.KeyFrames.Add(new DiscreteObjectKeyFrame("3", TimeSpan.FromSeconds(0)));
            Animation.KeyFrames.Add(new DiscreteObjectKeyFrame("2", TimeSpan.FromSeconds(1)));
            Animation.KeyFrames.Add(new DiscreteObjectKeyFrame("1", TimeSpan.FromSeconds(2)));
            Animation.KeyFrames.Add(new DiscreteObjectKeyFrame("GO!", TimeSpan.FromSeconds(3)));
            Animation.KeyFrames.Add(new DiscreteObjectKeyFrame(null, TimeSpan.FromSeconds(4)));

            // Setup story
            Story.Duration = TimeSpan.FromSeconds(4);
            Story.Children.Add(Animation);
            Storyboard.SetTarget(Animation, Counter);
            Storyboard.SetTargetProperty(Animation, new PropertyPath(ContentProperty));
            Story.Completed += Counter_Completet;

            // Init map
            ResetFigures();
            ResetPoints();
            Story.Begin();

            // Setup game loop
            GameLoop.Elapsed += PointCheck;
            GameLoop.Elapsed += LevelCheck;
            GameLoop.Start();
        }

        /// <summary>
        /// Start game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Counter_Completet(object sender, EventArgs e)
        {
            foreach (IFigure Figure in Canvas.Children.OfType<IFigure>())
                Figure.Start();
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

            // Stop figures
            foreach (IFigure Figure in Canvas.Children.OfType<IFigure>())
                Figure.Stop();
        }

        #region Points
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
        /// Spawn a fruit on random point in map
        /// </summary>
        /// <param name="Type">Fruit to spawn</param>
        private void SpwanFruit(Fruit.Fruits Type)
        {
            Spawn:
            // Random Position
            Random Rnd = new Random();
            int RndY = Rnd.Next(PointSpawning.Length);

            // All valid X position for this row
            List<int> ValidX = new List<int>();
            int TempXPos = 0;
            foreach (int Colomn in PointSpawning[RndY])
            {
                if (Colomn > 0)
                    for (int i = 0; i < Colomn; i++)
                    {
                        ValidX.Add(TempXPos);
                        TempXPos++;
                    }
                else
                    TempXPos += Math.Abs(Colomn);
            }
            int RndX = ValidX[Rnd.Next(ValidX.Count)];

            // Calc spawn position
            Point SpawnPosition = new Point(20 + (RndX * Distance) , 19 + (RndY * Distance));

            // Add fruit and positioning it
            int Index = Canvas.Children.Add(new Fruit()
            {
                Type = Type
            });
            Canvas.SetLeft(Canvas.Children[Index], SpawnPosition.X);
            Canvas.SetTop(Canvas.Children[Index], SpawnPosition.Y);
            Panel.SetZIndex(Canvas.Children[Index], 1);

            // Get all collectable near from placed fruit
            IEnumerable<ICollectable> Items = Canvas.Children.OfType<ICollectable>()
                .Where(Object => Math.Abs(Canvas.GetLeft((UIElement)Object) - Canvas.GetLeft(Canvas.Children[Index]) - 20) <= 20 &&
                Math.Abs(Canvas.GetTop((UIElement)Object) - Canvas.GetTop(Canvas.Children[Index]) - 20) <= 20 && Object != Canvas.Children[Index])
                .ToList();

            // If there is allready a fruit positioning again or if there is an energizer
            if (Items.Any(Item => Item is Fruit || Item is Energizer))
                goto Spawn;

            // Remove points on fruits position
            foreach (ICollectable item in Items)
                Canvas.Children.Remove((UIElement)item);
        }

        /// <summary>
        /// Check if pacman can eat a point
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PointCheck(object sender, ElapsedEventArgs e)
        {
            try
            {
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
            }
            catch (TaskCanceledException)
            {

            }
        }

        /// <summary>
        /// Check if level is done
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LevelCheck(object sender, ElapsedEventArgs e)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    // If level is done reset all
                    if (Canvas.Children.OfType<ICollectable>().Count() == 0)
                    {
                        Level++;

                        ResetFigures();
                        foreach (IFigure Figure in Canvas.Children.OfType<IFigure>())
                            Figure.Stop();

                        ResetPoints();


                        Story.Begin();
                    }
                }, DispatcherPriority.Loaded);
            }
            catch (TaskCanceledException)
            {

            }
        }

        /// <summary>
        /// Distance between points
        /// </summary>
        private const int Distance = 24;

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
        #endregion

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

            try
            {
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
            catch
            {
                return false;
            }
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
