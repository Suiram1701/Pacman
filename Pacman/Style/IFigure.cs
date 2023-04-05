using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pacman.Style
{
    internal interface IFigure
    {
        /// <summary>
        /// Name of the element
        /// </summary>
        string Name { get; }

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
