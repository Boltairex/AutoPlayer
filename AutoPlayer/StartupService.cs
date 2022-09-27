using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace AutoPlayer
{
    internal class StartupService
    {
        public static StartupService Instance { get; private set; }

        static readonly object threadLock = new object();

        public static StartupService SetStartupService()
        {
            lock (threadLock)
            {
                if (Instance == null)
                {
                    return new StartupService();
                }
                return Instance;
            }
        }

        private StartupService()
        {
            Instance = this;
            LoadLastConfiguration();
        }

        public void LoadLastConfiguration()
        {
            if (!File.Exists("configuration.xml"))
                return;

            var data = XmlDataMusicReader.ReadDataFromFile("configuration.xml");
            DateChecker.SetDateChecker(data);
        }
    }
}
