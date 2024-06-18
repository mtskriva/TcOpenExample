using ukazkaHmi.Wpf.Properties;
using ukazkaHmi.Wpf.Views.Data.OfflineReworkData;
using ukazkaHmi.Wpf.Views.Data.ProcessTraceability;

namespace ukazkaHmi.Wpf.DataTraceability
{
    public class DataTraceabilityViewModel : MenuControlViewModel
    {
        public DataTraceabilityViewModel()
        {
            this.Title = strings.ProductionData;
            this.AddCommand(typeof(ProcessTraceabilityView), strings.ProductionData);
            this.AddCommand(typeof(OfflineReworkDataView), strings.ReworkOfflineData);
        }
    }
}
