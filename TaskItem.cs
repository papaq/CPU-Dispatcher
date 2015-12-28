namespace CpuDispatcher
{
    public class TaskItem
    {
        public int Index { get; set; }

        public int Weight { get; set; }

        public int Wait { get; set; }

        public int Appear { get; set; }

        public int Start { get; set; }

        public int Finish { get; set; }

        public string State { get; set; }
    }
}