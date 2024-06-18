using ukazkaPlc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ukazkaHmi.Wpf.Views.Data.TechnologicalSettings
{
    public class TechnologicalSettingsViewModel
    {
        public TechnologicalDataManager TechnologicalData
        {
            get
            {
                return App.ukazkaPlc.MAIN._technology._technologySettings;
            }           
        }
    }
}
