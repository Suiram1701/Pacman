using Pacman.Style;
using Pacman.Style.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace Pacman.Collectable
{
    /// <summary>
    /// Interaktionslogik für Energizer.xaml
    /// </summary>
    public partial class Energizer : UserControl, ICollectable
    {
        private static readonly TextureHelper TextureHelper = new TextureHelper(Textures.Points, 12, 12, 1, 3);

        private Storyboard Story = new Storyboard();

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
