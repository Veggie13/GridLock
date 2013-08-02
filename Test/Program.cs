using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LaserPuzzle;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            char[] input;
            do
            {
                Puzzle puz = new Puzzle(9, 9, 30, false);
                puz.WritePuzzle(Console.Out, false);
                Console.WriteLine();
                puz.WritePuzzle(Console.Out, true);
                input = Console.In.ReadLine().ToCharArray();
                Console.Clear();
            } while (input.Length > 0 && input[0] == 't');
        }
    }
}
