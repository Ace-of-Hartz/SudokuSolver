using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public class Sudoku
    {
        private bool initialized;

        private Subset[] rows;
        private Subset[] cols;
        private Subset[] blocks;

        private ValueBlock[,] cells;

        private int length;
        private int subLength;

        public List<int[,]> Solutions { get; set; }

        public Sudoku(int sideLength)
        {
            double sqrt = Math.Sqrt(sideLength);
            if ((int)sqrt != sqrt)
            {
                throw new ArgumentException("Value must be a perfect square", "sideLength");
            }

            this.cells = new ValueBlock[sideLength, sideLength];
            this.length = sideLength;
            this.subLength = (int)sqrt;

            for (int x = 0; x < length; ++x)
            {
                for (int y = 0; y < length; ++y)
                {
                    this.cells[x, y] = new ValueBlock(this.length);
                }
            }

            this.Solutions = new List<int[,]>();

            initialized = false;
        }

        public Sudoku(int[,] sudoku)
        {
            int xLength = sudoku.GetLength(0);
            int yLength = sudoku.GetLength(1);

            if (xLength != yLength)
            {
                throw new ArgumentException("Lengths of the 2D array must be the same", "sudoku");
            }

            double sqrt = Math.Sqrt(xLength);
            if ((int)sqrt != sqrt)
            {
                throw new ArgumentException("Side lengths must be a perfect square");
            }

            this.length = xLength;
            this.subLength = (int)sqrt;

            this.cells = new ValueBlock[this.length, this.length];
            for (int x = 0; x < this.length; ++x)
            {
                for (int y = 0; y < this.length; ++y)
                {
                    this.cells[x, y] = new ValueBlock(this.length);
                    if (sudoku[x, y] != 0)
                    {
                        lock (cells[x, y].LockObject)
                        {
                            this.cells[x, y].Value = sudoku[x, y];
                            this.cells[x, y].Fixed = true;
                        }
                    }
                }
            }

            this.Solutions = new List<int[,]>();
        }

        private void InitializeSubsets()
        {
            if (!initialized)
            {
                rows = new Subset[this.length];
                cols = new Subset[this.length];
                blocks = new Subset[this.length];

                for (int x = 0; x < this.length; ++x)
                {
                    cols[x] = new Subset(this.length);
                    for (int y = 0; y < this.length; ++y)
                    {
                        if (rows[y] == null)
                        {
                            rows[y] = new Subset(this.length);
                        }

                        int block = (x / this.subLength) + ((y / this.subLength) * this.subLength);
                        int blockCell = (x % this.subLength) + ((y % this.subLength) * this.subLength);

                        if (blocks[block] == null)
                        {
                            blocks[block] = new Subset(this.length);
                        }

                        rows[y].Cells[x] = cells[x, y];
                        cols[x].Cells[y] = cells[x, y];
                        blocks[block].Cells[blockCell] = cells[x, y];
                    }
                }

                foreach (var row in this.rows)
                {
                    row.RegisterCells();
                }
                foreach (var col in this.cols)
                {
                    col.RegisterCells();
                }
                foreach (var block in this.blocks)
                {
                    block.RegisterCells();
                }

                initialized = true;
            }
        }

        public bool IsValid()
        {
            this.InitializeSubsets();

            foreach (var row in this.rows)
            {
                if (!row.IsValid())
                {
                    return false;
                }
            }
            foreach (var col in this.cols)
            {
                if (!col.IsValid())
                {
                    return false;
                }
            }
            foreach (var block in this.blocks)
            {
                if (!block.IsValid())
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsSolved()
        {
            this.InitializeSubsets();

            foreach (var row in this.rows)
            {
                if (!row.IsSolved())
                {
                    return false;
                }
            }
            foreach (var col in this.cols)
            {
                if (!col.IsSolved())
                {
                    return false;
                }
            }
            foreach (var block in this.blocks)
            {
                if (!block.IsSolved())
                {
                    return false;
                }
            }
            return true;
        }

        public void SetValue(int x, int y, int i)
        {
            lock (this.cells[y, x].LockObject)
            {
                this.cells[y, x].Value = i;
                this.cells[y, x].Fixed = true;
                this.cells[y, x].PossibleValues.Clear();
            }
        }

        public IEnumerable<int[,]> Solve()
        {
            this.InitializeSubsets();

            bool addedFixed;
            do
            {
                addedFixed = false;
                for (int x = 0; x < this.length; ++x)
                {
                    for (int y = 0; y < this.length; ++y)
                    {
                        lock (this.cells[x, y].LockObject)
                        {
                            if (this.cells[x, y].PossibleValues.Count == 1)
                            {
                                this.cells[x, y].Value = this.cells[x, y].PossibleValues.First();
                                this.cells[x, y].Fixed = true;
                                this.cells[x, y].PossibleValues.Clear();
                                addedFixed = true;
                            }
                        }
                    }
                }
            } while (addedFixed);

            if(!this.IsValid())
            {
                return null;
            }
            else if(this.IsSolved())
            {
                int[,] solution = Sudoku.To2dArray(this.cells);
                this.Solutions.Add(solution);
                return this.Solutions;
            }

            return this.Solve(0, 0);
        }

        private IEnumerable<int[,]> Solve(int x, int y)
        {
            int nextX;
            int nextY;

            while (true)
            {
                nextX = (x + 1) % this.length;
                nextY = nextX == 0 ? (y + 1) % this.length : y;

                if (cells[x, y].Fixed)
                {
                    x = nextX;
                    y = nextY;
                    continue;
                }
                break;
            }

            List<int> possibleValues;
            lock (cells[x, y].LockObject)
            {
                possibleValues = cells[x, y].PossibleValues.ToList();
            }
            foreach (int possibleValue in possibleValues)
            {
                cells[x, y].Value = possibleValue;
                if (this.IsValid())
                {
                    if (this.IsSolved())
                    {
                        int[,] solution = Sudoku.To2dArray(this.cells);
                        this.Solutions.Add(solution);
                        yield return solution;
                    }
                    foreach (var solution in Solve(nextX, nextY))
                    {
                        yield return solution;
                    }
                }
                lock (cells[x, y].LockObject)
                {
                    cells[x, y].ResetValue();
                }
            }
        }

        public override string ToString()
        {
            return Sudoku.ToString(Sudoku.To2dArray(this.cells));
        }

        //==============================================================================================

        public static int[,] To2dArray(ValueBlock[,] cells)
        {
            int length = cells.GetLength(0);
            int[,] result = new int[length, length];
            for (int x = 0; x < length; ++x)
            {
                for (int y = 0; y < length; ++y)
                {
                    result[x, y] = cells[x, y].Value;
                }
            }
            return result;
        }

        public static string ToString(int[,] values)
        {
            StringBuilder puzzleString = new StringBuilder();
            for (int x = 0; x < values.GetLength(0); ++x)
            {
                for (int y = 0; y < values.GetLength(1); ++y)
                {
                    puzzleString.AppendFormat("{0} ", values[x, y]);
                }
                puzzleString.Append(Environment.NewLine);
            }
            return puzzleString.ToString();
        }

        public static bool TryParseSudoku(string sudokuString, out Sudoku sudoku)
        {
            try
            {
                List<string> rows = sudokuString.Split(Environment.NewLine.ToCharArray())
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToList();

                int[,] puzzle = new int[rows.Count, rows.Count];

                for (int i = 0; i < rows.Count; ++i)
                {
                    List<string> cols = rows[i].Split(" |;,-=+".ToCharArray())
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToList();
                    for (int j = 0; j < rows.Count; ++j)
                    {
                        puzzle[i, j] = int.Parse(cols[i]);
                    }
                }

                sudoku = new Sudoku(puzzle);
                return true;
            }
            catch
            {
                sudoku = null;
                return false;
            }
        }
    }
}
