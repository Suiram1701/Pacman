using Pacman.Style;
using Pacman.Style.Textures;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Pacman.Collectable
{
    /// <summary>
    /// Interaktionslogik für Energizer.xaml
    /// </summary>
    public partial class Energizer : UserControl, ICollectable
    {
        private static readonly TextureHelper TextureHelper = new TextureHelper(Textures.Points, 12, 12, 1, 3);

        private readonly Storyboard Story = new Storyboard();

        Points ICollectable.Point => Points.P50;

        public Energizer()
        {
            InitializeComponent();

            // Setup keyframes
            ObjectAnimationUsingKeyFrames Animation = new ObjectAnimationUsingKeyFrames();
            Animation.Duration = TimeSpan.FromMilliseconds(500);
            Animation.RepeatBehavior = RepeatBehavior.Forever;

            Animation.KeyFrames.Add(new DiscreteObjectKeyFrame(TextureHelper[1], TimeSpan.FromMilliseconds(0)));
            Animation.KeyFrames.Add(new DiscreteObjectKeyFrame(TextureHelper[2], TimeSpan.FromMilliseconds(250)));

            // Setup animation
            Story.Children.Add(Animation);
            Storyboard.SetTarget(Animation, Texture);
            Storyboard.SetTargetProperty(Animation, new PropertyPath(Image.SourceProperty));

            // Start
            Texture.Source = TextureHelper[1];
            Story.Begin();
        }
    }
}
