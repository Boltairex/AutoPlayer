using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlayer
{
    public static class Handlers
    {
        public static void FileReadHandler(Exception exception)
        {
            App.WriteLine(exception);
        }
    }
}
