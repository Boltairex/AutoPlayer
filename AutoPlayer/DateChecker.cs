using System;
using System.Threading;
using System.Threading.Tasks;

namespace AutoPlayer
{
    /// <summary>
    /// Singleton object of date checking with callbacks.
    /// </summary>
    internal class DateChecker : BackLogger
    {
        public static DateChecker Instance { get; private set; }

        public static event Action DataExpired;
        public static event Action<MusicData> DataAvailable;

        public bool IsAvailable => selected >= 0;
        public MusicData? GetCurrentData => selected >= 0 ? data[selected] : null;

        CancellationTokenSource source;
        MusicData[] data;
        int selected = -1;

        static readonly object _lock = new object();

        public static DateChecker SetDateChecker(MusicData[] data)
        {
            lock (_lock)
            {
                if (Instance == null)
                    return new DateChecker(data);
                else
                {
                    Instance.data = data;
                    Instance.source.Cancel();
                    Instance.selected = -1;
                    if (!Instance.source.TryReset())
                        Instance.source = new CancellationTokenSource();

                    LegitAsync.NewAsync(Instance.TryRunData, data, Instance.source.Token);
                }
            }
            return Instance;
        }

        private DateChecker(MusicData[] loadData)
        {
            if (Instance != null)
                return;

            Instance = this;
            data = loadData;
            source = new CancellationTokenSource();

            LegitAsync.NewAsync(TryRunData, data, source.Token);
        }

        private async Task TryRunData(MusicData[] data, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                switch (Instance.selected)
                {
                    case -1:
                        for (int x = 0; x < data.Length; x++)
                        {
                            if (DateTime.Now < data[x].GetTodaysEnd() && DateTime.Now >= data[x].GetTodaysStart())
                            {
                                lock (_lock)
                                {
                                    Log($"Load {data[x].Tracks.Length} tracks.");
                                    Instance.SetData(x);
                                }
                                break;
                            }
                        }
                        await Task.Delay(10000, token);
                        break;

                    default:
                        if (DateTime.Now >= data[Instance.selected].GetTodaysEnd() || DateTime.Now < data[Instance.selected].GetTodaysStart())
                        {
                            lock (_lock)
                            {
                                Log($"Unload data (music is expired).");
                                Instance.SetData(-1);
                            }
                            break;
                        }
                        await Task.Delay(1000, token);
                        break;
                }
            }
        }

        private void SetData(int index)
        {
            selected = index;
            if (index == -1)
                DataExpired?.Invoke();
            else
                DataAvailable?.Invoke(data[index]);
        }
    }
}