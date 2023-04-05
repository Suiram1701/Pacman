using Pacman.Style.Textures;
using Pacman.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Pacman.Figures
{
    /// <summary>
    /// Interaktionslogik für Pacman.xaml
    /// </summary>
    public partial class Pacman : UserControl, IFigure
    {
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
                SetValue(DirectionProperty, value);

                // Change texture direction
                if (value != 0)
                {
                    AnimationKeyFrames[0].Value = TextureHelper[0, 0];
                    AnimationKeyFrames[1].Value = TextureHelper[(int)value, 0];
                    AnimationKeyFrames[2].Value = TextureHelper[(int)value, 1];
                }
                else
                {
                    AnimationKeyFrames[0].Value = TextureHelper[0, 0];
                    AnimationKeyFrames[1].Value = TextureHelper[0, 0];
                    AnimationKeyFrames[2].Value = TextureHelper[0, 0];
                }
            }
        }

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

            // Start
            Story.Begin();
        }

        #region Animation
        public bool IsAnimated { get; set; }

        public Storyboard Story { get; } = new Storyboard();

        /// <summary>
        /// All keyframes of the animation to edit animation
        /// </summary>
        List<ObjectKeyFrame> AnimationKeyFrames { get; }

        public void StartAnimation()
        {
            if (!IsAnimated)
                Story.Begin();
            IsAnimated = true;
        }

        public void EndAnimation()
        {
            if (IsAnimated)
                Story.Stop();
            IsAnimated = false;
        }
        #endregion
    }
}
