using ukazkaPlc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcoData;

namespace ukazkaHmi.Wpf.Views.Data.ProcessTraceability
{
    public class ProcessTraceabilityViewModel
    {
        public TcoDataExchangeDisplayViewModel ProcessData
        {
            get
            {
                return new TcoDataExchangeDisplayViewModel() { Model = App.ukazkaPlc.MAIN._technology._processTraceability };
            }           
        }
    }
}
