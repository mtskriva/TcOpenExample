using System;
using System.Windows.Controls;


namespace ukazkaHmi.Wpf
{
    /// <summary>
    /// Interaction logic for SystemMonitor.xaml
    /// </summary>
    public partial class SystemMonitor : UserControl
    {
        public SystemMonitor()
        {
            InitializeComponent();
            Console.SetOut(SystemDiagnosticsSingleton.Instance.ConsoleWriter);
        }
    }
}
