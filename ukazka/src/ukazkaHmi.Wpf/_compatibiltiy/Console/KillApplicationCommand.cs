using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ukazkaHmi.Wpf
{
    public class KillApplicationCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Application.Current.Shutdown();
        }
    }
}
