using System;
using System.Collections.Generic;
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
        /// Token to cancel all tasks
        /// </summary>
        private CancellationToken Ct;

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
        public async Task<List<Point>> GetPathAsync()
        {
            // Check which directions are valid
            List<Direction> AvailableDirects = new List<Direction>();
            #region Directions
            try
            {
                if ((BoolMap[(int)GhostPos.Y][(int)GhostPos.X - 1]))
                    AvailableDirects.Add(Direction.Left);
            }
            catch
            {

            }
            try
            {
                if (BoolMap[(int)GhostPos.Y + 1][(int)GhostPos.X])
                    AvailableDirects.Add(Direction.Down);
            }
            catch
            {

            }
            try
            {
                if (BoolMap[(int)GhostPos.Y][(int)GhostPos.X + 1])
                    AvailableDirects.Add(Direction.Right);
            }
            catch
            {

            }
            try
            {
                if (BoolMap[(int)GhostPos.Y - 1][(int)GhostPos.X])
                    AvailableDirects.Add(Direction.Up);
            }
            catch
            {

            }
            #endregion

            // New tasks for each branch
            CancellationTokenSource Cts = new CancellationTokenSource();
            Ct = Cts.Token;
            Task<List<Point>>[] Finders = AvailableDirects.Select(dir => Task.Run(() => GetPathInternal(GhostPos, dir) )).ToArray();

            // Cancel if one finished
            int FinishedTask = Task.WaitAny(Finders, 10000);
            Cts.Cancel();

            if (FinishedTask < 0)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("<!--     No path found!     --!>");
#endif

                return new List<Point>();
            }

            return await Finders[FinishedTask];
        }

        /// <summary>
        /// Continue at a point in a direction
        /// </summary>
        /// <param name="StartPos"><see cref="Point"/> to continue</param>
        /// <param name="Direct"><see cref="Direction"/> to continue</param>
        /// <param name="Ct">Cancel token</param>
        /// <param name="CurrentPath">Previous path</param>
        /// <returns></returns>
        private List<Point> GetPathInternal(Point StartPos, Direction Direct, List<Point> CurrentPath = null)
        {
            // Setup
            CurrentPath = CurrentPath ?? new List<Point>();
            Point p = StartPos;
            
            // First move
            switch (Direct)
            {
                case Direction.Left:
                    p.X--;
                    break;
                case Direction.Down:
                    p.Y++;
                    break;
                case Direction.Right:
                    p.X++;
                    break;
                case Direction.Up:
                    p.Y--;
                    break;
            }
            
        Direction:

            // If any task found a path return
            if (Ct.IsCancellationRequested)
                return null;

            // Check which directions are valid
            List<Direction> AvailableDirects = new List<Direction>();
            #region Directions
            try
            {
                if ((BoolMap[(int)p.Y][(int)p.X - 1]) && Direct != Direction.Right)
                    AvailableDirects.Add(Direction.Left);
            }
            catch
            {

            }
            try
            {
                if (BoolMap[(int)p.Y + 1][(int)p.X] && Direct != Direction.Up)
                    AvailableDirects.Add(Direction.Down);
            }
            catch
            {
                
            }
            try
            {
                if (BoolMap[(int)p.Y][(int)p.X + 1] && Direct != Direction.Left)
                    AvailableDirects.Add(Direction.Right);
            }
            catch
            {

            }
            try
            {
                if (BoolMap[(int)p.Y - 1][(int)p.X] && Direct != Direction.Down)
                    AvailableDirects.Add(Direction.Up);
            }
            catch
            {

            }
            #endregion

            // If near at pacman return
            IEnumerable<Point> NearPositions = AvailableDirects.Select(Dir =>
            {
                switch (Dir)
                {
                    case Direction.Left:
                        return new Point(p.X - 1, p.Y);
                    case Direction.Down:
                        return new Point(p.X, p.Y + 1);
                    case Direction.Right:
                        return new Point(p.X + 1, p.Y);
                    case Direction.Up:
                        return new Point(p.X, p.Y - 1);
                    default:
                        return new Point();
                }
            });
            Point NearAtPacman = NearPositions.FirstOrDefault(point => point == PacmanPos);

            if (NearPositions.Any(pos => pos == PacmanPos))
            {
                CurrentPath.Add(NearAtPacman);
                return CurrentPath;
            }

            if (AvailableDirects.Any(Dir => Dir != Direct) && AvailableDirects.Count > 1)     // New tasks on crossing
            {
                CurrentPath.Add(p);

                // New tasks for each branch
                Task<List<Point>>[] Crossing = AvailableDirects.Select(dir =>
                {
                    // Postpone coordinate
                    Point point = p;
                    switch (dir)
                    {
                        case Direction.Left:
                            point.X--;
                            break;
                        case Direction.Down:
                            point.Y++;
                            break;
                        case Direction.Right:
                            point.X++;
                            break;
                        case Direction.Up:
                            point.Y--;
                            break;
                    }

                    return Task.Run(() => GetPathInternal(point, dir, new List<Point>(CurrentPath)));
                }).ToArray();

                // Cancel if one finished
                int FinishedTask = Task.WaitAny(Crossing);
                return Crossing[FinishedTask].Result;
            }
            else if (AvailableDirects.Any(Dir => Dir != Direct) && AvailableDirects.Count == 1)     // Switch direction on curve
            {
                CurrentPath.Add(p);

                // Postpone coordinate
                switch (Direct)
                {
                    case Direction.Left:
                        p.X--;
                        break;
                    case Direction.Down:
                        p.Y++;
                        break;
                    case Direction.Right:
                        p.X++;
                        break;
                    case Direction.Up:
                        p.Y--;
                        break;
                }
                Direct = AvailableDirects[0];
                goto Direction;
            }
            else     // Follow current direction
            {
                CurrentPath.Add(p);

                // Postpone coordinate
                switch (Direct)
                {
                    case Direction.Left:
                        p.X--;
                        break;
                    case Direction.Down:
                        p.Y++;
                        break;
                    case Direction.Right:
                        p.X++;
                        break;
                    case Direction.Up:
                        p.Y--;
                        break;
                }
                goto Direction;
            }
        }
    }
}
