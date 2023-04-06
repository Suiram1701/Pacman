using Packman.Style.Textures;
using Pacman.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Timers;
using System.Windows.Threading;
using static Pacman.Game;

namespace Pacman.Figures
{
    /// <summary>
    /// Interaktionslogik für Pacman.xaml
    /// </summary>
    public partial class Pacman : UserControl, IFigure
    {
        /// <summary>
        /// A Texture helper to manage figures textures
        /// </summary>
        private static TextureHelper TextureHelper { get; } = new TextureHelper(Textures.Pacman, 22, 22, 5, 2);

        /// <summary>
        /// The current direction of pacman
        /// </summary>
        public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register("Direction", typeof(Direction), typeof(Pacman), new PropertyMetadata(Direction.None));
        public Direction Direction
        {
            get => (Direction)GetValue(DirectionProperty);
            set
            {
                // Check if direction go not in a wall
                if (!IsInField((int)Canvas.GetLeft(this), (int)Canvas.GetTop(this), (int)Height, (int)Width))
                    return;

                SetValue(DirectionProperty, value);

                // Change texture direction
                if (value != 0)
                {
                    // Replace key frames and reload animation
                    Story.Stop();
                    AnimationKeyFrames[0].Value = TextureHelper[0, 0];
                    AnimationKeyFrames[1].Value = TextureHelper[(int)value, 0];
                    AnimationKeyFrames[2].Value = TextureHelper[(int)value, 1];
                    Story.Begin();
                }
                else
                {
                    // Replace key frames and reload animation
                    Story.Stop();
                    AnimationKeyFrames[0].Value = TextureHelper[0, 0];
                    AnimationKeyFrames[1].Value = TextureHelper[0, 0];
                    AnimationKeyFrames[2].Value = TextureHelper[0, 0];
                    Story.Begin();
                }
            }
        }

        public Timer Timer { get; } = new Timer(50);

        public Pacman()
        {
            InitializeComponent();

            // Setup keyframes
            ObjectAnimationUsingKeyFrames Animation = new ObjectAnimationUsingKeyFrames();
            Animation.Duration = TimeSpan.FromMilliseconds(150);
            Animation.RepeatBehavior = RepeatBehavior.Forever;

            Animation.KeyFrames.Add(new DiscreteObjectKeyFrame(TextureHelper[0, 0], TimeSpan.FromMilliseconds(0)));
            Animation.KeyFrames.Add(new DiscreteObjectKeyFrame(TextureHelper[0, 0], TimeSpan.FromMilliseconds(50)));
            Animation.KeyFrames.Add(new DiscreteObjectKeyFrame(TextureHelper[0, 0], TimeSpan.FromMilliseconds(100)));
            AnimationKeyFrames = Animation.KeyFrames.Cast<ObjectKeyFrame>().ToList();

            // Setup animation
            Story.Children.Add(Animation);
            Storyboard.SetTarget(Animation, Texture);
            Storyboard.SetTargetProperty(Animation, new PropertyPath(Image.SourceProperty));

            // Setup movement timer
            Timer.Elapsed += MoveFigure;

            // Start
            Story.Begin();
            Timer.Start();
            return;
        }

        #region Animation and movement
        /// <summary>
        /// Movefigure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveFigure(object sender, ElapsedEventArgs e) =>
            Dispatcher.Invoke(() =>
            {
                switch (Direction)     // Check if figure dont hit a wall and move
                {
                    case Direction.Left:
                        if (!IsInField((int)Canvas.GetLeft(this) - 10, (int)Canvas.GetTop(this), (int)Height, (int)Width))
                            return;
                        Canvas.SetLeft(this, Canvas.GetLeft(this) - 10);
                        break;
                    case Direction.Down:
                        if (!IsInField((int)Canvas.GetLeft(this), (int)Canvas.GetTop(this) + 10, (int)Height, (int)Width))
                            return;
                        Canvas.SetTop(this, Canvas.GetTop(this) + 10);
                        break;
                    case Direction.Right:
                        if (!IsInField((int)Canvas.GetLeft(this) + 10, (int)Canvas.GetTop(this), (int)Height, (int)Width))
                            return;
                        Canvas.SetLeft(this, Canvas.GetLeft(this) + 10);
                        break;
                    case Direction.Up:
                        if (!IsInField((int)Canvas.GetLeft(this), (int)Canvas.GetTop(this) - 10, (int)Height, (int)Width))
                            return;
                        Canvas.SetTop(this, Canvas.GetTop(this) - 10);
                        break;
                }
            }, DispatcherPriority.Render);

        public bool IsAnimated { get; set; }

        public Storyboard Story { get; } = new Storyboard();

        /// <summary>
        /// All keyframes of the animation to edit animation
        /// </summary>
        List<ObjectKeyFrame> AnimationKeyFrames { get; }

        public void Start()
        {
            if (!IsAnimated)
            {
                Story.Begin();
                Timer.Start();
            }

            IsAnimated = true;
        }

        public void Stop()
        {
            if (IsAnimated)
            {
                Story.Stop();
                Timer.Stop();
            }

            IsAnimated = false;
        }
        #endregion
    }
}
