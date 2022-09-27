using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AutoPlayer
{
    public class AudioController
    {
        /// <summary>
        /// Triggered when Audio receive data.
        /// </summary>
        public static event Action<MusicData> AudioLoaded;
        /// <summary>
        /// Triggered when track was changed.
        /// </summary>
        public static event Action<TrackData> CurrentTrackChanged;
        /// <summary>
        /// Basic delay in 500 milliseconds.
        /// </summary>
        public static event Action<int> CurrentTimeEverySecond;

        public static AudioController Instance { get; private set; }
        static readonly object lockThread = new object();
        
        Volume? volume;
        WaveOut? waveOut;
        Mp3FileReader? mp3Reader;
        WaveFileReader? wavReader;
        MusicData? data;

        int currentPosition = 0;

        AudioController()
        {
            Instance = this;
            Reset();
            DateChecker.DataAvailable += DataAvailableHandle;
            DateChecker.DataExpired += DataExpiredHandle;
            CurrentTimeEverySecond += EverySecondCheck;
        }

        public static AudioController SetAudioController()
        {
            lock (lockThread)
            {
                if (Instance != null)
                    return Instance;
                else
                    return new AudioController();
            }
        }

        private void EverySecondCheck(int obj)
        {
            if (CurrentTimeInSeconds() >= TotalTimeInSeconds())
                SkipToNextAudio();
        }

        public int CurrentTimeInSeconds()
        {
            if (waveOut != null)
                return (int)(waveOut.GetPosition() / waveOut.OutputWaveFormat.AverageBytesPerSecond);
            return 0;
        }

        /// <summary>
        /// Total length of current track in seconds.
        /// </summary>
        /// <returns></returns>
        public int TotalTimeInSeconds()
        {
            if (mp3Reader != null)
                return (int)mp3Reader.TotalTime.TotalSeconds;
            else if (wavReader != null)
                return (int)wavReader.TotalTime.TotalSeconds;
            return 0;
        }
        
        public void DebugSetData(MusicData data)
        {
            DataAvailableHandle(data);
        }

        /// <summary>
        /// Pauses reader from playing.
        /// </summary>
        public void Stop()
        {
            if (waveOut != null && waveOut.PlaybackState == PlaybackState.Playing)
            {
                waveOut.Stop();
            }
        }

        /// <summary>
        /// Stars or resumes player.
        /// </summary>
        public void Start()
        {
            if (waveOut != null && waveOut.PlaybackState != PlaybackState.Playing && data != null && data.TracksCount > 0)
            {
                waveOut.Play();
            }
        }

        /// <summary>
        /// Changing volume of current song.
        /// </summary>
        /// <param name="intVolume"></param>
        public void ChangeVolume(int intVolume)
        {
            if (volume == null || waveOut == null)
                return;

            volume.SetVolume(intVolume);
            waveOut.Volume = volume;
        }

        /// <summary>
        /// Plays next music from playlist.
        /// </summary>
        public void SkipToNextAudio()
        {
            if (waveOut == null || data == null)
                return;

            currentPosition += 1;
            if (currentPosition >= data.TracksCount)
                currentPosition = 0;
            Play();      
        }

        /// <summary>
        /// Plays previous music from playlist.
        /// </summary>
        public void SkipToPreviousAudio()
        {
            if (waveOut == null || data == null)
                return;

            currentPosition -= 1;
            if (currentPosition < 0)
                currentPosition = data.TracksCount - 1;
            Play();
        }

        /// <summary>
        /// Creates data about current track.
        /// </summary>
        /// <returns>If data does not exists, returns null.</returns>
        public List<TrackViewData> GetInfoAboutTracks()
        {
            if(data == null)
                return null;

            List<TrackViewData> info = new List<TrackViewData>();
            for(int x = 0; x < data.TracksCount; x++)
                info.Add(new TrackViewData(data.Tracks[x], x));

            return info;
        }

        void Play()
        {
            if (data == null)
                return;

            if (waveOut != null)
            {
                waveOut.Stop();
                waveOut.Dispose();
            }
            waveOut = new WaveOut();
            StopTimer();
            
            var music = data.Tracks[currentPosition].GetFullPath();
            if (data.Tracks[currentPosition].GetFormat() == FileFormat.MP3)
            {
                mp3Reader = new Mp3FileReader(music);
                if (wavReader != null)
                {
                    wavReader.Dispose();
                    wavReader = null;
                }
                waveOut.Init(mp3Reader);
            }
            else
            {
                wavReader = new WaveFileReader(music);
                if (mp3Reader != null)
                {
                    mp3Reader.Dispose();
                    mp3Reader = null;
                }
                waveOut.Init(wavReader);
            }

            volume = data.Tracks[currentPosition].GetVolume();
            waveOut.Volume = volume;
            waveOut.Play();
            StartTimer();
            CurrentTrackChanged?.Invoke(data.Tracks[currentPosition]);
        }

        void DataExpiredHandle()
        {
            Reset();
        }

        void DataAvailableHandle(MusicData obj)
        {
            if (obj.Shuffle)
            {
                TrackData[] tracks = new TrackData[obj.TracksCount];
                List<int> tracksShuffle = new List<int>();
                for (int x = 0; x < obj.TracksCount; x++)
                    tracksShuffle.Add(x);

                Random random = new Random();

                int i;
                for (int x = 0; x < tracks.Count(); x++)
                {
                    i = random.Next(0, tracksShuffle.Count);
                    tracks[x] = obj.Tracks[tracksShuffle[i]];
                    tracksShuffle.RemoveAt(i);
                }

                this.data = new MusicData(tracks, true, obj.Start, obj.Length, obj.End);
            }
            else
                data = obj;

            AudioLoaded?.Invoke(data);
            Play();
        }

/*        long Skip(int seconds)
        {
            return mp3Reader.Position += mp3Reader.WaveFormat.AverageBytesPerSecond * seconds;
        }*/

        /// <summary>
        /// Prepares object for the next cycle.
        /// </summary>
        void Reset()
        {
            if (waveOut != null)
            {
                waveOut.Stop();
                waveOut.Dispose();
            }

            if (wavReader != null)
            {
                wavReader.Dispose();
                wavReader = null;
            }

            if (mp3Reader != null)
            {
                mp3Reader.Dispose();
                mp3Reader = null;
            }

            StopTimer();
            currentPosition = 0;
            data = null;
            volume = null;
            waveOut = new WaveOut();
        }

        CancellationTokenSource source;
        void StartTimer()
        {
            if (source == null || !source.TryReset())
                source = new CancellationTokenSource();

            LegitAsync.NewAsync(InnerSecondsTimer, source.Token);
        }

        void StopTimer()
        {
            if(source != null)
                source.Cancel();
        }

        async Task InnerSecondsTimer(CancellationToken token) 
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(500, token);
                CurrentTimeEverySecond?.Invoke(CurrentTimeInSeconds());
            }
        }
    }
    /// <summary>
    /// Info to display on GUI.
    /// </summary>
    public class TrackViewData
    {
        readonly TrackData data;
        readonly int position;

        public TrackViewData(TrackData data, int position)
        {
            this.data = data;
            this.position = position;
        }

        public int Position => position;

        public string Name => data.Name;

        public int DefaultVolume => data.Volume.Value;

        public string Extension => data.Extension;
    }
}