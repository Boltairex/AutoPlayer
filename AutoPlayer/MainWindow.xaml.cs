using AutoPlayer.Templates;
using NAudio.Wave;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AutoPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PlayerDockPanel panel;
        BindingExpression bind1;
        BindingExpression bind2;

        BackLogger logger = new BackLogger();

        public ObservableCollection<TrackViewData> CurrentData { get; } = new ObservableCollection<TrackViewData>();

        public int Volume
        {
            get { return (int)GetValue(VolumeProperty); }
            set { SetValue(VolumeProperty, value); }
        }

        public static readonly DependencyProperty VolumeProperty =
            DependencyProperty.Register("Volume", typeof(int), typeof(TestWindow), new PropertyMetadata(100));

        public float TimelineMax
        {
            get { return (float)GetValue(TimelineMaxProperty); }
            set { SetValue(TimelineMaxProperty, value); }
        }

        public static readonly DependencyProperty TimelineMaxProperty =
            DependencyProperty.Register("TimelineMax", typeof(float), typeof(TestWindow), new PropertyMetadata(0f));

        public float TimelineCurrent
        {
            get { return (float)GetValue(TimelineCurrentProperty); }
            set { SetValue(TimelineCurrentProperty, value); }
        }

        public static readonly DependencyProperty TimelineCurrentProperty =
            DependencyProperty.Register("TimelineCurrent", typeof(float), typeof(TestWindow), new PropertyMetadata(0f));


        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();

            AudioController.CurrentTrackChanged += AudioController_CurrentTrackChanged;
            AudioController.AudioLoaded += AudioController_AudioLoaded;
            AudioController.CurrentTimeEverySecond += AudioController_CurrentTimeEverySecond;
            DateChecker.DataExpired += DateChecker_DataExpired;

            panel = FindName("SliderPanel") as PlayerDockPanel;
            logger.Log("UI, Main window initialized. Events are set.");
  /*          bind1 = panel.SetBinding(TimelineMaxProperty, "TimelineMax");
            bind2 = panel.SetBinding(TimelineCurrentProperty, "TimelineCurrent");*/
        }

        private void DateChecker_DataExpired()
        {
            CurrentData.Clear();
        }

        private void AudioController_CurrentTimeEverySecond(int obj)
        {
            TimelineCurrent = obj;
        }

        private void AudioController_AudioLoaded(MusicData obj)
        {
            CurrentData.Clear();
            var data = AudioController.Instance.GetInfoAboutTracks();
            logger.Log($"UI, audio load requested with {obj.Tracks.Length} tracks.");
            foreach (var d in data)
                CurrentData.Add(d);
        }

        private void AudioController_CurrentTrackChanged(TrackData obj)
        {
            logger.Log($"UI, Next track: {obj.Name}");
            TimelineCurrent = 0;
            TimelineMax = AudioController.Instance.TotalTimeInSeconds();
            AudioController.Instance.ChangeVolume(Volume);
            Title = obj.Name;
        }

        void OpenFile(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "";
            dialog.DefaultExt = ".XML";
            dialog.Filter = "(.XML)|*.XML";
            dialog.ValidateNames = true;

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                try
                {
                    string filename = dialog.FileName;
                    var data = XmlDataMusicReader.ReadDataFromFile(filename);
                    DateChecker.SetDateChecker(data);
                    XmlDataMusicReader.CreateBaseConfigurationFrom(filename);
                    logger.Log("UI, configuration loaded. Timer is set. Configuration copied to exe location.");
                    /*AudioController.Instance.DebugSetData(data[0]);*/
                }
                catch (Exception ex)
                {
                    Handlers.FileReadHandler(ex);
                }
            }
        }

        public void Play(object sender, EventArgs e)
        {
            AudioController.Instance.Start();
        }

        public void Stop(object sender, EventArgs e)
        {
            AudioController.Instance.Stop();
        }

        public void Next(object sender, EventArgs e)
        {
            AudioController.Instance.SkipToNextAudio();
        }

        public void Previous(object sender, EventArgs e)
        {
            AudioController.Instance.SkipToPreviousAudio();
        }

        private void ChangedValue(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue == e.OldValue)
                return;

            Volume = (int)Math.Min(100, Math.Max(0, e.NewValue));
            AudioController.Instance.ChangeVolume(Volume);
        }
    }
}