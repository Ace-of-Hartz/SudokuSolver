using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public class Subset
    {
        public readonly int _Length;
        public int Length
        {
            get
            {
                return this._Length;
            }
        }

        private ValueBlock[] _Cells;
        public ValueBlock[] Cells
        {
            get
            {
                return this._Cells;
            }
        }

        public Subset(int length)
        {
            this._Length = length;
            this._Cells = new ValueBlock[this.Length];
        }

        public Subset(ValueBlock[] cells)
        {
            this._Length = cells.Length;
            this._Cells = cells;
        }

        public Subset(Subset other)
        {
            this._Length = other.Length;
            this._Cells = other.Cells.Select(c => new ValueBlock(c)).ToArray();
        }

        internal void RegisterCells()
        { 
            foreach (var cell in this._Cells)
            {
                lock (cell.LockObject)
                {
                    if (cell.Fixed)
                    {
                        cell.PossibleValues.Clear();
                    }
                    else
                    {
                        cell.ValueSet += (object o, EventArgs e) => 
                        {
                            OldValue value = e as OldValue;
                            if(value != null)
                            {
                                foreach(var c in this._Cells.Where(c => !c.Fixed))
                                {
                                    c.PossibleValues.Add(value.Value);
                                }
                            }
                            this.UpdateRow();
                        };
                    }
                }
            }
            UpdateRow();
        }

        private void UpdateRow()
        {
            List<int> usedValues = this._Cells.Select(c => c.Value).ToList();
            foreach (var cell in this._Cells)
            {
                if (cell.Fixed)
                {
                    continue;
                }
                foreach (var val in usedValues)
                {
                    lock (cell.LockObject)
                    {
                        cell.PossibleValues.Remove(val);
                    }
                }
            }
        }

        public bool IsValid()
        {
            HashSet<int> usedValues = new HashSet<int>();
            foreach (var cell in this._Cells)
            {
                if (cell.Value == 0 && cell.PossibleValues.Count == 0)
                {
                    return false;
                }
                if (cell.Value == 0)
                {
                    continue;
                }
                if (!usedValues.Add(cell.Value))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsSolved()
        {
            HashSet<int> usedValues = new HashSet<int>();
            foreach (var cell in this._Cells)
            {
                if (cell.Value == 0)
                {
                    return false;
                }
                if (!usedValues.Add(cell.Value))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
