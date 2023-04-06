using Pacman.Extension;
using Pacman.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Shapes;
using static Packman.Properties.Settings;
using Pacman.Figures;

namespace Pacman
{
    /// <summary>
    /// Interaktionslogik für Game.xaml
    /// </summary>
    public partial class Game : Window
    {
        public Game()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Ckeck if move direction of pacman change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == (Key)Default.Control_Left)
                Pacman.Direction = Direction.Left;

            else if (e.Key == (Key)Default.Control_Down)
                Pacman.Direction = Direction.Down;

            else if (e.Key == (Key)Default.Control_Right)
                Pacman.Direction = Direction.Right;

            else if (e.Key == (Key)Default.Control_Up)
                Pacman.Direction = Direction.Up;
        }
    }
}
