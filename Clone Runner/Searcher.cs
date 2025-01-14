﻿using System;
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
        BackgroundWorker worker;

        public delegate void ProgressChangedHandler(int current, int total);
        public delegate void DupeFoundHandler(FileInfo first, FileInfo dupe);
        public event ProgressChangedHandler OnProgressChanged;
        public event DupeFoundHandler OnDupeFound;

        public Searcher()
        {

        }

        protected virtual void ProgressChanged(int current, int total)
        {
            OnProgressChanged?.Invoke(current, total);
        }

        protected virtual void DupeFound(FileInfo first, FileInfo dupe)
        {
            OnDupeFound?.Invoke(first, dupe);
        }

        public void Start(List<string> loc)
        {
            this.locations = loc;
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerAsync();
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            DupeFoundEventArgs args = (DupeFoundEventArgs)e.UserState;
            DupeFound(args.First, args.Dupe);
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<FileInfo> files = new List<FileInfo>();
            List<FileInfo> foundFiles = new List<FileInfo>();
            foreach (string location in this.locations)
            {
                this.FindFiles(location, files);
            }
            int count = 0;
            foreach (FileInfo info in files)
            {
                foreach (FileInfo compareInfo in files)
                {
                    if (info.FullName == compareInfo.FullName) continue;
                    if (foundFiles.Contains(compareInfo)) continue;
                    bool identical = false;
                    identical |= (info.LastWriteTime == compareInfo.LastWriteTime);
                    identical |= (info.Length == compareInfo.Length);
                    Console.WriteLine(String.Format("{0}: {1}", info.Name, identical));
                    if (identical)
                    {
                        foundFiles.Add(info);
                        foundFiles.Add(compareInfo);
                        //DupeFound(info, compareInfo);
                        worker.ReportProgress(count, new DupeFoundEventArgs(info, compareInfo));
                    }
                }
                count++;
                ProgressChanged(count, files.Count);
            }
        }

        private void FindFiles(string location, List<FileInfo> files)
        {
            try
            {
                string[] paths = Directory.GetFiles(location);
                foreach (string path in paths)
                {
                    files.Add(new FileInfo(path));
                }
                ProgressChanged(0, files.Count);
                string[] directories = Directory.GetDirectories(location);
                foreach (string dir in directories)
                {
                    this.FindFiles(dir, files);
                }
            }
            catch (UnauthorizedAccessException) { }
        }

    }

    public class DupeFoundEventArgs : EventArgs
    {
        public FileInfo First { get; set; }
        public FileInfo Dupe { get; set; }

        public DupeFoundEventArgs(FileInfo first, FileInfo dupe)
        {
            this.First = first;
            this.Dupe = dupe;
        }
    }
}
