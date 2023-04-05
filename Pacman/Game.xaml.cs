﻿using Pacman.Extension;
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
            new Style.Pacman();
        }
    }
}
