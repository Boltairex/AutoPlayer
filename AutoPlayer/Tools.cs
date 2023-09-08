using System;

namespace AutoPlayer
{
    public static class Tools
    {
        public static string GetCurrentDate { get => $"{DateTime.Now.ToString().Replace(':', '-').Replace('/', '-')}"; }
    }
}
