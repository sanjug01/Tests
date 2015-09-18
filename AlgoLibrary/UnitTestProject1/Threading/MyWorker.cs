using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AlGoUnitTests.Threading
{
    public class MyWorker
    {
        // use a readonly object for lock
        private readonly object _myLock = new object();
        private string _name;

        public MyWorker(string name)
        {
            Index = 1;
            _name = name;
        }

        private int Index { get; set; }

        public int ReadIndex()
        {
            lock (_myLock)
            {
                return Index;
            }
        }


        public void WriteIndex(int newValue)
        {
            lock(_myLock)
            {
                Index = newValue;
            }
        }

        public void DoWork(int milisec)
        {
            Thread.Sleep(milisec);
            // Thread.SpinWait(100);
            System.Diagnostics.Debug.WriteLine("Worker {0} done work!", _name);
        }

    }
}
