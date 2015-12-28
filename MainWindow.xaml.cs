using System;
using System.Collections.Generic;
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
        private int _currentTick = -1;

        private int _currentTime;
        private int _tick;
        private int _weightFrom;
        private int _weightTo;
        private int _freqFrom;
        private int _freqTo;
        //private Semaphore _sem = new Semaphore(0, 2);
        //private Stopwatch stopwatch = new Stopwatch();
        private List<TaskItem> _listOfTasks = new List<TaskItem>();
        readonly Random _rnd = new Random();

        // T A S K S
        private int _currentIndex;
        private int _nextAppears;

        private int _currentTaskProcess = -1;
        private int _leftOfTask;

        // S Y S T E M
        private int _systemWaitsGenTime;
        private int _systemWaitsSince;


        public MainWindow()
        {
            InitializeComponent();
            listView.ItemsSource = _listOfTasks;
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            PlaySound(Directory.GetParent("..") + "\\Resources\\201741.wav");
            Clear();
            SetSystemSpecs();
            SetButtonStartVisibility(false);
            listView.Items.Refresh();
        }

        private void Clear()
        {
            //stopwatch.Reset();
            _listOfTasks.Clear();
            ClearFlags();
            ClearInfo();
            //listView.Items.Clear();
        }

        private void ClearFlags()
        {
            _currentTime = _currentIndex = _nextAppears = 0;

            _currentTaskProcess = _currentTick = -1;
            _leftOfTask = _systemWaitsGenTime = _systemWaitsSince = 0;
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
            _tick = Convert.ToInt16(textBoxTick.Text);
            _weightFrom = Convert.ToInt16(textBoxWeightFrom.Text);
            _weightTo = Convert.ToInt16(textBoxWeightTo.Text);
            _freqFrom = Convert.ToInt16(textBoxFreqFrom.Text);
            _freqTo = Convert.ToInt16(textBoxFreqTo.Text);
            _nextAppears = _rnd.Next(_freqFrom, _freqTo);
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

        private void CreateTask()
        {
            while (true)
            {
                if (_nextAppears > _currentTime + _tick || _tick < 1)
                    return;
                _listOfTasks.Add(new TaskItem()
                {
                    Index = _currentIndex++,
                    Weight = _rnd.Next(_weightFrom, _weightTo + 1),
                    Appear = _nextAppears,
                    Start = -1,
                    Finish = -1,
                    State = "wait",
                });
                _nextAppears += _rnd.Next(_freqFrom, _freqTo + 1);
            }
        }

        private void ExecuteTask()
        {
            var currentMs = 0;
            if (_currentTaskProcess != -1)
            {
                var task = _listOfTasks.Find(item => item.Index == _currentTaskProcess);
                if (_leftOfTask <= _tick)
                {
                    currentMs = _leftOfTask;
                    task.Finish = _currentTime + _leftOfTask - 1;
                    task.State = "done";
                    _currentTaskProcess = -1;
                    _systemWaitsSince = task.Finish + 1;
                }
                else
                {
                    currentMs = _tick;
                    _leftOfTask -= _tick;
                }
            }

            for (; currentMs < _tick; currentMs++)
            {
                var taskList = _listOfTasks.FindAll(item =>
                    item.Appear < _currentTime + currentMs && item.State == "wait");
                if (taskList.Count == 0)
                    continue;
                _systemWaitsGenTime += _currentTime + currentMs - _systemWaitsSince;

                var task = taskList[_rnd.Next(0, taskList.Count)];
                task.Start = _currentTime + currentMs;

                if (task.Weight <= _tick - currentMs)
                {
                    currentMs += task.Weight - 1;
                    task.Finish = task.Start + task.Weight - 1;
                    task.State = "done";
                    _systemWaitsSince = task.Finish + 1;
                }
                else
                {
                    task.State = "process";
                    _currentTaskProcess = task.Index;
                    _leftOfTask = task.Weight + currentMs - _tick;
                    currentMs = _tick;
                }
            }
        }

        private void UpdateWaitOption()
        {
            foreach (var task in _listOfTasks.Where(task => task.Start == -1))
                task.Wait = _currentTime - task.Appear;
        }

        private void FIllInfo()
        {
            labelTdone.Content = _listOfTasks.FindAll(task => task.State == "done").Count;
            labelTwait.Content = _listOfTasks.FindAll(task => task.State == "wait").Count;
            labelTicks.Content = _currentTick;
            labelSidle.Content = _systemWaitsGenTime;
            textBoxAvWait.Text = ((double)_listOfTasks.Sum(task => task.Wait) / _listOfTasks.Count).ToString();
        }

        private void buttonTick_Click(object sender, RoutedEventArgs e)
        {
            _currentTick++;
            CreateTask();
            ExecuteTask();
            _currentTime += _tick;

            UpdateWaitOption();
            FIllInfo();
            listView.Items.Refresh();
        }

    }
}
