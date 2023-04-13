using Pacman.Style.Textures;
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
using Pacman.PathFinding;
using Pacman.Extension;
using Image = System.Windows.Controls.Image;
using System.ComponentModel;
using System.Diagnostics;

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
                AnimationKeyFrames[0].Value = TextureHelper[!IsEatable ? (int)value : 4, !IsEatable ? (int)Direction - 1 : !_CurrentWhite ? 0 : 2];
                AnimationKeyFrames[1].Value = TextureHelper[!IsEatable ? (int)value : 4, !IsEatable ? (int)Direction + 3 : !_CurrentWhite ? 1 : 3];
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

                int PreviewX = (int)Canvas.GetLeft(this) + (value == Direction.Left ? -25 : value == Direction.Right ? +25 : 0);     // X Position to check direction
                int PreviewY = (int)Canvas.GetTop(this) + (value == Direction.Up ? -25 : value == Direction.Down ? +25 : 0);     // Y Position to check direction

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

                // Change texture direction
                if (value != 0)
                {
                    // Replace key frames and reload animation
                    Story.Stop();
                    AnimationKeyFrames[0].Value = TextureHelper[!IsEatable ? (int)Color : 4, !IsEatable ? (int)value - 1 : !_CurrentWhite ? 0 : 2];
                    AnimationKeyFrames[1].Value = TextureHelper[!IsEatable ? (int)Color : 4, !IsEatable ? (int)value + 3 : !_CurrentWhite ? 1 : 3];
                    Story.Begin();
                }
                else
                {
                    // Replace key frames and reload animation
                    Story.Stop();
                    AnimationKeyFrames[0].Value = TextureHelper[!IsEatable ? (int)Color : 4, !IsEatable ? 1 : !_CurrentWhite ? 0 : 2];
                    AnimationKeyFrames[1].Value = TextureHelper[!IsEatable ? (int)Color : 4, !IsEatable ? 5 : !_CurrentWhite ? 1 : 3];
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

        #region Eatable
        public static readonly DependencyProperty IsEatableProperty = DependencyProperty.Register("IsEatable", typeof(bool), typeof (Ghost), new PropertyMetadata(false));
        public bool IsEatable
        {
            get => (bool)GetValue(IsEatableProperty);
            set
            {
                SetValue(IsEatableProperty, value);

                // Change tetxure
                AnimationKeyFrames[0].Value = TextureHelper[!IsEatable ? (int)Color : 4, !IsEatable ? (int)Direction - 1 : 0];
                AnimationKeyFrames[1].Value = TextureHelper[!IsEatable ? (int)Color : 4, !IsEatable ? (int)Direction + 3 : 1];

                // If starting start timer
                if (value)
                    _EatTimer.Start();
                else
                    _EatTimer.Stop();
            }
        }

        /// <summary>
        /// Trigger for blinking ghost and disable eatable state after 5s
        /// </summary>
        private readonly Timer _EatTimer = new Timer(1000);

        /// <summary>
        /// Is eatable ghost current white
        /// </summary>
        private bool _CurrentWhite = false;

        /// <summary>
        /// How many secounds after begin eatable state
        /// </summary>
        private int _EatTriggerd = 0;

        /// <summary>
        /// Blinking ghosts and end state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void EatTimer(object sender, ElapsedEventArgs e) =>
            Dispatcher.Invoke(() =>
            {
                // End state after 5s
                if (_EatTriggerd > 5)
                {
                    // Reset
                    _EatTriggerd = 0;

                    IsEatable = false;
                    return;
                }

                // Blinking ghost
                if (_CurrentWhite)
                {
                    // Reload ani
                    Story.Stop();
                    AnimationKeyFrames[0].Value = TextureHelper[4, 0];
                    AnimationKeyFrames[1].Value = TextureHelper[4, 1];
                    Story.Begin();
                    _CurrentWhite = false;
                }
                else
                {
                    // Reload ani
                    Story.Stop();
                    AnimationKeyFrames[0].Value = TextureHelper[4, 2];
                    AnimationKeyFrames[1].Value = TextureHelper[4, 3];
                    Story.Begin();
                    _CurrentWhite = true;
                }

                _EatTriggerd++;
            });

        #endregion

        /// <summary>
        /// Next point to pacman
        /// </summary>
        public Point? Point { get; set; } = null;

        /// <summary>
        /// Trigger actions to move ghost
        /// </summary>
        public Timer Timer { get; } = new Timer(25);

        private readonly Stopwatch Sw = new Stopwatch();

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

            // EatTimer
            _EatTimer.Elapsed += EatTimer;

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
                    Sw.Start();
                    if (Point == null)
                        Point = new PathFinder(this.GetPosition(), Pacman.Instance.GetPosition()).GetNextPoint();
#if DEBUG
                    // If ghost is for more than 1s stuck goto next path point
                    if (Sw.ElapsedMilliseconds > 2500)
                    {
#if DEBUG
                        System.Diagnostics.Debug.WriteLine($"<!--     {Color}: RESET!     --!>");
#endif
                        Sw.Reset();
                        Point = null;
                        goto PathFinding;
                    }

                    Point a = this.GetPosition();
                    Point b = Point ?? throw new ArgumentNullException(nameof(Point));
                    double _distanceX_ = a.X >= b.X ? a.X - b.X : b.X - a.X;
                    double _distanceY_ = a.Y >= b.Y ? a.Y - b.Y : b.Y - a.Y;
                    double _distance_ = Math.Sqrt(Math.Pow(_distanceX_, 2) + Math.Pow(_distanceY_, 2));
                    System.Diagnostics.Debug.WriteLine($"<!-- {Color}: Distance to next path point = {_distance_}. Own Position = {this.GetPosition()}     --!>");
#endif
                    // Set direction with path
                    if (this.GetPosition() != Point && IsOutside)     // Direct to next path point
                    {
                        if (this.GetPosition().X > Point?.X)     // Left
                            Direction = Direction.Left;

                        else if (this.GetPosition().Y < Point?.Y)     // Down
                            Direction = Direction.Down;

                        else if (this.GetPosition().X < Point?.X)     // Right
                            Direction = Direction.Right;

                        else if (this.GetPosition().Y > Point?.Y)     // Up
                            Direction = Direction.Up;
                    }
                    else     // Remove path pont
                    {
#if DEBUG
                        System.Diagnostics.Debug.WriteLine($"<!-- {Color}: Next point!     --!>");
#endif
                        Point = null;
                        Sw.Reset();
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

                    switch (Direction)     // Check if figure dont hit a wall and move
                    {
                        case Direction.Left:
                            if (!IsInField((int)Canvas.GetLeft(this) - 7, (int)Canvas.GetTop(this), (int)Height, (int)Width, HouseBorder: false))
                                goto Stop;
                            Canvas.SetLeft(this, Canvas.GetLeft(this) - 5);
                            break;
                        case Direction.Down:
                            if (!IsInField((int)Canvas.GetLeft(this), (int)Canvas.GetTop(this) + 10, (int)Height, (int)Width, HouseBorder: false))
                                goto Stop;
                            Canvas.SetTop(this, Canvas.GetTop(this) + 5);
                            break;
                        case Direction.Right:
                            if (!IsInField((int)Canvas.GetLeft(this) + 7, (int)Canvas.GetTop(this), (int)Height, (int)Width, HouseBorder: false))
                                goto Stop;
                            Canvas.SetLeft(this, Canvas.GetLeft(this) + 5);
                            break;
                        case Direction.Up:
                            if (!IsInField((int)Canvas.GetLeft(this), (int)Canvas.GetTop(this) - 10, (int)Height, (int)Width, HouseBorder: false))
                                goto Stop;
                            Canvas.SetTop(this, Canvas.GetTop(this) - 5);
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
                // Reset all
                Point = null;
                Story.Pause();
                Timer.Stop();
                Direction = Direction.None;
            }

            IsAnimated = false;
        }
        #endregion
    }
}
