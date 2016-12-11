using System;
using System.Collections.Generic;
using System.IO;

namespace Explorer.Models
{
    public class ExplorerObj
    {
        private Dictionary<string, int> _counters = new Dictionary<string, int> { ["Small"] = 0, ["Medium"] = 0, ["Large"] = 0 };
        private List<DirectoryObject> _items = new List<DirectoryObject>();
        private List<string> _errors = new List<string>();
        private static readonly string rootName = "Root";

        public ExplorerObj(){}

        public List<DirectoryObject> Items { get { return _items; } }
        public List<string> ErrorsList { get { return _errors; } }

        public string CurrentPath { get; private set; }
        public int SmallFiles { get { return _counters["Small"]; } }
        public int MediumFiles { get { return _counters["Medium"]; } }
        public int LargeFiles { get { return _counters["Large"]; } }
        public bool IsRoot { get; private set; }


        private IEnumerable<DirectoryObject> GetItems(string currentPath)
        {
            var result = new List<DirectoryObject>();
            try
            {
                DirectoryInfo di = new DirectoryInfo(currentPath);
                DirectoryInfo[] diArr = di.GetDirectories();
                foreach (var item in diArr)
                    result.Add(new DirectoryObject(item.Name, false));

                FileInfo[] fiArr = di.GetFiles();
                foreach (var item in fiArr)
                    result.Add(new DirectoryObject(item.Name, true));
                return result;
            }
            catch (Exception ex)
            {
                ErrorsList.Add(ex.Message);
                return result;
            }
        }

        private void CheckGroup(FileInfo fi)
        {
            var ratio = 1024 * 1024;
            if (fi.Length >= 100 * ratio)
                _counters["Large"]++;
            else
            {
                if (fi.Length <= 10 * ratio)
                    _counters["Small"]++;
                else
                {
                    if (fi.Length <= 50 * ratio)
                        _counters["Medium"]++;
                }
            }
        }

        private void GroupByCount(string baseDirectory)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(baseDirectory);
                FileInfo[] fiArr = di.GetFiles();

                foreach (FileInfo fi in fiArr)
                    CheckGroup(fi);

                string[] subdirectoryEntries = Directory.GetDirectories(baseDirectory);
                foreach (string subdirectory in subdirectoryEntries)
                    GroupByCount(subdirectory);
            }
            catch (Exception ex)
            {
                ErrorsList.Add(ex.Message);
                return;
            }
            return;
        }

        private static ExplorerObj CreateInfo(string name)
        {
            var newDirectory = new ExplorerObj();
            newDirectory.CurrentPath = name;
            if (name == rootName)
            {
                var driveInfo = DriveInfo.GetDrives();
                foreach (var info in driveInfo)
                {
                    newDirectory.Items.Add(new DirectoryObject(info.Name, false));
                    newDirectory.GroupByCount(info.Name);
                }
                newDirectory.IsRoot = true;
            }
            else
            {
                newDirectory.Items.AddRange(newDirectory.GetItems(newDirectory.CurrentPath));
                newDirectory.GroupByCount(newDirectory.CurrentPath);
                newDirectory.IsRoot = false;
            }
            return newDirectory;
        }

        public static ExplorerObj GoToParentDirectory(string id)
        {
            if (string.IsNullOrEmpty(id))
                return CreateInfo(Directory.GetCurrentDirectory());

            var parent = Directory.GetParent(id);
            if (parent == null)
                return CreateInfo("Root");
            return CreateInfo(parent.FullName);
        }      

        public static ExplorerObj GoToChild(string path, string file)
        {
            var str = path == rootName ? file : path + "\\" + file;
            if (Directory.Exists(str))
                return CreateInfo(str);
            return null;
        }

        public class DirectoryObject
        {
            public string Name { get; private set; }
            public bool IsFile { get; private set; }

            public DirectoryObject(string name, bool isfile)
            {
                Name = name;
                IsFile = isfile;
            }
        }
    } 
}