using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Controls
{
    public class StateChangedEventArgs : EventArgs
    {
        public bool State { get; private set; }

        public StateChangedEventArgs(bool state)
        {
            State = state;
        }
    }
}
