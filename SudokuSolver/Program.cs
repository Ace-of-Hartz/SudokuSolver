using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    class Program
    {
        public static void Main(string[] args)
        {
            Sudoku puzzle = new Sudoku(9);

            puzzle.SetValue(3, 0, 5);
            puzzle.SetValue(6, 0, 7);

            puzzle.SetValue(0, 1, 4);
            puzzle.SetValue(4, 1, 1);
            puzzle.SetValue(6, 1, 6);

            puzzle.SetValue(0, 2, 3);
            puzzle.SetValue(5, 2, 8);

            puzzle.SetValue(2, 3, 6);
            puzzle.SetValue(5, 3, 4);
            puzzle.SetValue(6, 3, 1);
            puzzle.SetValue(7, 3, 2);

            puzzle.SetValue(0, 4, 5);
            puzzle.SetValue(8, 4, 8);

            puzzle.SetValue(1, 5, 2);
            puzzle.SetValue(2, 5, 8);
            puzzle.SetValue(3, 5, 7);
            puzzle.SetValue(6, 5, 5);

            puzzle.SetValue(3, 6, 6);
            puzzle.SetValue(8, 6, 5);

            puzzle.SetValue(2, 7, 7);
            puzzle.SetValue(4, 7, 9);
            puzzle.SetValue(8, 7, 2);

            puzzle.SetValue(2, 8, 1);
            puzzle.SetValue(5, 8, 2);

            Console.WriteLine("\n------------------------------\n");

            Console.Write(puzzle);

            Console.WriteLine("\n------------------------------\n");

            foreach(var solution in puzzle.Solve())
            {
                Console.WriteLine(Sudoku.ToString(solution));
                Console.WriteLine("\n------------------------------\n");
            }


            Console.WriteLine("DONE");
            Console.ReadLine();
        }
    }
}
