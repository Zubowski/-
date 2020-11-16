using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lab2
{
    class Program
    {
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
        static void Main(string[] args)
        {
            for (int q = 0; q < 15; q++)
            {
                Random random = new Random();

                List<double> list = new List<double>();
                int n = 0;
                do
                {
                    Console.WriteLine("Задайте количество элементов");
                }
                while (!int.TryParse(Console.ReadLine(), out n));

                double[] arr = new double[n];
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = random.NextDouble() * 100;
                }
                int[] indArr = new int[n];
                for (int i = 0; i < n; i++)
                {
                    indArr[i] = i;
                }

                Random rand = new Random();

                for (int i = indArr.Length - 1; i >= 1; i--)
                {
                    int j = rand.Next(i + 1);

                    int tmp = indArr[j];
                    indArr[j] = indArr[i];
                    indArr[i] = tmp;
                }

                Stopwatch stopwatch1 = new Stopwatch();
                stopwatch1.Start();
                foreach (var v in arr)
                {
                    //for (int i = 0; i < 1000; i++)
                    {
                        var tmp = Math.Pow(v, 1.789);
                    }
                }
                stopwatch1.Stop();
                Console.WriteLine("Последовательное выполнение: " + stopwatch1.ElapsedMilliseconds);

                int m = 0;
                do
                {
                    Console.WriteLine("Задайте количество потоков");
                }
                while (!int.TryParse(Console.ReadLine(), out m));

                int interval = n / m;
                int ost = n % m;
                int ind = ost - 1;
                //Console.WriteLine(ind.ToString());
                mStrc[] masInd = new mStrc[m];
                int tmpInd = 0;
                for (int i = 0; i < m; i++)
                {

                    for (int j = 0; j < interval; j++)
                    {
                        ++ind;
                    }
                    masInd[i] = new mStrc(tmpInd, ind);
                    tmpInd = ind;
                }
                Task[] threads = new Task[m];

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                int ind1 = 0;
                foreach (var el in masInd)
                {
                    threads[ind1] = new Task(() =>
                    {

                    Func(arr, el.cur, el.prev);
                    //Func2(arr, indArr, el.cur, el.prev);
                    });
                    threads[ind1].Start();
                    ++ind1;
                }
                Task.WaitAll(threads);
                stopwatch.Stop();
                Console.WriteLine(stopwatch.ElapsedMilliseconds.ToString());

                Console.ReadLine();
            }
            Console.ReadLine();

        }

        static void Func(double[] arr, int indArr, int prevInd)
        {
            for (int i = prevInd; i < indArr; i++)
            {
                //for (int j = 0; j < 1000; j++)
                    arr[i] = Math.Pow(arr[i], 1.789);
            }
        }
        static void Func2(double[] arr, int[] indArr, int curInd, int prevInd)
        {
            for (int i = prevInd; i < curInd; i++)
            {
                for (int j = 0; j < indArr[i]; j++)
                {
                    var tmp = Math.Pow(arr[i], 1.789);
                }
            }
        }
    }
}
