using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CpuDispatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        private Dispatcher _dispatcher;
        //public List<TaskItem> _listOfTasks = new List<TaskItem>();

        public MainWindow()
        {
            InitializeComponent();
            _dispatcher = new Dispatcher();
            listView.ItemsSource = _dispatcher.ListOfTasks;
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            PlaySound(Directory.GetParent("..") + "\\Resources\\01-the-screaming-sheep.wav");
            Clear();
            SetSystemSpecs();
            SetButtonStartVisibility(false);
            listView.Items.Refresh();
        }

        private void Clear()
        {
            _dispatcher = new Dispatcher();
            ClearInfo();
        }
        
        private void ClearInfo()
        {
            labelTdone.Content = "0";
            labelTwait.Content = "0";
            labelTicks.Content = "0";
            labelSidle.Content = "0";
            textBoxAvWait.Text = "0";
        }

        private void SetSystemSpecs()
        {
            _dispatcher.Tick = Convert.ToInt16(textBoxTick.Text);
            _dispatcher.WeightFrom = Convert.ToInt16(textBoxWeightFrom.Text);
            _dispatcher.WeightTo = Convert.ToInt16(textBoxWeightTo.Text);
            _dispatcher.FreqFrom = Convert.ToInt16(textBoxFreqFrom.Text);
            _dispatcher.FreqTo = Convert.ToInt16(textBoxFreqTo.Text);
        }

        private void SetButtonStartVisibility(bool visible)
        {
            if (buttonStart == null)
                return;
            buttonStart.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
        }

        private void textBoxTick_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetButtonStartVisibility(true);
        }

        private void textBoxWeightFrom_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetButtonStartVisibility(true);
        }

        private void textBoxWeightTo_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetButtonStartVisibility(true);
        }

        private void textBoxFreqFrom_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetButtonStartVisibility(true);
        }

        private void textBoxFreqTo_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetButtonStartVisibility(true);
        }

        static void PlaySound(string strName)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(strName);
            player.Play();
        }
        
        private void FIllInfo()
        {
            labelTdone.Content = _dispatcher.ListOfTasks.FindAll(task => task.State == "done").Count;
            labelTwait.Content = _dispatcher.ListOfTasks.FindAll(task => task.State == "wait").Count;
            labelTicks.Content = _dispatcher.CurrentTick;
            labelSidle.Content = _dispatcher.SystemWaitsGenTime;
            textBoxAvWait.Text = ((double)_dispatcher.ListOfTasks.Sum(task => task.Wait) /
                _dispatcher.ListOfTasks.Count).ToString(CultureInfo.InvariantCulture);
        }

        private void buttonTick_Click(object sender, RoutedEventArgs e)
        {
            _dispatcher.MakeOneTick();
            FIllInfo();

            listView.ItemsSource = null;
            listView.ItemsSource = _dispatcher.ListOfTasks;
        }

        private void buttonTest_Click(object sender, RoutedEventArgs e)
        {
            var testsDialog = new Tests();
            testsDialog.ShowDialog();
        }
    }
}
