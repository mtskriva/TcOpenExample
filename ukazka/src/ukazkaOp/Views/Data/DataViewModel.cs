using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcOpen.Inxton.Local.Security.Wpf;
using ukazkaHmi.Wpf;
using ukazkaHmi.Wpf.Views.Data.OfflineReworkData;
using ukazkaHmi.Wpf.Views.Data.ProcessSettings;
using ukazkaHmi.Wpf.Views.Data.ProcessTraceability;
using ukazkaHmi.Wpf.Views.Data.ReworkSettings;
using ukazkaHmi.Wpf.Views.Data.TechnologicalSettings;
using ukazkaHmi.Wpf.Properties;


namespace ukazkaOp.Data
{
    public class DataViewModel : MenuControlViewModel
    {
        public DataViewModel()
        {
            this.Title = strings.Data;
            this.AddCommand(typeof(ProcessSettingsView), strings.ProcessData);
            this.AddCommand(typeof(TechnologicalSettingsView), strings.TechData);
            this.AddCommand(typeof(ReworkSettingsView), strings.ReworkData);
            this.AddCommand(typeof(OfflineReworkDataView), strings.ReworkOfflineData);
            this.AddCommand(typeof(ProcessTraceabilityView), strings.ProductionData);
        }
    }
}
