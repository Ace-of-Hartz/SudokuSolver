using SudokuSolver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SudokuSolver.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _Solve(int[][] sudoku)
        {
            int[,] sudokuValues = new int[sudoku.Length, sudoku.Length];
            for (var i = 0; i < sudoku.Length; ++i)
            {
                if (sudoku[i].Length != sudoku.Length)
                {
                    return new JsonResult()
                    {
                        Data = false
                    };
                }
                for (var j = 0; j < sudoku[i].Length; ++j)
                {
                    sudokuValues[i, j] = sudoku[i][j];
                }
            }

            Sudoku sudokuObj = new Sudoku(sudokuValues);
            Debug.WriteLine(sudokuObj);

            int[,] solvedSudoku = sudokuObj.Solve().FirstOrDefault();
            if (sudokuObj == null)
            {
                return new JsonResult()
                {
                    Data = false
                };
            }
            else
            {
                int[][] solvedPuzzle = new int[solvedSudoku.GetLength(0)][];
                for(int i = 0; i < solvedPuzzle.Length; ++i)
                {
                    solvedPuzzle[i] = new int[solvedSudoku.GetLength(1)];
                    for(int j = 0; j < solvedPuzzle[i].Length; ++j)
                    {
                        solvedPuzzle[i][j] = solvedSudoku[i, j];
                    }
                }

                return new JsonResult()
                {
                    Data = solvedPuzzle
                };
            }
        }
    }
}