using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Controls
{
    public class ValueChangedEventArgs : EventArgs
    {
        public double Value { get; private set; }

        public ValueChangedEventArgs(double value)
        {
            Value = value;
        }
    }
}
