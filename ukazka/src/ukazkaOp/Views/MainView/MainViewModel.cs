using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TcOpen.Inxton;
using TcOpen.Inxton.Local.Security.Wpf;
using ukazkaHmi.Wpf;
using ukazkaHmi.Wpf.Views.Diagnostics;
using ukazkaOp.Data;
using ukazkaHmi.Wpf.Properties;
using ukazkaOp.Views.Operator;
using ukazkaPlc;

namespace ukazkaOp.Views.MainView
{
    public class MainViewModel : MenuControlViewModel
    {
        public MainViewModel()
        {
            Title = "TECHNOLOGY";
            OpenCommand(this.AddCommand(typeof(OperatorView), strings.Operator));
            AddCommand(typeof(DataView), strings.Data);
            AddCommand(typeof(UserManagementGroupManagementView), strings.UserManagement);
            AddCommand(typeof(DiagnosticsView), strings.Diagnostics);
            OpenLoginWindowCommand = new TcOpen.Inxton.Input.RelayCommand(a => OpenLoginWindow());
            LogOutWindowCommand = new TcOpen.Inxton.Input.RelayCommand(a => TcOpen.Inxton.TcoAppDomain.Current.AuthenticationService.DeAuthenticateCurrentUser());
            OpenLanguageCommand = new TcOpen.Inxton.Input.RelayCommand(a => OpenLanguageWindow());
            CloseApplicationCommand = new TcOpen.Inxton.Input.RelayCommand(a => ClloseApplication());
        }

        private void ClloseApplication()
        {
            TcoAppDomain.Current.Dispatcher.Invoke(
             (Action)(() =>
             {
                 var win = new ShutdownView();
                 var viewInstance = Activator.CreateInstance((Type)win.GetType());

                 win = viewInstance as ShutdownView;
                 if (win != null)
                 {
                     win.DataContext = App.AppShutdownModel;
                     win.ShowDialog();


                 }
             })
           );
        }

        public TcOpen.Inxton.Input.RelayCommand CloseApplicationCommand { get; private set; }
        public TcOpen.Inxton.Input.RelayCommand OpenLoginWindowCommand { get; private set; }
        public TcOpen.Inxton.Input.RelayCommand LogOutWindowCommand { get; private set; }
        public TcOpen.Inxton.Input.RelayCommand OpenLanguageCommand { get; private set; }



        private void OpenLanguageWindow()
        {
            TcoAppDomain.Current.Dispatcher.Invoke(
           (Action)(() =>
           {

            
               var win = new LanguageSelectionView();
               var viewInstance = Activator.CreateInstance((Type)win.GetType());

               win = viewInstance as LanguageSelectionView;
               if (win != null)
               {
                   win.DataContext = App.LanguageSelectionModel;
                   win.ShowDialog();


               }
           })
       );
        }
        public void OpenLoginWindow()
        {
            var loginWindow = new LoginWindow();
            loginWindow.ShowDialog();
        }
        public ukazkaPlcTwinController ukazkaPlc { get { return App.ukazkaPlc; } }
    }

}
