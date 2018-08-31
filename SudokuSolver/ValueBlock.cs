using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public class OldValue : EventArgs
    {
        public int Value { get; set; }
        
        public OldValue(int value)
        {
            this.Value = value;
        }
    }

    public class ValueBlock
    {
        private int _Value;
        public int Value
        {
            get
            {
                return this._Value;
            }
            set
            {
                this._Value = value;
                this.ValueSet?.Invoke(this, null);
            }
        }

        private readonly object _LockObject;
        public object LockObject
        {
            get
            {
                return this._LockObject;
            }
        }

        public bool Fixed { get; set; }

        private HashSet<int> _PossibleValues;
        public HashSet<int> PossibleValues
        {
            get
            {
                return this._PossibleValues;
            }
        }

        public event EventHandler ValueSet;

        public ValueBlock(int numValues)
        {
            this._LockObject = new object();

            this._PossibleValues = new HashSet<int>();
            for(int i = 1; i <= numValues; ++i)
            {
                this._PossibleValues.Add(i);
            }
        }

        public ValueBlock(int value, bool Fixed, HashSet<int> possibleValues)
        {
            this._LockObject = new object();

            this.Value = value;
            this.Fixed = Fixed;

            this._PossibleValues = possibleValues;
        }

        public ValueBlock(ValueBlock other)
        {
            this._LockObject = new object();

            this.Value = other.Value;
            this.Fixed = other.Fixed;
            this._PossibleValues = other._PossibleValues;
        }

        public void ResetValue()
        {
            int oldValue = this.Value;
            this._Value = 0;
            this.ValueSet?.Invoke(this, new OldValue(oldValue));
        }
    }
}
