using Vortex.Presentation.Wpf;

namespace ukazkaPlc
{
    public class TcoTaskedSequencerTaskCommandViewModel : RenderableViewModel
    {      
        TcoTaskedSequencer component;
        public TcoTaskedSequencer Component
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

        public override object Model { get => Component; set => Component = (TcoTaskedSequencer)value; }
    }
}
