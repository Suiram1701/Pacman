using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace Pacman.Figures
{
    public enum Direction
    {
        None = 0,
        Left = 1,
        Down = 2,
        Right = 3,
        Up = 4,
    }

    internal interface IFigure
    {
        /// <summary>
        /// Direction to move the figure
        /// </summary>
        Direction Direction { get; set; }

        /// <summary>
        ///  Storyboard for animation
        /// </summary>
        Storyboard Story { get; }

        /// <summary>
        /// Run the animation or not
        /// </summary>
        bool IsAnimated { get; set; }

        /// <summary>
        /// Start animation
        /// </summary>
        void StartAnimation();

        /// <summary>
        /// Stop animation
        /// </summary>
        void EndAnimation();
    }
}
