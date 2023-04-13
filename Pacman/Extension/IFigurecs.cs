using Pacman.Figures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Pacman.Extension
{
    internal static class IFigurecs
    {
        private static readonly int Factor = 23;
        private static readonly Point StartPos = new Point(17, 17);

        /// <summary>
        /// Calculate the canvas position to to map coordinate position
        /// </summary>
        /// <returns>Position</returns>
        public static Point GetPosition(this IFigure figure) =>
            new Point(
                ((int)Canvas.GetLeft((UIElement)figure) - (int)StartPos.X) / Factor,     // X
                ((int)Canvas.GetTop((UIElement)figure) - (int)StartPos.Y) / Factor);     // Y
    }
}
