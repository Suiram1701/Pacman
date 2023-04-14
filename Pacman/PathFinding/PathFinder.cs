using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using static Pacman.PathFinding.Map;

namespace Pacman.PathFinding
{
    /// <summary>
    /// Help the ghosts to find pacman
    /// </summary>
    public class PathFinder
    {
        /// <summary>
        /// Own pos to calculate
        /// </summary>
        private readonly Point GhostPos;

        /// <summary>
        /// Target pos to calculate
        /// </summary>
        private readonly Point PacmanPos;

        /// <summary>
        /// Init new Pathfinder
        /// </summary>
        /// <param name="GhostPos">Own pos</param>
        /// <param name="PacmanPos">Terget pos (Pacman)</param>
        public PathFinder(Point GhostPos, Point PacmanPos)
        {
            this.GhostPos = new Point(GhostPos.X, GhostPos.Y);
            this.PacmanPos = new Point(PacmanPos.X, PacmanPos.Y);
        }

        /// <summary>
        /// Calculate the shortest path to pacman
        /// </summary>
        /// <param name="Hunted"><see langword="true"/> when the ghost want to flee from pacman</param>
        /// <returns>Shortest path</returns>
        public Point? GetNextPoint(bool Hunted)
        {
            // Check which directions are valid
            List<Direction> AvailableDirs = GetDirections(GhostPos).ToList();

            // Get nearest direction to pacman
            Direction Direct;
            Direction[] Directs;
            try
            {
                Directs = AvailableDirs.OrderBy(Dir => GetDistance(CalculateWithDirect(GhostPos, Dir), PacmanPos)).ToArray();
            }
            catch
            {
                return null;
            }

            // When hunted least direction und when not nearest
            Direct = Hunted ? Directs[Directs.Length - 1] : Directs[0];

            return CalculateWithDirect(GhostPos, Direct);
        }

        /// <summary>
        /// Calculate distance between two points
        /// </summary>
        /// <param name="a">Point one</param>
        /// <param name="b">Point two</param>
        /// <returns>Distance between them</returns>
        private double GetDistance(Point a, Point b)
        {
            double distanceX = a.X >= b.X ? a.X - b.X : b.X - a.X;
            double distanceY = a.Y >= b.Y ? a.Y - b.Y : b.Y - a.Y;
            return Math.Sqrt(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2));
        }

        /// <summary>
        /// Get all possible directions of this position
        /// </summary>
        /// <param name="Position">Current position to look</param>
        /// <param name="CurrentDirect">Current direction (if set the opposite will not return)</param>
        /// <returns></returns>
        private IEnumerable<Direction> GetDirections(Point Position, Direction CurrentDirect = Direction.None)
        {
            if (CheckInsideMap(CalculateWithDirect(Position, Direction.Left)) && CurrentDirect != Direction.Right)
                yield return Direction.Left;

            if (CheckInsideMap(CalculateWithDirect(Position, Direction.Down)) && CurrentDirect != Direction.Up)
                yield return Direction.Down;

            if (CheckInsideMap(CalculateWithDirect(Position, Direction.Right)) && CurrentDirect != Direction.Left)
                yield return Direction.Right;

            if (CheckInsideMap(CalculateWithDirect(Position, Direction.Up)) && CurrentDirect != Direction.Down)
                yield return Direction.Up;
        }

        /// <summary>
        /// Calculate the <paramref name="P"/> with the <paramref name="Direct"/>
        /// </summary>
        /// <param name="p">Position to calculate</param>
        /// <param name="Direct">Direction to calvulate</param>
        /// <returns>The result</returns>
        private Point CalculateWithDirect(Point p, Direction Direct)
        {
            switch (Direct)
            {
                case Direction.Left:
                    return new Point(--p.X, p.Y);
                case Direction.Down:
                    return new Point(p.X, ++p.Y);
                case Direction.Right:
                    return new Point(++p.X, p.Y);
                case Direction.Up:
                    return new Point(p.X, --p.Y);
                default:
                    return p;
            }
        }
    }
}
