using System;
using System.Linq;
using TcOpen.Inxton.Data.Json;
using TcOpen.Inxton.RepositoryDataSet;
using Vortex.Adapters.Connector.Tc3.Adapter;
using Vortex.Connector;
using ukazkaPlc;

namespace ukazkaPlcConnector
{
    public class Entry
    {

        


        private static ukazkaPlcTwinController plc =  new ukazkaPlcTwinController(Tc3ConnectorAdapter.Create(851,true));

     

        private static ApplicationSettings _settings = new ApplicationSettings();


        public static ukazkaPlc.ukazkaPlcTwinController Plc => plc;

        private static ukazkaPlc.ukazkaPlcTwinController _plcDesign;
        public static ukazkaPlc.ukazkaPlcTwinController PlcDesign
        {
            get
            {
                if (_plcDesign == null) _plcDesign = new ukazkaPlc.ukazkaPlcTwinController(new ConnectorAdapter(typeof(DummyConnector)));
                return _plcDesign;
            }
        }

        public static ApplicationSettings Settings { get { return _settings; } } 

        /// <summary>
        /// Load specific parameters stored in json file stored in 'ukazkaPlcConnector.Properties.Settings.Default.SettingsLocation'
        /// </summary>
        /// <param name="setId">Name for set</param>
        public static void LoadAppSettings(string setId,bool inDevelopingMode)
        {
            if (!inDevelopingMode)
            {

                RepositoryDataSetHandler<ApplicationSettings> _settings = RepositoryDataSetHandler<ApplicationSettings>.CreateSet(new JsonRepository<EntitySet<ApplicationSettings>>(new JsonRepositorySettings<EntitySet<ApplicationSettings>>(Properties.Settings.Default.SettingsLocation)));//todo tco adresar
                var result = _settings.Repository.Queryable.FirstOrDefault(p => p._EntityId == setId);
 
                var set = new EntitySet<ApplicationSettings>();
                set._Modified = DateTime.Now;
                set._EntityId = setId;

                if (result == null)
                {
                    set._Created = DateTime.Now;

                    _settings.Create(setId, set);
                }


            Entry._settings = _settings.Read(setId).Item;
            }
            plc = Entry._settings.DeployMode == DeployMode.Dummy
                ? new ukazkaPlcTwinController(new ConnectorAdapter(typeof(DummyConnector)))
                : Entry._settings.DeployMode == DeployMode.Local
                    ? new ukazkaPlcTwinController(Tc3ConnectorAdapter.Create(851, Settings.ShowConsoleOutput))
                    : new ukazkaPlcTwinController(Tc3ConnectorAdapter.Create(Entry._settings.PlcAmsId, 851, Settings.ShowConsoleOutput));
        }

      
    }

}
