using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace LaserPuzzle
{
    public class Puzzle
    {
        public enum Constraints
        {
            None = 0,
            DisallowParallelLaserBlocks = 1,
            DisallowOutwardCornerLasers = 2,
            DisallowOutwardEdgeLasers = 4,
            DisallowOutwardBoundaryLasers = 6,
            DisallowStartingZeros = 8,
            AttemptUniqueSolution = 16
        }

        private static readonly Dictionary<Direction, string> SYMBOLS;
        static Puzzle()
        {
            SYMBOLS = new Dictionary<Direction, string>();
            SYMBOLS[Direction.None] = ".";
            SYMBOLS[Direction.North] = "^";
            SYMBOLS[Direction.South] = "v";
            SYMBOLS[Direction.East] = ">";
            SYMBOLS[Direction.West] = "<";
        }

        private int[,] NumberGrid;
        private List<KeyValuePair<int, int>> Positions = new List<KeyValuePair<int, int>>();
        private Direction[,] LaserGrid;

        public Puzzle(int rows, int cols, int numLasers, Constraints constraints)
        {
            WIDTH = cols;
            HEIGHT = rows;
            
            NumberGrid = new int[rows, cols];
            LaserGrid = new Direction[rows, cols];

            Generator gen = new Generator(rows, cols, numLasers);
            do
            {
                gen.CreatePuzzle(this, out NumberGrid, out Positions, out LaserGrid);
            } while (!TestSolution(constraints));

            NUM_LASERS = Positions.Count;
        }

        public Puzzle(int[,] numbers, Direction[,] lasers, List<KeyValuePair<int, int>> laserLocs)
        {
            WIDTH = numbers.GetLength(1);
            HEIGHT = numbers.GetLength(0);

            NumberGrid = new int[HEIGHT, WIDTH];
            LaserGrid = new Direction[HEIGHT, WIDTH];

            Array.Copy(numbers, NumberGrid, numbers.Length);
            Array.Copy(lasers, LaserGrid, lasers.Length);
            Positions.AddRange(laserLocs);

            NUM_LASERS = Positions.Count;
        }

        private int WIDTH;
        public int ColumnCount
        {
            get { return WIDTH; }
        }

        private int HEIGHT;
        public int RowCount
        {
            get { return HEIGHT; }
        }

        private int NUM_LASERS;
        public int LaserCount
        {
            get { return NUM_LASERS; }
        }

        public static bool HasConstraint(Constraints value, Constraints target)
        {
            return (value & target) == target;
        }
        
        public bool TestSolution(Constraints constraints)
        {
            if (HasConstraint(constraints, Constraints.DisallowParallelLaserBlocks))
            {
                #region Ensure row-wise laser blocks do not have both East and West (redundancy)
                for (int row = 0; row < HEIGHT; row++)
                {
                    var RowPositions =
                        Positions.FindAll(delegate(KeyValuePair<int, int> coord)
                        {
                            return coord.Key == row;
                        });
                    if (RowPositions.Count < 2) continue;
                    RowPositions.Add(new KeyValuePair<int, int>(row, WIDTH + 1));
                    int colStart = RowPositions[0].Value;
                    int colEnd = colStart;
                    for (int colIndex = 1; colIndex < RowPositions.Count; colIndex++)
                    {
                        if (RowPositions[colIndex].Value - colEnd > 1)
                        {
                            var laserBlock = RowPositions.FindAll(delegate(KeyValuePair<int, int> coord)
                            {
                                return coord.Value >= colStart && coord.Value <= colEnd;
                            });
                            if (laserBlock.Count > 1 &&
                                laserBlock.Exists(delegate(KeyValuePair<int, int> coord)
                                {
                                    return LaserGrid[coord.Key, coord.Value] == Direction.East;
                                }) && laserBlock.Exists(delegate(KeyValuePair<int, int> coord)
                                {
                                    return LaserGrid[coord.Key, coord.Value] == Direction.West;
                                }))
                            {
                                return false;
                            }

                            colEnd = colStart = RowPositions[colIndex].Value;
                        }
                        else colEnd = RowPositions[colIndex].Value;
                    }
                }
                #endregion
                #region Ensure row-wise laser blocks at the edges do not have both East or West and outward-facing (redundancy)
                {
                    var RowPositions = Positions.FindAll(delegate(KeyValuePair<int, int> coord)
                    {
                        return coord.Key == 0;
                    });
                    if (RowPositions.Count > 1)
                    {
                        RowPositions.Add(new KeyValuePair<int, int>(0, WIDTH + 1));
                        int colStart = RowPositions[0].Value;
                        int colEnd = colStart;
                        for (int colIndex = 1; colIndex < RowPositions.Count; colIndex++)
                        {
                            if (RowPositions[colIndex].Value - colEnd > 1)
                            {
                                var laserBlock = RowPositions.FindAll(delegate(KeyValuePair<int, int> coord)
                                {
                                    return coord.Value >= colStart && coord.Value <= colEnd;
                                });
                                if (laserBlock.Count > 1 &&
                                    laserBlock.Exists(delegate(KeyValuePair<int, int> coord)
                                    {
                                        return LaserGrid[coord.Key, coord.Value] == Direction.East ||
                                            LaserGrid[coord.Key, coord.Value] == Direction.West;
                                    }) && laserBlock.Exists(delegate(KeyValuePair<int, int> coord)
                                    {
                                        return LaserGrid[coord.Key, coord.Value] == Direction.North;
                                    }))
                                {
                                    return false;
                                }

                                colEnd = colStart = RowPositions[colIndex].Value;
                            }
                            else colEnd = RowPositions[colIndex].Value;
                        }
                    }
                }
                {
                    var RowPositions = Positions.FindAll(delegate(KeyValuePair<int, int> coord)
                    {
                        return coord.Key == HEIGHT - 1;
                    });
                    if (RowPositions.Count > 1)
                    {
                        RowPositions.Add(new KeyValuePair<int, int>(HEIGHT - 1, WIDTH + 1));
                        int colStart = RowPositions[0].Value;
                        int colEnd = colStart;
                        for (int colIndex = 1; colIndex < RowPositions.Count; colIndex++)
                        {
                            if (RowPositions[colIndex].Value - colEnd > 1)
                            {
                                var laserBlock = RowPositions.FindAll(delegate(KeyValuePair<int, int> coord)
                                {
                                    return coord.Value >= colStart && coord.Value <= colEnd;
                                });
                                if (laserBlock.Count > 1 &&
                                    laserBlock.Exists(delegate(KeyValuePair<int, int> coord)
                                    {
                                        return LaserGrid[coord.Key, coord.Value] == Direction.East ||
                                            LaserGrid[coord.Key, coord.Value] == Direction.West;
                                    }) && laserBlock.Exists(delegate(KeyValuePair<int, int> coord)
                                    {
                                        return LaserGrid[coord.Key, coord.Value] == Direction.South;
                                    }))
                                {
                                    return false;
                                }

                                colEnd = colStart = RowPositions[colIndex].Value;
                            }
                            else colEnd = RowPositions[colIndex].Value;
                        }
                    }
                }
                #endregion
                #region Ensure column-wise laser blocks do not have both North and South (redundancy)
                for (int col = 0; col < WIDTH; col++)
                {
                    var ColPositions =
                        Positions.FindAll(delegate(KeyValuePair<int, int> coord)
                        {
                            return coord.Value == col;
                        });
                    ColPositions.Sort(delegate(KeyValuePair<int, int> a, KeyValuePair<int, int> b)
                    {
                        return a.Key.CompareTo(b.Key);
                    });
                    if (ColPositions.Count < 2) continue;
                    int rowStart = ColPositions[0].Key;
                    int rowEnd = rowStart;
                    ColPositions.Add(new KeyValuePair<int, int>(HEIGHT + 1, col));
                    for (int rowIndex = 1; rowIndex < ColPositions.Count; rowIndex++)
                    {
                        if (ColPositions[rowIndex].Key - rowEnd > 1)
                        {
                            var laserBlock = ColPositions.FindAll(delegate(KeyValuePair<int, int> coord)
                            {
                                return coord.Key >= rowStart && coord.Key <= rowEnd;
                            });
                            if (laserBlock.Count > 1 &&
                                laserBlock.Exists(delegate(KeyValuePair<int, int> coord)
                                {
                                    return LaserGrid[coord.Key, coord.Value] == Direction.North;
                                }) && laserBlock.Exists(delegate(KeyValuePair<int, int> coord)
                                {
                                    return LaserGrid[coord.Key, coord.Value] == Direction.South;
                                }))
                            {
                                return false;
                            }

                            rowEnd = rowStart = ColPositions[rowIndex].Key;
                        }
                        else rowEnd = ColPositions[rowIndex].Key;
                    }
                }
                #endregion
                #region Ensure column-wise laser blocks at the edges do not have both North or South and outward-facing (redundancy)
                {
                    var ColPositions =
                        Positions.FindAll(delegate(KeyValuePair<int, int> coord)
                        {
                            return coord.Value == 0;
                        });
                    ColPositions.Sort(delegate(KeyValuePair<int, int> a, KeyValuePair<int, int> b)
                    {
                        return a.Key.CompareTo(b.Key);
                    });
                    if (ColPositions.Count > 1)
                    {
                        int rowStart = ColPositions[0].Key;
                        int rowEnd = rowStart;
                        ColPositions.Add(new KeyValuePair<int, int>(HEIGHT + 1, 0));
                        for (int rowIndex = 1; rowIndex < ColPositions.Count; rowIndex++)
                        {
                            if (ColPositions[rowIndex].Key - rowEnd > 1)
                            {
                                var laserBlock = ColPositions.FindAll(delegate(KeyValuePair<int, int> coord)
                                {
                                    return coord.Key >= rowStart && coord.Key <= rowEnd;
                                });
                                if (laserBlock.Count > 1 &&
                                    laserBlock.Exists(delegate(KeyValuePair<int, int> coord)
                                    {
                                        return LaserGrid[coord.Key, coord.Value] == Direction.North ||
                                            LaserGrid[coord.Key, coord.Value] == Direction.South;
                                    }) && laserBlock.Exists(delegate(KeyValuePair<int, int> coord)
                                    {
                                        return LaserGrid[coord.Key, coord.Value] == Direction.West;
                                    }))
                                {
                                    return false;
                                }

                                rowEnd = rowStart = ColPositions[rowIndex].Key;
                            }
                            else rowEnd = ColPositions[rowIndex].Key;
                        }
                    }
                }
                {
                    var ColPositions =
                        Positions.FindAll(delegate(KeyValuePair<int, int> coord)
                        {
                            return coord.Value == WIDTH - 1;
                        });
                    ColPositions.Sort(delegate(KeyValuePair<int, int> a, KeyValuePair<int, int> b)
                    {
                        return a.Key.CompareTo(b.Key);
                    });
                    if (ColPositions.Count > 1)
                    {
                        int rowStart = ColPositions[0].Key;
                        int rowEnd = rowStart;
                        ColPositions.Add(new KeyValuePair<int, int>(HEIGHT + 1, WIDTH - 1));
                        for (int rowIndex = 1; rowIndex < ColPositions.Count; rowIndex++)
                        {
                            if (ColPositions[rowIndex].Key - rowEnd > 1)
                            {
                                var laserBlock = ColPositions.FindAll(delegate(KeyValuePair<int, int> coord)
                                {
                                    return coord.Key >= rowStart && coord.Key <= rowEnd;
                                });
                                if (laserBlock.Count > 1 &&
                                    laserBlock.Exists(delegate(KeyValuePair<int, int> coord)
                                    {
                                        return LaserGrid[coord.Key, coord.Value] == Direction.North ||
                                            LaserGrid[coord.Key, coord.Value] == Direction.South;
                                    }) && laserBlock.Exists(delegate(KeyValuePair<int, int> coord)
                                    {
                                        return LaserGrid[coord.Key, coord.Value] == Direction.East;
                                    }))
                                {
                                    return false;
                                }

                                rowEnd = rowStart = ColPositions[rowIndex].Key;
                            }
                            else rowEnd = ColPositions[rowIndex].Key;
                        }
                    }
                }
                #endregion
            }
            
            if (HasConstraint(constraints, Constraints.DisallowOutwardCornerLasers))
            {
                #region Ensure corner lasers do not point out of the puzzle (dual-solution)
                if (LaserGrid[0, 0] == Direction.North || LaserGrid[0, 0] == Direction.West)
                    return false;
                if (LaserGrid[0, WIDTH - 1] == Direction.North || LaserGrid[0, WIDTH - 1] == Direction.East)
                    return false;
                if (LaserGrid[HEIGHT - 1, 0] == Direction.South || LaserGrid[HEIGHT - 1, 0] == Direction.West)
                    return false;
                if (LaserGrid[HEIGHT - 1, WIDTH - 1] == Direction.South || LaserGrid[HEIGHT - 1, WIDTH - 1] == Direction.East)
                    return false;
                #endregion
            }

            if (HasConstraint(constraints, Constraints.DisallowOutwardEdgeLasers))
            {
                #region Ensure west edge lasers do not point out of the puzzle
                {
                    var ColPositions =
                        Positions.FindAll(delegate(KeyValuePair<int, int> coord)
                        {
                            return coord.Value == 0;
                        });
                    foreach (KeyValuePair<int, int> coord in ColPositions)
                    {
                        if (coord.Key == 0 || coord.Key == HEIGHT - 1)
                            continue;
                        if (LaserGrid[coord.Key, 0] == Direction.West)
                            return false;
                    }
                }
                #endregion
                #region Ensure east edge lasers do not point out of the puzzle
                {
                    var ColPositions =
                        Positions.FindAll(delegate(KeyValuePair<int, int> coord)
                        {
                            return coord.Value == WIDTH - 1;
                        });
                    foreach (KeyValuePair<int, int> coord in ColPositions)
                    {
                        if (coord.Key == 0 || coord.Key == HEIGHT - 1)
                            continue;
                        if (LaserGrid[coord.Key, WIDTH - 1] == Direction.East)
                            return false;
                    }
                }
                #endregion
                #region Ensure north edge lasers do not point out of the puzzle
                {
                    var RowPositions =
                        Positions.FindAll(delegate(KeyValuePair<int, int> coord)
                        {
                            return coord.Key == 0;
                        });
                    foreach (KeyValuePair<int, int> coord in RowPositions)
                    {
                        if (coord.Value == 0 || coord.Value == WIDTH - 1)
                            continue;
                        if (LaserGrid[0, coord.Value] == Direction.North)
                            return false;
                    }
                }
                #endregion
                #region Ensure south edge lasers do not point out of the puzzle
                {
                    var RowPositions =
                        Positions.FindAll(delegate(KeyValuePair<int, int> coord)
                        {
                            return coord.Key == HEIGHT - 1;
                        });
                    foreach (KeyValuePair<int, int> coord in RowPositions)
                    {
                        if (coord.Value == 0 || coord.Value == WIDTH - 1)
                            continue;
                        if (LaserGrid[HEIGHT - 1, coord.Value] == Direction.South)
                            return false;
                    }
                }
                #endregion
            }

            if (HasConstraint(constraints, Constraints.DisallowStartingZeros))
            {
                #region Ensure there are no zeros in the puzzle
                for (int row = 0; row < HEIGHT; row++)
                for (int col = 0; col < WIDTH; col++)
                {
                    if (Positions.Contains(new KeyValuePair<int,int>(row, col)))
                        continue;
                    if (NumberGrid[row, col] == 0)
                        return false;
                }
                #endregion
            }

            if (HasConstraint(constraints, Constraints.AttemptUniqueSolution))
                return false; // not yet attempting to implement this...

#if false
            int[,] TestNumberGrid = new int[HEIGHT, WIDTH];
            Array.Clear(TestNumberGrid, 0, TestNumberGrid.Length);

            GeneratePuzzle(null, TestNumberGrid, Positions, Solution);

            for (int row = 0; row < HEIGHT; row++)
            for (int col = 0; col < WIDTH; col++)
            {
                if (!Positions.Contains(new KeyValuePair<int, int>(row, col)) &&
                    TestNumberGrid[row, col] != NumberGrid[row, col])
                    return false;
            }
#endif
            return true;
        }

        public void WritePuzzle(TextWriter writer, bool showAnswer)
        {
            for (int row = 0; row < HEIGHT; row++)
            {
                string[] thisRow = new string[WIDTH];
                for (int col = 0; col < WIDTH; col++)
                {
                    if (!Positions.Contains(new KeyValuePair<int,int>(row, col)))
                        thisRow[col] = NumberGrid[row, col].ToString();
                    else if (showAnswer)
                        thisRow[col] = SYMBOLS[LaserGrid[row, col]];
                    else
                        thisRow[col] = SYMBOLS[Direction.None];
                }

                writer.WriteLine(string.Join(" ", thisRow));
            }
            writer.Flush();
        }

        public int[,] RenderPuzzleGrid(Dictionary<Direction, int> dirValues, bool hideAnswers)
        {
            int[,] result = new int[HEIGHT, WIDTH];
            Array.Copy(NumberGrid, result, NumberGrid.Length);
            foreach (KeyValuePair<int, int> coord in Positions)
            {
                int row = coord.Key;
                int col = coord.Value;
                Direction dir = hideAnswers ? Direction.None : LaserGrid[row, col];
                result[row, col] = dirValues[dir];
            }
            return result;
        }
    }
}
