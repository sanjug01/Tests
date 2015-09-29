using System;
using System.Threading;
using AlgoLibrary;
using AlGoUnitTests.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlGoUnitTests
{
    [TestClass]
    public class ThreadingUnitTest
    {
        [TestMethod]
        public void Test_Start3Threads()
        {
            MyScheduler scheduler = new MyScheduler();
            scheduler.Start3Threads();

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Test_Start3ThreadsAsync()
        {
            MyScheduler scheduler = new MyScheduler();
            var task = scheduler.Start3ThreadsAsync();

            Assert.IsNotNull(task);

            // takes max delay (about 1 min)
            task.Wait();
            Assert.AreEqual(1, task.Result);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Test_OddEven()
        {
            OddEvenWorker worker = new OddEvenWorker();

            Thread oddThread = new Thread(() => { worker.PrintOddValues(20); });
            Thread evenThread = new Thread(() => { worker.PrintEvenValues(20); });
            oddThread.Name = "OddTh";
            evenThread.Name = "EvenTh";

            oddThread.Start();
            evenThread.Start();


            oddThread.Join();
            evenThread.Join();

            Assert.IsTrue(true);
        }


        [TestMethod]
        public void Test_OddEvenWithSemaphore()
        {
            OddEvenWorker worker = new OddEvenWorker();

            Thread oddThread = new Thread(() => { worker.PrintOddValuesSemaphore(20); });
            Thread evenThread = new Thread(() => { worker.PrintEvenValuesSemaphore(20); });
            oddThread.Name = "OddTh";
            evenThread.Name = "EvenTh";


            Semaphore semaphore = new Semaphore(1, 4, "MySemaphore");
            semaphore.WaitOne();

            oddThread.Start();
            evenThread.Start();

            // don't let them go until semaphore released
            semaphore.Release();

            oddThread.Join();
            evenThread.Join();

            Assert.IsTrue(true);
        }


        [TestMethod]
        public void Test_OddEvenWithMutex()
        {
            OddEvenWorker worker = new OddEvenWorker();

            Thread oddThread = new Thread(() => { worker.PrintOddValuesMutex(20); });
            Thread evenThread = new Thread(() => { worker.PrintEvenValuesMutex(20); });
            oddThread.Name = "OddTh";
            evenThread.Name = "EvenTh";


            Mutex myMutex = new Mutex(true, "OddEven");

            oddThread.Start();
            evenThread.Start();

            // don't let them start until we release the mutex
            myMutex.ReleaseMutex();


            oddThread.Join();
            evenThread.Join();

            Assert.IsTrue(true);
        }

        [TestMethod]
        // this has an loooong loop, should not be a test
        public void Test_ProduceConsumer()
        {
            MyResource resource = new MyResource(10);

            MyConsumerProducer worker = new MyConsumerProducer(resource);

            Thread consumerThread = new Thread(() => { worker.Consume(); });
            Thread producerThread = new Thread(() => { worker.Produce(); });
            consumerThread.Name = "consumeTh";
            producerThread.Name = "produceTh";

            consumerThread.Start();
            producerThread.Start();

            // allow worker to loop
            Thread.Sleep(10000);
            worker.StopLoop = true ;

            producerThread.Join();
            producerThread.Join();

            Assert.IsTrue(true);
        }

    }
}
