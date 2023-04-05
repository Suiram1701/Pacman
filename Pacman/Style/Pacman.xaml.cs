using Packman.Style.Textures;
using Pacman.Style;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Pacman.Style
{
    /// <summary>
    /// Interaktionslogik für Pacman.xaml
    /// </summary>
    public partial class Pacman : UserControl, IFigure
    {
        private static TextureHelper TextureHelper { get; } = new TextureHelper(Textures.Pacman, 22, 22, 5, 2);

        public Pacman()
        {
            InitializeComponent();

            // Setup animation
            ObjectAnimationUsingKeyFrames Animation = new ObjectAnimationUsingKeyFrames();
            Animation.Duration = TimeSpan.FromMilliseconds(250);
            Animation.RepeatBehavior = RepeatBehavior.Forever;

            Animation.KeyFrames.Add(new DiscreteObjectKeyFrame(TextureHelper[1, 0], TimeSpan.FromMilliseconds(0)));
            Animation.KeyFrames.Add(new DiscreteObjectKeyFrame(TextureHelper[1, 1], TimeSpan.FromMilliseconds(125)));

            Storyboard Story = new Storyboard();
            Story.Children.Add(Animation);
            Storyboard.SetTarget(Animation, Texture);
            Storyboard.SetTargetProperty(Animation, new PropertyPath(Image.SourceProperty));

            Story.Begin();
        }

        #region Animation
        public bool IsAnimated { get; set; }

        public void StartAnimation()
        {
            IsAnimated = true;
        }

        public void EndAnimation()
        {
            IsAnimated = false;
        }
        #endregion
    }
}
