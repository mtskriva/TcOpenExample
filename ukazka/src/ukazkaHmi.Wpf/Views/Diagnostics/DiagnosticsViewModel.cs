

using System;
using System.Windows;
using System.Windows.Controls;
using TcoCore;
using TcoIo;
using TcOpen.Inxton;
using TcOpen.Inxton.Input;
using ukazkaPlc;

namespace ukazkaHmi.Wpf.Views.Diagnostics
{
    public class DiagnosticsViewModel : MenuControlViewModel
    {
        public DiagnosticsViewModel() : base()
        {

            ShowTopologyCommand = new RelayCommand(a => ShowTopology());
        }

        private void ShowTopology()
        {
            TcoAppDomain.Current.Dispatcher.Invoke(
           () =>
           {
               var win = new TopologyView();
               var viewInstance = Activator.CreateInstance(win.GetType());
        
                win = viewInstance as TopologyView;
               if (win != null)
               {
                   win.DataContext = ukazkaPlc.GVL_iXlinker;
                   win.Show();

                  
               }
           }
           );
        }
        public TopologyRenderer TopologyView { get; } = new TopologyRenderer();
        public ukazkaPlcTwinController ukazkaPlc { get { return App.ukazkaPlc; } }

        public RelayCommand ShowTopologyCommand { get; }
    }
}
