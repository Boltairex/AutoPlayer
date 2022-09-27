using NAudio.Wave;
using System;
using System.Linq;
using System.Media;
using System.Windows.Controls;

namespace AutoPlayer
{
    public class AudioController
    {
        Volume volume;
        WaveOut waveOut;
        Mp3FileReader reader; // e nie ma jeszcze singletona to jakos dzielilam zmienne woozy
        private MusicData data;
        int currentPosition = 0;
        public int TotalTime() => reader.TotalTime.Seconds;
        static readonly object lockThread = new object();
        public static AudioController Instance { get; set; } = new AudioController();
       
        private AudioController()
        {
            lock (lockThread) {
                if(Instance != null )
                    return;
                Instance = this;
                volume = new Volume();
                waveOut = new WaveOut();
                DateChecker.DataAvailable += Instance_DataAvailable;
                DateChecker.DataExpired += Instance_DataExpired;
            }
        }
        private void Instance_DataExpired()
        {
            Stop();
            volume = new();
            waveOut = new();
        }

        private void Instance_DataAvailable(MusicData obj)
        {
            data = obj;
            Play();   
        }
        public void Play()
        {
            var music = data.Tracks[currentPosition].GetFullPath();
            reader = new Mp3FileReader(music);
            volume.SetVolume(100); // poczatkowe volume, zmienic eventualnie
            waveOut.Volume = volume.GetVolumeInt()/100f; //setting volume at the start 
            waveOut.Pause();
            waveOut.Init(reader);
            waveOut.Play();
        }
        public void DebugSetData(MusicData data)
        {
            this.data = data;
            Play();
        }
        public void Stop()
        {
            if(waveOut != null)
                waveOut.Stop();
        }
        public void Resume()
        {
            if (waveOut != null)
                waveOut.Play();
        }
        public long Skip(int seconds)
        {
            return reader.Position += reader.WaveFormat.AverageBytesPerSecond * seconds;
        }
        public void ChangeVolume(int intVolume)
        {
            volume.SetVolume(intVolume);
            waveOut.Volume = volume;
        }
        public void SkipToNextAudio()
        {
            currentPosition += 1;
            if(data.Tracks.Length <= currentPosition)
                    currentPosition = 0;
            Play();      
        }
    }
}
