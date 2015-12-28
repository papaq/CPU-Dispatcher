using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpuDispatcher
{
    class Task
    {
        public int Index;

        public int Weight;

        public int StartTime;

        public int FinishTime;

        public Task(int ind, int weight, int st, int fi)
        {
            Index = ind;
            Weight = weight;
            StartTime = st;
            FinishTime = fi;
        }
    }
}
