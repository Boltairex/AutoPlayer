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
using System.Windows.Shapes;

namespace AutoPlayer
{
    /// <summary>
    /// Logika interakcji dla klasy Window1.xaml
    /// </summary>
    public partial class ConsoleWindow : Window
    {
        TextBlock block;

        public ConsoleWindow()
        {
            InitializeComponent();

            block = this.FindName("Out") as TextBlock;
        }

        public void Print(string value)
        {
            block.TextWrapping = TextWrapping.Wrap;
            block.Text = block.Text + value + "\n" ;
        }
    }
}
