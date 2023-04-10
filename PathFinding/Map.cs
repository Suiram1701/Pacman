using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinding
{
    /// <summary>
    /// A help map view to see walls
    /// </summary>
    internal static class Map
    {
        static Map()
        {
            // Map data
            int[][] Ints = new int[29][]
            {
                new int[] {12, -2, 12},
                new int[] {1, -4, 1, -5, 1, -2, 1, -5, 1, -4, 1},
                new int[] {-5, 1, -5, 1, -2, 1, -5, 1, -5},
                new int[] {1, -4, 1, -5, 1, -2, 1, -5, 1, -4, 1},
                new int[] {26},
                new int[] {1, -4, 1, -2, 1, -8, 1, -2, 1, -4, 1},
                new int[] {1, -4, 1, -2, 1, -8, 1, -2, 1, -4, 1},
                new int[] {6, -2, 4, -2, 4, -2, 6},
                new int[] {-5, 1, -2, 1, -8, 1, -2, 1, -5},
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
                new int[] {-1, 2, -2, 7, -2, 7, -2, 2, -1},
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

        public static bool[][] BoolMap { get; private set; } = new bool[29][];
    }
}
