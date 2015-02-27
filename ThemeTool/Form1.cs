using QN;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ThemeTool
{
    public partial class Form1 : Form
    {
        string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

        private static bool IsSyncing = false;
        private static object lockObj = new object();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            f2.FormClosed += f2_FormClosed;
            f2.ShowDialog();
        }

        void f2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form2 f2 = sender as Form2;

            IList<Data> list = LoadList();

            if (!File.Exists(configPath))
            {
                File.Create(configPath).Close();
            }

            Data d = new Data();

            d.Src = f2.textBox1.Text.Trim();
            d.Target = f2.textBox2.Text.Trim();

            if (string.IsNullOrWhiteSpace(d.Src))
            {
                MessageBox.Show("主题路径不能为空。");
                return;
            }

            if (string.IsNullOrWhiteSpace(d.Target))
            {
                MessageBox.Show("子网站主题路径不能为空。");
                return;
            }

            list.Add(d);

            using (StreamWriter sw = new StreamWriter(configPath, false))
            {
                sw.Write(QJson.Serialize(list));
            }

            Bind();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Bind();
        }

        private void Bind()
        {
            IList<Data> list = LoadList();
            this.dataGridView1.DataSource = list;

            //Watcher.Run(list.ToArray());
        }

        private List<Data> LoadList()
        {
            List<Data> result = null;
            if (File.Exists(configPath))
            {
                using (StreamReader sr = new StreamReader(configPath))
                {
                    result = QJson.Deserialize<List<Data>>(sr.ReadToEnd());
                }
            }

            return result ?? new List<Data>();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            IList<Data> list = LoadList();

            foreach (DataGridViewRow r in this.dataGridView1.SelectedRows)
            {
                if (list.Any(m => string.Compare(m.Src, r.Cells["Src"].Value.ToString(), true) == 0))
                {
                    list.Remove(list.Single(m => string.Compare(m.Src, r.Cells["Src"].Value.ToString(), true) == 0));
                }
            }

            using (StreamWriter sw = new StreamWriter(configPath, false))
            {
                sw.Write(QJson.Serialize(list));
            }

            Bind();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DoSync();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DoSync();
        }

        private void DoSync()
        {
            lock (lockObj)
            {
                if (!IsSyncing)
                {
                    try
                    {
                        IsSyncing = true;

                        label1.Text = "同步中";

                        Sync.Do(LoadList());
                    }
                    finally
                    {
                        label1.Text = "已同步";

                        IsSyncing = false;
                    }
                }
            }
        }

        private void cbAuto_CheckedChanged(object sender, EventArgs e)
        {
            this.timer1.Enabled = this.cbAuto.Checked;
        }
    }
}