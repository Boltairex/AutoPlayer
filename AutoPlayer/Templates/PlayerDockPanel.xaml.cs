using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoPlayer.Templates
{
    /// <summary>
    /// Logika interakcji dla klasy PlayerDockPanel.xaml
    /// </summary>
    public partial class PlayerDockPanel : UserControl
    {
        public event EventHandler PreviousButton;
        public event EventHandler PlayButton;
        public event EventHandler StopButton;
        public event EventHandler NextButton;

        public PlayerDockPanel()
        {
            InitializeComponent();
        }

        public float TimelineCurrent
        {
            get { return (float)GetValue(TimelineCurrentProperty); }
            set { SetValue(TimelineCurrentProperty, value); }
        }

        public static readonly DependencyProperty TimelineCurrentProperty =
            DependencyProperty.Register("TimelineCurrent", typeof(float), typeof(PlayerDockPanel), new PropertyMetadata(0f));

        public float TimelineMax
        {
            get { return (float)GetValue(TimelineMaxProperty); }
            set { SetValue(TimelineMaxProperty, value); }
        }
 
        public static readonly DependencyProperty TimelineMaxProperty =
            DependencyProperty.Register("TimelineMax", typeof(float), typeof(PlayerDockPanel), new PropertyMetadata(0f));

        public void Previous(object sender, RoutedEventArgs e)
        {
            PreviousButton?.Invoke(sender, e);
        }

        public void Play(object sender, RoutedEventArgs e)
        {
            PlayButton?.Invoke(sender, e);
        }

        public void Stop(object sender, RoutedEventArgs e)
        {
            StopButton?.Invoke(sender, e);
        }

        public void Next(object sender, RoutedEventArgs e)
        {
            NextButton?.Invoke(sender, e);
        }
    }
}
