using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace AlGoUnitTests.Threading
{
    public class OddEvenWorker
    {
        protected readonly object _lock;
        protected Semaphore _semaphore;
        protected bool _isOdd;

        public OddEvenWorker()
        {
            _isOdd = true;
            _lock = new object();
            _semaphore = new Semaphore(1, 4, "MySemaphore");

        }
        public void PrintOddValues(int max)
        {
            int i = 1;
            while ( i <= max )
            {
                Monitor.Enter(_lock);
                if(_isOdd)
                {
                    System.Diagnostics.Debug.Write(i + " -> ");
                    i += 2;
                    _isOdd = false;
                }
                
                Monitor.Exit(_lock);

            }
        }

        public void PrintOddValuesSemaphore(int max)
        {

            int i = 1;
            while (i <= max)
            {
                _semaphore.WaitOne();
                if (_isOdd)
                {
                    System.Diagnostics.Debug.Write(i + " -> ");
                    i += 2;
                    _isOdd = false;
                }

                _semaphore.Release();

            }
        }


        public void PrintOddValuesMutex(int max)
        {

            bool isNewMutex = false;
            Mutex myMutex = new Mutex(false, "OddEven", out isNewMutex);
            System.Diagnostics.Debug.WriteLine("Odd Mutex:" + myMutex.ToString() + " isCreated:" + isNewMutex);

            int i = 1;
            while (i <= max)
            {
                myMutex.WaitOne();
                if (_isOdd)
                {
                    System.Diagnostics.Debug.Write(i + " -> ");
                    i += 2;
                    _isOdd = false;
                }

                myMutex.ReleaseMutex();

            }
        }

        public void PrintEvenValues(int max)
        {
            int i = 2;
            while (i <= max)
            {
                Monitor.Enter(_lock);
                if (!_isOdd)
                {
                    System.Diagnostics.Debug.Write(i + " -> ");
                    i += 2;
                    _isOdd = true;
                }
                Monitor.Exit(_lock);
            }
        }


        public void PrintEvenValuesSemaphore(int max)
        {
            int i = 2;
            while (i <= max)
            {
                _semaphore.WaitOne();
                if (!_isOdd)
                {
                    System.Diagnostics.Debug.Write(i + " -> ");
                    i += 2;
                    _isOdd = true;
                }
                _semaphore.Release();
            }
        }


        public void PrintEvenValuesMutex(int max)
        {
            bool isNewMutex = false;
            Mutex myMutex = new Mutex(false, "OddEven", out isNewMutex);
            System.Diagnostics.Debug.WriteLine("Even Mutex:" + myMutex.ToString() + " isCreated:" + isNewMutex);

            int i = 2;
            while (i <= max)
            {
                myMutex.WaitOne();
                if (!_isOdd)
                {
                    System.Diagnostics.Debug.Write(i + " -> ");
                    i += 2;
                    _isOdd = true;
                }
                myMutex.ReleaseMutex();
            }
        }
    }
    
}
