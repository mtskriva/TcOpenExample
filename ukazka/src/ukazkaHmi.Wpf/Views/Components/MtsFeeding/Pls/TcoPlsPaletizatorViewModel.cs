using ukazkaHmi.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TcoCore;
using TcOpen.Inxton.Local.Security;
using Vortex.Presentation.Wpf;

namespace ukazkaPlc
{
    public class TcoPlsPaletizatorViewModel : RenderableViewModel
    {
        public TcoPlsPaletizatorViewModel()
        { }

        public TcoPlsPaletizator Component { get; private set; } = new TcoPlsPaletizator();

        public override object Model { get => Component; set { Component = (TcoPlsPaletizator)value; } }


    }

    public class TcoPlsPaletizatorServiceViewModel : TcoPlsPaletizatorViewModel
    {
    }

}
