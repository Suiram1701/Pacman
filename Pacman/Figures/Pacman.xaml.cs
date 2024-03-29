﻿using Pacman.Style;
using Pacman.Style.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using static Pacman.Game;
using Timer = System.Timers.Timer;

namespace Pacman.Figures
{
    /// <summary>
    /// Interaktionslogik für Pacman.xaml
    /// </summary>
    public partial class Pacman : UserControl, IFigure
    {
        public static Pacman Instance { get; private set; }

        /// <summary>
        /// A Texture helper to manage figures textures
        /// </summary>
        private static readonly TextureHelper TextureHelper = new TextureHelper(Textures.Pacman, 22, 22, 6, 11);

        /// <summary>
        /// The current direction of pacman
        /// </summary>
        public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register("Direction", typeof(Direction), typeof(Pacman), new PropertyMetadata(Direction.None));
        public Direction Direction
        {
            get => (Direction)GetValue(DirectionProperty);
            set
            {
                if (!IsAnimated && _IsDying)
                    return;

                int PreviewX = (int)Canvas.GetLeft(this) + (value == Direction.Left ? -20 : value == Direction.Right ? +20 : 0);     // X Position to check direction
                int PreviewY = (int)Canvas.GetTop(this) + (value == Direction.Up ? -20 : value == Direction.Down ? +20 : 0);     // Y Position to check direction

                // Check if direction go not in a wall
                if (!IsInField(PreviewX, PreviewY, (int)Height, (int)Width))
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

        /// <summary>
        /// Rounds for direction change tolerance (help for player)
        /// </summary>
        private const int MaxToleranceRounds = 7;

        /// <summary>
        /// Clicked direction for tolerance
        /// </summary>
        private Direction PreviewDirection = Direction.None;
        private int ToleranceRounds = 0;

        public Timer Timer { get; } = new Timer(20);

        public Pacman()
        {
            InitializeComponent();
            Instance = this;

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
        }

        void IFigure.Reset()
        {
            // Reset
            _IsDying = false;
            ToleranceRounds = 0;

            // Reset Direct and texture
            Direction = Direction.None;
            AnimationKeyFrames[0].Value = TextureHelper[0, 0];
            AnimationKeyFrames[1].Value = TextureHelper[0, 0];
            AnimationKeyFrames[2].Value = TextureHelper[0, 0];
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
                    if (_IsDying)
                        return;

                    // Direction chage tolerance
                    if (PreviewDirection != Direction.None)
                    {
                        int PreviewX = (int)Canvas.GetLeft(this) + (PreviewDirection == Direction.Left ? -20 : PreviewDirection == Direction.Right ? +20 : 0);     // X Position to check tolerance direction
                        int PreviewY = (int)Canvas.GetTop(this) + (PreviewDirection == Direction.Up ? -20 : PreviewDirection == Direction.Down ? +20 : 0);     // Y Position to check tolerance direction

                        // Check if tolerance is valid
                        if (IsInField(PreviewX, PreviewY, (int)Height, (int)Width))
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
                            if (!IsInField((int)Canvas.GetLeft(this) - 7, (int)Canvas.GetTop(this), (int)Height, (int)Width))
                                goto Stop;
                            Canvas.SetLeft(this, Canvas.GetLeft(this) - 5);
                            break;
                        case Direction.Down:
                            if (!IsInField((int)Canvas.GetLeft(this), (int)Canvas.GetTop(this) + 10, (int)Height, (int)Width))
                                goto Stop;
                            Canvas.SetTop(this, Canvas.GetTop(this) + 5);
                            break;
                        case Direction.Right:
                            if (!IsInField((int)Canvas.GetLeft(this) + 7, (int)Canvas.GetTop(this), (int)Height, (int)Width))
                                goto Stop;
                            Canvas.SetLeft(this, Canvas.GetLeft(this) + 5);
                            break;
                        case Direction.Up:
                            if (!IsInField((int)Canvas.GetLeft(this), (int)Canvas.GetTop(this) - 10, (int)Height, (int)Width))
                                goto Stop;
                            Canvas.SetTop(this, Canvas.GetTop(this) - 5);
                            break;
                    }
                    Story.Resume();
                    return;

                Stop:     // Pacman stop
                    Story.Pause();
                    Texture.Source = TextureHelper[(int)Direction, 0];

                }, DispatcherPriority.Render);
            }
            catch (TaskCanceledException)
            {

            }
        }

        private bool _IsDying = false;
        public void DieAnimation(EventHandler Competed)
        {
            // Setup animation
            ObjectAnimationUsingKeyFrames Animation = new ObjectAnimationUsingKeyFrames();
            Animation.Duration = TimeSpan.FromMilliseconds(2400);
            Animation.KeyFrames.Add(new DiscreteObjectKeyFrame(TextureHelper[5, 0], TimeSpan.FromMilliseconds(200)));
            Animation.KeyFrames.Add(new DiscreteObjectKeyFrame(TextureHelper[5, 1], TimeSpan.FromMilliseconds(400)));
            Animation.KeyFrames.Add(new DiscreteObjectKeyFrame(TextureHelper[5, 2], TimeSpan.FromMilliseconds(600)));
            Animation.KeyFrames.Add(new DiscreteObjectKeyFrame(TextureHelper[5, 3], TimeSpan.FromMilliseconds(800)));
            Animation.KeyFrames.Add(new DiscreteObjectKeyFrame(TextureHelper[5, 4], TimeSpan.FromMilliseconds(1000)));
            Animation.KeyFrames.Add(new DiscreteObjectKeyFrame(TextureHelper[5, 5], TimeSpan.FromMilliseconds(1200)));
            Animation.KeyFrames.Add(new DiscreteObjectKeyFrame(TextureHelper[5, 6], TimeSpan.FromMilliseconds(1400)));
            Animation.KeyFrames.Add(new DiscreteObjectKeyFrame(TextureHelper[5, 7], TimeSpan.FromMilliseconds(1600)));
            Animation.KeyFrames.Add(new DiscreteObjectKeyFrame(TextureHelper[5, 8], TimeSpan.FromMilliseconds(1800)));
            Animation.KeyFrames.Add(new DiscreteObjectKeyFrame(TextureHelper[5, 9], TimeSpan.FromMilliseconds(2000)));
            Animation.KeyFrames.Add(new DiscreteObjectKeyFrame(TextureHelper[5, 10], TimeSpan.FromMilliseconds(2200)));
            Animation.Completed += Competed;

            Storyboard Story = new Storyboard();
            Story.Children.Add(Animation);
            Storyboard.SetTarget(Animation, Texture);
            Storyboard.SetTargetProperty(Animation, new PropertyPath(Image.SourceProperty));

            // Start
            _IsDying = true;
            Stop();
            Story.Begin();
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
