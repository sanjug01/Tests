using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AlGoUnitTests.Threading
{
    public class MyScheduler
    {
        public void Start3Threads()
        {
            MyWorker[] workers = {
                new MyWorker("w1"),
                new MyWorker("w2"),
                new MyWorker("w3"),
            };

            Thread[] threads = {
                new Thread(() => { workers[0].DoWork(2000); }),
                new Thread(() => { workers[1].DoWork(10000); }),
                new Thread(() => { workers[0].DoWork(5000); }),
            };

            int i = 0;
            foreach(var th in threads)
            {
                th.Name = "th" + i;
                i++;
                th.Start();
            }

            // verify they are active
            i = 0;
            foreach(var th in threads)
            {
                System.Diagnostics.Debug.WriteLine("Th {0} is alive:{1}", th.Name, th.IsAlive);
            }

            System.Diagnostics.Debug.WriteLine("Joining ... {0}", threads[2].Name);
            threads[2].Join();
            foreach (var th in threads)
            {
                System.Diagnostics.Debug.WriteLine("Th {0} is alive:{1}", th.Name, th.IsAlive);
            }
        }

        public async Task<int> Start3ThreadsAsync()
        {
            MyWorker[] workers = {
                new MyWorker("w1"),
                new MyWorker("w2"),
                new MyWorker("w3"),
            };

            Thread[] threads = {
                new Thread(() => { workers[0].DoWork(2000); }),
                new Thread(() => { workers[1].DoWork(10000); }),
                new Thread(() => { workers[0].DoWork(5000); }),
            };

            int i = 0;
            foreach (var th in threads)
            {
                th.Name = "th" + i;
                i++;
                th.Start();
            }

            // verify they are active
            i = 0;
            foreach (var th in threads)
            {
                System.Diagnostics.Debug.WriteLine("Th {0} is alive:{1}", th.Name, th.IsAlive);
            }

            int maxDelay = 10000; //ms
            await Task.Delay(maxDelay);
                
            // wait for the longest thread
            threads[2].Join();
          
            return 1;
        }
    }
}
