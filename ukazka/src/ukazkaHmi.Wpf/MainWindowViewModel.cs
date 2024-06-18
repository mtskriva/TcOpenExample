using ukazkaPlc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcoCore;


namespace ukazkaHmi.Wpf
{
    public class MainWindowViewModel
    {
        public ukazkaPlcTwinController ukazkaPlc { get { return App.ukazkaPlc; } }        
    }
}
