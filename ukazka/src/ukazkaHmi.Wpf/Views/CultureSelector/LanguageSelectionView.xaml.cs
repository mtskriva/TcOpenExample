
using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;

using ukazkaHmi.Wpf.Properties;

namespace ukazkaHmi.Wpf
{

    public partial class LanguageSelectionView : Window
    {
        public LanguageSelectionView()
        {
          
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}