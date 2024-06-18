using ukazkaPlc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ukazkaProductionPlaner.Planer.View;
using ukazkaInstructor;
using ukazkaStatistic.Statistics.View;
using ukazkaTagsDictionary.View;
using ukazkaPlcConnector;

namespace ukazkaHmi.Wpf.Views.Operator
{
    public class OperatorViewModel
    {
        public OperatorViewModel()
        {
            ukazkaPlc.MAIN._technology._automatAllTask.ExecuteDialog = () => 
            {
                return MessageBox.Show(ukazkaHmi.Wpf.Properties.strings.AutomatAllWarning, "Automat", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;                    
            };

            ukazkaPlc.MAIN._technology._groundAllTask.ExecuteDialog = () =>
            {
                return MessageBox.Show(ukazkaHmi.Wpf.Properties.strings.GroundAllWarning, "Automat", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
            };

            ukazkaPlc.MAIN._technology._automatAllTask.Roles = DefaultRoles.technology_automat_all;
            ukazkaPlc.MAIN._technology._groundAllTask.Roles = DefaultRoles.technology_ground_all;

            ProductionPlanViewModel = new ProductionPlanViewModel(App.ProductionPlaner);
            InstructorViewModel = new InstructorViewModel(App.CuxInstructor);
            InstructorParalellViewModel = new InstructorViewModel(App.CuxParalellInstructor);
            StatisticViewModel = new StatisticsDataViewModel(App.CuxStatistic);
            TagsPairingViewModel = new TagsPairingViewModel(App.CuxTagsPairing);



        }

        public ukazkaPlcTwinController ukazkaPlc { get { return App.ukazkaPlc; } }

        public ProductionPlanViewModel ProductionPlanViewModel { get; private set; }
        public InstructorViewModel InstructorViewModel { get; private set; }
        public InstructorViewModel InstructorParalellViewModel { get; private set; }
        public StatisticsDataViewModel StatisticViewModel { get; private set; }
        public TagsPairingViewModel TagsPairingViewModel { get; private set; }
    }
}
