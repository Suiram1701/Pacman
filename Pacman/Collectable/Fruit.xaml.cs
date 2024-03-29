﻿using Pacman.Style;
using Pacman.Style.Textures;
using System.Windows.Controls;

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

        /// <summary>
        /// Variety of this fruit
        /// </summary>
        public Fruits Type
        {
            get => _Type;
            set
            {
                _Type = value;
                Texture.Source = TextureHelper[(int)value];
            }
        }
        private Fruits _Type;

        public Points Point => FruitPoints[(int)Type];

        public Fruit()
        {
            InitializeComponent();
        }
    }
}
