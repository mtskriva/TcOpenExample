

namespace ukazkaHmi.Wpf
{
    public class SystemDiagnosticsViewModel
    {
        private readonly KillApplicationCommand killAppCmd = new KillApplicationCommand();

        public KillApplicationCommand KillAppCmd
        {
            get
            {
                return this.killAppCmd;
            }
        }

        public SystemDiagnosticsSingleton Model
        {
            get
            {
                return SystemDiagnosticsSingleton.Instance;
            }
        }
    }
}
