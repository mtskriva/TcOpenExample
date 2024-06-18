
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcOpen.Inxton.Local.Security.Wpf;
using Vortex.Presentation.Wpf;
using ukazkaPlc;

namespace ukazkaHmi.Wpf.Views
{
    public class TechnologyViewModel 
    {
        public TechnologyViewModel()
        {
            
        }

        public ukazkaPlcTwinController ukazkaPlc { get { return App.ukazkaPlc; } }

    }
}
