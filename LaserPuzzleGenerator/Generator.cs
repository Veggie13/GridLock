using System;
using System.Collections.Generic;
using System.Linq;
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

        private void ChooseDirections(Direction[] Directions)
        {
            Random rand = new Random();
            for (int n = 0; n < NUM_LASERS; n++)
            {
                Directions[n] = CHOICES[(int)(rand.NextDouble() * 4)];
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

        public void CreatePuzzle(Puzzle puz, out int[,] NumberGrid, out List<KeyValuePair<int,int>> Positions, out Direction[,] LaserGrid, bool attemptUnique)
        {
            LaserGrid = new Direction[HEIGHT, WIDTH];
            NumberGrid = new int[HEIGHT, WIDTH];
            //long validCount = 0;
            bool valid = true;

            do
            {
                Array.Clear(NumberGrid, 0, NumberGrid.Length);

                Direction[] Solution = new Direction[NUM_LASERS];
                ChooseDirections(Solution);

                Positions = new List<KeyValuePair<int, int>>();
                ChoosePositions(Positions);

                GeneratePuzzle(LaserGrid, NumberGrid, Positions, Solution);

                valid = attemptUnique ? puz.TestSolution() : true;

                /*
                long numSolutions = (long)Math.Pow(CHOICES.Length, NUM_LASERS);
                Direction[] PossibleSolution = new Direction[NUM_LASERS];
                validCount = 0;
                for (long nSolution = 0; nSolution < numSolutions; nSolution++)
                {
                    for (int nLaser = 0; nLaser < NUM_LASERS; nLaser++)
                    {
                        long val = (nSolution % (long)Math.Pow(CHOICES.Length, nLaser + 1)) / (long)Math.Pow(CHOICES.Length, nLaser);
                        PossibleSolution[nLaser] = CHOICES[val];
                    }

                    if (TestSolution(NumberGrid, Positions, PossibleSolution) && ++validCount > 1)
                        break;
                }

                */
                //if (!valid) Console.WriteLine("Trying again.");
            } while (!valid);
        }
    }
}
