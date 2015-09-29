using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AlGoUnitTests.Threading
{



    public class DiningPhilosophers
    {
        class Philosopher
        {
            private int _id;
            private readonly object _fork1;
            private readonly object _fork2;

            public Philosopher(int id, int f1, int f2)
            {
                _id = id;
                _fork1 = f1;
                _fork2 = f2;
             
            }


            // thread method
            public void Eat()
            {
                while(true)
                {
                    lock(_fork1)
                    {
                        System.Diagnostics.Debug.WriteLine("{0} has fork1", _id);
                        lock(_fork2)
                        {
                            System.Diagnostics.Debug.WriteLine("{0} has fork1", _id);
                            System.Diagnostics.Debug.WriteLine("{0} eating", _id);
                        }
                    }

                }
            }

        }

        private readonly int _count;
        private object[] forks;


        private MyResource _resource;
        public DiningPhilosophers(MyResource resource)
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
