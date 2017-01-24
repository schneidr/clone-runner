using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Resources;
using System.IO;

namespace Clone_Runner
{
    public partial class Form1 : Form
    {
        ResourceManager res_man;
        CultureInfo cul;
        Searcher searcher;

        public Form1()
        {
            InitializeComponent();
            // https://www.codeproject.com/tips/580043/how-to-make-a-multi-language-application-in-csharp
            cul = CultureInfo.CreateSpecificCulture("en");
            locationListBox.Items.Add(@"C:\Users\geschnei\Desktop\test");
            searchButton.Enabled = (locationListBox.Items.Count > 0);
            this.searcher = new Searcher();
            this.searcher.OnProgressChanged += Searcher_ProgressChanged;
            this.searcher.OnDupeFound += Searcher_OnDupeFound;
        }

        private void Searcher_OnDupeFound(FileInfo first, FileInfo dupe)
        {
            ListViewGroup g = null;
            foreach (ListViewGroup group in resultListView.Groups)
            {
                if (group.Tag == first)
                {
                    g = group;
                    break;
                }
            }
            if (g == null)
            {
                g = new ListViewGroup(first.Name);
                resultListView.Groups.Add(g);
                ListViewItem firstItem = new ListViewItem(first.Name);
                resultListView.Items.Add(firstItem);
            }
            ListViewItem dupeItem = new ListViewItem(dupe.Name);
            resultListView.Items.Add(dupeItem);
        }

        private void Searcher_ProgressChanged(int current, int total)
        {
            if (current == 0)
            {
                progressStatusLabel.Text = String.Format("Files found: {0}", total);
            }
            else
            {
                progressStatusLabel.Text = String.Format("Processed: {0}/{1}", current, total);
                progressProgressBar.Value = current;
                progressProgressBar.Maximum = total;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void locationAddButton_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (!locationListBox.Items.Contains(folderBrowserDialog1.SelectedPath))
                {
                    locationListBox.Items.Add(folderBrowserDialog1.SelectedPath);
                }
            }
            searchButton.Enabled = (locationListBox.Items.Count > 0);
            
        }

        private void locationRemoveButton_Click(object sender, EventArgs e)
        {
            if (locationListBox.SelectedIndex >= 0)
            {
                locationListBox.Items.RemoveAt(locationListBox.SelectedIndex);
            }
            searchButton.Enabled = (locationListBox.Items.Count > 0);
        }

        private void locationListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            locationRemoveButton.Enabled = (locationListBox.SelectedIndex >= 0);
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
            List<string> locations = locationListBox.Items.Cast<string>().ToList();
            searcher.Start(locations);
        }
    }
}
