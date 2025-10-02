using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InputLog.Core.Util
{
    public class RollingAverage
    {
        private int Window;
        private Queue<int> NumsInWindow;
        private double Sum;

        public RollingAverage(int window)
        {
            Window = window;
            NumsInWindow = new Queue<int>(Window);
            Sum = 0;
        }

        public void Add(int i)
        {
            if (NumsInWindow.Count == Window)
            {
                int old = NumsInWindow.Dequeue();
                Sum -= old;
            }
            Sum += i;
            NumsInWindow.Enqueue(i);
        }

        public double GetAverage()
        {
            return Sum/Window;
        }

    }
}
