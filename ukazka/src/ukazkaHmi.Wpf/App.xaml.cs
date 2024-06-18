using Raven.Embedded;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using TcOpen.Inxton.Data;
using TcOpen.Inxton.Instructor;
using TcOpen.Inxton.Local.Security;
using TcOpen.Inxton.Local.Security.Wpf;
using TcOpen.Inxton.Security;
using TcOpen.Inxton.TcoCore.Wpf;
using TcOpen.Inxton.RepositoryDataSet;
using Vortex.Presentation.Wpf;
using ukazkaDataMerge.Rework;
using ukazkaInstructor.TcoSequencer;
using ukazkaPlc;
using ukazkaPlcConnector;
using ukazkaProductionPlaner.Planer;
using ukazkaStatistic.Statistics;
using TcOpen.Inxton.Data.MongoDb;
using TcOpen.Inxton.RavenDb;
using System.Diagnostics;
using System.Windows.Media;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using ukazkaHmi.Wpf.Properties;
using System.Globalization;
using System.Threading;
using MongoDB.Driver;
using ukazkaTagsDictionary;
using Vortex.Connector;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using MongoDB.Driver.Core.Configuration;
using System.Net;
using System.Net.Sockets;

namespace ukazkaHmi.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {

            Color primaryColor = SwatchHelper.Lookup[MaterialDesignColor.Indigo];
            Color accentColor = SwatchHelper.Lookup[MaterialDesignColor.Lime];
            ITheme theme = Theme.Create(new MaterialDesignLightTheme(), primaryColor, accentColor);
            Resources.SetTheme(theme);
            base.OnStartup(e);
        }

        public App()
        {

            SetCulture();



            const string SetId = "default";
            Entry.LoadAppSettings(SetId, RepositoryEntry.IsDebug());

            if (!RepositoryEntry.IsDebug())
                DataExchangeActive = Entry.Settings.DataExchange;
            else
                DataExchangeActive = true; //should be true  ,do  exchange data are hanlded by this instance 

            Console.WriteLine("-------------------------------Settings-----------------------------------");
            Console.WriteLine(JsonConvert.SerializeObject(Entry.Settings, Formatting.Indented));
            Console.WriteLine("--------------------------------------------------------------------------");

            GeAssembliesVersion("Tc");
            GeAssembliesVersion("Vortex");
            StopIfRunning();




            // This starts the twin connector operations
            ukazkaPlc.Connector.BuildAndStart().ReadWriteCycleDelay = Entry.Settings.ReadWriteCycleDelay;




            switch (Entry.Settings.DatabaseEngine)
            {
                case DatabaseEngine.RavenDbEmbded:
                    StartRavenDBEmbeddedServer();
                    RepositoryEntry.CreateSecurityManageUsingRavenDb(true, true);
                    SetUpRepositoriesUsingRavenDb();
                    CuxTagsPairing = new TagsPairingController(RepositoryDataSetHandler<TagItem>.CreateSet(new RavenDbRepository<EntitySet<TagItem>>(new RavenDbRepositorySettings<EntitySet<TagItem>>(new string[] { Entry.Settings.GetConnectionString() }, "TagsDictionary", "", ""))), "TagsCfg"); ;

                    // TcOpen app setup
                    TcOpen.Inxton.TcoAppDomain.Current.Builder
                        .SetUpLogger(new TcOpen.Inxton.Logging.SerilogAdapter(new LoggerConfiguration()
                                                .WriteTo.Console()
                                                .WriteTo.File(new Serilog.Formatting.Compact.RenderedCompactJsonFormatter(), "logs\\logs.log")
                                                .Enrich.WithEnvironmentName()
                                                .Enrich.WithEnvironmentUserName()
                                                .Enrich.WithEnrichedProperties()))
                        .SetDispatcher(TcoCore.Wpf.Threading.Dispatcher.Get) // This is necessary for UI operation.  
                        .SetSecurity(SecurityManager.Manager.Service)
                        .SetEditValueChangeLogging(Entry.Plc.Connector)
                        .SetLogin(() => { var login = new LoginWindow(); login.ShowDialog(); })
                        .SetPlcDialogs(DialogProxyServiceWpf.Create(new IVortexObject[] { ukazkaPlc.MAIN._technology._cu00x._processData, ukazkaPlc.MAIN._technology._cu00x._groupInspection, ukazkaPlc.MAIN._technology._cu00x._automatTask, ukazkaPlc.MAIN._technology._cu00x._groundTask }));



                    break;
                case DatabaseEngine.MongoDb:

           
                    Console.WriteLine("Starting DB server...");
                    StartMongoDbServer(Entry.Settings.MongoPath, Entry.Settings.MongoArgs, Entry.Settings.MongoDbRun);
                    Console.WriteLine("Starting DB server...done");

                    Console.WriteLine("Checking database server...");
                    CheckDatabaseAccessibility(Entry.Settings.GetConnectionString());
                    Console.WriteLine($"Checking database server...done");


                    RepositoryEntry.CreateSecurityManageUsingMongoDb(true, true);
                    SetUpRepositoriesUsingMongoDb();
                    CuxTagsPairing = new TagsPairingController(RepositoryDataSetHandler<TagItem>.CreateSet(new MongoDbRepository<EntitySet<TagItem>>(new MongoDbRepositorySettings<EntitySet<TagItem>>(Entry.Settings.GetConnectionString(), Entry.Settings.DbName, "TagsDictionary"))), "TagsCfg");

                    // TcOpen app setup
                    TcOpen.Inxton.TcoAppDomain.Current.Builder
                        .SetUpLogger(new TcOpen.Inxton.Logging.SerilogAdapter(new LoggerConfiguration()
                                                .WriteTo.Console()
                                                .WriteTo.File(new Serilog.Formatting.Compact.RenderedCompactJsonFormatter(), "logs\\logs.log")
                                                .WriteTo.MongoDBBson($@"{Entry.Settings.GetConnectionString()}/{Entry.Settings.DbName}", "log",
                                                                     Entry.Settings.LogRestrictedToMiniummLevel, 50, TimeSpan.FromSeconds(1), Entry.Settings.CappedMaxSizeMb, Entry.Settings.CappedMaxDocuments)
                                                                    .MinimumLevel.Information()
                                                .Enrich.WithEnvironmentName()
                                                .Enrich.WithEnvironmentUserName()
                                                .Enrich.WithEnrichedProperties()))
                        .SetDispatcher(TcoCore.Wpf.Threading.Dispatcher.Get) // This is necessary for UI operation.  
                        .SetSecurity(SecurityManager.Manager.Service)
                        .SetEditValueChangeLogging(Entry.Plc.Connector)
                        .SetLogin(() => { var login = new LoginWindow(); login.ShowDialog(); })
                        .SetPlcDialogs(DialogProxyServiceWpf.Create(new IVortexObject[] { ukazkaPlc.MAIN._technology._cu00x._processData, ukazkaPlc.MAIN._technology._cu00x._groupInspection, ukazkaPlc.MAIN._technology._cu00x._automatTask, ukazkaPlc.MAIN._technology._cu00x._groundTask }));





                    break;
                default:
                    break;
            }




            // Otherwise undocumented feature in official IVF, for details refer to internal documentation.
            LazyRenderer.Get.CreateSecureContainer = (permissions) => new PermissionBox { Permissions = permissions, SecurityMode = SecurityModeEnum.Invisible };






            // Create user roles for this application.
            Roles.Create();

            // Starts the retrieval loop from of the messages from the PLC
            // If you have more TcOpen.Inxton application make sure you retrieve the messages only one of them.
            ukazkaPlc.MAIN._technology._logger.StartLoggingMessages(TcoCore.eMessageCategory.Info);

            SetUpExternalAuthenticationDevice();


            // Authenticates default user, change this line if you need to authenticate different user.

            Console.WriteLine(Entry.Settings.AutologinUserName.ToString());
            Console.WriteLine(Entry.Settings.AutologinUserPassword);
            SecurityManager.Manager.Service.AuthenticateUser(Entry.Settings.AutologinUserName, Entry.Settings.AutologinUserPassword);













        }
        // this is usage, should be placed in remote exec



        /// <summary>
        /// this is remontely invoked from plc , 
        /// </summary>
        /// <param name="pairTagTask"></param>


        private static void SetCulture()
        {
            Culture = Settings.Default.Culture;
            CultureInfo ci = new CultureInfo(Culture);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            LanguageSelectionModel = new LanguageSelectionViewModel();
            LanguageSelectionModel.AddCulture("sk-SK", Path.Combine(Assembly.GetExecutingAssembly().Location, @"\..\..\..\Assets\CulturalFlags\sk.png"));
            LanguageSelectionModel.AddCulture("cs-CZ", Path.Combine(Assembly.GetExecutingAssembly().Location, @"\..\..\..\Assets\CulturalFlags\cz.png"));
            LanguageSelectionModel.AddCulture("en-US", Path.Combine(Assembly.GetExecutingAssembly().Location, @"\..\..\..\Assets\CulturalFlags\us.png"));
        }

        private static void GeAssembliesVersion(string contains)
        {


            Assembly
            .GetExecutingAssembly()
            .GetReferencedAssemblies()
            .Where(assembly => assembly.FullName.Contains(contains)).Where(a => a.Version.Major != 0 || a.Version.Minor != 0)
            .ToList()
            .ForEach(assembly =>
            {
                var info = Assembly.Load(assembly)?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion;
                Console.WriteLine($"{assembly.Name}\t{info.Split('+').First()}\t{assembly}");
            });
        }

        public static void CheckDatabaseAccessibility(string connectionString)
        {
            ConnectionString mongoStr = new ConnectionString(connectionString);

            string host = "";
            int port = 0;

            try
            {
                IPEndPoint endPoint = (IPEndPoint)mongoStr.Hosts.First();
                host = endPoint.Address.ToString();
                port = endPoint.Port;
            }
            catch
            {
                //localhost, dns name
                DnsEndPoint endPoint = (DnsEndPoint)mongoStr.Hosts.First();
                host = endPoint.Host;
                port = endPoint.Port;
            }

            while (true)
            {
                //Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + " INF] Checking database server '" + host + "'...");
                if (IsPortOpen(host, port, new TimeSpan(5000)))
                {
                    break;
                }
                else
                {
                    Thread.Sleep(2000);
                }
            }

            //Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + " INF] Checking database server '" + host + "'... Done");
        }

        private static bool IsPortOpen(string host, int port, TimeSpan timeout)
        {
            try
            {
                //using (var client = new TcpClient())
                //{
                //    var result = client.BeginConnect(host, port, null, null);
                //    var success = result.AsyncWaitHandle.WaitOne(timeout);
                //    if (!success)
                //    {
                //        return false;
                //    }

                //    client.EndConnect(result);
                //}

                using (var tcpClient = new TcpClient())
                {
                    tcpClient.Connect(host, port);
                    tcpClient.Client.Disconnect(false);
                    tcpClient.Dispose();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        private static void SetUpExternalAuthenticationDevice()
        {
            try
            {
                SecurityManager.Manager.Service.ExternalAuthorization = TcOpen.Inxton.Local.Security.Readers.ExternalTokenAuthorization.CreateComReader("COM3");
            }
            catch (Exception ex)
            {
                TcOpen.Inxton.TcoAppDomain.Current.Logger.Warning($"Authentication device was not properly initialized:'{ex.Message}'", ex);
            }
        }
        /// <summary>
        /// Starts mongo on local machine
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="args"></param>
        /// <param name="run"></param>
        public static void StartMongoDbServer(string filePath, string args, bool run)
        {
            bool runLocalMongo = run;
            var fileName = Path.GetFileName(filePath);
            bool isMongoRunning = System.Diagnostics.Process.GetProcesses().Where(p => p.ProcessName.Contains("mongod")).Any();
            if (!isMongoRunning && runLocalMongo)
            {
                var proc = new Process();
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.StartInfo.FileName = filePath;
                proc.StartInfo.Arguments = args;
                proc.Start();

            }
        }
        /// <summary>
        /// Starts embedded instance of RavenDB server.
        /// IMPORTANT! 
        /// CHECK EULA BEFORE USING @ https://ravendb.org/terms
        /// GET APPROPRIATE LICENCE https://ravendb.org/buy FREE COMUNITY EDITION IS ASLO AVAILABLE, BUT YOU NEED TO REGISTER.
        /// STORAGE IS DIRECTED TO THE BIN FOLDER OF REDIRECT 
        /// `DataDirectory` property in this method to persist tha data elsewhere.
        /// </summary>
        private static void StartRavenDBEmbeddedServer()
        {
            // Start embedded RavenDB server

            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine("Starting embedded RavenDB server instance. " +
                "\nYou should not use this instance in production. " +
                "\nUsing embedded RavenDB server you agree to the respective EULA." +
                "\nYou will need to register the licence." +
                "\nThe data are strored in temporary 'bin' folder of your application, " +
                "\nif you want to persist your data safely redirect the DataDirectory into different location.");
            Console.WriteLine("---------------------------------------------------");

            EmbeddedServer.Instance.StartServer(new ServerOptions
            {
                DataDirectory = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName, "tmp", "data"),
                AcceptEula = true,
                ServerUrl = "http://127.0.0.1:8080",
            });

            // EmbeddedServer.Instance.OpenStudioInBrowser();
        }


        private void SetUpRepositoriesUsingRavenDb()
        {
            var ProcessDataRepoSettings = new RavenDbRepositorySettings<PlainProcessData>(new string[] { Entry.Settings.GetConnectionString() }, "ProcessSettings", "", "");
            RepositoryEntry.InitializeRepository(ukazkaPlc.MAIN._technology._processSettings, new RavenDbRepository<PlainProcessData>(ProcessDataRepoSettings), DataExchangeActive);

            var TechnologicalDataRepoSettings = new RavenDbRepositorySettings<PlainTechnologyData>(new string[] { Entry.Settings.GetConnectionString() }, "TechnologySettings", "", "");
            RepositoryEntry.InitializeRepository(ukazkaPlc.MAIN._technology._technologySettings, new RavenDbRepository<PlainTechnologyData>(TechnologicalDataRepoSettings), DataExchangeActive);

            var ReworklDataRepoSettings = new RavenDbRepositorySettings<PlainProcessData>(new string[] { Entry.Settings.GetConnectionString() }, "ReworkSettings", "", "");
            RepositoryEntry.InitializeRepository(ukazkaPlc.MAIN._technology._reworkSettings, new RavenDbRepository<PlainProcessData>(ReworklDataRepoSettings), DataExchangeActive);

            //Statistics
            var _statisticsDataHandler = RepositoryDataSetHandler<StatisticsDataItem>.CreateSet(new RavenDbRepository<EntitySet<StatisticsDataItem>>(new RavenDbRepositorySettings<EntitySet<StatisticsDataItem>>(new string[] { Entry.Settings.GetConnectionString() }, "Statistics", "", "")));
            var _statisticsConfigHandler = RepositoryDataSetHandler<StatisticsConfig>.CreateSet(new RavenDbRepository<EntitySet<StatisticsConfig>>(new RavenDbRepositorySettings<EntitySet<StatisticsConfig>>(new string[] { Entry.Settings.GetConnectionString() }, "StatisticsConfig", "", "")));


            CuxStatistic = new StatisticsDataController(ukazkaPlc.MAIN._technology._cu00x.AttributeShortName, _statisticsDataHandler, _statisticsConfigHandler);



            var Traceability = new RavenDbRepositorySettings<PlainProcessData>(new string[] { Entry.Settings.GetConnectionString() }, "Traceability", "", "");
            RepositoryEntry.InitializeRepository(ukazkaPlc.MAIN._technology._processTraceability, new RavenDbRepository<PlainProcessData>(Traceability), false);
            RepositoryEntry.InitializeRepository(ukazkaPlc.MAIN._technology._cu00x._processData, new RavenDbRepository<PlainProcessData>(Traceability), DataExchangeActive);

            //count data
            //count data
            if (DataExchangeActive)
            {
                new RavenDbRepository<PlainProcessData>(Traceability).OnUpdateDone = (id, data) => { CuxStatistic.Count(data); };
            }

            Rework = new ReworkModel(new RavenDbRepository<PlainProcessData>(ReworklDataRepoSettings), new RavenDbRepository<PlainProcessData>(Traceability));

            //Production planer         
            var _productionPlanHandler = RepositoryDataSetHandler<ProductionItem>.CreateSet(new RavenDbRepository<EntitySet<ProductionItem>>(new RavenDbRepositorySettings<EntitySet<ProductionItem>>(new string[] { Entry.Settings.GetConnectionString() }, "ProductionPlan", "", "")));

            ProductionPlaner = new ProductionPlanController(_productionPlanHandler, "ProductionPlanerTest", new RavenDbRepository<PlainProcessData>(ProcessDataRepoSettings));

            Action prodPlan = () => GetProductionPlan(ukazkaPlc.MAIN._technology._cu00x._productionPlaner);
            ukazkaPlc.MAIN._technology._cu00x._productionPlaner.InitializeExclusively(prodPlan);

            //Instructors
            var _instructionPlanHandler = RepositoryDataSetHandler<InstructionItem>.CreateSet(new RavenDbRepository<EntitySet<InstructionItem>>(new RavenDbRepositorySettings<EntitySet<InstructionItem>>(new string[] { Entry.Settings.GetConnectionString() }, "Instructions", "", "")));

            CuxInstructor = new InstructorController(_instructionPlanHandler, new InstructableSequencer(ukazkaPlc.MAIN._technology._cu00x._automatTask));
            CuxParalellInstructor = new InstructorController(_instructionPlanHandler, new InstructableSequencer(ukazkaPlc.MAIN._technology._cu00x._automatTask._paralellTask));



        }


        private void SetUpRepositoriesUsingMongoDb()
        {
            var ProcessDataRepoSettings = new MongoDbRepositorySettings<PlainProcessData>(Entry.Settings.GetConnectionString(), Entry.Settings.DbName, "ProcessSettings");
            RepositoryEntry.InitializeRepository(ukazkaPlc.MAIN._technology._processSettings, new MongoDbRepository<PlainProcessData>(ProcessDataRepoSettings), DataExchangeActive);
            RepositoryEntry.InitializeIndexProcessDataRepositoryMongoDb(ProcessDataRepoSettings);

            var TechnologicalDataRepoSettings = new MongoDbRepositorySettings<PlainTechnologyData>(Entry.Settings.GetConnectionString(), Entry.Settings.DbName, "TechnologySettings");
            RepositoryEntry.InitializeRepository(ukazkaPlc.MAIN._technology._technologySettings, new MongoDbRepository<PlainTechnologyData>(TechnologicalDataRepoSettings), DataExchangeActive);

            var ReworklDataRepoSettings = new MongoDbRepositorySettings<PlainProcessData>(Entry.Settings.GetConnectionString(), Entry.Settings.DbName, "ReworkSettings");
            RepositoryEntry.InitializeRepository(ukazkaPlc.MAIN._technology._reworkSettings, new MongoDbRepository<PlainProcessData>(ReworklDataRepoSettings), DataExchangeActive);

            //Statistics
            var _statisticsDataHandler = RepositoryDataSetHandler<StatisticsDataItem>.CreateSet(new MongoDbRepository<EntitySet<StatisticsDataItem>>(new MongoDbRepositorySettings<EntitySet<StatisticsDataItem>>(Entry.Settings.GetConnectionString(), Entry.Settings.DbName, "Statistics")));
            var _statisticsConfigHandler = RepositoryDataSetHandler<StatisticsConfig>.CreateSet(new MongoDbRepository<EntitySet<StatisticsConfig>>(new MongoDbRepositorySettings<EntitySet<StatisticsConfig>>(Entry.Settings.GetConnectionString(), Entry.Settings.DbName, "StatisticsConfig")));


            CuxStatistic = new StatisticsDataController(ukazkaPlc.MAIN._technology._cu00x.AttributeShortName, _statisticsDataHandler, _statisticsConfigHandler);



            var Traceability = new MongoDbRepositorySettings<PlainProcessData>(Entry.Settings.GetConnectionString(), Entry.Settings.DbName, "Traceability");
            RepositoryEntry.InitializeRepository(ukazkaPlc.MAIN._technology._processTraceability, new MongoDbRepository<PlainProcessData>(Traceability), false);
            RepositoryEntry.InitializeRepository(ukazkaPlc.MAIN._technology._cu00x._processData, new MongoDbRepository<PlainProcessData>(Traceability), DataExchangeActive);

            //count data
            new MongoDbRepository<PlainProcessData>(Traceability).OnUpdateDone = (id, data) => { CuxStatistic.Count(data); };

            RepositoryEntry.InitializeIndexProcessDataRepositoryMongoDb(Traceability);

            Rework = new ReworkModel(new MongoDbRepository<PlainProcessData>(ReworklDataRepoSettings), new MongoDbRepository<PlainProcessData>(Traceability));

            //Production planer         
            var _productionPlanHandler = RepositoryDataSetHandler<ProductionItem>.CreateSet(new MongoDbRepository<EntitySet<ProductionItem>>(new MongoDbRepositorySettings<EntitySet<ProductionItem>>(Entry.Settings.GetConnectionString(), Entry.Settings.DbName, "ProductionPlan")));

            ProductionPlaner = new ProductionPlanController(_productionPlanHandler, "ProductionPlanerTest", new MongoDbRepository<PlainProcessData>(ProcessDataRepoSettings));

            Action prodPlan = () => GetProductionPlan(ukazkaPlc.MAIN._technology._cu00x._productionPlaner);
            ukazkaPlc.MAIN._technology._cu00x._productionPlaner.InitializeExclusively(prodPlan);

            //Instructors
            var _instructionPlanHandler = RepositoryDataSetHandler<InstructionItem>.CreateSet(new MongoDbRepository<EntitySet<InstructionItem>>(new MongoDbRepositorySettings<EntitySet<InstructionItem>>(Entry.Settings.GetConnectionString(), Entry.Settings.DbName, "Instructions")));

            CuxInstructor = new InstructorController(_instructionPlanHandler, new InstructableSequencer(ukazkaPlc.MAIN._technology._cu00x._automatTask));
            CuxParalellInstructor = new InstructorController(_instructionPlanHandler, new InstructableSequencer(ukazkaPlc.MAIN._technology._cu00x._automatTask._paralellTask));



        }

        private void GetProductionPlan(ProductionPlaner productionPlaner)
        {
            ProductionItem item;

            ProductionPlaner.RefreshItems(out item);
            productionPlaner._requiredProcessSettings.Synchron = item.Key;
            productionPlaner._productonPlanCompleted.Synchron = ProductionPlaner.ProductionPlanCompleted;
            productionPlaner._productionPlanIsEmpty.Synchron = ProductionPlaner.ProductionPlanEmpty;



        }



        /// <summary>
        /// Gets the twin connector for this application.
        /// </summary>
        public static ukazkaPlcTwinController ukazkaPlc
        {
            get
            {
                return designTime ? Entry.PlcDesign : Entry.Plc;
            }
        }

        static string Culture = "";
        public static ReworkModel Rework { get; private set; }
        public static ProductionPlanController ProductionPlaner { get; private set; }
        public static InstructorController CuxInstructor { get; private set; }
        public static InstructorController CuxParalellInstructor { get; private set; }
        public static StatisticsDataController CuxStatistic { get; private set; }
        public static TagsPairingController CuxTagsPairing { get; private set; }
        public static LanguageSelectionViewModel LanguageSelectionModel { get; private set; }
        public static ShutdownViewModel AppShutdownModel { get; private set; } = new ShutdownViewModel();
        public bool DataExchangeActive { get; private set; } = true;


        /// <summary>
        /// Determines whether the application at design time. (true when at design, false at runtime)
        /// </summary>
        private static bool designTime = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());


        /// <summary>
        /// Checks that no other instance of this program is running on this system.
        /// </summary>
        private void StopIfRunning()
        {
            var processes = System.Diagnostics.Process.GetProcessesByName(Assembly.GetEntryAssembly().GetName().Name);

            if (processes.Count() > 1)
            {
                MessageBox.Show("This program is already running on this system. We cannot run another instance of this program.",
                                "Checking for running processes", MessageBoxButton.OK, MessageBoxImage.Error);
                KillOtherInstances();
                Application.Current.Shutdown(-1);
            }
        }


        private void KillOtherInstances() => Process
                    .GetProcessesByName(Process.GetCurrentProcess().ProcessName)
                    .Where(process => process.Id != Process.GetCurrentProcess().Id)
                    .ToList()
                    .ForEach(p => p.Kill());


    }
}
