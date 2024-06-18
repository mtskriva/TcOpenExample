using Vortex.Presentation.Wpf;

namespace ukazkaPlc
{
    public class TcoBasicDoorViewModel : RenderableViewModel
    {
        public TcoBasicDoorViewModel() : base()
        {
            Component = new TcoBasicDoor();
        }

        public TcoBasicDoor Component { get; internal set; }

        public override object Model { get => Component; set => Component = value as TcoBasicDoor; }
    }
}
