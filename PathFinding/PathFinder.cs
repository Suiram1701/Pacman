using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static PathFinding.Map;

namespace PathFinding
{
    /// <summary>
    /// Help the ghosts to find pacman
    /// </summary>
    public class PathFinder
    {
        /// <summary>
        /// Own pos to calculate
        /// </summary>
        private readonly Point GhostPos ;

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
        /// <returns>Shortest path</returns>
        public IEnumerable<Point> GetPath()
        {
            // Check which directions are valid
            List<Direction> AvailableDirs = GetDirections(GhostPos).ToList();

            // Get nearest direction to pacman
            Direction Direct;
            try
            {
                Direct = AvailableDirs.OrderBy(Dir => GetDistance(CalculateWithDirect(GhostPos, Dir), PacmanPos)).ToArray()[0];
            }
            catch
            {
                yield break;
            }
     
            // Setup
            Point Pos = GhostPos;

            // Set Timeout to calculate
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (sw.ElapsedMilliseconds <= 2500)
            {
                // First move
                Pos = CalculateWithDirect(Pos, Direct);
                // Check which directions are valid
                List<Direction> AvailableDirects = GetDirections(Pos, Direct).ToList();

                // If near at pacman return
                IEnumerable<Point> NearPositions = AvailableDirects.Select(Dir => CalculateWithDirect(Pos, Dir));
                Point NearAtPacman = NearPositions.FirstOrDefault(point => point == PacmanPos);

                if (NearPositions.Any(pos => pos == PacmanPos))
                {
                    yield return NearAtPacman;
                    break;
                }

                if (AvailableDirects.Any(Dir => Dir != Direct) && AvailableDirects.Count > 1)     // Get shortest direction
                {
                    // Get nearest direction to pacman
                    try
                    {
                        Direct = AvailableDirs.OrderBy(Dir => GetDistance(CalculateWithDirect(GhostPos, Dir), PacmanPos)).ToArray()[0];
                    }
                    catch
                    {
                        yield break;
                    }

                    yield return Pos;
                }
                else if (AvailableDirects.Any(Dir => Dir != Direct) && AvailableDirects.Count == 1)     // Switch direction on curve
                {
                    // Add curve
                    Direct = AvailableDirects[0];
                    yield return Pos;
                }
                else     // Follow current direction
                    yield return Pos;
            }
            sw.Stop();
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
