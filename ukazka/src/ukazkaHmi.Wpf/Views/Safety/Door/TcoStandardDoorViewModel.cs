

using Vortex.Presentation.Wpf;

namespace ukazkaPlc
{
    public class TcoStandardDoorViewModel
        : RenderableViewModel
    {
        public TcoStandardDoorViewModel() : base()
        {
            Component = new TcoStandardDoor();
        }

        public TcoStandardDoor Component { get; internal set; }

        public override object Model { get => Component; set => Component = value as TcoStandardDoor; }
    }
}
