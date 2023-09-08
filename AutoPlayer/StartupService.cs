using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace AutoPlayer
{
    internal class StartupService : BackLogger
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
        }

        public void LoadLastConfiguration()
        {
            if (!File.Exists("./configuration.xml"))
                return;

            try
            {
                var data = XmlDataMusicReader.ReadDataFromFile("./configuration.xml");
                DateChecker.SetDateChecker(data);
            }
            catch (Exception ex)
            {
                Handlers.FileReadHandler(ex);
            }
        }
    }
}
