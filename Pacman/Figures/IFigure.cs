﻿using System.Timers;
using System.Windows.Media.Animation;

namespace Pacman.Figures
{
    /// <summary>
    /// Current direction of the figure
    /// </summary>
    public enum Direction
    {
        None = 0,
        Right = 1,
        Down = 2,
        Left = 3,
        Up = 4,
    }

    internal interface IFigure
    {
        /// <summary>
        /// Direction to move the figure
        /// </summary>
        Direction Direction { get; set; }

        Timer Timer { get; }

        /// <summary>
        ///  Storyboard for animation
        /// </summary>
        Storyboard Story { get; }

        /// <summary>
        /// Run the animation or not
        /// </summary>
        bool IsAnimated { get; set; }

        /// <summary>
        /// Start animation and movement
        /// </summary>
        void Start();

        /// <summary>
        /// Stop animation and movement
        /// </summary>
        void Stop();

        /// <summary>
        /// Reset the figure
        /// </summary>
        void Reset();
    }
}
