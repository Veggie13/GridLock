using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace LaserPuzzle
{
    internal class Generator
    {
        private static readonly Direction[] CHOICES = { Direction.North, Direction.South, Direction.East, Direction.West };

        private int WIDTH = 9;
        private int HEIGHT = 9;
        private int NUM_LASERS = 30;

        public Generator(int rows, int cols, int numLasers)
        {
            WIDTH = cols;
            HEIGHT = rows;
            NUM_LASERS = numLasers;
        }

        private void ChoosePositions(List<KeyValuePair<int, int>> Positions)
        {
            int lasersRemaining = NUM_LASERS;
            int cellsRemaining = HEIGHT * WIDTH;
            Random rand = new Random();
            for (int row = 0; row < HEIGHT; row++)
            for (int col = 0; col < WIDTH; col++)
            {
                if (rand.NextDouble() < (double)lasersRemaining / cellsRemaining)
                {
                    Positions.Add(new KeyValuePair<int, int>(row, col));
                    lasersRemaining--;
                }

                cellsRemaining--;
            }
        }

        private void ChooseDirections(List<KeyValuePair<int, int>> Positions, bool disallowOutwardCorners, bool disallowOutwardEdges, Direction[] Directions)
        {
            Random rand = new Random();
            for (int n = 0; n < NUM_LASERS; n++)
            {
            theStart:
                Directions[n] = CHOICES[(int)(rand.NextDouble() * 4) % 4];
                if (disallowOutwardCorners)
                {
                    if (Positions[n].Value == 0)
                    {
                        if (Positions[n].Key == 0 &&
                            (Directions[n] == Direction.North ||
                             Directions[n] == Direction.West))
                            goto theStart;
                        else if (Positions[n].Key == HEIGHT - 1 &&
                            (Directions[n] == Direction.South ||
                             Directions[n] == Direction.West))
                            goto theStart;
                    }
                    else if (Positions[n].Value == WIDTH - 1)
                    {
                        if (Positions[n].Key == 0 &&
                            (Directions[n] == Direction.North ||
                             Directions[n] == Direction.East))
                            goto theStart;
                        else if (Positions[n].Key == HEIGHT - 1 &&
                            (Directions[n] == Direction.South ||
                             Directions[n] == Direction.East))
                            goto theStart;
                    }
                }
                if (disallowOutwardEdges)
                {
                    if (Positions[n].Key == 0 &&
                        Positions[n].Value != 0 &&
                        Positions[n].Value != WIDTH - 1 &&
                        Directions[n] == Direction.North)
                        goto theStart;
                    if (Positions[n].Key == HEIGHT - 1 &&
                        Positions[n].Value != 0 &&
                        Positions[n].Value != WIDTH - 1 &&
                        Directions[n] == Direction.South)
                        goto theStart;
                    if (Positions[n].Value == 0 &&
                        Positions[n].Key != 0 &&
                        Positions[n].Key != HEIGHT - 1 &&
                        Directions[n] == Direction.West)
                        goto theStart;
                    if (Positions[n].Value == WIDTH - 1 &&
                        Positions[n].Key!= 0 &&
                        Positions[n].Key != HEIGHT - 1 &&
                        Directions[n] == Direction.East)
                        goto theStart;
                }
            }
        }
        
        private void GeneratePuzzle(Direction[,] LaserGrid, int[,] NumberGrid, List<KeyValuePair<int,int>> Positions, Direction[] Solution)
        {
            int nLaser = 0;
            for (int row = 0; row < HEIGHT; row++)
            for (int col = 0; col < WIDTH; col++)
            {
                if ((nLaser < NUM_LASERS) && (Positions[nLaser].Key == row) && (Positions[nLaser].Value == col))
                {
                    int rowInc = 0, colInc = 0;
                    if (LaserGrid != null) LaserGrid[row, col] = Solution[nLaser];
                    switch (Solution[nLaser])
                    {
                    case Direction.North:
                        rowInc = -1;
                        break;
                    case Direction.South:
                        rowInc = 1;
                        break;
                    case Direction.East:
                        colInc = 1;
                        break;
                    case Direction.West:
                        colInc = -1;
                        break;
                    default:
                        throw new Exception();
                    }
                    nLaser++;

                    for (int i = row + rowInc, j = col + colInc;
                            i >= 0 && i < HEIGHT && j >= 0 && j < WIDTH;
                            i += rowInc, j += colInc)
                    {
                        NumberGrid[i, j]++;
                    }
                }
                else if (LaserGrid != null)
                    LaserGrid[row, col] = Direction.None;
            }
        }

        public void CreatePuzzle(bool disallowOutwardCorners, bool disallowOutwardEdges, out int[,] NumberGrid, out List<KeyValuePair<int,int>> Positions, out Direction[,] LaserGrid)
        {
            LaserGrid = new Direction[HEIGHT, WIDTH];
            NumberGrid = new int[HEIGHT, WIDTH];
            //long validCount = 0;
            bool valid = true;

            Array.Clear(NumberGrid, 0, NumberGrid.Length);

            Positions = new List<KeyValuePair<int, int>>();
            ChoosePositions(Positions);

            Direction[] Solution = new Direction[NUM_LASERS];
            ChooseDirections(Positions, disallowOutwardCorners, disallowOutwardEdges, Solution);

            GeneratePuzzle(LaserGrid, NumberGrid, Positions, Solution);
        }
    }
}
