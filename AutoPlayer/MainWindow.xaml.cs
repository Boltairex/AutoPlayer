using NAudio.Wave;
using System.Windows;
using System.Windows.Controls;

namespace AutoPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Volume volume = new Volume();
        WaveOut waveOut = new WaveOut();
       
       
        public MainWindow()
        {
            InitializeComponent(); 
        }
        private void OpenFile(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "";
            dialog.DefaultExt = ".XML";
            dialog.Filter = "(.XML)|*.XML";

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            if (result == true){
                string filename = dialog.FileName;
                var data = XmlDataMusicReader.ReadDataFromFile(filename);
                //DateChecker.SetDateChecker(data);
                AudioController.Instance.DebugSetData(data[0]);
            }
        }
        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider s = sender as Slider; //s.value ->float
            AudioController.Instance.ChangeVolume((int)s.Value);
        }
        private void StopButton(object sender, RoutedEventArgs e)
        {
            AudioController.Instance.Stop();
        }
        private void ResumeButton(object sender, RoutedEventArgs e)
        {
            AudioController.Instance.Resume();
        }
        private void SkipToNext(object sender, RoutedEventArgs e)
        {
            AudioController.Instance.SkipToNextAudio();
        }
    }
}
