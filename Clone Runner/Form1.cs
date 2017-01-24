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
        //ResourceManager res_man;
        //CultureInfo cul;
        Searcher searcher;

        public Form1()
        {
            InitializeComponent();
            // https://www.codeproject.com/tips/580043/how-to-make-a-multi-language-application-in-csharp
            //cul = CultureInfo.CreateSpecificCulture("en");
            locationListBox.Items.Add(@"C:\Users\geschnei\Desktop\test");
            searchButton.Enabled = (locationListBox.Items.Count > 0);
            this.searcher = new Searcher();
            this.searcher.OnProgressChanged += Searcher_OnProgressChanged;
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
                g.Name = first.Name;
                g.Tag = first;
                resultListView.Groups.Add(g);
                ListViewItem firstItem = new ListViewItem(first.Name);
                firstItem.SubItems.Add(first.LastWriteTime.ToString());
                firstItem.SubItems.Add(formatFileSize(first.Length));
                firstItem.SubItems.Add("");
                firstItem.SubItems.Add(Path.GetDirectoryName(first.FullName));
                resultListView.Items.Add(firstItem);
            }
            ListViewItem dupeItem = new ListViewItem(dupe.Name);
            dupeItem.SubItems.Add(dupe.LastWriteTime.ToString());
            dupeItem.SubItems.Add(formatFileSize(dupe.Length));
            dupeItem.SubItems.Add("");
            dupeItem.SubItems.Add(Path.GetDirectoryName(dupe.FullName));
            resultListView.Items.Add(dupeItem);
        }

        private void Searcher_OnProgressChanged(int current, int total)
        {
            if (current == 0)
            {
                progressStatusLabel.Text = String.Format("Files found: {0}", total);
            }
            else
            {
                progressStatusLabel.Text = String.Format("Processed: {0}/{1}", current, total);
                /*progressProgressBar.Value = current;
                progressProgressBar.Maximum = total;*/
            }
        }

        private string formatFileSize(double len)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return String.Format("{0:0.##} {1}", len, sizes[order]);
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

        private void largeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            largeIconsToolStripMenuItem.Checked = true;
            smallIconsToolStripMenuItem.Checked = false;
            listToolStripMenuItem.Checked = false;
            tilesToolStripMenuItem.Checked = false;
            detailsToolStripMenuItem.Checked = false;
            resultListView.View = View.LargeIcon;
        }

        private void smallIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            largeIconsToolStripMenuItem.Checked = false;
            smallIconsToolStripMenuItem.Checked = true;
            listToolStripMenuItem.Checked = false;
            tilesToolStripMenuItem.Checked = false;
            detailsToolStripMenuItem.Checked = false;
            resultListView.View = View.SmallIcon;
        }

        private void listToolStripMenuItem_Click(object sender, EventArgs e)
        {
            largeIconsToolStripMenuItem.Checked = false;
            smallIconsToolStripMenuItem.Checked = false;
            listToolStripMenuItem.Checked = true;
            tilesToolStripMenuItem.Checked = false;
            detailsToolStripMenuItem.Checked = false;
            resultListView.View = View.List;
        }

        private void tilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            largeIconsToolStripMenuItem.Checked = false;
            smallIconsToolStripMenuItem.Checked = false;
            listToolStripMenuItem.Checked = false;
            tilesToolStripMenuItem.Checked = true;
            detailsToolStripMenuItem.Checked = false;
            resultListView.View = View.Tile;
        }

        private void detailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            largeIconsToolStripMenuItem.Checked = false;
            smallIconsToolStripMenuItem.Checked = false;
            listToolStripMenuItem.Checked = false;
            tilesToolStripMenuItem.Checked = false;
            detailsToolStripMenuItem.Checked = true;
            resultListView.View = View.Details;
        }
    }
}
