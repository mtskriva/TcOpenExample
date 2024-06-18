using ukazkaPlc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ukazkaHmi.Wpf.Views.Data.ProcessSettings
{
    public class ProcessSettingsViewModel
    {
        public ProcessDataManager ProcessData
        {
            get
            {
                return App.ukazkaPlc.MAIN._technology._processSettings;
            }           
        }
    }
}
