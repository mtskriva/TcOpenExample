
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ukazkaHmi.Wpf.Properties;
using System.Globalization;
using System.Windows.Input;
using TcOpen.Inxton.Input;

namespace ukazkaHmi.Wpf
{
    public class ShutdownViewModel
        
    {
        public ShutdownViewModel()
        {
            ShutdownCommand = new RelayCommand(a => ShutdownApplication());
        }

        
        public RelayCommand ShutdownCommand { get; private set; }

        public void ShutdownApplication()
        {


            var result = MessageBox.Show(strings.ShutdownApplicationMessageBox, strings.ShutdownApplicationMessageBoxCaption, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            Application.Current.Shutdown();
        }
    }



}
