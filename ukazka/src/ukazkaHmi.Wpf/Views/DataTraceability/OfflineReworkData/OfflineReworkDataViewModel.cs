
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using TcOpen.Inxton.Input;
using ukazkaDataMerge.Rework;
using ukazkaHmi.Wpf;
using ukazkaPlc;

namespace ukazkaHmi.Wpf.Views.Data.OfflineReworkData
{
    public class OfflineReworkDataViewModel
{
    public OfflineReworkDataViewModel
        ()
    {
        MergeCommmand = new RelayCommand(MergeDataAsync);
    }


    public ProcessDataManager SourceData
    {
        get
        {
                return App.ukazkaPlc.MAIN._technology._reworkSettings;
        }
    }



    public ProcessDataManager TargetData
    {
        get
        {
            return App.ukazkaPlc.MAIN._technology._cu00x._processData;
        }
    }


    public ReworkModel ReworkModel
    {
        get
        {
            return App.Rework;
        }
    }
    public RelayCommand MergeCommmand { get; private set; }


    public void MergeDataAsync(object parameter)
    {
        var param = (Tuple<string, string>)parameter;

        var source = param.Item1 as string;
        var target = param.Item2 as string;

        ReworkModel.ReworkEntityAsync(source, target);
    }


}



    public class MultiParamConverter : IMultiValueConverter
{

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        Tuple<string, string> tuple = new Tuple<string, string>(
           (string)values[0], (string)values[1]);
        return (object)tuple;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}


}