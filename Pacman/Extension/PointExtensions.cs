using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pacman.Extension
{
    internal static class PointExtensions
    {
        private static readonly double Factor = 23.75;
        private static readonly Point StartPos = new Point(17, 17);

        /// <summary>
        /// Convert point scale to canvas scale
        /// </summary>
        /// <param name="p">Point to convert</param>
        /// <param name="x">Canvas.Left value</param>
        /// <param name="y">Canvas.Top value</param>
        public static Tuple<double, double> ToCanvasScale(this Point p)
        {
            double x = p.X * Factor + StartPos.X;
            double y = p.Y * Factor + StartPos.Y;
            return new Tuple<double, double>(x, y);
        }
    }
}
