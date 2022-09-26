using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AutoPlayer
{
    static class LegitAsync
    {
        public static void NewAsync(Delegate method, params object[] arguments)
        {
            Application.Current.Dispatcher.Invoke(method, arguments);
        }
    }
}
