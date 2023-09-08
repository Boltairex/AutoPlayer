using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlayer
{
    /// <summary>
    /// Very simple and autistic logger (total opposite of SOLID)
    /// </summary>
    public class BackLogger
    {
        static StringBuilder loggerCache = new StringBuilder();
        static readonly object _lock = new object();

        static BackLogger()
        {
            App.Current.Exit += OnExit;
        }

        static void OnExit(object sender, System.Windows.ExitEventArgs e)
        {
            lock (_lock)
            {
                Directory.CreateDirectory("./Logs/");
                File.WriteAllText($"./Logs/Logs{Tools.GetCurrentDate}.txt", loggerCache.ToString());
            }
        }

        public virtual void Log(object message, LogType type = LogType.Info)
        {
            lock (_lock)
            {
                loggerCache.Append($"{DateTime.UtcNow.ToShortTimeString()} [{type}] {message}\n");
            }
        }

        public virtual void Log(Exception exception)
        {
            lock (_lock)
            { 
                var inner = "";
                if (exception.InnerException != null)
                    inner = $"\n\t[inner:{exception.InnerException.GetType()}]: {exception.InnerException.Message}";
                loggerCache.Append($"{DateTime.UtcNow.ToShortTimeString()} [{LogType.Exception}:{exception.GetType()}] {exception.Message}{inner}\n");
            } 
        }
    }

    public enum LogType 
    { 
        Exception,
        Info,
        Warning
    }

}
