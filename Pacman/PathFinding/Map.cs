using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pacman.PathFinding
{
    /// <summary>
    /// A help map view to see walls
    /// </summary>
    internal static class Map
    {
        /// <summary>
        /// Direction of the path algorithm
        /// </summary>
        public enum Direction
        {
            None = 0,
            Left = 1,
            Down = 2,
            Right = 3,
            Up = 4
        }

        /// <summary>
        /// Convert int array map to bool array map
        /// </summary>
        static Map()
        {
            // Map data
            int[][] Ints = new int[29][]
            {
                new int[] {12, -2, 12},
                new int[] {1, -4, 1, -5, 1, -2, 1, -5, 1, -4, 1},
                new int[] {1, -4, 1, -5, 1, -2, 1, -5, 1, -4, 1},
                new int[] {1, -4, 1, -5, 1, -2, 1, -5, 1, -4, 1},
                new int[] {26},
                new int[] {1, -4, 1, -2, 1, -8, 1, -2, 1, -4, 1},
                new int[] {1, -4, 1, -2, 1, -8, 1, -2, 1, -4, 1},
                new int[] {6, -2, 4, -2, 4, -2, 6},
                new int[] {-5, 1, -5, 1, -2, 1, -5, 1, -5},
                new int[] {-5, 1, -5, 1, -2, 1, -5, 1, -5},
                new int[] {-5, 1, -2, 10, -2, 1, -5},
                new int[] {-5, 1, -2, 1, -8, 1, -2, 1, -5},
                new int[] {-5, 1, -2, 1, -8, 1, -2, 1, -5},
                new int[] {-5, 1, -2, 1, -8, 1, -2, 1, -5},
                new int[] {-5, 1, -2, 1, -8, 1, -2, 1, -5},
                new int[] {-5, 1, -2, 1, -8, 1, -2, 1, -5},
                new int[] {-5, 1, -2, 10, -2, 1, -5},
                new int[] {-5, 1, -2, 1, -8, 1, -2, 1, -5},
                new int[] {-5, 1, -2, 1, -8, 1, -2, 1, -5},
                new int[] {12, -2, 12},
                new int[] {1, -4, 1, -5, 1, -2, 1, -5, 1, -4, 1},
                new int[] {1, -4, 1, -5, 1, -2, 1, -5, 1, -4, 1},
                new int[] {3, -2, 16, -2, 3},
                new int[] {-2, 1, -2, 1, -2, 1, -8, 1, -2, 1, -2, 1, -2},
                new int[] {-2, 1, -2, 1, -2, 1, -8, 1, -2, 1, -2, 1, -2},
                new int[] {6, -2, 4, -2, 4, -2, 6},
                new int[] {1, -10, 1, -2, 1, -10, 1},
                new int[] {1, -10, 1, -2, 1, -10, 1},
                new int[] {26}
            };

            // Procede map data
            for (int i = 0; i <= Ints.Length - 1; i++)
            {
                Array.Resize(ref BoolMap[i], 26);
                int index = 0;
                foreach (int j in Ints[i])
                {
                    // Value to set
                    bool ValueToSet = j > 0;

                    for (int n = 0; n < Math.Abs(j); n++)
                    {
                        BoolMap[i][index] = ValueToSet;
                        index++;
                    }
                }
            }
        }

        /// <summary>
        /// Map in bool array
        /// </summary>
        private static bool[][] BoolMap { get; set; } = new bool[29][];

        /// <summary>
        /// Check if give position is inside the map
        /// </summary>
        /// <param name="p">Point to check</param>
        /// <returns>If the given position is inside the it returns <see langword="true"/></returns>
        public static bool CheckInsideMap(Point p)
        {
            // If outside map intercept exeption
            try
            {
                // Check
                if (BoolMap[(int)p.Y][(int)p.X])
                    return true;
            }
            catch
            {
            }

            return false;
        }
    }
}
