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
        static StartupService service;
        static ConsoleWindow window;

        App()
        {
            service = StartupService.SetStartupService();
            TestWindow window = new TestWindow();
            window.Show();
        }

        static void SetConsoleWindow()
        {
            if (window == null)
                window = new ConsoleWindow();
            if (!window.IsActive)
                window.Show();
        }

        /// <summary>
        /// Use <see cref="WriteLine(object)"/> instead.
        /// </summary>
        /// <param name="val"></param>
        [Obsolete]
        public static void Print(object val)
        {
            if (val == null)
                return;

            SetConsoleWindow();
            window.Print(val);
        }

        public static void WriteLine(object val)
        {
            if (val == null)
                return;
            SetConsoleWindow();
            window.Println(val);
        } 

        public static void Write(object val)
        {
            if (val == null)
                return;
            SetConsoleWindow();
            window.Print(val);
        }
    }
}