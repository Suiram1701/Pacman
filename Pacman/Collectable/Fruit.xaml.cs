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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pacman.Collectable
{
    /// <summary>
    /// Interaktionslogik für Fruit.xaml
    /// </summary>
    public partial class Fruit : UserControl, ICollectable
    {
        /// <summary>
        /// All fruits. Their int values are their texture helper index
        /// </summary>
        public enum Fruits
        {
            Cherry = 0,
            Strawberry = 1,
            Peach = 2,
            Apple = 3,
            Grape = 4,
            Galaxian = 5,
            Bell = 6,
            Key = 7
        }

        /// <summary>
        /// Points per fruit.
        /// <see cref="Fruits"/> int value equals index here
        /// </summary>
        public static readonly Points[] FruitPoints =
        {
            Points.P100,
            Points.P300,
            Points.P500,
            Points.P700,
            Points.P1000,
            Points.P2000,
            Points.P3000,
            Points.P5000
        };

        private static readonly TextureHelper TextureHelper = new TextureHelper(Textures.Fruits, 13, 13, 1, 8);

        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(Fruits), typeof(Fruits), new PropertyMetadata(Fruits.Cherry));
        public Fruits Type
        {
            get => (Fruits)GetValue(TypeProperty);
            set
            {
                SetValue(TypeProperty, value);
                Texture.Source = TextureHelper[(int)value];
            }
        }

        public Points Point => FruitPoints[(int)Type];

        public Fruit()
        {
            InitializeComponent();
        }
    }
}
