using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AutoPlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static ConsoleWindow window;
        static MainWindow start;
        static BackLogger logger = new BackLogger();

        App()
        {
            try
            {
                AudioController.SetAudioController();

                start = new MainWindow();
                start.Show();

                //After all services like AudioController and windows
                logger.Log("System, Set audio controller. Request last configuration to be loaded.");
                StartupService.SetStartupService().LoadLastConfiguration();
            }
            catch(Exception e)
            {
                WriteLine(e);
            }
        }

        static void SetConsoleWindow()
        {
            if (window == null)
                window = new ConsoleWindow();
            if (!window.IsActive)
                window.Show();
        }


        public static void WriteLine(Exception ex)
        {
            if (ex == null)
                return;

            logger.Log(ex);

            SetConsoleWindow();
            window.Println($"[ERROR] {ex.Message}\nfrom ({ex.Source}), in ({ex.TargetSite})");
        }

        public static void WriteLine(object val)
        {
            if (val == null)
                return;

            if (val is Exception ex)
                logger.Log(ex);
            else
                logger.Log(val);
            
            SetConsoleWindow();
            window.Println(val);
        } 

        public static void Write(object val)
        {
            if (val == null)
                return;

            if (val is Exception ex)
                logger.Log(ex);
            else
                logger.Log(val);

            SetConsoleWindow();
            window.Print(val);
        }
    }
}