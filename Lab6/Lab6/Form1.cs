using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace Lab6
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            DrawingControl.SetDoubleBuffered(dataGridView1);
            this.Load += Form1_Load;
            dataGridView1.CellClick += DataGridView1_CellClick;
            dataGridView1.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.Sunken;

            var lifeRules = new Func<int, bool, bool>((p, state) =>
            {
                if (p == 3)
                    return true;
                else
                if (p == 2)
                    return state;
                else
                    return false;
            });
        }

        bool lifeRules(int p, bool state)
        {
            if (p == 3)
                return true;
            else
            if (p == 2)
                return state;
            else
                return false;
        }

        int CalcPotential(int i, int j)
        {
            int p = 0;
            for (int x = i - 1; x <= i + 1; x++)
                for (int y = j - 1; y <= j + 1; y++)
                {
                    if (x < 0 || y < 0 || x >= dataGridView1.Columns.Count ||
                    y >= dataGridView1.Rows.Count || (x == i && y == j))
                        continue;
                    if (dataGridView1[x, y].Style.BackColor==Color.Black) ++p;
                }
            return p;
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor != Color.Black)
            {
                dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.Black;
                dataGridView1[e.ColumnIndex, e.RowIndex].Style.SelectionBackColor = Color.Black;
                current[e.ColumnIndex, e.RowIndex] = true;
            }
            else
            {
                dataGridView1[e.ColumnIndex, e.RowIndex].Style.SelectionBackColor = Color.White;
                dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.White;
                current[e.ColumnIndex, e.RowIndex] = false;
            }
            
        }

        bool[,] current;
        bool[,] next;
        int n;
        private void Form1_Load(object sender, EventArgs e)
        {
            n = 100;
            current = new bool[n, n];
            next = new bool[n, n];
            dataGridView1.ColumnCount = n;
            dataGridView1.RowCount = n;
            for (int i = 0; i < n; i++)
            {
                dataGridView1.Columns[i].Width = 5;
                dataGridView1.Rows[i].Height = 5;
                for(int j = 0; j < n; j++)
                {
                    current[i, j] = false;
                    next[i, j] = false;
                }
            }
        }

        void RunParallel()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (true)
            {
                Parallel.For(0, dataGridView1.Columns.Count, (i) =>
                {
                    for (int j = 0; j < dataGridView1.Rows.Count; j++)
                    {
                        var p = CalcPotential(i, j);
                        
                        next[i, j] = lifeRules(p, current[i, j]);
                    }
                });
                bool noChange = true;
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (current[i, j] != next[i, j])
                        {
                            noChange = false;
                            break;
                        }
                    }
                    if (noChange == false) break;
                }
                if (noChange == false)
                {

                    for (int i = 0; i < n; i++)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            current[i, j] = next[i, j];
                        }
                    }
                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {

                        for (int j = 0; j < dataGridView1.Rows.Count; j++)
                        {
                            if (current[i, j] == true) dataGridView1.Rows[j].Cells[i].Style.BackColor = Color.Black;
                            else dataGridView1.Rows[j].Cells[i].Style.BackColor = Color.White;
                        }
                    }
                    //Thread.Sleep(100);
                }
                else break;

            }
            stopwatch.Stop();
            label2.BeginInvoke(new Action(() => { label2.Text = stopwatch.ElapsedMilliseconds.ToString(); }));
        }
        void RunCons() 
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (true)
            {
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    for (int j = 0; j < dataGridView1.Rows.Count; j++)
                    {
                        var p = CalcPotential(i, j);
                        next[i, j] = lifeRules(p, current[i, j]);
                    }
                bool noChange = true;
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (current[i, j] != next[i, j])
                        {
                            noChange = false;
                            break;
                        }
                    }
                    if (noChange == false) break;
                }
                if (noChange == false)
                {
                    
                    for(int i = 0; i < n; i++)
                    {
                        for(int j = 0; j < n; j++)
                        {
                            current[i, j] = next[i, j];
                        }
                    }
                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {

                        for (int j = 0; j < dataGridView1.Rows.Count; j++)
                        {
                            if (current[i, j] == true) dataGridView1.Rows[j].Cells[i].Style.BackColor = Color.Black;
                            else dataGridView1.Rows[j].Cells[i].Style.BackColor = Color.White;
                        }
                    }
                    //Thread.Sleep(100);
                }
                else break;
                
            }
            stopwatch.Stop();
            label2.BeginInvoke(new Action(() => {label2.Text = stopwatch.ElapsedMilliseconds.ToString(); }));
        }
        Thread thread;
        
        private void button1_Click(object sender, EventArgs e)
        {

            thread = new Thread(RunCons);
            thread.Start();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    next[i, j] = false;
                }
            }
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    var val = random.Next(0, 2);
                    if (val == 1) { current[i, j] = true;  dataGridView1.Rows[j].Cells[i].Style.BackColor = Color.Black; }
                    else { current[i, j] = false; dataGridView1.Rows[j].Cells[i].Style.BackColor = Color.White; }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            thread.Abort();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            thread = new Thread(RunParallel);
            thread.Start();
        }
    }
}
