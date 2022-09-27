using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AutoPlayer
{
    /// <summary>
    /// Logika interakcji dla klasy TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        public ObservableCollection<TrackData> Items1 { get; } = new ObservableCollection<TrackData>();

        public CancellationTokenSource Token;
        public bool IsRunning;

        public TestWindow()
        {
/*            Token = new CancellationTokenSource();

            var data = XmlDataMusicReader.ReadDataFromFile(@"C:\Users\Boltu\Desktop\music.xml");
            XmlDataMusicReader.CreateBaseConfigurationFrom(@"C:\Users\Boltu\Desktop\music.xml");
            MusicData music = data.First();
            
            for(int x = 1; x < data.Length; x++)
            {
                music = music.MergeTracks(data[x]);
            }

            foreach (var track in music.Tracks)
                Items1.Add(track);

            var checker = DateChecker.SetDateChecker(data);
*/
            DataContext = this;
            InitializeComponent();
        }

        public int Volume
        {
            get { return (int)GetValue(VolumeProperty); }
            set { SetValue(VolumeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Volume.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolumeProperty =
            DependencyProperty.Register("Volume", typeof(int), typeof(TestWindow), new PropertyMetadata(100));

        public float Value
        {
            get { return (float)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(float), typeof(TestWindow), new PropertyMetadata(110f));

        public float Current
        {
            get { return (float)GetValue(CurrentProperty); }
            set { SetValue(CurrentProperty, value); }
        }

        public static readonly DependencyProperty CurrentProperty =
            DependencyProperty.Register("Current", typeof(float), typeof(TestWindow), new PropertyMetadata(40f));

        public void Play(object sender, EventArgs e)
        {

        }

        /*
        public async Task Timer(TestWindow window, CancellationToken token)
        {
            while (true)
            {
                await Task.Delay(1000, token);
                if (token.IsCancellationRequested)
                    return;
                window.Current++;
                App.Print(window.Current.ToString());
            }
        }
        */

        public void Stop(object sender, EventArgs e)
        {

        }

        public void Previous(object sender, EventArgs e)
        {

        }

        public void Next(object sender, EventArgs e)
        {

        }

        private void LoadConfiguration(object sender, RoutedEventArgs e)
        {

        }

        private void Checker_DataAvailable2(MusicData obj)
        {

        }

        private void Checker_DataExpired2()
        {

        }

        private void ChangedValue(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue == e.OldValue)
                return;

            if(sender is Slider slider)
            {
                Volume = (int)Math.Min(100, Math.Max(0, e.NewValue));
            }
            App.WriteLine(Volume);
        }   
    }
}
