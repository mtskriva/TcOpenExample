using ukazkaHmi.Wpf.Properties;
using ukazkaHmi.Wpf.Views.Data.ProcessSettings;
using ukazkaHmi.Wpf.Views.Data.ReworkSettings;
using ukazkaHmi.Wpf.Views.Data.TechnologicalSettings;

using ukazkaHmi.Wpf.Views.Data.OfflineReworkData;

namespace ukazkaHmi.Wpf.Data
{
    public class DataViewModel : MenuControlViewModel
    {
        public DataViewModel()
        {
            this.Title = strings.Settings;
            this.AddCommand(typeof(ProcessSettingsView), strings.ProcessData);
            this.AddCommand(typeof(TechnologicalSettingsView), strings.TechnologicalSettings);
            this.AddCommand(typeof(ReworkSettingsView), strings.ReworkData);
        }
    }
}
