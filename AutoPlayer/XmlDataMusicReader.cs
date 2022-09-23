﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AutoPlayer
{
    static class XmlDataMusicReader
    {
        static string filename;

        public static MusicData[] ReadDataFromFile(string path)
        {
            using (var reader = new StreamReader(path))
            {
                filename = Path.GetFileName(path);
                return XmlReadData(reader.BaseStream);
            }
            throw new Exception("Something went wrong.");
        }

        public static MusicData[] XmlReadData(Stream stream)
        {
            List<MusicData> musics = new List<MusicData>();

            XmlReader r = XmlReader.Create(stream);

            while (r.Read())
            {
                if (r.NodeType != XmlNodeType.Element)
                    continue;

                switch (r.Name)
                {
                    case "time":
                        musics.Add(LoadTime(ref r).Build());
                        break;
                }
            }

            return musics.ToArray();
        }

        public static MusicDataBuilder LoadTime(ref XmlReader r)
        {
            MusicDataBuilder builder = new MusicDataBuilder();
            int depth = r.Depth;

            if (!r.TryGetAttribute("set", out var time))
                throw new ArgumentException("Time value is empty! 'time' node: missing 'set' attribute.");

            if (!r.TryGetAttribute("shuffle", out var shuffle))
                shuffle = "false";

            builder.WithShuffle(shuffle.ToBool())
                .SetTime(time);

            while (r.Read() && depth < r.Depth)
            {
                if (r.NodeType != XmlNodeType.Element)
                    continue;

                switch (r.Name)
                {
                    case "playlist":
                        var playlist = LoadPlayList(ref r);

                        var files = Directory.EnumerateFiles(playlist.Path + '\\');
                        foreach(var file in files)
                            builder.AddTrack(file, playlist.Volume);
                        break;
                }
            }

            return builder;
        }

        public static PlaylistNode LoadPlayList(ref XmlReader r) 
        {
            if (!r.TryGetAttribute("path", out var path))
                throw new ArgumentException("No path for playlist!");

            if (!r.TryGetAttribute("volume", out var volume))
                volume = "100";

            return new PlaylistNode()
            {
                Path = path,
                Volume = volume
            };
        }

        static bool TryGetAttribute(this XmlReader r, string keyword, out string value)
        {
            if (r.GetAttribute(keyword) is string val && !string.IsNullOrEmpty(val))
            {
                value = val;
                return true;
            }
            value = string.Empty;
            return false;
        }

        public static FileFormat FormatFromString(this string val)
        {
            val = val.ToLower();
            return (val) switch
            {
                "mp3" => FileFormat.MP3,
                "wav" => FileFormat.WAV,
                _ => throw new ArgumentException("Wrong type parsed!", val)
            };
        }

        public static bool ToBool(this string str)
        {
            str = str.ToLower();
            return (str) switch
            {
                "true" => true,
                "yes" => true,
                "false" => false,
                "no" => false,
                _ => throw new ArgumentException($"{nameof(str)} is not valid boolean! Value: {str}", str)
            };
        }
    }

    public struct PlaylistNode
    {
        public string Path;
        public string? Volume;
    }

    public class MusicDataBuilder
    {
        public TimeSpan Start { get; set; }
        public TimeSpan Length { get; set; }
        public bool Shuffle { get; set; } = false;
        public List<TrackData> MusicData { get; set; }

        public MusicDataBuilder()
        {
            MusicData = new List<TrackData>();
        }

        public MusicDataBuilder SetTime(string time)
        {
            string[] values = time.Split('-');

            if (values.Length != 2)
                throw new ArgumentException("Wrong time format! Try: HH:MM-HH:MM (from, to).", time);

            var st = values[0].Split(':');
            Start = new TimeSpan(int.Parse(st[0]), int.Parse(st[1]), 0);

            st = values[1].Split(':');
            Length = new TimeSpan(int.Parse(st[0]), int.Parse(st[1]), 0).Subtract(Start);

            return this;
        }

        public MusicDataBuilder WithShuffle(bool val) {
            Shuffle = val;
            return this;
        }

        public MusicDataBuilder AddTrack(string fullPath, string? volume)
        {
            TrackData data = new TrackData(fullPath, volume != null ? new Volume().FromString(volume) : new Volume().SetVolume(100));
            MusicData.Add(data);
            return this;
        }

        public MusicData Build()
        {
            return new MusicData(this);
        }
    }

    public class MusicData
    {
        public readonly TrackData[] Tracks;
        public readonly int TracksCount;
        public readonly TimeSpan Start;
        public readonly TimeSpan Length;
        public readonly TimeSpan End;
        public bool Shuffle;

        public DateTime GetTodaysStart()
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Start.Hours, Start.Minutes, DateTime.Now.Second, DateTime.Now.Millisecond);
        }

        public DateTime GetTodaysEnd()
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, End.Hours, End.Minutes, DateTime.Now.Second, DateTime.Now.Millisecond);
        }

        public double TimeLeftInSeconds()
        {
            return GetTodaysEnd().Subtract(DateTime.Now).TotalSeconds;
        }

        public MusicData(MusicDataBuilder builder)
        {
            Tracks = builder.MusicData.ToArray();
            TracksCount = Tracks.Length;
            Shuffle = builder.Shuffle;
            Start = builder.Start;
            Length = builder.Length;
            End = builder.Start.Add(builder.Length);
        }
    }

    /// <summary>
    /// Data about specified music sample.
    /// </summary>
    public class TrackData
    {
        public string Fullpath;
        public string Name;
        public Volume Volume;
        public FileFormat Format;

        public TrackData(string path, Volume volume)
        {
            Fullpath = path;
            Name = Path.GetFileName(path);
            Volume = volume;
            Format = Name.Split('.').Last().FormatFromString();
        }

        public MemoryStream GetStreamFromFile()
        {
            if (!File.Exists(Fullpath))
                throw new FileNotFoundException("Brak pliku w lokalizacji " + Fullpath);

            MemoryStream stream = new MemoryStream();
            var bytes = File.ReadAllBytes(Fullpath);
            stream.Write(bytes);

            return stream;
        }
    }

    public class Volume
    {
        int volume = 100;

        public float GetVolumeFloat() => volume / 100f;

        public int GetVolumeInt() => volume;

        public Volume SetVolume(float volume)
        {
            this.volume = (int)(Math.Min(0, Math.Max(1, volume)) * 100);
            return this;
        }

        public Volume SetVolume(int volume)
        {
            this.volume = Math.Min(0, Math.Max(100, volume));
            return this;
        }

        public Volume FromString(string volume)
        {
            volume = volume.Replace('.', ',');
            if (float.TryParse(volume, out var vol1))
                SetVolume(vol1);
            else if (int.TryParse(volume, out var vol2))
                SetVolume(vol2);
            else
                throw new ArgumentException("Failed to parse volume.", volume);

            return this;
        }
    }

    public enum FileFormat
    {
        MP3,
        WAV
    }
}