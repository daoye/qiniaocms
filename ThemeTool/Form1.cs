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
            IList<Data> list = LoadList();

            if (!File.Exists(configPath))
            {
                File.Create(configPath);
            }

            Data d = new Data();

            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "选择主题路径。";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                d.Src = dlg.SelectedPath;
            }

            dlg.Description = "选择子站点主题路径。";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                d.Target = dlg.SelectedPath;
            }

            if (string.IsNullOrWhiteSpace(d.Src))
            {
                MessageBox.Show("主题路径不能为空。");
                return;
            }

            if (list.Any(m => string.Compare(m.Src, d.Src, true) == 0))
            {
                MessageBox.Show("这个主题正在监听中。");
                return;
            }

            if (string.IsNullOrWhiteSpace(d.Target))
            {
                MessageBox.Show("子站点主题路径不能为空。");
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