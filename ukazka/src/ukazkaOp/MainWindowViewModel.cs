using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcoCore;
using ukazkaPlc;


namespace ukazkaOp
{
    public class MainWindowViewModel
    {
        public ukazkaPlcTwinController ukazkaPlc { get { return App.ukazkaPlc; } }
    }
}
