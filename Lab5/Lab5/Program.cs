using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lab5
{
    class Program
    {
        static int indW = 0;
        static int indR = 0;
        static bool bFull = false;
        static int[] buf = new int[10];
        //static int[] cntW = new int[25];
        //static int[] cntR = new int[25];
        static int msgCnt = 50;
        static int rdCnt = 50;
        static ManualResetEvent[] readyToWrite = new ManualResetEvent[10];
        static ManualResetEvent[] readyToRead = new ManualResetEvent[10];
        //static Thread[] writerThreads = new Thread[25];
        //static Thread[] readerThreads = new Thread[25];
        //static ManualResetEventSlim[] readerReset = new ManualResetEventSlim[25];
        //static ManualResetEventSlim[] writerReset = new ManualResetEventSlim[25];
        class myWriter
        {
            public int ind { get; set; }
            public Task writerThread { get; set; }
            public ManualResetEventSlim writerReset { get; set; }
            public int priority { get; set; }
            public int writeCount { get; set; }
            public myWriter(ManualResetEventSlim writerReset, int priority)
            {

                this.writerReset = writerReset;
                this.priority = priority;
            }
            public myWriter()
            {
                writerReset = new ManualResetEventSlim();
            }
        }
        class myReader
        {
            public int ind { get; set; }
            public Task readerThread { get; set; }
            public ManualResetEventSlim readerReset { get; set; }
            public int priority { get; set; }
            public int readCount { get; set; }
            public myReader(ManualResetEventSlim readerReset, int priority)
            {

                this.readerReset = readerReset;
                this.priority = priority;
            }
            public myReader()
            {
                readerReset = new ManualResetEventSlim();
            }
        }
        static List<myWriter> myWriters = new List<myWriter>();
        static List<myReader> myReaders = new List<myReader>();
        static void Main(string[] args)
        {
            Random random = new Random();
            //int n = 25;
            
            
            for(int i = 0; i < readyToWrite.Length; i++)
            {
                readyToWrite[i] = new ManualResetEvent(true);
                readyToRead[i] = new ManualResetEvent(false);
            }
            readyToWrite[0].Set();
            for (int i = 0; i < 5; i++)
            {
                var myWriter = new myWriter();
                myWriter.ind = i;
                myWriter.writerReset.Set();
                myWriter.priority = random.Next(0, 4);
                myWriters.Add(myWriter);
                Console.WriteLine(i + " писатель имеет приоритет" + myWriter.priority);
                var myReader = new myReader();
                myReader.ind = i;
                myReader.readerReset.Set();
                myReader.priority = random.Next(0, 4);
                myReaders.Add(myReader);

                //cntW[i] = 0;
                //cntR[i] = 0;

            }
            Task wrtMng = new Task(writerManager);
            Task rdMng = new Task(readerManager);
            wrtMng.Start();
            rdMng.Start();
            Task.WaitAll(wrtMng);
            Task.WaitAll(rdMng);
            foreach(var mr in myWriters) { Console.WriteLine(mr.ind + " писатель писал " + mr.writeCount + " раз"); }
            Console.ReadLine();
            //for(int i = 0; i < n; i++)
            //{

            //    writerThreads[i] = new Thread(() => { });
            //    int priority = random.Next(0, 4);
            //    switch (priority)
            //    {
            //        case 0: writerThreads[i].Priority = ThreadPriority.Lowest; break;
            //        case 1: writerThreads[i].Priority = ThreadPriority.BelowNormal; break;
            //        case 2: writerThreads[i].Priority = ThreadPriority.Normal; break;
            //        case 3: writerThreads[i].Priority = ThreadPriority.AboveNormal; break;
            //        case 4: writerThreads[i].Priority = ThreadPriority.Highest; break;
            //    }
            //    writerThreads[i].Start();

            //    readerThreads[i] = new Thread(() => { });
            //    priority = random.Next(0, 4);
            //    switch (priority)
            //    {
            //        case 0: readerThreads[i].Priority = ThreadPriority.Lowest; break;
            //        case 1: readerThreads[i].Priority = ThreadPriority.BelowNormal; break;
            //        case 2: readerThreads[i].Priority = ThreadPriority.Normal; break;
            //        case 3: readerThreads[i].Priority = ThreadPriority.AboveNormal; break;
            //        case 4: readerThreads[i].Priority = ThreadPriority.Highest; break;
            //    }
            //    readerThreads[i].Start();
            //    var red = new List<int>();


            //}
        }
        static void readerManager()
        {
            while (true)
            {
                if (rdCnt != 0)
                {
                    while (true)
                    {
                        var ind = getReader();
                        if (ind != -1)
                        {
                            myReaders[ind].readerThread = new Task(() => { Read(ind); });
                            myReaders[ind].readerThread.Start();
                            Interlocked.Decrement(ref rdCnt);
                            break;
                        }
                    }
                }
                else break;
                
               
            }
        }
        static void writerManager()
        {
            while(true)
            {
                if (msgCnt != 0)
                {
                    while (true)
                    {
                        var ind = getWriter();
                        //Console.WriteLine(ind);
                        if (ind != -1)
                        {
                            myWriters[ind].writerThread = new Task(() => { Write(ind); });
                            myWriters[ind].writerThread.Start();
                            Interlocked.Decrement(ref msgCnt);
                            break;
                        }
                        else Thread.Sleep(100);
                    }
                }
                else { Task[] task = new Task[5];for (int i = 0; i < 5; i++) { task[i] = myWriters[i].writerThread; } Task.WaitAll(task); break; }
                //lock ("write")
                //{
                //    buf[indW] = 1;
                //    Interlocked.Increment(ref cntW[i]);
                //    if (indW == 9) Interlocked.Exchange(ref indW, 0);
                //    else Interlocked.Increment(ref indW);


                //}
            }
        }
        static int getWriter()
        {
            lock ("this")
            {
                int ind = -1;

                ind = getIndexWriter(4);
                if (ind != -1) return ind;

                ind = getIndexWriter(3);
                if (ind != -1) return ind;

                ind = getIndexWriter(2);
                if (ind != -1) return ind;

                ind = getIndexWriter(1);
                if (ind != -1) return ind;

                ind = getIndexWriter(0);
                return ind;
            }
        }
        static int getReader()
        {
            lock ("this")
            {
                int ind = -1;

                ind = getIndexReader(4);
                if (ind != -1) return ind;

                ind = getIndexReader(3);
                if (ind != -1) return ind;

                ind = getIndexReader(2);
                if (ind != -1) return ind;

                ind = getIndexReader(1);
                if (ind != -1) return ind;

                ind = getIndexReader(0);
                return ind;
            }
        }

        private static int getIndexWriter(int priority)
        {
            int ind = -1;
            for(int i = 0; i < myWriters.Count;i++)
            {
                if (myWriters[i].priority == priority && myWriters[i].writerReset.IsSet) 
                { myWriters[i].writerReset.Reset(); ind = myWriters[i].ind;break; }
            }
            return ind;
        }

        private static int getIndexReader(int priority)
        {
            int ind = -1;
            foreach (var e in myReaders)
            {
                if (e.priority == priority && e.readerReset.IsSet) { e.readerReset.Reset(); ind = e.ind;break; }
            }
            return ind;
        }
        static void Write(int ind)
        {
            int tmp = 0;
            readyToWrite[indW].WaitOne();
            lock ("write lock")
            {
                readyToWrite[indW].Reset();
                int tmpIndW = indW;
                for (int i = 0; i < 1000000; i++) tmp = i;
                if (buf[indW] == 0)
                {
                    buf[indW] = tmp;
                    myWriters[ind].writeCount += 1;

                }
                else
                {
                    while (true)
                    {
                        if (buf[indW] == 0) break;
                    }
                    buf[indW] = tmp;
                    myWriters[ind].writeCount += 1;

                }

                //Console.WriteLine(ind + " писатель записал в индекс " + indW);
                if (indW != 9) Interlocked.Increment(ref indW);
                else Interlocked.Exchange(ref indW, 0);

                readyToRead[tmpIndW].Set();

                myWriters[ind].writerReset.Set();
            }
            
        }

        static void Read(int ind)
        {
            readyToRead[indR].WaitOne();
            lock ("read lock")
            {
                readyToRead[indR].Reset();
                int tmpIndR = indR;
                int tmp = 0;
                for (int i = 0; i < 1000000; i++) tmp = i;

                //buf[indR] = 0;

                if (buf[indR] != 0)
                {
                    buf[indR] = 0;
                    myReaders[ind].readCount += 1;

                }
                else
                {
                    while (true)
                    {
                        if (buf[indR] != 0) break;
                    }
                    buf[indR] = 0;
                    myReaders[ind].readCount += 1;

                }

                //myReaders[ind].readCount += 1;

                //Console.WriteLine("   " + ind + " читатель прочитал индекс " + indR);
                if (indR != 9) Interlocked.Increment(ref indR);
                else Interlocked.Exchange(ref indR, 0);

                readyToWrite[tmpIndR].Set();
                myReaders[ind].readerReset.Set();
            }
            
        }





    }
}
