using System;
using System.Linq;

namespace ukazkaHmi.Wpf
{
    public class ConsoleWriterEventArgs : EventArgs
    {
        public ConsoleWriterEventArgs(string value)
        {
            this.Value = value;
        }

        public string Value
        {
            get; private set;
        }
    }
}
