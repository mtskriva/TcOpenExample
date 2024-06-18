using ukazkaPlc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ukazkaHmi.Wpf.Views.Data.ReworkSettings
{
    public class ReworkSettingsViewModel
    {
        public ProcessDataManager Data
        {
            get
            {
                return App.ukazkaPlc.MAIN._technology._reworkSettings;
            }           
        }
    }
}
