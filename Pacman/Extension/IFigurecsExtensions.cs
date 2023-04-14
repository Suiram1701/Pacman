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
    internal static class IFigurecsExtensions
    {
        private static readonly double Factor = 23.75;
        private static readonly Point StartPos = new Point(17, 17);

        /// <summary>
        /// Calculate the canvas position to to map coordinate position
        /// </summary>
        /// <returns>Position</returns>
        public static Point GetPosition(this IFigure figure) =>
            new Point(
                (Canvas.GetLeft((UIElement)figure) - StartPos.X) / Factor,     // X
                (Canvas.GetTop((UIElement)figure) - StartPos.Y) / Factor);     // Y
    }
}
