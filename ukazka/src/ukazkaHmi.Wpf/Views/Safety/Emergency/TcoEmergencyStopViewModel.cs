using Vortex.Presentation.Wpf;

namespace ukazkaPlc
{
    public class TcoEmergencyStopViewModel : RenderableViewModel
    {
        public TcoEmergencyStopViewModel() : base()
        {
            Component = new TcoEmergencyStop();
        }

        public TcoEmergencyStop Component { get; internal set; }

        public override object Model { get => Component; set => Component = value as TcoEmergencyStop; }
    }
}
