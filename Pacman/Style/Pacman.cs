using Packman.Style.Textures;
using Pacman.Extension;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Pacman.Style
{
    internal class Pacman : Control
    {
        private static TextureHelper TextureHelper { get; } = new TextureHelper(Textures.Pacman, 22, 22, 4, 2);

        static Pacman()
        {
            return;
        }
    }
}
