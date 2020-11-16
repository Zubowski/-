using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
    class Program
    {
        static int[] prime;
        static void Main(string[] args)
        {
            for (int i = 0; i < 12; i++) 
            {
                Console.WriteLine("n: ");
                int n = int.Parse(Console.ReadLine());
                Stopwatch stopwatch1 = new Stopwatch();
                stopwatch1.Start();
                SieveOfEratosthenes(n);
                stopwatch1.Stop();
                Console.WriteLine(stopwatch1.ElapsedMilliseconds.ToString());
                Console.ReadLine();
            }
            Console.ReadLine();
        }

        
        public static void SieveOfEratosthenes(int n)
        {


            prime = new int[n + 1];

            for (int i = 0; i < n; i++)
                prime[i] = 2;

            for (int p = 2; p <= (int)Math.Sqrt(n); p++)
            {
                for (int q = p + 1; q <= (int)Math.Sqrt(n); q++)
                {
                    if (prime[q] != 0)
                    {
                        if (q % p != 0) prime[q] = 1;
                        else prime[q] = 0;
                    }
                }
                for (int i = (int)Math.Sqrt(n); i <= n; i++)
                {
                    if (prime[i] != 0)
                    {
                        if (i % p != 0) prime[i] = 1;
                        else prime[i] = 0;
                    }
                }


            }
            //for (int i = 2; i <= n; i++)
            //{
            //    if (prime[i] == 1)
            //        Console.Write(i + " ");
            //}
        }
        public static void SieveOfEratosthenesParallel(int n)
        {
            Console.WriteLine("Кол-во потоков: ");
            int CT = (Convert.ToInt32(Console.ReadLine()));
            Task[] threads = null;
            prime = new int[n + 1];
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < n; i++)
                prime[i] = 2;

            for (int p = 2; p <= (int)Math.Sqrt(n); p++)
            {
                for (int q = p + 1; q <= (int)Math.Sqrt(n); q++)
                {
                    if (prime[q] != 0)
                    {
                        if (q % p != 0) prime[q] = 1;
                        else prime[q] = 0;
                    }
                }
            }
            int interval = (n - (int)Math.Sqrt(n)) / CT;
            int ost = n % CT;
            int ind = ost - 1;
            //Console.WriteLine(ind.ToString());
            mStrc[] masInd = new mStrc[CT];
            int tmpInd = 0;
            for (int i = 0; i < CT; i++)
            {

                for (int j = 0; j < interval; j++)
                {
                    ++ind;
                }
                masInd[i] = new mStrc(tmpInd + (int)Math.Sqrt(n), ind + (int)Math.Sqrt(n));
                tmpInd = ind;
            }
            threads = new Task[CT];


            int ind1 = 0;
            foreach (var el in masInd)
            {
                threads[ind1] = new Task(() =>
                {

                    Func(el.cur, el.prev, n);
                });
                threads[ind1].Start();
                ++ind1;
            }


            //for (int i = (int)Math.Sqrt(n); i <= n; i++)
            //{
            //    if (prime[i] != 0)
            //    {
            //        if (i % p != 0) prime[i] = 1;
            //        else prime[i] = 0;
            //    }
            //}



            Task.WaitAll(threads);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds.ToString());
            //for (int i = 2; i <= n; i++)
            //{
            //    if (prime[i] == 1)
            //        Console.Write(i + " ");
            //}
        }
        static void Func(int endInd, int startInd, int n)
        {
            for (int p = 2; p <= (int)Math.Sqrt(n); p++)
            {
                for (int i = startInd; i <= endInd; i++)
                {
                    if (prime[i] != 0)
                    {
                        if (i % p != 0) prime[i] = 1;
                        else prime[i] = 0;
                    }
                }


            }
        }
    }
    class mStrc
    {
        public int prev { get; set; }
        public int cur { get; set; }
        public mStrc(int prev, int cur)
        {
            this.cur = cur;
            this.prev = prev;
        }

    }
}
