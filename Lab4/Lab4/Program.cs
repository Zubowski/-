using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lab4
{
    class Program
    {
        static string[] buf;
        static bool isEmpty;
        static object locker = new object();
        static AutoResetEvent ReaderResetEvent = new AutoResetEvent(false);
        static AutoResetEvent WriterResetEvent = new AutoResetEvent(false);
        static SemaphoreSlim ReaderSemaphore = new SemaphoreSlim(1);
        static SemaphoreSlim WriterSemaphore = new SemaphoreSlim(1);
        static void Main(string[] args)
        {
            isEmpty = true;
            buf = new string[50000000];

            #region lock
            int n = 10;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Task[] ReaderTasks = new Task[n];
            Task[] WriterTasks = new Task[n];
            for (int i = 0; i < n; i++)
            {
                ReaderTasks[i] = new Task(ReaderLock);
                ReaderTasks[i].Start();

                WriterTasks[i] = new Task(WriterLock);
                WriterTasks[i].Start();
            }
            Task.WaitAll(ReaderTasks);
            Task.WaitAll(WriterTasks);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);

            #endregion

            #region resetEvent
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            //WriterResetEvent.Set();
            //int n = 10;
            //Task[] ReaderTasks = new Task[n];
            //Task[] WriterTasks = new Task[n];
            //for (int i = 0; i < n; i++)
            //{
            //    ReaderTasks[i] = new Task(ReaderEvent);
            //    ReaderTasks[i].Start();

            //    WriterTasks[i] = new Task(WriterEvent);
            //    WriterTasks[i].Start();
            //}
            //Task.WaitAll(ReaderTasks);
            //Task.WaitAll(WriterTasks);
            //stopwatch.Stop();
            //Console.WriteLine(stopwatch.ElapsedMilliseconds);
            #endregion

            #region Semaphore
            //ReaderSemaphore.Wait();
            //int n = 10;
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            //Task[] ReaderTasks = new Task[n];
            //Task[] WriterTasks = new Task[n];
            //for (int i = 0; i < n; i++)
            //{
            //    ReaderTasks[i] = new Task(ReaderSemaphoreF);
            //    ReaderTasks[i].Start();

            //    WriterTasks[i] = new Task(WriterSemaphoreF);
            //    WriterTasks[i].Start();
            //}
            //Task.WaitAll(ReaderTasks);
            //Task.WaitAll(WriterTasks);
            //stopwatch.Stop();
            //Console.WriteLine(stopwatch.ElapsedMilliseconds);

            #endregion
            Console.ReadLine();

        }
        static void ReaderLock()
        {
            while (true)
            {
                if (isEmpty == false)
                {
                    lock (locker)
                    {
                        if (isEmpty != true)
                        {
                            for(int i = 0; i < buf.Length; i++)
                            {
                                buf[i] = "";
                            }
                            
                            Console.WriteLine("READED");
                            isEmpty = true;
                            break;
                        }
                    }

                }
            }
        }
        static void WriterLock()
        {
            while (true)
            {
                if (isEmpty == true)
                {
                    lock (locker)
                    {
                        if (isEmpty != false)
                        {
                            for(int i = 0; i < buf.Length; i++)
                            {
                                buf[i] = "text";
                            }
                            
                            Console.WriteLine("WRITED");
                            isEmpty = false;
                            break;
                        }
                    }


                }
            }

        }

        static void ReaderEvent()
        {

            ReaderResetEvent.WaitOne();
            for (int i = 0; i < buf.Length; i++)
            {
                buf[i] = "";
            }
            Console.WriteLine("READED");
            WriterResetEvent.Set();



        }
        static void WriterEvent()
        {

            WriterResetEvent.WaitOne();
            for (int i = 0; i < buf.Length; i++)
            {
                buf[i] = "text";
            }
            Console.WriteLine("WRITED");

            ReaderResetEvent.Set();

        }

        static void ReaderSemaphoreF()
        {

            ReaderSemaphore.Wait();
            for (int i = 0; i < buf.Length; i++)
            {
                buf[i] = "";
            }
            Console.WriteLine("READED");
            WriterSemaphore.Release();



        }
        static void WriterSemaphoreF()
        {

            WriterSemaphore.Wait();
            for (int i = 0; i < buf.Length; i++)
            {
                buf[i] = "text";
            }
            Console.WriteLine("WRITED");

            ReaderSemaphore.Release();

        }
    }
}