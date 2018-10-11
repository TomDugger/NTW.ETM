using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTW.Data.Models
{
    public struct DoubleStruct
    {
        private string _name;
        public string Name {
            get { return _name; }
            set { _name = value; }
        }

        private int _value;
        public int Value {
            get { return _value; }
            set { _value = value; }
        }
    }
}
