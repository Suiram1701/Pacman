using Pacman.Style;
using Pacman.Style.Textures;
using System.Windows.Controls;

namespace Pacman.Collectable
{
    /// <summary>
    /// Interaktionslogik für Point.xaml
    /// </summary>
    public partial class Point : UserControl, ICollectable
    {
        private static readonly TextureHelper TextureHelper = new TextureHelper(Textures.Points, 12, 12, 1, 2);

        Points ICollectable.Point => Points.P10;

        public Point()
        {
            InitializeComponent();
            Texture.Source = TextureHelper[0];
        }
    }
}
