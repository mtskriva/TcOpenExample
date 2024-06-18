using System.Linq;
using Vortex.Presentation.Wpf;

namespace ukazkaPlc
{
    public class TcoTaskedServiceTaskCommandViewModel : RenderableViewModel
    {
        public TcoTaskedServiceTaskCommandViewModel()
        {
            
        }

        TcoTaskedService component;
        public TcoTaskedService Component
        {
            get => component;
            private set
            {
                if (component == value)
                {
                    return;
                }

                SetProperty(ref component, value);
            }
        }        

        public override object Model { get => Component; set => Component = (TcoTaskedService)value; }
    }
}
