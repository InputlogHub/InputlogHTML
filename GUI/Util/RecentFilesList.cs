using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using InputLog.Core.Util;

namespace GUI.Util
{
    /// <summary>
    /// An Recent Files list (also called MRU - Most Recently Used) displays the files that a program used recently in a menu. 
    /// </summary>
    public class RecentFilesList
    {
        #region Fields
        // The application's name.
        private readonly string ApplicationName;

        // A list of the files.
        private readonly int NumFiles;
        private readonly List<FileInfo> FileInfos;

        // The Recent File menu item.
        private readonly ToolStripMenuItem ParentMenuItem;

        // The menu items used to display files.
        private readonly ToolStripSeparator Separator;
        private readonly ToolStripMenuItem[] MenuItems;

        // Raised when the user selects a file from the Recent Files list.
        public delegate void FileSelectedEventHandler(string fileName);
        public event FileSelectedEventHandler FileSelected; 

        // Limit to the path length shown
        private const int MAX_PATH_LENGTH = 70;

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="menuItem"></param>
        /// <param name="numFiles"></param>
        public RecentFilesList(string applicationName, ToolStripMenuItem menuItem, int numFiles)
        {
            ApplicationName = applicationName;
            ParentMenuItem = menuItem;
            ParentMenuItem.Enabled = false;
            NumFiles = numFiles;
            FileInfos = new List<FileInfo>();

            // Make a separator.
            Separator = new ToolStripSeparator {Visible = false};
            ParentMenuItem.DropDownItems.Add(Separator);

            // Make the menu items we may later need.
            MenuItems = new ToolStripMenuItem[NumFiles + 1];
            for (int i = 0; i < NumFiles; i++)
            {
                MenuItems[i] = new ToolStripMenuItem {Visible = false};
                ParentMenuItem.DropDownItems.Add(MenuItems[i]);
            }
            ParentMenuItem.DropDownItems.Add(Separator);
            ToolStripItem toolStripItem = ParentMenuItem.DropDownItems.Add("Clear list");
            toolStripItem.Click += ClearRecentFiles;

            // Reload items from the registry.
            LoadFiles();

            // Display the items.
            ShowFiles();
        }

        /// <summary>
        /// LoadFiles uses the RegistryTools class's GetSetting method to load file names stored in the Registry.
        /// </summary>
        private void LoadFiles()
        {
            // Reload items from the registry.
            for (int i = 0; i < NumFiles; i++)
            {
                var fileName = (string) RegistryTools.GetSetting(ApplicationName, "FilePath" + i, "");
                if (fileName != "")
                {
                    FileInfos.Add(new FileInfo(fileName));
                }
            }
        }

        /// <summary>
        /// Save the current items in the Registry.
        /// </summary>
        private void SaveFiles()
        {
            // Delete the saved entries so the event handler isn't installed twice.
            DeleteAll();

            // Save the current entries.
            var index = 0;
            foreach (var fileInfo in FileInfos)
            {
                RegistryTools.SaveSetting(ApplicationName, "FilePath" + index,
                                          fileInfo.FullName);
                index++;
            }
        }

        /// <summary>
        /// Delete all the saved entries from the registry including the merge folder setting
        /// </summary>
        private void DeleteAll()
        {
            for (int i = 0; i < NumFiles; i++)
            {
                RegistryTools.DeleteSetting(ApplicationName, "FilePath" + i);
            }
            RegistryTools.DeleteSetting(ApplicationName, "DirPath");
        }

        /// <summary>
        /// Remove a file's info from the list.
        /// </summary>
        /// <param name="fileName"></param>
        private void RemoveFileInfo(string fileName)
        {
            // Remove occurrences of the file's information  from the FileInfos list and then inserts
            // the file at the beginning of the list. This prevents the list from containing duplicates.
            if (FileInfos != null && fileName != null)
            {
                for (int i = FileInfos.Count - 1; i >= 0; i--)
                {
                    if (FileInfos[i].FullName == fileName) FileInfos.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Add a file to the list, rearranging if necessary.
        /// </summary>
        /// <param name="fileName"></param>
        public void AddFile(string fileName)
        {
            // Remove the file from the list.
            RemoveFileInfo(fileName);

            // Add the file to the beginning of the list.
            FileInfos.Insert(0, new FileInfo(fileName));

            // If we have too many items, remove the last one.
            if (FileInfos.Count > NumFiles) FileInfos.RemoveAt(NumFiles);

            // Display the files.
            ShowFiles();

            // Update the Registry.
            SaveFiles();
        }
 
       /// <summary>
        /// Remove a file from the list, rearranging if necessary.
       /// </summary>
       /// <param name="fileName"></param>
        public void RemoveFile(string fileName)
        {
            // Remove the file from the list.
            RemoveFileInfo(fileName);

            // Display the files.
            ShowFiles();

            // Update the Registry.
            SaveFiles();
        }

        /// <summary>
        /// Display the files in the menu items.
        /// </summary>
        private void ShowFiles()
        {
            if(FileInfos.Count > 0)
            {
                ParentMenuItem.Enabled = true;
                Separator.Visible = true;
            }
            else return;

            for (int i = 0; i < FileInfos.Count; i++)
            {
                string shortPath = StringUtils.ShortenPathname(FileInfos[i].ToString(), MAX_PATH_LENGTH);
                MenuItems[i].Text = string.Format("&{0}. {1}", i + 1, shortPath); //.Name);
                MenuItems[i].Visible = true;
                MenuItems[i].Tag = FileInfos[i];
                MenuItems[i].Click -= FileClick;
                MenuItems[i].Click += FileClick;
            }
            for (int i = FileInfos.Count; i < NumFiles; i++)
            {
                MenuItems[i].Visible = false;
                MenuItems[i].Click -= FileClick;
            }
        }

        /// <summary>
        /// Removes all entries from the registry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearRecentFiles(object sender, EventArgs e)
        {
            DeleteAll();
            FileInfos.Clear();
            ParentMenuItem.Enabled = false;
        }

        /// <summary>
        /// The user selected a file from the menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileClick(object sender, EventArgs e)
        {
            // Don't bother if no one wants to catch the event.
            if (FileSelected != null)
            {
                // Get the corresponding FileInfo object.
                var menuItem = sender as ToolStripMenuItem;
                if (menuItem != null)
                {
                    var fileInfo = menuItem.Tag as FileInfo;
                   
                    // Raise the event.
                    if (fileInfo != null)
                    {
                        FileSelected(fileInfo.FullName);
                    }
                }
            }
        }
    }
}
