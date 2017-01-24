using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;

namespace Clone_Runner
{
    class Searcher
    {
        private List<string> locations;
        private List<string> files;
        BackgroundWorker worker;

        public delegate void ProgressChangedHandler(int current, int total);
        public event ProgressChangedHandler ProgressChanged;

        public Searcher()
        {
            this.files = new List<string>();
        }

        protected virtual void OnProgressChanged(int current, int total)
        {
            ProgressChanged?.Invoke(current, total);
        }

        public void Start(List<string> loc)
        {
            this.locations = loc;
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerAsync();
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // throw new NotImplementedException();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (string location in this.locations)
            {
                this.FindFiles(location);
            }
        }

        private void FindFiles(string location)
        {
            try
            {
                this.files.AddRange(Directory.GetFiles(location));
                OnProgressChanged(0, this.files.Count);
                string[] directories = Directory.GetDirectories(location);
                foreach (string dir in directories)
                {
                    this.FindFiles(dir);
                }
            }
            catch (UnauthorizedAccessException) { }
        }
    }
}
