using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AlGoUnitTests.Threading
{
    public class MyResource
    {
        private int[] _buffer;
        private int _buffIndex;

        public MyResource(int size)
        {
            _buffer = new int[size];
            _buffIndex = 0;
        }

        public void Add(int n)
        {
            while(true)
            {
                lock (_buffer) // Monitor.Enter
                {
                    while (_buffIndex == _buffer.Length)
                    {
                        Monitor.Wait(_buffer);
                    }

                    if (_buffIndex < _buffer.Length)
                    {
                        _buffer[_buffIndex++] = n;
                        Monitor.PulseAll(_buffer);
                        return;
                    }
                }
            }

            // unreachable
            // return
        }

        public int Remove()
        {
            while(true)
            {
                Monitor.Enter(_buffer);
                while (_buffIndex == 0)
                {
                    Monitor.Wait(_buffer);
                }

                if (_buffIndex > 0)
                {
                    int val = _buffer[--_buffIndex];
                    Monitor.PulseAll(_buffer);
                    Monitor.Exit(_buffer);
                    return val;
                }
                
            }

            // unreachable
            // return 0;
        }

    }

    public class MyConsumerProducer
    {
     
           
        private MyResource _resource;
        public MyConsumerProducer(MyResource resource)
        {
            _resource = resource;
            StopLoop = false;
        }
        
        public bool StopLoop { get; set; }

        public void Produce()
        {
            Random rand = new Random();
            int i;

            // infinite loop
            while (!this.StopLoop)
            {
                i = rand.Next();
                _resource.Add(i);
                System.Diagnostics.Debug.WriteLine(" prod --> {0}", i);
            }

        }

        public void Consume()
        {
            int i;

            // infinite loop
            while (!this.StopLoop)
            {
                i = _resource.Remove();
                System.Diagnostics.Debug.WriteLine(" cons <-- {0}", i);
            }
        }
    }
}
