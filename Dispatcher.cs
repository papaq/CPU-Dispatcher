using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CpuDispatcher
{
    public class Dispatcher
    {
        private int _currentTime;

        // T A S K S
        private int _currentIndex;
        private int _nextAppears;

        private int _currentTaskProcess = -1;
        private int _leftOfTask;

        // O P T I O N S
        public int CurrentTick = -1;

        public int Tick;
        public int WeightFrom;
        public int WeightTo;
        public int FreqFrom;
        public int FreqTo;

        public List<TaskItem> ListOfTasks = new List<TaskItem>();
        readonly Random _rnd = new Random();
        
        // S Y S T E M
        public int SystemWaitsGenTime;
        private int _systemWaitsSince;

        public Dispatcher()
        {
            _nextAppears = _rnd.Next(FreqFrom, FreqTo);
        }

        private void CreateTask()
        {
            while (true)
            {
                if (_nextAppears > _currentTime + Tick || Tick < 1)
                    return;
                ListOfTasks.Add(new TaskItem()
                {
                    Index = _currentIndex++,
                    Weight = _rnd.Next(WeightFrom, WeightTo + 1),
                    Appear = _nextAppears,
                    Start = -1,
                    Finish = -1,
                    State = "wait",
                });
                _nextAppears += _rnd.Next(FreqFrom, FreqTo + 1);
            }
        }

        private void ExecuteTask()
        {
            var currentMs = 0;
            if (_currentTaskProcess != -1)
            {
                var task = ListOfTasks.Find(item => item.Index == _currentTaskProcess);
                if (_leftOfTask <= Tick)
                {
                    currentMs = _leftOfTask;
                    task.Finish = _currentTime + _leftOfTask - 1;
                    task.State = "done";
                    _currentTaskProcess = -1;
                    _systemWaitsSince = task.Finish + 1;
                }
                else
                {
                    currentMs = Tick;
                    _leftOfTask -= Tick;
                }
            }

            for (; currentMs < Tick; currentMs++)
            {
                var taskList = ListOfTasks.FindAll(item =>
                    item.Appear < _currentTime + currentMs && item.State == "wait");
                if (taskList.Count == 0)
                    continue;
                SystemWaitsGenTime += _currentTime + currentMs - _systemWaitsSince;

                var task = taskList[_rnd.Next(0, taskList.Count)];
                task.Start = _currentTime + currentMs;

                if (task.Weight <= Tick - currentMs)
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
                    _leftOfTask = task.Weight + currentMs - Tick;
                    currentMs = Tick;
                }
            }
        }

        private void UpdateWaitOption()
        {
            foreach (var task in ListOfTasks.Where(task => task.Start == -1))
                task.Wait = _currentTime - task.Appear;
        }

        public void MakeOneTick()
        {
            CurrentTick++;
            CreateTask();
            ExecuteTask();

            _currentTime += Tick;
            UpdateWaitOption();
        }

        public double CountAverageTime(int n)
        {
            for (var i = 0; i < n; i++)
                MakeOneTick();

            return ((double)ListOfTasks.Sum(task => task.Wait) / ListOfTasks.Count);
        }

        public double CountSystemIdlePart(int n)
        {
            for (var i = 0; i < n; i++)
                MakeOneTick();

            return ((double)SystemWaitsGenTime / _currentTime);
        }

        public Point CountRatio_WaitingTasks_To_AvTimeWaiting(int n)
        {
            var tasksWaiting = 0;
            for (var i = 0; i < n; i++)
            {
                MakeOneTick();
                tasksWaiting += ListOfTasks.FindAll(t => t.State == "wait").Count;
            }
            return new Point(ListOfTasks.Sum(task => task.Wait) * n, tasksWaiting * ListOfTasks.Count) ;
        }
    }
}
