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

        /*App()
        {
            /*var data = XmlDataMusicReader.ReadDataFromFile(@"C:\Users\Boltu\Desktop\music.xml");
            foreach(var d in data)
            {
                Print(d.Start.ToString());
                Print(d.Length.ToString());
                Print(d.End.ToString());
                Print(d.Tracks.Length.ToString());
            }

            for(int x = 0; x < 100; x++)
            {
                Print("Spam " + x);
            }
        }
        */
        public static void Print(string val)
        {
            if (window == null)
                window = new ConsoleWindow();

            if (!window.IsActive)
                window.Show();

            window.Print(val);
        }
    }      
    
}