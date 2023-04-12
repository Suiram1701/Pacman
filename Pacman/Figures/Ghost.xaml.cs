﻿using Pacman.Style.Textures;
using Pacman.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.TextFormatting;
using System.Windows.Threading;
using static Pacman.Game;
using PathFinding;
using Pacman.Extension;
using Image = System.Windows.Controls.Image;
using System.ComponentModel;

namespace Pacman.Figures
{
    /// <summary>
    /// Interaktionslogik für Ghost.xaml
    /// </summary>
    public partial class Ghost : UserControl, IFigure
    {
        /// <summary>
        /// Color of this ghost
        /// </summary>
        public enum Colors
        {
            Red = 0,
            Purple = 1,
            Cyan = 2,
            Orange = 3
        }

        /// <summary>
        /// A Texture helper to manage figures textures
        /// </summary>
        private readonly static TextureHelper TextureHelper = new TextureHelper(Textures.Ghost, 14, 14, 5, 8);

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Colors), typeof(Ghost), new PropertyMetadata(Colors.Red));
        public Colors Color
        {
            get => (Colors)GetValue(ColorProperty);
            set
            {
                SetValue(ColorProperty, value);

                // Update texture
                AnimationKeyFrames[0].Value = TextureHelper[(int)value, (int)Direction - 1];
                AnimationKeyFrames[1].Value = TextureHelper[(int)value, (int)Direction + 3];
            }
        }

        public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register("Direction", typeof(Direction), typeof(Ghost), new PropertyMetadata(Direction.None));
        public Direction Direction
        {
            get => (Direction)GetValue(DirectionProperty);
            set
            {
                if (!IsAnimated)
                    return;

                int PreviewX = (int)Canvas.GetLeft(this) + (value == Direction.Left ? -20 : value == Direction.Right ? +20 : 0);     // X Position to check direction
                int PreviewY = (int)Canvas.GetTop(this) + (value == Direction.Up ? -20 : value == Direction.Down ? +20 : 0);     // Y Position to check direction

                // Check if direction go not in a wall
                if (!IsInField(PreviewX, PreviewY, (int)Height, (int)Width, HouseBorder: false))
                {
                    PreviewDirection = value;
                    return;
                }

                SetValue(DirectionProperty, value);

                // Reset tolerance
                ToleranceRounds = 0;
                PreviewDirection = Direction.None;

                if (IsOutside)
                    System.Diagnostics.Debug.WriteLine($"{(int)Color}, {(int)value - 1}");

                // Change texture direction
                if (value != 0)
                {
                    // Replace key frames and reload animation
                    Story.Stop();
                    AnimationKeyFrames[0].Value = TextureHelper[(int)Color, (int)value - 1];
                    AnimationKeyFrames[1].Value = TextureHelper[(int)Color, (int)value + 3];
                    Story.Begin();
                }
                else
                {
                    // Replace key frames and reload animation
                    Story.Stop();
                    AnimationKeyFrames[0].Value = TextureHelper[(int)Color, 1];
                    AnimationKeyFrames[1].Value = TextureHelper[(int)Color, 5];
                    Story.Begin();
                }
            }
        }

        /// <summary>
        /// Rounds for direction change tolerance
        /// </summary>
        private const int MaxToleranceRounds = 7;

        /// <summary>
        /// Clicked direction for tolerance
        /// </summary>
        private Direction PreviewDirection = Direction.None;
        private int ToleranceRounds = 0;

        /// <summary>
        /// <see langword="true"/> if the ghost is outside the house
        /// </summary>
        public static readonly DependencyProperty IsOutsideProperty = DependencyProperty.Register("IsOutside", typeof(bool), typeof(Ghost), new PropertyMetadata(false));
        public bool IsOutside
        {
            get => (bool)GetValue(IsOutsideProperty);
            set => SetValue(IsOutsideProperty, value);
        }

        /// <summary>
        /// Path to follow pacman
        /// </summary>
        private List<Point> Path = new List<Point>();

        public Timer Timer { get; } = new Timer(30);

        public Ghost()
        {
            InitializeComponent();

            // Setup keyframes
            ObjectAnimationUsingKeyFrames Animation = new ObjectAnimationUsingKeyFrames();
            Animation.Duration = TimeSpan.FromMilliseconds(150);
            Animation.RepeatBehavior = RepeatBehavior.Forever;

            Animation.KeyFrames.Add(new DiscreteObjectKeyFrame(TextureHelper[(int)Color, 1], TimeSpan.FromMilliseconds(0)));
            Animation.KeyFrames.Add(new DiscreteObjectKeyFrame(TextureHelper[(int)Color, 5], TimeSpan.FromMilliseconds(75)));
            AnimationKeyFrames = Animation.KeyFrames.Cast<ObjectKeyFrame>().ToList();

            // Setup animation
            Story.Children.Add(Animation);
            Storyboard.SetTarget(Animation, Texture);
            Storyboard.SetTargetProperty(Animation, new PropertyPath(Image.SourceProperty));

            // Setup movement timer
            Timer.Elapsed += MoveFigure;

            // Start only if it isnt in xaml designer
            Story.Begin();
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;
            Timer.Start();
        }

        #region Animation and movement
        /// <summary>
        /// Movefigure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveFigure(object sender, ElapsedEventArgs e)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    if (!IsOutside)     // Only move when outside
                        return;

                    // If path followed get new path
                    PathFinding:
                    if (Path.Count <= 0 && IsOutside)
                    {
                        Path = new PathFinder(this.GetPosition(), Pacman.Instance.GetPosition()).GetPath().Take(10).ToList();     // Follow only last 10 points
#if DEBUG
                        System.Diagnostics.Debug.WriteLine($"<!--     {Color}: Next path!     --!>");
#endif
                    }
#if DEBUG
                    Point a = this.GetPosition();
                    Point b = Path[0];
                    double _distanceX_ = a.X >= b.X ? a.X - b.X : b.X - a.X;
                    double _distanceY_ = a.Y >= b.Y ? a.Y - b.Y : b.Y - a.Y;
                    double _distance_ = Math.Sqrt(Math.Pow(_distanceX_, 2) + Math.Pow(_distanceY_, 2));
                    System.Diagnostics.Debug.WriteLine($"<!-- {Color}: Distance to next path point = {_distance_}. Own Position = {this.GetPosition()}     --!>");
#endif
                    // Set direction with path
                    if (this.GetPosition() != Path[0] && IsOutside)     // Direct to next path point
                    {
                        if (this.GetPosition().X > Path[0].X)     // Left
                            Direction = Direction.Left;

                        else if (this.GetPosition().Y < Path[0].Y)     // Down
                            Direction = Direction.Down;

                        else if (this.GetPosition().X < Path[0].X)     // Right
                            Direction = Direction.Right;

                        else if (this.GetPosition().Y > Path[0].Y)     // Up
                            Direction = Direction.Up;
                    }
                    else     // Remove path pont
                    {
#if DEBUG
                        System.Diagnostics.Debug.WriteLine($"<!-- {Color}: Next point!     --!>");
#endif
                        Path.Remove(Path[0]);
                        goto PathFinding;
                    }

                    // Direction chage tolerance
                    if (PreviewDirection != Direction.None)
                    {
                        int PreviewX = (int)Canvas.GetLeft(this) + (PreviewDirection == Direction.Left ? -20 : PreviewDirection == Direction.Right ? +20 : 0);     // X Position to check tolerance direction
                        int PreviewY = (int)Canvas.GetTop(this) + (PreviewDirection == Direction.Up ? -20 : PreviewDirection == Direction.Down ? +20 : 0);     // Y Position to check tolerance direction

                        // Check if tolerance is valid
                        if (IsInField(PreviewX, PreviewY, (int)Height, (int)Width, HouseBorder: false))
                            Direction = PreviewDirection;
                        else
                            ToleranceRounds++;

                        // If tolerance is over limit reset tolerance
                        if (ToleranceRounds > MaxToleranceRounds)
                        {
                            ToleranceRounds = 0;
                            PreviewDirection = Direction.None;
                        }
                    }

                    // Teleport between the two sides
                    if (Canvas.GetLeft(this) > 608 && Canvas.GetTop(this) >= 313 && Canvas.GetTop(this) <= 387)
                        Canvas.SetLeft(this, 8);

                    else if (Canvas.GetLeft(this) < 8 && Canvas.GetTop(this) >= 313 && Canvas.GetTop(this) <= 387)
                        Canvas.SetLeft(this, 608);

                    switch (Direction)     // Check if figure dont hit a wall and move
                    {
                        case Direction.Left:
                            if (!IsInField((int)Canvas.GetLeft(this) - 7, (int)Canvas.GetTop(this), (int)Height, (int)Width, HouseBorder: false))
                                goto Stop;
                            Canvas.SetLeft(this, Canvas.GetLeft(this) - 7);
                            break;
                        case Direction.Down:
                            if (!IsInField((int)Canvas.GetLeft(this), (int)Canvas.GetTop(this) + 10, (int)Height, (int)Width, HouseBorder: false))
                                goto Stop;
                            Canvas.SetTop(this, Canvas.GetTop(this) + 7);
                            break;
                        case Direction.Right:
                            if (!IsInField((int)Canvas.GetLeft(this) + 7, (int)Canvas.GetTop(this), (int)Height, (int)Width, HouseBorder: false))
                                goto Stop;
                            Canvas.SetLeft(this, Canvas.GetLeft(this) + 7);
                            break;
                        case Direction.Up:
                            if (!IsInField((int)Canvas.GetLeft(this), (int)Canvas.GetTop(this) - 10, (int)Height, (int)Width, HouseBorder: false))
                                goto Stop;
                            Canvas.SetTop(this, Canvas.GetTop(this) - 7);
                            break;
                    }
                    Story.Resume();
                    return;

                Stop:     // Figure stop
                    Story.Pause();
                    Texture.Source = TextureHelper[(int)Direction, 1];

                }, DispatcherPriority.Render);
            }
            catch (TaskCanceledException)
            {

            }
        }

        public bool IsAnimated { get; set; } = true;

        public Storyboard Story { get; } = new Storyboard();

        /// <summary>
        /// All keyframes of the animation to edit animation
        /// </summary>
        List<ObjectKeyFrame> AnimationKeyFrames { get; }

        public void Start()
        {
            if (!IsAnimated)
            {
                Story.Resume();
                Timer.Start();
            }

            IsAnimated = true;
        }

        public void Stop()
        {
            if (IsAnimated)
            {
                Story.Pause();
                Timer.Stop();
                Direction = Direction.None;
            }

            IsAnimated = false;
        }
        #endregion
    }
}
