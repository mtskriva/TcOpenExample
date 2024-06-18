using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ukazkaTests
{
    internal class ukazkaApp
    {
        private Process appProcess;
        private void StartApp()
        {
            var slnFolder = Path.GetFullPath(Path.Combine(Assembly.GetExecutingAssembly().Location, "..\\..\\..\\..\\..\\..\\..\\..\\..\\")); //Environment.GetEnvironmentVariable("slnFolder") ?? @"..\";

#if DEBUG
            var ukazkaFolder = @"templates\mts-s-template\t\src\ukazkaHmi.Wpf\bin\Debug\net48";
#else
            var ukazkaFolder = @"templates\mts-s-template\t\src\ukazkaHmi.Wpf\bin\Release\net48";
#endif
            var ukazkaExe = "ukazkaHmi.Wpf.exe";
            var applicationPath = Path.GetFullPath(Path.Combine(slnFolder, ukazkaFolder, ukazkaExe));
            var app = Path.GetFullPath(applicationPath);
            if (File.Exists(applicationPath))
            {
                appProcess = Process.Start(applicationPath);
                System.Threading.Thread.Sleep(10000);
            }
            else
                throw new EntryPointNotFoundException(applicationPath + "Not found. Current PWD " + Environment.CurrentDirectory);

        }

        public void KillApp()
        {
            appProcess?.Kill();
            _instance = null;
        }

        private ukazkaApp()
        {
            this.StartApp();
        }


        static ukazkaApp _instance;

        public static ukazkaApp Get
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ukazkaApp();
                }

                return _instance;
            }

        }



    }
}
