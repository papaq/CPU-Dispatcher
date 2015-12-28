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

namespace CpuDispatcher
{
    /// <summary>
    /// Interaction logic for Tests.xaml
    /// </summary>
    public partial class Tests : Window
    {
        public Tests()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var gr = new Graph(0);
            gr.ShowDialog();
        }

        private void buttonTest2_Click(object sender, RoutedEventArgs e)
        {
            var gr = new Graph(1);
            gr.ShowDialog();
        }

        private void buttonTest3_Click(object sender, RoutedEventArgs e)
        {
            var gr = new Graph(3);
            gr.ShowDialog();
        }

        private void buttonTest1_2_Click(object sender, RoutedEventArgs e)
        {
            var gr = new Graph(2);
            gr.ShowDialog();
        }
    }
}
